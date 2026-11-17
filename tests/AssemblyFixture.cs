using System.Reflection;
using System.Text.Json;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer.Tests;

/// <summary>
/// Loads the built plugin DLL with MetadataLoadContext (reflection only, no Crestron runtime needed)
/// </summary>
public static class AssemblyFixture
{
    private const string PluginAssemblyName = "epi-tripleplay-iptvserver.4Series";

    private static readonly Lazy<MetadataLoadContext> LazyContext = new(CreateContext);
    private static readonly Lazy<Assembly> LazyAssembly = new(LoadPluginAssembly);

    // Test output is tests/bin/{Configuration}/net8.0/
    private static string Configuration
    {
        get
        {
            var parts = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            return parts[^2];
        }
    }

    private static string RepoRoot =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    public static string SourceDirectory => Path.Combine(RepoRoot, "src");

    public static string PluginCsprojPath => Path.Combine(SourceDirectory, PluginAssemblyName + ".csproj");

    // The plugin csproj uses <OutputPath>bin\$(Configuration)\</OutputPath> and <TargetFramework>net8</TargetFramework>.
    private static string PluginDllPath =>
        Path.Combine(SourceDirectory, "bin", Configuration, "net8", PluginAssemblyName + ".dll");

    public static MetadataLoadContext Context => LazyContext.Value;

    public static Assembly PluginAssembly => LazyAssembly.Value;

    private static MetadataLoadContext CreateContext()
    {
        if (!File.Exists(PluginDllPath))
            throw new FileNotFoundException($"Plugin DLL not found at '{PluginDllPath}'. Build the plugin first.", PluginDllPath);

        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var dllByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Priority 1: plugin output dir
        foreach (var dll in Directory.GetFiles(Path.GetDirectoryName(PluginDllPath)!, "*.dll"))
            dllByName[Path.GetFileName(dll)] = dll;

        // Priority 2: .NET runtime
        foreach (var dll in Directory.GetFiles(runtimeDir, "*.dll"))
            dllByName.TryAdd(Path.GetFileName(dll), dll);

        // Priority 3: packages from project.assets.json. Not deps.json: the PepperDashEssentials reference
        // uses <ExcludeAssets>runtime</ExcludeAssets>, so deps.json leaves those dependencies out.
        var assetsJsonPath = Path.Combine(SourceDirectory, "obj", "project.assets.json");
        if (File.Exists(assetsJsonPath))
        {
            foreach (var path in ResolveProjectAssetsAssemblies(assetsJsonPath))
                dllByName.TryAdd(Path.GetFileName(path), path);
        }

        return new MetadataLoadContext(new PathAssemblyResolver(dllByName.Values));
    }

    private static IEnumerable<string> ResolveProjectAssetsAssemblies(string assetsJsonPath)
    {
        var nugetDir = Environment.GetEnvironmentVariable("NUGET_PACKAGES");
        if (string.IsNullOrWhiteSpace(nugetDir))
            nugetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");

        using var stream = File.OpenRead(assetsJsonPath);
        using var doc = JsonDocument.Parse(stream);

        if (!doc.RootElement.TryGetProperty("targets", out var targets))
            yield break;

        // Prefer the RID-less target: a RID target lists runtime assets only.
        var allTargets = targets.EnumerateObject().ToList();
        var target = allTargets.FirstOrDefault(t => !t.Name.Contains('/'));
        if (target.Value.ValueKind != JsonValueKind.Object)
            target = allTargets.FirstOrDefault();
        if (target.Value.ValueKind != JsonValueKind.Object)
            yield break;

        foreach (var lib in target.Value.EnumerateObject())
        {
            var slash = lib.Name.LastIndexOf('/');
            if (slash < 0) continue;
            var packageId = lib.Name[..slash].ToLowerInvariant();
            var version = lib.Name[(slash + 1)..];

            if (!lib.Value.TryGetProperty("compile", out var assets) &&
                !lib.Value.TryGetProperty("runtime", out assets))
                continue;

            foreach (var asset in assets.EnumerateObject())
            {
                if (!asset.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    continue;

                var dllPath = Path.Combine(nugetDir, packageId, version, asset.Name.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(dllPath))
                    yield return dllPath;
            }
        }
    }

    private static Assembly LoadPluginAssembly() => Context.LoadFromAssemblyPath(PluginDllPath);

    /// <summary>
    /// Non-abstract types whose base-type chain includes EssentialsPluginDeviceFactory&lt;T&gt;
    /// </summary>
    public static List<Type> FindFactoryTypes() =>
        PluginAssembly.GetTypes().Where(t => !t.IsAbstract && InheritsFromFactory(t)).ToList();

    private static bool InheritsFromFactory(Type type)
    {
        for (var b = type.BaseType; b != null; b = b.BaseType)
            if (b.IsGenericType && b.GetGenericTypeDefinition().Name.StartsWith("EssentialsPluginDeviceFactory"))
                return true;
        return false;
    }

    public static Type GetPluginType(string name) =>
        PluginAssembly.GetTypes().Single(t => t.Name == name);

    private static readonly Lazy<string[]> AllSourceContents = new(() =>
        Directory.GetFiles(SourceDirectory, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                     && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .Select(File.ReadAllText)
            .ToArray());

    /// <summary>
    /// Source of the file that declares the class (for values only set at runtime, which MetadataLoadContext can't run)
    /// </summary>
    public static string? FindSourceForClass(string className) =>
        AllSourceContents.Value.FirstOrDefault(content => DeclaresClass(content, className));

    private static bool DeclaresClass(string content, string className)
    {
        var needle = "class " + className;
        for (var i = content.IndexOf(needle, StringComparison.Ordinal); i >= 0;
             i = content.IndexOf(needle, i + 1, StringComparison.Ordinal))
        {
            var after = i + needle.Length;
            var next = after < content.Length ? content[after] : ' ';
            if (!char.IsLetterOrDigit(next) && next != '_')
                return true;
        }
        return false;
    }
}

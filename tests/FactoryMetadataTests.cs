using System.Text.RegularExpressions;
using System.Xml.Linq;
using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer.Tests;

/// <summary>
/// Factory values are set in the constructor, which MetadataLoadContext can't run, so these read the source.
/// </summary>
public class FactoryMetadataTests
{
    private static string FactorySource =>
        AssemblyFixture.FindSourceForClass("TriplePlayIptvFactory")
        ?? throw new InvalidOperationException("TriplePlayIptvFactory source not found");

    [Fact]
    public void MinimumEssentialsFrameworkVersion_Is_Numeric_Core_Of_Pinned_Package()
    {
        var match = Regex.Match(FactorySource, @"MinimumEssentialsFrameworkVersion\s*=\s*""([^""]+)""");
        match.Success.Should().BeTrue("the factory must set MinimumEssentialsFrameworkVersion");
        var minimum = match.Groups[1].Value;

        // System.Version throws on a prerelease suffix, and Essentials then skips the plugin silently.
        Regex.IsMatch(minimum, @"^\d+\.\d+\.\d+$").Should().BeTrue($"'{minimum}' must be plain major.minor.patch");

        var pinned = XDocument.Load(AssemblyFixture.PluginCsprojPath)
            .Descendants("PackageReference")
            .Single(e => (string?)e.Attribute("Include") == "PepperDashEssentials")
            .Attribute("Version")!.Value;
        var pinnedCore = Regex.Match(pinned, @"^\d+\.\d+\.\d+").Value;

        minimum.Should().Be(pinnedCore);
        new Version(minimum).Major.Should().BeGreaterThanOrEqualTo(3);
    }

    [Theory]
    [InlineData("tripleplayiptv")]
    [InlineData("tripleplay")]
    public void Factory_Registers_TypeName(string typeName)
    {
        TypeNames().Should().Contain(typeName);
    }

    [Fact]
    public void TypeNames_Are_Lower_Case_And_Unique()
    {
        var names = TypeNames();

        names.Should().OnlyHaveUniqueItems();
        names.Should().OnlyContain(n => n == n.ToLowerInvariant());
    }

    private static List<string> TypeNames()
    {
        var block = Regex.Match(FactorySource, @"TypeNames\s*=\s*new\s+List<string>\s*\(\)\s*\{([^}]*)\}");
        block.Success.Should().BeTrue("the factory must assign TypeNames");

        return Regex.Matches(block.Groups[1].Value, @"""([^""]+)""").Select(m => m.Groups[1].Value).ToList();
    }
}

using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer.Tests;

/// <summary>
/// Checks the JSON contract on the compiled config classes
/// </summary>
public class ConfigDeserializationTests
{
    [Theory]
    [InlineData("TriplePlayIptvConfig")]
    [InlineData("TriplePlayServicesPresetsConfig")]
    public void Config_Has_Parameterless_Constructor(string className)
    {
        AssemblyFixture.GetPluginType(className).GetConstructor(Type.EmptyTypes).Should().NotBeNull();
    }

    [Theory]
    [InlineData("TriplePlayIptvConfig", "control")]
    [InlineData("TriplePlayIptvConfig", "pollTimeMs")]
    [InlineData("TriplePlayIptvConfig", "warningTimeoutMs")]
    [InlineData("TriplePlayIptvConfig", "errorTimeoutMs")]
    [InlineData("TriplePlayIptvConfig", "stbId")]
    [InlineData("TriplePlayIptvConfig", "presetsMaxAllowed")]
    [InlineData("TriplePlayIptvConfig", "presets")]
    [InlineData("TriplePlayIptvConfig", "presetsList")]
    [InlineData("TriplePlayServicesPresetsConfig", "id")]
    [InlineData("TriplePlayServicesPresetsConfig", "channelNumber")]
    [InlineData("TriplePlayServicesPresetsConfig", "name")]
    [InlineData("TriplePlayServicesPresetsConfig", "iconPath")]
    [InlineData("TriplePlayServicesPresetsConfig", "isWatchable")]
    public void Config_Property_Has_JsonPropertyAttribute(string className, string jsonName)
    {
        var type = AssemblyFixture.GetPluginType(className);

        var hasAttribute = type.GetProperties().Any(p =>
            p.CustomAttributes.Any(a =>
                a.AttributeType.Name == "JsonPropertyAttribute"
                && a.ConstructorArguments.Any(arg => string.Equals(arg.Value?.ToString(), jsonName, StringComparison.Ordinal))));

        hasAttribute.Should().BeTrue($"{className} should map \"{jsonName}\"");
    }

    [Fact]
    public void Control_Is_EssentialsControlPropertiesConfig()
    {
        var control = AssemblyFixture.GetPluginType("TriplePlayIptvConfig").GetProperty("Control")!;

        control.PropertyType.Name.Should().Be("EssentialsControlPropertiesConfig");
    }
}

using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer.Tests;

public class FactoryDiscoveryTests
{
    [Fact]
    public void Assembly_Loads_Successfully()
    {
        AssemblyFixture.PluginAssembly.Should().NotBeNull();
    }

    [Fact]
    public void Assembly_Name_Matches_Expected()
    {
        AssemblyFixture.PluginAssembly.GetName().Name.Should().Be("epi-tripleplay-iptvserver.4Series");
    }

    [Fact]
    public void Factory_Count_Matches_Expected()
    {
        AssemblyFixture.FindFactoryTypes().Should().HaveCount(1);
    }

    [Fact]
    public void Factory_Exists_ByName()
    {
        AssemblyFixture.FindFactoryTypes().Select(t => t.Name).Should().Contain("TriplePlayIptvFactory");
    }

    [Fact]
    public void All_Factories_Have_Parameterless_Constructor()
    {
        foreach (var factory in AssemblyFixture.FindFactoryTypes())
            factory.GetConstructor(Type.EmptyTypes).Should().NotBeNull($"{factory.Name} needs one for plugin discovery");
    }

    [Fact]
    public void Factory_Builds_TriplePlayIptvDevice()
    {
        var factory = AssemblyFixture.GetPluginType("TriplePlayIptvFactory");
        var deviceType = factory.BaseType!.GetGenericArguments().Single();

        deviceType.Name.Should().Be("TriplePlayIptvDevice");
    }

    // Mobile Control creates a messenger for each of these interfaces; the React set-top box page relies on them.
    [Theory]
    [InlineData("ISetTopBoxControls")]
    [InlineData("IChannel")]
    [InlineData("IColor")]
    [InlineData("IDPad")]
    [InlineData("INumericKeypad")]
    [InlineData("ISetTopBoxNumericKeypad")]
    [InlineData("ITransport")]
    [InlineData("IUiDisplayInfo")]
    [InlineData("ITvPresetsProvider")]
    [InlineData("IRoutingSource")]
    [InlineData("IRoutingOutputs")]
    [InlineData("IBridgeAdvanced")]
    public void Device_Implements_Interface(string interfaceName)
    {
        var device = AssemblyFixture.GetPluginType("TriplePlayIptvDevice");

        device.GetInterfaces().Select(i => i.Name).Should().Contain(interfaceName);
    }
}

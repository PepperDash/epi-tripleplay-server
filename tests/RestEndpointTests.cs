using System.Text;
using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer.Tests;

public class RestEndpointTests
{
    [Theory]
    [InlineData(false, "10.1.2.3", 80, "http://10.1.2.3/")]   // Uri drops the scheme's default port
    [InlineData(false, "10.1.2.3", 8080, "http://10.1.2.3:8080/")]
    [InlineData(true, "10.1.2.3", 8443, "https://10.1.2.3:8443/")]
    [InlineData(true, "tripleplay.local", 443, "https://tripleplay.local/")]
    public void Builds_Base_Uri_With_Port(bool https, string address, int port, string expected)
    {
        RestEndpoint.BaseUri(https, address, port).AbsoluteUri.Should().Be(expected);
    }

    [Theory]
    [InlineData(false, 0, 80)]
    [InlineData(false, 65536, 80)]
    [InlineData(true, 0, 443)]     // the old HTTPS client used http:// and port 80 here
    [InlineData(true, -1, 443)]
    public void Uses_Scheme_Default_Port_When_Port_Is_Out_Of_Range(bool https, int port, int expectedPort)
    {
        var uri = RestEndpoint.BaseUri(https, "10.1.2.3", port);

        uri.Scheme.Should().Be(https ? "https" : "http");
        uri.Port.Should().Be(expectedPort);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void No_Authorization_Without_Username(string? username)
    {
        RestEndpoint.BasicAuthorization(username!, "password").Should().BeNull();
    }

    [Fact]
    public void Basic_Authorization_Encodes_Credentials()
    {
        var header = RestEndpoint.BasicAuthorization("admin", "password")!;

        header.Scheme.Should().Be("Basic");
        Encoding.Latin1.GetString(Convert.FromBase64String(header.Parameter!)).Should().Be("admin:password");
    }

    [Fact]
    public void Basic_Authorization_Treats_Null_Password_As_Empty()
    {
        var header = RestEndpoint.BasicAuthorization("admin", null!)!;

        Encoding.Latin1.GetString(Convert.FromBase64String(header.Parameter!)).Should().Be("admin:");
    }

    [Fact]
    public void Basic_Authorization_Uses_Latin1()
    {
        var header = RestEndpoint.BasicAuthorization("ädmin", "pässword")!;

        Convert.FromBase64String(header.Parameter!).Should().Equal(Encoding.Latin1.GetBytes("ädmin:pässword"));
    }
}

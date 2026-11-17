using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer.Tests;

/// <summary>
/// Deserializes the sample server responses in documents/
/// </summary>
public class ResponseObjectsTests
{
    private static string Sample(string fileName) =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Samples", fileName));

    [Fact]
    public void GetAllServices_Result_Deserializes()
    {
        var response = JsonConvert.DeserializeObject<ServicesResponse>(Sample("TriplePlay.GetAllServices.Result.json"))!;

        response.Jsonrpc.Should().Be("2.0");
        response.Results.Should().NotBeEmpty();

        var first = response.Results[0];
        first.Id.Should().Be(1);
        first.ChannelNumber.Should().Be(1);
        first.Name.Should().Be("CP24");
        first.IconPath.Should().Be("/portalImages/services/1_v2.png");
        first.IsFavourite.Should().BeFalse();
        first.IsWatchable.Should().BeTrue();
        first.TypeSpecificData.IpAddress.Should().Be("239.0.8.2");
        first.TypeSpecificData.Port.Should().Be(2802);
    }

    [Fact]
    public void Error_Response_Deserializes()
    {
        var response = JsonConvert.DeserializeObject<ErrorResponse>(Sample("TriplePlay.Error.Response.json"))!;

        response.Error.Code.Should().Be(-32601);
        response.Error.Message.Should().StartWith("server error. method not found.");
    }

    [Fact]
    public void Acknowledge_Response_Deserializes()
    {
        var response = JsonConvert.DeserializeObject<AcknowledgementResponse>(Sample("TriplePlay.Acknowledge.Response.json"))!;

        response.Result.Should().BeTrue();
    }
}

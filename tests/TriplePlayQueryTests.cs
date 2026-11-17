using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer.Tests;

public class TriplePlayQueryTests
{
    [Theory]
    [InlineData(0, "GetAllServices")]
    [InlineData(-1, "GetAllServices")]
    [InlineData(3, "")]
    [InlineData(3, null)]
    public void Returns_Null_For_Invalid_StbId_Or_Method(int stbId, string? method)
    {
        TriplePlayQuery.Build(stbId, method!, null!, null, null).Should().BeNull();
        TriplePlayQuery.BuildCall(stbId, method!, null!, null, null).Should().BeNull();
    }

    [Fact]
    public void Builds_Call_With_StbId_Only()
    {
        TriplePlayQuery.BuildCall(5, "GetAllServices", null!, null, null)
            .Should().Be(@"{""jsonrpc"":""2.0"",""method"":""GetAllServices"",""params"":[5]}");
    }

    [Fact]
    public void Builds_Call_With_Int_Param()
    {
        TriplePlayQuery.BuildCall(3, "SetVolume", null!, 15, null)
            .Should().Be(@"{""jsonrpc"":""2.0"",""method"":""SetVolume"",""params"":[3,15]}");
    }

    [Fact]
    public void Builds_Call_With_String_Param()
    {
        TriplePlayQuery.BuildCall(3, "HandleKeyPress", "ChannelUp", null, null)
            .Should().Be(@"{""jsonrpc"":""2.0"",""method"":""HandleKeyPress"",""params"":[3,""ChannelUp""]}");
    }

    [Fact]
    public void Builds_Call_With_Bool_Param()
    {
        TriplePlayQuery.BuildCall(3, "SetMute", null!, null, true)
            .Should().Be(@"{""jsonrpc"":""2.0"",""method"":""SetMute"",""params"":[3,true]}");
    }

    [Fact]
    public void Orders_Params_String_Then_Int_Then_Bool()
    {
        TriplePlayQuery.BuildCall(3, "M", "s", 7, false)
            .Should().Be(@"{""jsonrpc"":""2.0"",""method"":""M"",""params"":[3,""s"",7,false]}");
    }

    [Fact]
    public void Leaves_Out_Empty_String_Param()
    {
        TriplePlayQuery.BuildCall(3, "M", "", null, null)
            .Should().Be(@"{""jsonrpc"":""2.0"",""method"":""M"",""params"":[3]}");
    }

    [Fact]
    public void Query_Is_Url_Encoded_Call()
    {
        var query = TriplePlayQuery.Build(3, "SetVolume", null!, 15, null);

        query.Should().Be("call=%7B%22jsonrpc%22%3A%222.0%22%2C%22method%22%3A%22SetVolume%22%2C%22params%22%3A%5B3%2C15%5D%7D");
    }

    [Theory]
    // Requests from documents/TriplePlay.postman_collection.json, which encodes only the quotes
    [InlineData(3, "GetPowerStatus", null, "{%22jsonrpc%22:%222.0%22,%22method%22:%22GetPowerStatus%22,%22params%22:[3]}")]
    [InlineData(3, "GetAudioState", null, "{%22jsonrpc%22:%222.0%22,%22method%22:%22GetAudioState%22,%22params%22:[3]}")]
    [InlineData(3, "SetVolume", 15, "{%22jsonrpc%22:%222.0%22,%22method%22:%22SetVolume%22,%22params%22:[3,15]}")]
    public void Query_Decodes_To_Documented_Request(int stbId, string method, int? intParam, string documentedCall)
    {
        var query = TriplePlayQuery.Build(stbId, method, null!, intParam, null)!;

        Uri.UnescapeDataString(query["call=".Length..]).Should().Be(Uri.UnescapeDataString(documentedCall));
    }
}

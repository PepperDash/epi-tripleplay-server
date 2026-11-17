using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer.Tests;

public class MonitorTimesTests
{
    [Fact]
    public void Defaults_Are_30_180_300_Seconds()
    {
        MonitorTimes.Defaults.PollTimeMs.Should().Be(30000);
        MonitorTimes.Defaults.WarningTimeMs.Should().Be(180000);
        MonitorTimes.Defaults.ErrorTimeMs.Should().Be(300000);
    }

    [Fact]
    public void Uses_Configured_Times()
    {
        var times = MonitorTimes.Resolve(10000, 60000, 120000);

        times.PollTimeMs.Should().Be(10000);
        times.WarningTimeMs.Should().Be(60000);
        times.ErrorTimeMs.Should().Be(120000);
    }

    [Fact]
    public void Unset_Config_Uses_Defaults()
    {
        MonitorTimes.Resolve(0, 0, 0).Should().BeEquivalentTo(MonitorTimes.Defaults);
    }

    [Fact]
    public void Fills_Each_Unset_Time_With_Its_Default()
    {
        var times = MonitorTimes.Resolve(0, 60000, -1);

        times.PollTimeMs.Should().Be(MonitorTimes.DefaultPollTimeMs);
        times.WarningTimeMs.Should().Be(60000);
        times.ErrorTimeMs.Should().Be(MonitorTimes.DefaultErrorTimeMs);
    }

    [Theory]
    [InlineData(1000, 4999, 60000)]     // warning under the 5 s StatusMonitorBase minimum
    [InlineData(1000, 4000, 4999)]      // error under the minimum
    [InlineData(10000, 120000, 60000)]  // warning after error: StatusMonitorBase throws
    [InlineData(60000, 60000, 120000)]  // poll not shorter than warning: warns between polls
    [InlineData(200000, 0, 0)]          // poll longer than the default warning
    public void Unworkable_Config_Uses_All_Defaults(long poll, long warning, long error)
    {
        MonitorTimes.Resolve(poll, warning, error).Should().BeSameAs(MonitorTimes.Defaults);
    }

    [Fact]
    public void Accepts_Warning_Equal_To_Error()
    {
        var times = MonitorTimes.Resolve(10000, 60000, 60000);

        times.WarningTimeMs.Should().Be(60000);
        times.ErrorTimeMs.Should().Be(60000);
    }
}

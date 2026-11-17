using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer.Tests;

public class ScalingTests
{
    // Volume joins scale 0-65535 to the TriplePlay 0-100 range: ScaleWithLimits(level, 65535, 0, 100, 0)
    [Theory]
    [InlineData(0, 0)]
    [InlineData(655, 0)]       // 0.999 truncates to 0
    [InlineData(656, 1)]
    [InlineData(32767, 49)]
    [InlineData(32768, 50)]
    [InlineData(65534, 99)]
    [InlineData(65535, 100)]
    public void Scales_Volume_Range(int level, int expected)
    {
        Scaling.ScaleWithLimits(level, 65535, 0, 100, 0).Should().Be(expected);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(int.MinValue, 0)]
    [InlineData(70000, 100)]
    [InlineData(int.MaxValue, 100)]
    public void Clamps_At_Input_Limits(int level, int expected)
    {
        Scaling.ScaleWithLimits(level, 65535, 0, 100, 0).Should().Be(expected);
    }

    [Fact]
    public void Does_Not_Overflow_On_Large_Ranges()
    {
        // (value - inMin) * (outMax - outMin) exceeds int here; the helper multiplies in long
        Scaling.ScaleWithLimits(60000, 65535, 0, 65535, 0).Should().Be(60000);
    }

    [Fact]
    public void Honours_Non_Zero_Minimums()
    {
        Scaling.ScaleWithLimits(150, 200, 100, 20, 10).Should().Be(15);
    }
}

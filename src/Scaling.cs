namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Replaces CrestronEnvironment.ScaleWithLimits so the scaling can be unit tested
    /// </summary>
    public static class Scaling
    {
        /// <summary>
        /// Scales a value from the input range to the output range, clamping at the input limits.
        /// Same argument order and integer math as CrestronEnvironment.ScaleWithLimits(int, int, int, int, int).
        /// </summary>
        public static int ScaleWithLimits(int value, int inMax, int inMin, int outMax, int outMin)
        {
            if (value <= inMin) return outMin;
            if (value >= inMax) return outMax;
            return (int)(outMin + (long)(value - inMin) * (outMax - outMin) / (inMax - inMin));
        }
    }
}

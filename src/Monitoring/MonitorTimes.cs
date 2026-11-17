namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Poll, warning and error times for the communication monitor, taken from config with defaults
    /// </summary>
    public sealed class MonitorTimes
    {
        /// <summary>
        /// Default poll interval
        /// </summary>
        public const long DefaultPollTimeMs = 30000;

        /// <summary>
        /// Default time without a response before the monitor reports a warning
        /// </summary>
        public const long DefaultWarningTimeMs = 180000;

        /// <summary>
        /// Default time without a response before the monitor reports an error
        /// </summary>
        public const long DefaultErrorTimeMs = 300000;

        /// <summary>
        /// Shortest warning or error time Essentials' StatusMonitorBase accepts
        /// </summary>
        public const long MinimumTimeoutMs = 5000;

        private MonitorTimes(long pollTimeMs, long warningTimeMs, long errorTimeMs)
        {
            PollTimeMs = pollTimeMs;
            WarningTimeMs = warningTimeMs;
            ErrorTimeMs = errorTimeMs;
        }

        /// <summary>
        /// The defaults: poll every 30 s, warning after 180 s and error after 300 s without a response
        /// </summary>
        public static readonly MonitorTimes Defaults =
            new MonitorTimes(DefaultPollTimeMs, DefaultWarningTimeMs, DefaultErrorTimeMs);

        /// <summary>
        /// Poll interval in milliseconds
        /// </summary>
        public long PollTimeMs { get; private set; }

        /// <summary>
        /// Time without a response before a warning, in milliseconds
        /// </summary>
        public long WarningTimeMs { get; private set; }

        /// <summary>
        /// Time without a response before an error, in milliseconds
        /// </summary>
        public long ErrorTimeMs { get; private set; }

        /// <summary>
        /// Uses the configured times, replacing any that are not set (zero or less) with its default.
        /// If the result cannot work — a timeout under 5 s, a warning after the error, or a poll interval
        /// not shorter than the warning time — all three defaults are used instead.
        /// </summary>
        public static MonitorTimes Resolve(long pollTimeMs, long warningTimeMs, long errorTimeMs)
        {
            var poll = pollTimeMs > 0 ? pollTimeMs : DefaultPollTimeMs;
            var warning = warningTimeMs > 0 ? warningTimeMs : DefaultWarningTimeMs;
            var error = errorTimeMs > 0 ? errorTimeMs : DefaultErrorTimeMs;

            if (warning < MinimumTimeoutMs || error < MinimumTimeoutMs || warning > error || poll >= warning)
                return Defaults;

            return new MonitorTimes(poll, warning, error);
        }
    }
}

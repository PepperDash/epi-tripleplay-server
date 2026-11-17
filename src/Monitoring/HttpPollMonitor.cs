using System;
using System.Threading;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Polls the server on a timer. Any HTTP response marks it online; with no response it moves to
    /// warning, then error, after the configured times. Follows Essentials' GenericCommunicationMonitor,
    /// which needs a socket or serial client and so can't be used over HTTP.
    /// </summary>
    public sealed class HttpPollMonitor : StatusMonitorBase
    {
        private readonly long _pollTimeMs;
        private readonly Action _poll;
        private readonly object _timerLock = new object();
        private Timer _pollTimer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">monitored device</param>
        /// <param name="times">poll, warning and error times</param>
        /// <param name="poll">sends the poll request</param>
        public HttpPollMonitor(IKeyed parent, MonitorTimes times, Action poll)
            : base(parent, times.WarningTimeMs, times.ErrorTimeMs)
        {
            _pollTimeMs = times.PollTimeMs;
            _poll = poll ?? throw new ArgumentNullException(nameof(poll));
        }

        /// <summary>
        /// Starts polling, with the first poll straight away
        /// </summary>
        public override void Start()
        {
            lock (_timerLock)
            {
                if (_pollTimer != null) return;
                _pollTimer = new Timer(_ => Poll(), null, 0, _pollTimeMs);
            }
        }

        /// <summary>
        /// Stops polling and the warning and error timers
        /// </summary>
        public override void Stop()
        {
            lock (_timerLock)
            {
                if (_pollTimer == null) return;
                _pollTimer.Dispose();
                _pollTimer = null;
            }

            StopErrorTimers();
        }

        /// <summary>
        /// Call for every HTTP response, whatever its status code: an answer means the server is reachable
        /// </summary>
        public void ResponseReceived()
        {
            Status = MonitorStatus.IsOk;
            ResetErrorTimers();
        }

        private void Poll()
        {
            StartErrorTimers();

            try
            {
                _poll();
            }
            catch
            {
                // Nothing may escape a timer thread. A poll that can't be sent gets no response,
                // and the warning and error timers report that.
            }
        }
    }
}

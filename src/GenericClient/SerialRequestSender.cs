using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Sends HTTP requests one at a time, in the order they were queued, from a dedicated background thread
    /// </summary>
    public sealed class SerialRequestSender : IDisposable
    {
        private readonly HttpClient _client;
        private readonly BlockingCollection<HttpRequestMessage> _requests;

        // Held in a field so the worker thread is not collected.
        private readonly Thread _worker;

        /// <summary>
        /// Starts the worker thread
        /// </summary>
        /// <param name="client">client to send with; its BaseAddress resolves relative request URIs</param>
        /// <param name="capacity">maximum queued requests; Queue blocks while the queue is full</param>
        /// <param name="name">worker thread name</param>
        public SerialRequestSender(HttpClient client, int capacity, string name)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _requests = new BlockingCollection<HttpRequestMessage>(capacity);

            _worker = new Thread(ProcessRequests) { IsBackground = true, Name = name };
            _worker.Start();
        }

        /// <summary>
        /// Raised on the worker thread for every response, including HTTP error statuses
        /// </summary>
        public event EventHandler<GenericClientResponseEventArgs> ResponseReceived;

        /// <summary>
        /// Raised on the worker thread when a request fails without a response (connection error, timeout)
        /// or a ResponseReceived handler throws
        /// </summary>
        public event EventHandler<RequestFailedEventArgs> RequestFailed;

        /// <summary>
        /// Queues a request. Blocks while the queue is full, like the CrestronQueue this replaces.
        /// </summary>
        /// <returns>false when the sender has been disposed; the caller still owns the request</returns>
        public bool Queue(HttpRequestMessage request)
        {
            try
            {
                _requests.Add(request);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// Stops taking requests; the worker finishes the queued ones and exits
        /// </summary>
        public void Dispose()
        {
            _requests.CompleteAdding();
        }

        // Blocks waiting for each response, which keeps requests in order. This is a dedicated background
        // thread, so it holds no pool thread and can't keep the program open.
        private void ProcessRequests()
        {
            foreach (var request in _requests.GetConsumingEnumerable())
            {
                using (request)
                {
                    try
                    {
                        using (var response = _client.SendAsync(request).GetAwaiter().GetResult())
                        {
                            var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            var handler = ResponseReceived;
                            if (handler != null)
                                handler(this, new GenericClientResponseEventArgs((int)response.StatusCode, body));
                        }
                    }
                    catch (Exception ex)
                    {
                        OnRequestFailed(request, ex);
                    }
                }
            }
        }

        // Nothing may escape the worker thread: an unhandled exception there ends the program.
        private void OnRequestFailed(HttpRequestMessage request, Exception ex)
        {
            try
            {
                var handler = RequestFailed;
                if (handler != null)
                    handler(this, new RequestFailedEventArgs(request.RequestUri, ex));
            }
            catch
            {
                // A failing failure handler has nowhere left to report to.
            }
        }
    }

    /// <summary>
    /// Request failure details
    /// </summary>
    public class RequestFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RequestFailedEventArgs(Uri requestUri, Exception exception)
        {
            RequestUri = requestUri;
            Exception = exception;
        }

        /// <summary>
        /// URI of the failed request; absolute once HttpClient has resolved it against BaseAddress
        /// </summary>
        public Uri RequestUri { get; private set; }

        /// <summary>
        /// The failure
        /// </summary>
        public Exception Exception { get; private set; }
    }
}

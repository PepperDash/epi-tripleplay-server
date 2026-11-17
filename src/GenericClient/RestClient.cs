using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// HTTP or HTTPS client for the TriplePlay server. Requests are sent one at a time, in the order they were queued.
    /// </summary>
    public sealed class RestClient : IRestfulComms, IDisposable
    {
        private const string DefaultRequestType = "GET";

        // Same capacity as the CrestronQueue this replaces. Add blocks while the queue is full.
        private const int MaxQueuedRequests = 20;

        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

        private readonly HttpClient _client;
        private readonly BlockingCollection<HttpRequestMessage> _requests =
            new BlockingCollection<HttpRequestMessage>(MaxQueuedRequests);

        // Held in a field so the worker thread is not collected.
        private readonly Thread _worker;

        /// <summary>
        /// Creates a client from the device's control properties
        /// </summary>
        /// <param name="key">client key</param>
        /// <param name="controlConfig">control properties; method must be http or https</param>
        public RestClient(string key, ControlPropertiesConfig controlConfig)
            : this(key, controlConfig, null)
        {
        }

        /// <summary>
        /// Creates a client that sends through the given message handler (used by tests)
        /// </summary>
        /// <param name="key">client key</param>
        /// <param name="controlConfig">control properties; method must be http or https</param>
        /// <param name="handler">message handler, or null to use the default handler</param>
        public RestClient(string key, ControlPropertiesConfig controlConfig, HttpMessageHandler handler)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (controlConfig == null || controlConfig.TcpSshProperties == null)
                throw new ArgumentNullException(nameof(controlConfig));

            Key = key;

            var tcp = controlConfig.TcpSshProperties;
            var isHttps = controlConfig.Method == eControlMethod.Https;
            var scheme = isHttps ? "https" : "http";

            BaseUri = tcp.Port >= 1 && tcp.Port <= 65535
                ? new Uri(string.Format("{0}://{1}:{2}/", scheme, tcp.Address, tcp.Port))
                : new Uri(string.Format("{0}://{1}/", scheme, tcp.Address));

            if (handler == null)
            {
                var clientHandler = new HttpClientHandler();
                // The Crestron HttpsClient this replaces ran with host and peer verification off.
                if (isHttps)
                    clientHandler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                handler = clientHandler;
            }

            _client = new HttpClient(handler) { BaseAddress = BaseUri, Timeout = RequestTimeout };

            if (!string.IsNullOrEmpty(tcp.Username))
            {
                var credentials = Encoding.Latin1.GetBytes(string.Format("{0}:{1}", tcp.Username, tcp.Password ?? ""));
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
            }

            this.LogVerbose("Client created for {BaseUri}", BaseUri);

            _worker = new Thread(ProcessRequests) { IsBackground = true, Name = Key + "-requests" };
            _worker.Start();
        }

        /// <summary>
        /// Base URI requests are sent to
        /// </summary>
        public Uri BaseUri { get; private set; }

        #region IRestfulComms Members

        /// <summary>
        /// Implements IKeyed interface
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Client response event
        /// </summary>
        public event EventHandler<GenericClientResponseEventArgs> ResponseReceived;

        /// <summary>
        /// Queues a GET request to the client
        /// </summary>
        /// <param name="request">path and query, relative to the base URI</param>
        /// <param name="contentString">request body</param>
        public void SendRequest(string request, string contentString)
        {
            SendRequest(DefaultRequestType, request, contentString);
        }

        /// <summary>
        /// Queues a request to the client
        /// </summary>
        /// <param name="requestType">HTTP method</param>
        /// <param name="path">path and query, relative to the base URI</param>
        /// <param name="content">request body; sent as application/json when not empty</param>
        public void SendRequest(string requestType, string path, string content)
        {
            if (string.IsNullOrEmpty(path))
            {
                this.LogError("SendRequest: request path is null or empty");
                return;
            }

            var method = new HttpMethod(string.IsNullOrEmpty(requestType)
                ? DefaultRequestType
                : requestType.ToUpperInvariant());

            var request = new HttpRequestMessage(method, path.TrimStart('/'));
            if (!string.IsNullOrEmpty(content))
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            this.LogDebug("Request {Method} {Url} {Content}", method, new Uri(BaseUri, request.RequestUri), content);

            try
            {
                _requests.Add(request);
            }
            catch (InvalidOperationException)
            {
                // The client has been disposed.
                request.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// Stops the request worker and releases the HTTP client
        /// </summary>
        public void Dispose()
        {
            _requests.CompleteAdding();
            _client.Dispose();
        }

        // Runs on the worker thread. It blocks waiting for each response, which keeps requests in order;
        // it is a dedicated background thread, so it holds no pool thread and can't keep the program open.
        private void ProcessRequests()
        {
            foreach (var request in _requests.GetConsumingEnumerable())
            {
                try
                {
                    using (request)
                    using (var response = _client.SendAsync(request).GetAwaiter().GetResult())
                    {
                        var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        OnResponseReceived(new GenericClientResponseEventArgs((int)response.StatusCode, body));
                    }
                }
                catch (Exception ex)
                {
                    // Nothing may escape: an unhandled exception on this thread ends the program.
                    this.LogDebug(ex, "Request {Url} failed: {Message}", request.RequestUri, ex.Message);
                }
            }
        }

        private void OnResponseReceived(GenericClientResponseEventArgs args)
        {
            var handler = ResponseReceived;
            if (handler != null)
                handler(this, args);
        }
    }
}

using System;
using System.Net.Http;
using System.Text;
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

        // Same capacity as the CrestronQueue this replaces.
        private const int MaxQueuedRequests = 20;

        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

        private readonly HttpClient _client;
        private readonly SerialRequestSender _sender;

        /// <summary>
        /// Creates a client from the device's control properties
        /// </summary>
        /// <param name="key">client key</param>
        /// <param name="controlConfig">control properties; method must be http or https</param>
        public RestClient(string key, ControlPropertiesConfig controlConfig)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (controlConfig == null || controlConfig.TcpSshProperties == null)
                throw new ArgumentNullException(nameof(controlConfig));

            Key = key;

            var tcp = controlConfig.TcpSshProperties;
            var isHttps = controlConfig.Method == eControlMethod.Https;
            BaseUri = RestEndpoint.BaseUri(isHttps, tcp.Address, tcp.Port);

            var handler = new HttpClientHandler();
            // The Crestron HttpsClient this replaces ran with host and peer verification off.
            if (isHttps)
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            _client = new HttpClient(handler) { BaseAddress = BaseUri, Timeout = RequestTimeout };
            _client.DefaultRequestHeaders.Authorization = RestEndpoint.BasicAuthorization(tcp.Username, tcp.Password);

            _sender = new SerialRequestSender(_client, MaxQueuedRequests, Key + "-requests");
            _sender.ResponseReceived += (sender, args) =>
            {
                var responseHandler = ResponseReceived;
                if (responseHandler != null)
                    responseHandler(this, args);
            };
            _sender.RequestFailed += (sender, args) =>
                this.LogDebug(args.Exception, "Request {Url} failed: {Message}", args.RequestUri, args.Exception.Message);

            this.LogVerbose("Client created for {BaseUri}", BaseUri);
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

            if (!_sender.Queue(request))
                request.Dispose();
        }

        #endregion

        /// <summary>
        /// Stops the request worker and releases the HTTP client
        /// </summary>
        public void Dispose()
        {
            _sender.Dispose();
            _client.Dispose();
        }
    }
}

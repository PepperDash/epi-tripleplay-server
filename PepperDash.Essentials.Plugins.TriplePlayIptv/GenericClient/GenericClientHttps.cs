using System;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net;
using Crestron.SimplSharp.Net.Http;
using Crestron.SimplSharp.Net.Https;
using PepperDash.Core;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Http client
    /// </summary>
    public class GenericClientHttps : IRestfulComms
    {
        private const string DefaultRequestType = "GET";
        private readonly HttpsClient _clientHttps;


        private readonly CrestronQueue<Action> _requestQueue = new CrestronQueue<Action>(20);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="controlConfig"></param>
        public GenericClientHttps(string key, ControlPropertiesConfig controlConfig)
        {
            if (string.IsNullOrEmpty(key) || controlConfig == null)
            {
                Debug.Console(0, Debug.ErrorLogLevel.Error,
                    "GenericClient key or host is null or empty, failed to instantiate client");
                return;
            }

            Key = string.Format("{0}-{1}-client", key, controlConfig.Method).ToLower();

            Host = (controlConfig.TcpSshProperties.Port >= 1 && controlConfig.TcpSshProperties.Port <= 65535)
                ? String.Format("http://{0}:{1}", controlConfig.TcpSshProperties.Address,
                    controlConfig.TcpSshProperties.Port)
                : String.Format("http://{0}", controlConfig.TcpSshProperties.Address);
            Port = (controlConfig.TcpSshProperties.Port >= 1 && controlConfig.TcpSshProperties.Port <= 65535)
                ? controlConfig.TcpSshProperties.Port
                : 80;
            Username = controlConfig.TcpSshProperties.Username ?? "";
            Password = controlConfig.TcpSshProperties.Password ?? "";
            AuthorizationBase64 = EncodeBase64(Username, Password);

            Debug.Console(0, this, "{0}", new String('-', 80));
            Debug.Console(0, this, "GenericClient: Key = {0}", Key);
            Debug.Console(0, this, "GenericClient: Host = {0}", Host);
            Debug.Console(0, this, "GenericClient: Port = {0}", Port);
            Debug.Console(0, this, "GenericClient: Username = {0}", Username);
            Debug.Console(0, this, "GenericClient: Password = {0}", Password);
            Debug.Console(0, this, "GenericClient: AuthorizationBase64 = {0}", AuthorizationBase64);


            _clientHttps = new HttpsClient
            {
                UserName = Username,
                Password = Password,
                KeepAlive = false,
                HostVerification = false,
                PeerVerification = false
            };


            //_requestHttps.Header.ContentType = "application/json";


            Debug.Console(0, this, "{0}", new String('-', 80));
        }

        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string AuthorizationBase64 { get; set; }

        #region IRestfulComms Members

        /// <summary>
        /// Implements IKeyed interface
        /// </summary>
        public string Key { get; private set; }

        public void SendRequest(string requestType, string path, string content)
        {
            var request = new HttpsClientRequest
            {
                RequestType =
                    (Crestron.SimplSharp.Net.Https.RequestType)
                        Enum.Parse(typeof (Crestron.SimplSharp.Net.Https.RequestType), requestType, true),
                Url = new UrlParser(String.Format("{0}/{1}", Host, path)),
                ContentString = content.Replace("\"", "%22")
            };

            request.Header.SetHeaderValue("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(AuthorizationBase64))
            {
                _clientHttps.AuthenticationMethod = AuthMethod.BASIC;
                request.Header.SetHeaderValue("Authorization", AuthorizationBase64);
            }

            Debug.Console(1, this, @"Request:
url: {0}
content: {1}
requestType: {2}", request.Url, request.ContentString, request.RequestType);

            if (_clientHttps.ProcessBusy)
            {
                _requestQueue.Enqueue(() => _clientHttps.DispatchAsync(request, (response, error) =>
                {
                    if (response == null)
                    {
                        Debug.Console(0, this, "DispatchRequest: response is null, error: {0}", error);
                        return;
                    }

                    OnResponseRecieved(new GenericClientResponseEventArgs(response.Code, response.ContentString));
                }));
            }
        }

        /// <summary>
        /// Client response event
        /// </summary>
        public event EventHandler<GenericClientResponseEventArgs> ResponseReceived;

        /// <summary>
        /// Sends OR queues a request to the client
        /// </summary>
        /// <param name="request"></param>
        /// <param name="contentString"></param>
        public void SendRequest(string request, string contentString)
        {
            if (string.IsNullOrEmpty(request))
            {
                Debug.Console(0, this, Debug.ErrorLogLevel.Error, "SendRequest: request is null or empty");
                return;
            }


            SendRequest(DefaultRequestType, request, contentString);
        }

        #endregion

        // dispatches requests to the client
        /*private void DispatchHttpsRequest(string request, string contentString, Crestron.SimplSharp.Net.Https.RequestType requestType)
        {
            if (string.IsNullOrEmpty(request))
            {
                Debug.Console(0, this, "DispatchHttpRequest: request is null or empty, cannot dispatch request");
                return;
            }

            try
            {
                _requestHttps.Url.Parse(request);
                _requestHttps.Url.Parse(string.Format("{0}?{1}", request, contentString));
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url = {0}", _requestHttps.Url);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.Fragment = {0}", _requestHttps.Url.Fragment);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.Hostname = {0}", _requestHttps.Url.Hostname);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.HostnameAndPort = {0}", _requestHttps.Url.HostnameAndPort);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.HostnameType = {0}", _requestHttps.Url.HostnameType);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.IsAbsoluteUri = {0}", _requestHttps.Url.IsAbsoluteUri);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.IsDefaultPort = {0}", _requestHttps.Url.IsDefaultPort);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.IsFile = {0}", _requestHttps.Url.IsFile);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.IsLoopback = {0}", _requestHttps.Url.IsLoopback);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.IsUnc = {0}", _requestHttps.Url.IsUnc);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.Params = {0}", _requestHttps.Url.Params);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.Path = {0}", _requestHttps.Url.Path);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.PathAndParams = {0}", _requestHttps.Url.PathAndParams);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.Port = {0}", _requestHttps.Url.Port);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.Protocol = {0}", _requestHttps.Url.Protocol);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url.Url = {0}", _requestHttps.Url.Url);

                _requestHttps.RequestType = requestType;

               _dispatchHttpsError = _clientHttps.DispatchAsync(_requestHttps, (response, error) =>
                {
                    if (response == null)
                    {
                        Debug.Console(0, this, "DispatchRequest: response is null, error: {0}", error);
                        return;
                    }

                    OnResponseRecieved(new GenericClientResponseEventArgs(response.Code, response.ContentString));
                });

                Debug.Console(0, this, "DispatchHttpsRequest: _dispatchHttpError '{0}'", _dispatchHttpsError);
            }
            catch (Exception ex)
            {
                Debug.Console(0, this, Debug.ErrorLogLevel.Error, "DispatchHttpsRequest Exception: {0}", ex);
            }
        }*/

        // client response event handler
        private void OnResponseRecieved(GenericClientResponseEventArgs args)
        {
            Debug.Console(0, this, "OnResponseReceived: args.Code = {0} | args.ContentString = {1}", args.Code,
                args.ContentString);

            CheckRequestQueue();

            var handler = ResponseReceived;
            if (handler == null)
            {
                return;
            }

            handler(this, args);
        }

        // Checks request queue and issues next request
        private void CheckRequestQueue()
        {
            Debug.Console(0, this, "CheckRequestQueue: _requestQueue.Count = {0}", _requestQueue.Count);
            var nextRequest = _requestQueue.TryToDequeue();
            Debug.Console(0, this, "CheckRequestQueue: _requestQueue.TryToDequeue was {0}",
                (nextRequest == null) ? "unsuccessful" : "successful");
            if (nextRequest != null)
            {
                nextRequest();
            }
        }

        // encodes username and password, returning a Base64 encoded string
        private string EncodeBase64(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                return "";
            }

            try
            {
                var base64String =
                    Convert.ToBase64String(
                        System.Text.Encoding.GetEncoding("ISO-8859-1")
                            .GetBytes(string.Format("{0}:{1}", username, password)));
                return string.Format("Basic {0}", base64String);
            }
            catch (Exception err)
            {
                Debug.Console(0, this, Debug.ErrorLogLevel.Error, "EncodeBase64 Exception:\r{0}", err);
                return "";
            }
        }
    }
}
using System;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Http;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Http client
    /// </summary>
    public class GenericClientHttp : IKeyed
    {
        /// <summary>
        /// Implements IKeyed interface
        /// </summary>
        public string Key { get; private set; }

        private eControlMethod Method { get; set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string AuthorizationBase64 { get; set; }

        private static HttpClient _clientHttp;
        private static HttpClientRequest _requestHttp;
        private static HttpClient.DISPATCHASYNC_ERROR _dispatchHttpError;

        private readonly CrestronQueue<Action> _requestQueue = new CrestronQueue<Action>(20);

        /// <summary>
        /// Client response event
        /// </summary>
        public event EventHandler<GenericClientResponseEventArgs> ResponseReceived;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="controlConfig"></param>
        public GenericClientHttp(string key, EssentialsControlPropertiesConfig controlConfig)
        {
            if (string.IsNullOrEmpty(key) || controlConfig == null)
            {
                Debug.Console(0, Debug.ErrorLogLevel.Error, "GenericClient key or host is null or empty, failed to instantiate client");
                return;
            }

            Key = string.Format("{0}-{1}-client", key, controlConfig.Method).ToLower();
            Method = controlConfig.Method;
            Host = controlConfig.TcpSshProperties.Address;
            Port = (controlConfig.TcpSshProperties.Port >= 1 && controlConfig.TcpSshProperties.Port <= 65535) ? controlConfig.TcpSshProperties.Port : 80;
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

            _clientHttp.HostAddress = Host;
            _clientHttp.Port = Port;
            _clientHttp.UserName = Username;
            _clientHttp.Password = Password;
            _clientHttp.KeepAlive = false;

            //_requestHttp.Header.ContentType = "application/json";
            _requestHttp.Header.SetHeaderValue("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(AuthorizationBase64))
            {
                _requestHttp.Header.SetHeaderValue("Authorization", AuthorizationBase64);
            }

            Debug.Console(0, this, "{0}", new String('-', 80));
        }

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

            char[] charsToTrim = { '/' };
            var url = request.StartsWith("http")
                ? string.Format("{0}", request.Trim(charsToTrim))
                : string.Format("{0}://{1}/{2}", Method.ToString().ToLower(), Host, request.Trim(charsToTrim));

            Debug.Console(0, this, "{0}", new String('-', 80));
            Debug.Console(0, this, "SendRequest: request = {0}", request);
            Debug.Console(0, this, "SendRequest: contentString = {0}", contentString);
            Debug.Console(0, this, "SendRequest: url = {0}", url);

            Debug.Console(0, this, "SendRequest: _clientHttp.ProcessBusy {0}", _clientHttp.ProcessBusy);
            if (_clientHttp.ProcessBusy)
                _requestQueue.Enqueue(() => DispatchHttpRequest(url, contentString, Crestron.SimplSharp.Net.Http.RequestType.Get));
            else
                DispatchHttpRequest(url, contentString, Crestron.SimplSharp.Net.Http.RequestType.Get);
            
            Debug.Console(0, this, "{0}", new String('-', 80));
        }

        // dispatches requests to the client
        private void DispatchHttpRequest(string request, string contentString, Crestron.SimplSharp.Net.Http.RequestType requestType)
        {
            Debug.Console(0, this, "{0}", new String('-', 80));
            Debug.Console(0, this, "DispatchHttpRequest: request = {0} | contentString = {1} | requestType = {2}", request, contentString, requestType.ToString());

            if (string.IsNullOrEmpty(request))
            {
                Debug.Console(0, this, "DispatchHttpRequest: request is null or empty, cannot dispatch request");
                return;
            }

            try
            {
                _requestHttp.Url.Parse(request);
                _requestHttp.Url.Parse(string.Format("{0}?{1}", request, contentString));
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url = {0}", _requestHttp.Url);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.Fragment = {0}", _requestHttp.Url.Fragment);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.Hostname = {0}", _requestHttp.Url.Hostname);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.HostnameAndPort = {0}", _requestHttp.Url.HostnameAndPort);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.HostnameType = {0}", _requestHttp.Url.HostnameType);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.IsAbsoluteUri = {0}", _requestHttp.Url.IsAbsoluteUri);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.IsDefaultPort = {0}", _requestHttp.Url.IsDefaultPort);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.IsFile = {0}", _requestHttp.Url.IsFile);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.IsLoopback = {0}", _requestHttp.Url.IsLoopback);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.IsUnc = {0}", _requestHttp.Url.IsUnc);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.Params = {0}", _requestHttp.Url.Params);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.Path = {0}", _requestHttp.Url.Path);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.PathAndParams = {0}", _requestHttp.Url.PathAndParams);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.Port = {0}", _requestHttp.Url.Port);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.Protocol = {0}", _requestHttp.Url.Protocol);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url.Url = {0}", _requestHttp.Url.Url);

                _requestHttp.RequestType = requestType;

                _dispatchHttpError = _clientHttp.DispatchAsync(_requestHttp, (response, error) =>
                {
                    if (response == null)
                    {
                        Debug.Console(0, this, "DispatchRequest: response is null, error: {0}", error);
                        return;
                    }

                    OnResponseRecieved(new GenericClientResponseEventArgs(response.Code, response.ContentString));
                });

                Debug.Console(0, this, "DispatchHttpsRequest: _dispatchHttpError '{0}'", _dispatchHttpError);
            }
            catch (Exception ex)
            {
                Debug.Console(0, this, Debug.ErrorLogLevel.Error, "DispatchHttpRequest Exception: {0}", ex);
            }
            Debug.Console(0, this, "{0}", new String('-', 80));
        }

        

        // client response event handler
        private void OnResponseRecieved(GenericClientResponseEventArgs args)
        {
            Debug.Console(0, this, "OnResponseReceived: args.Code = {0} | args.ContentString = {1}", args.Code, args.ContentString);

            CheckRequestQueue();

            var handler = ResponseReceived;
            if (handler == null) return;

            handler(this, args);
        }

        // Checks request queue and issues next request
        private void CheckRequestQueue()
        {
            Debug.Console(0, this, "CheckRequestQueue: _requestQueue.Count = {0}", _requestQueue.Count);
            var nextRequest = _requestQueue.TryToDequeue();
            Debug.Console(0, this, "CheckRequestQueue: _requestQueue.TryToDequeue was {0}", (nextRequest == null) ? "unsuccessful" : "successful");
            if (nextRequest != null) nextRequest();
        }

        // encodes username and password, returning a Base64 encoded string
        private string EncodeBase64(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                return "";

            try
            {
                var base64String = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(string.Format("{0}:{1}", username, password)));
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
using System;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net;
using Crestron.SimplSharp.Net.Http;
using Crestron.SimplSharp.Net.Https;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Http client
    /// </summary>
    public class GenericClient : IKeyed
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

        private static HttpsClient _clientHttps;
        private static HttpsClientRequest _requestHttps;
        private static HttpsClient.DISPATCHASYNC_ERROR _dispatchHttpsError;

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
        public GenericClient(string key, EssentialsControlPropertiesConfig controlConfig)
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

            switch (Method)
            {
                case eControlMethod.Http:
                    {
                        if (_clientHttp == null)
                            _clientHttp = new HttpClient();
                        if (_requestHttp == null)
                            _requestHttp = new HttpClientRequest();

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
                        
                        break;
                    }
                case eControlMethod.Https:
                    {
                        if (_clientHttps == null)
                            _clientHttps = new HttpsClient();
                        if (_requestHttps == null)
                            _requestHttps = new HttpsClientRequest();

                        _clientHttps.UserName = Username;
                        _clientHttps.Password = Password;
                        _clientHttps.KeepAlive = false;
                        _clientHttps.HostVerification = false;
                        _clientHttps.PeerVerification = false;

                        //_requestHttps.Header.ContentType = "application/json";
                        _requestHttps.Header.SetHeaderValue("Content-Type", "application/json");

                        if (!string.IsNullOrEmpty(AuthorizationBase64))
                        {
                            _clientHttps.AuthenticationMethod = AuthMethod.BASIC;
                            _requestHttps.Header.SetHeaderValue("Authorization", AuthorizationBase64);
                        }
                        break;
                    }
                default:
                    {
                        Debug.Console(0, this, "GenericClient: invalid eControlMethod '{0}'", Method);
                        break;
                    }
            }
        }

        /// <summary>
        /// Sends OR queues a request to the client
        /// </summary>
        /// <param name="request"></param>
        public void SendRequest(string request)
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

            Debug.Console(0, this, "SendRequest: url = {0}", url);

            switch (Method)
            {
                case eControlMethod.Http:
                    {
                        Debug.Console(0, this, "SendRequest: _clientHttp.ProcessBusy {0}", _clientHttp.ProcessBusy);
                        if (_clientHttp.ProcessBusy)
                            _requestQueue.Enqueue(() => DispatchHttpRequest(url, Crestron.SimplSharp.Net.Http.RequestType.Get));
                        else
                            DispatchHttpRequest(request, Crestron.SimplSharp.Net.Http.RequestType.Get);
                        break;
                    }
                case eControlMethod.Https:
                    {
                        Debug.Console(0, this, "SendRequest: _clientHttps.ProcessBusy {0}", _clientHttps.ProcessBusy);
                        if (_clientHttps.ProcessBusy)
                            _requestQueue.Enqueue(() => DispatchHttpsRequest(url, Crestron.SimplSharp.Net.Https.RequestType.Get));
                        else
                            DispatchHttpsRequest(request, Crestron.SimplSharp.Net.Https.RequestType.Get);
                        break;
                    }
                default:
                    {
                        Debug.Console(0, this, Debug.ErrorLogLevel.Error, "SendRequest: invalid method '{0}'", Method);
                        break;
                    }
            }
        }

        // dispatches requests to the client
        private void DispatchHttpRequest(string request, Crestron.SimplSharp.Net.Http.RequestType requestType)
        {
            Debug.Console(0, this, "DispatchHttpRequest: request = {0} | requestType = {1}", request, requestType.ToString());

            if (string.IsNullOrEmpty(request))
            {
                Debug.Console(0, this, "DispatchHttpRequest: request is null or empty, cannot dispatch request");
                return;
            }

            try
            {
                request = string.Format("{0}://{1}/triplecare/jsonrpchandler.php", Method.ToString().ToLower(), Host.Trim('/'));
                Debug.Console(0, this, "Over-writing request = {0}", request);
                /*
                [20:19:43.426]App 1:[iptv-tuner-1] SendCommand: method = ChannelUp | param =
                [20:19:43.431]App 1:[iptv-tuner-1-http-client] SendRequest: url = http://10.1.0.199/triplecare/jsonrpchandler.php?call={"jsonrpc":"2.0","method":"ChannelUp","params":[3]}
                [20:19:43.433]App 1:[iptv-tuner-1-http-client] SendRequest: _clientHttp.ProcessBusy False
                [20:19:43.439]App 1:[iptv-tuner-1-http-client] DispatchHttpRequest: request = /triplecare/jsonrpchandler.php?call={"jsonrpc":"2.0","method":"ChannelUp","params":[3]} | requestType = Get
                [20:19:43.442]App 1:[iptv-tuner-1-http-client] Over-writing request = http://10.1.0.199/triplecare/jsonrpchandler.php
                
                // over-wrote request to test issue, currently finding the json body of the actual request (^^^see above^^^) is most likely causing the issue 
                [20:19:43.454]App 1:[iptv-tuner-1-http-client] uri = http://10.1.0.199/triplecare/jsonrpchandler.php
                [20:19:43.459]App 1:[iptv-tuner-1-http-client] uri.AbsolutePath = /triplecare/jsonrpchandler.php
                [20:19:43.461]App 1:[iptv-tuner-1-http-client] uri.AbsoluteUri = http://10.1.0.199/triplecare/jsonrpchandler.php
                [20:19:43.464]App 1:[iptv-tuner-1-http-client] uri.Authority = 10.1.0.199
                [20:19:43.466]App 1:[iptv-tuner-1-http-client] uri.DnsSafeHost = 10.1.0.199
                [20:19:43.469]App 1:[iptv-tuner-1-http-client] uri.Fragment =
                [20:19:43.471]App 1:[iptv-tuner-1-http-client] uri.Host = 10.1.0.199
                [20:19:43.474]App 1:[iptv-tuner-1-http-client] uri.HostNameType = IPv4
                [20:19:43.476]App 1:[iptv-tuner-1-http-client] uri.IsAbsoluteUri = True
                [20:19:43.479]App 1:[iptv-tuner-1-http-client] uri.IsDefaultPort = True
                [20:19:43.480]App 1:[iptv-tuner-1-http-client] uri.IsFile = False
                [20:19:43.482]App 1:[iptv-tuner-1-http-client] uri.IsLoopback = False
                [20:19:43.484]App 1:[iptv-tuner-1-http-client] uri.IsUnc = False
                [20:19:43.488]App 1:[iptv-tuner-1-http-client] uri.IsWellFormedOriginalString = True
                [20:19:43.491]App 1:[iptv-tuner-1-http-client] uri.LocalPath = /triplecare/jsonrpchandler.php
                [20:19:43.492]App 1:[iptv-tuner-1-http-client] uri.OriginalString = http://10.1.0.199/triplecare/jsonrpchandler.php
                [20:19:43.495]App 1:[iptv-tuner-1-http-client] uri.Port = 80
                [20:19:43.497]App 1:[iptv-tuner-1-http-client] uri.PathAndQuery = /triplecare/jsonrpchandler.php
                [20:19:43.499]App 1:[iptv-tuner-1-http-client] uri.Query =
                [20:19:43.501]App 1:[iptv-tuner-1-http-client] uri.Scheme = http
                [20:19:43.503]App 1:[iptv-tuner-1-http-client] uri.UserEscaped = False
                [20:19:43.505]App 1:[iptv-tuner-1-http-client] uri.UseerInfo =
                [20:19:43.508]App 1:[iptv-tuner-1-http-client] DispatchHttpRequest: uri - http://10.1.0.199/triplecare/jsonrpchandler.php
                [20:19:43.510]App 1:[iptv-tuner-1-http-client] DispatchHttpRequest: _requestHttp.Url = http://10.1.0.199/triplecare/jsonrpchandler.php
                [20:19:43.516]App 1:[iptv-tuner-1-http-client] DispatchHttpsRequest: _dispatchHttpError 'PENDING'
                [20:19:43.626]App 1:[plugin-bridge-1] EiscApiAdvanced change: Bool 511=False
                [20:19:43.628]App 1:[plugin-bridge-1] Executing Action: System.Action`1[[System.Boolean, mscorlib, Version=3.5.0.0, Culture=neutral, PublicKeyToken=969DB8053D3322AC]]
                [20:19:44.794]App 1:[iptv-tuner-1-http-client] DispatchRequest: response is null, error: UNKNOWN_ERROR
                */
                var uri = new Uri(request);                                
                Debug.Console(0, this, "uri = {0}", uri);
                Debug.Console(0, this, "uri.AbsolutePath = {0}", uri.AbsolutePath);
                Debug.Console(0, this, "uri.AbsoluteUri = {0}", uri.AbsoluteUri);
                Debug.Console(0, this, "uri.Authority = {0}", uri.Authority);
                Debug.Console(0, this, "uri.DnsSafeHost = {0}", uri.DnsSafeHost);
                Debug.Console(0, this, "uri.Fragment = {0}", uri.Fragment);
                Debug.Console(0, this, "uri.Host = {0}", uri.Host);
                Debug.Console(0, this, "uri.HostNameType = {0}", uri.HostNameType);
                Debug.Console(0, this, "uri.IsAbsoluteUri = {0}", uri.IsAbsoluteUri);
                Debug.Console(0, this, "uri.IsDefaultPort = {0}", uri.IsDefaultPort);
                Debug.Console(0, this, "uri.IsFile = {0}", uri.IsFile);
                Debug.Console(0, this, "uri.IsLoopback = {0}", uri.IsLoopback);
                Debug.Console(0, this, "uri.IsUnc = {0}", uri.IsUnc);
                Debug.Console(0, this, "uri.IsWellFormedOriginalString = {0}", uri.IsWellFormedOriginalString());
                Debug.Console(0, this, "uri.LocalPath = {0}", uri.LocalPath);
                Debug.Console(0, this, "uri.OriginalString = {0}", uri.OriginalString);
                Debug.Console(0, this, "uri.Port = {0}", uri.Port);
                Debug.Console(0, this, "uri.PathAndQuery = {0}", uri.PathAndQuery);
                Debug.Console(0, this, "uri.Query = {0}", uri.Query);
                Debug.Console(0, this, "uri.Scheme = {0}", uri.Scheme);
                //Debug.Console(0, this, "uri.Segments = {0}", uri.Segments);
                Debug.Console(0, this, "uri.UserEscaped = {0}", uri.UserEscaped);
                Debug.Console(0, this, "uri.UseerInfo = {0}", uri.UserInfo);

                _requestHttp.Url.Parse(uri.AbsoluteUri);
                _requestHttp.RequestType = requestType;
                
                Debug.Console(0, this, "DispatchHttpRequest: uri - {0}", uri);
                Debug.Console(0, this, "DispatchHttpRequest: _requestHttp.Url = {0}", _requestHttp.Url);

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
        }

        // dispatches requests to the client
        private void DispatchHttpsRequest(string request, Crestron.SimplSharp.Net.Https.RequestType requestType)
        {
            if (string.IsNullOrEmpty(request))
            {
                Debug.Console(0, this, "DispatchHttpRequest: request is null or empty, cannot dispatch request");
                return;
            }

            try
            {
                var uri = new Uri(request);
                _requestHttps.Url.Parse(uri.AbsoluteUri);
                _requestHttps.RequestType = requestType;

                Debug.Console(0, this, "DispatchHttpsRequest: uri - {0}", uri);
                Debug.Console(0, this, "DispatchHttpsRequest: _requestHttps.Url = {0}", _requestHttps.Url);
                

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
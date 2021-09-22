using System;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
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
        /// Client string response event
        /// </summary>
        public event EventHandler<GenericClientStringResponseEventArgs> StringResponseRecieved;

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

            Debug.Console(2, this, "{0}", new String('-', 80));
            Debug.Console(2, this, "GenericClient: Key = {0}", Key);
            Debug.Console(2, this, "GenericClient: Host = {0}", Host);
            Debug.Console(2, this, "GenericClient: Port = {0}", Port);
            Debug.Console(2, this, "GenericClient: Username = {0}", Username);
            Debug.Console(2, this, "GenericClient: Password = {0}", Password);
            Debug.Console(2, this, "GenericClient: AuthorizationBase64 = {0}", AuthorizationBase64);

            switch (Method)
            {
                case eControlMethod.Http:
                    {
                        if (_clientHttp == null)
                            _clientHttp = new HttpClient();
                        if (_requestHttp == null)
                            _requestHttp = new HttpClientRequest();                                              

                        // TODO [ ] Turn off verbose logging
                        _clientHttp.Verbose = true;

                        break;
                    }
                case eControlMethod.Https:
                    {
                        if (_clientHttps == null)
                            _clientHttps = new HttpsClient();
                        if (_requestHttps == null)
                            _requestHttps = new HttpsClientRequest();

                        // TODO [ ] Turn off verbose logging
                        _clientHttps.Verbose = true;

                        break;
                    }
                default:
                    {
                        Debug.Console(0, this, "GenericClient: invalid eControlMethod '{0}'", Method);
                        break;
                    }
            }
            
            Debug.Console(2, this, "{0}", new String('-', 80));
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
                Debug.Console(1, this, Debug.ErrorLogLevel.Error, "SendRequest: request is null or empty");
                return;
            }

            char[] charsToTrim = { '/' };
            var url = request.StartsWith("http")
                ? string.Format("{0}", request.Trim(charsToTrim))
                : string.Format("{0}://{1}/{2}", Method.ToString().ToLower(), Host, request.Trim(charsToTrim));

            Debug.Console(2, this, "{0}", new String('-', 80));
            Debug.Console(2, this, "SendRequest: request = {0}", request);
            Debug.Console(2, this, "SendRequest: contentString = {0}", contentString);
            Debug.Console(2, this, "SendRequest: url = {0}", url);

            switch (Method)
            {
                case eControlMethod.Http:
                    {
                        Debug.Console(1, this, "SendRequest: _clientHttp.ProcessBusy {0}", _clientHttp.ProcessBusy);
                        if (_clientHttp.ProcessBusy)
                            _requestQueue.Enqueue(() => DispatchHttpRequest(url, contentString, Crestron.SimplSharp.Net.Http.RequestType.Get));
                        else
                            DispatchHttpRequest(url, contentString, Crestron.SimplSharp.Net.Http.RequestType.Get);
                        break;
                    }
                case eControlMethod.Https:
                    {
                        Debug.Console(1, this, "SendRequest: _clientHttps.ProcessBusy {0}", _clientHttps.ProcessBusy);
                        if (_clientHttps.ProcessBusy)
                            _requestQueue.Enqueue(() => DispatchHttpsRequest(url, contentString, Crestron.SimplSharp.Net.Https.RequestType.Get));
                        else
                            DispatchHttpsRequest(url, contentString, Crestron.SimplSharp.Net.Https.RequestType.Get);
                        break;
                    }
                default:
                    {
                        Debug.Console(1, this, Debug.ErrorLogLevel.Error, "SendRequest: invalid method '{0}'", Method);
                        break;
                    }
            }
            Debug.Console(2, this, "{0}", new String('-', 80));

        }

        // dispatches requests to the client
        private void DispatchHttpRequest(string request, string contentString, Crestron.SimplSharp.Net.Http.RequestType requestType)
        {
            Debug.Console(2, this, "{0}", new String('-', 80));
            Debug.Console(1, this, "DispatchHttpRequest: request = {0} | contentString = {1} | requestType = {2}", request, contentString, requestType.ToString());

            if (string.IsNullOrEmpty(request))
            {
                Debug.Console(1, this, "DispatchHttpRequest: request is null or empty, cannot dispatch request");
                return;
            }

            try
            {
                _clientHttp.UserName = Username;
                _clientHttp.Password = Password;
                _clientHttp.KeepAlive = false; 

                _requestHttp.Url.Parse(request);
                Debug.Console(2, this, "DispatchHttpRequest: _requestHttp.Url = {0}", _requestHttp.Url);
                
                _requestHttp.RequestType = requestType;
                _requestHttp.Header.ContentType = "application/json";
                _requestHttp.Header.SetHeaderValue("Content-Type", "application/json");

                if (!string.IsNullOrEmpty(contentString))
                    _requestHttp.ContentString = contentString;
                
                if (!string.IsNullOrEmpty(AuthorizationBase64))
                    _requestHttp.Header.SetHeaderValue("Authorization", AuthorizationBase64);

                //_dispatchHttpError = _clientHttp.DispatchAsync(_requestHttp, (response, error) =>
                //{
                //    if (response == null)
                //    {
                //        Debug.Console(1, this, "DispatchRequest: response is null, error: {0}", error);
                //        return;
                //    }

                //    OnResponseRecieved(new GenericClientResponseEventArgs(response.Code, response.ContentString));
                //});

                //_dispatchHttpError = _clientHttp.GetAsync(_requestHttp.Url.Url, (response, error) =>
                _dispatchHttpError = _clientHttp.GetAsync(request, (response, error) =>
                {
                    if (response == null)
                    {
                        Debug.Console(1, this, "DispatchRequest: response is null, error: {0}", error);
                        return;
                    }

                    OnStringResponseRecieved(new GenericClientStringResponseEventArgs(response));
                });
                
                Debug.Console(1, this, "DispatchHttpsRequest: _dispatchHttpError '{0}'", _dispatchHttpError);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, Debug.ErrorLogLevel.Error, "DispatchHttpRequest Exception: {0}", ex);
            }
            Debug.Console(2, this, "{0}", new String('-', 80));
        }

        // dispatches requests to the client
        private void DispatchHttpsRequest(string request, string contentString, Crestron.SimplSharp.Net.Https.RequestType requestType)
        {
            if (string.IsNullOrEmpty(request))
            {
                Debug.Console(1, this, "DispatchHttpRequest: request is null or empty, cannot dispatch request");
                return;
            }

            try
            {
                _clientHttps.UserName = Username;
                _clientHttps.Password = Password;
                _clientHttps.KeepAlive = false;
                _clientHttps.HostVerification = false;
                _clientHttps.PeerVerification = false;

                _requestHttps.Url.Parse(request);
                Debug.Console(2, this, "DispatchHttpsRequest: _requestHttps.Url = {0}", _requestHttps.Url);

                _requestHttps.RequestType = requestType;
                _requestHttps.Header.ContentType = "application/json";
                _requestHttps.Header.SetHeaderValue("Content-Type", "application/json");

                if (!string.IsNullOrEmpty(contentString))
                    _requestHttps.ContentString = contentString;

                if (!string.IsNullOrEmpty(AuthorizationBase64))
                {
                    _clientHttps.AuthenticationMethod = AuthMethod.BASIC;
                    _requestHttps.Header.SetHeaderValue("Authorization", AuthorizationBase64);
                }

               //_dispatchHttpsError = _clientHttps.DispatchAsync(_requestHttps, (response, error) =>
               // {
               //     if (response == null)
               //     {
               //         Debug.Console(1, this, "DispatchRequest: response is null, error: {0}", error);
               //         return;
               //     }

               //     OnResponseRecieved(new GenericClientResponseEventArgs(response.Code, response.ContentString));
               // });

                _dispatchHttpsError = _clientHttps.GetAsync(request, (response, error) =>
                {
                    if (response == null)
                    {
                        Debug.Console(1, this, "DispatchRequest: response is null, error: {0}", error);
                        return;
                    }

                    OnStringResponseRecieved(new GenericClientStringResponseEventArgs(response));
                });

                Debug.Console(1, this, "DispatchHttpsRequest: _dispatchHttpError '{0}'", _dispatchHttpsError);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, Debug.ErrorLogLevel.Error, "DispatchHttpsRequest Exception: {0}", ex);
            }
        }

        // client response event handler
        private void OnResponseRecieved(GenericClientResponseEventArgs args)
        {
            Debug.Console(2, this, "OnResponseReceived: args.Code = {0} | args.ContentString = {1}", args.Code, args.ContentString);

            CheckRequestQueue();

            var handler = ResponseReceived;
            if (handler == null) return;

            handler(this, args);
        }

        private void OnStringResponseRecieved(GenericClientStringResponseEventArgs args)
        {
            Debug.Console(2, this, "OnStringResponseReceived: args.ContentString = {0}", args.ContentString);

            CheckRequestQueue();

            var handler = StringResponseRecieved;
            if (handler == null) return;

            handler(this, args);
        }

        // Checks request queue and issues next request
        private void CheckRequestQueue()
        {
            Debug.Console(2, this, "CheckRequestQueue: _requestQueue.Count = {0}", _requestQueue.Count);
            var nextRequest = _requestQueue.TryToDequeue();
            Debug.Console(2, this, "CheckRequestQueue: _requestQueue.TryToDequeue was {0}", (nextRequest == null) ? "unsuccessful" : "successful");
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
                Debug.Console(1, this, Debug.ErrorLogLevel.Error, "EncodeBase64 Exception:\r{0}", err);
                return "";
            }
        }
    }
}
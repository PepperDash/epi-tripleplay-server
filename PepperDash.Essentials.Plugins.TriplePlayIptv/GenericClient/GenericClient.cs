using System;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net;
using Crestron.SimplSharp.Net.Http;
using Crestron.SimplSharp.Net.Https;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace TriplePlayIptvPlugin
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
		private string Host { get; set; }
		private int Port { get; set; }
		private string Username { get; set; }
		private string Password { get; set; }
		private string AuthorizationBase64 { get; set; }        

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

			Key = string.Format("{0}-{1}-client", key, controlConfig.Method);
			Method = controlConfig.Method;
		    Host = controlConfig.TcpSshProperties.Address.TrimEnd('/') ?? "";            
            
			switch (Method)
			{
				case eControlMethod.Http:
					{
                        if(_clientHttp == null)
                            _clientHttp = new HttpClient();
					    if (_requestHttp == null)
					        _requestHttp = new HttpClientRequest();

                        _clientHttp.Port = (controlConfig.TcpSshProperties.Port >= 1 && controlConfig.TcpSshProperties.Port <= 65535) ? controlConfig.TcpSshProperties.Port : 80;
                        _clientHttp.UserName = controlConfig.TcpSshProperties.Username ?? "";
					    _clientHttp.Password = controlConfig.TcpSshProperties.Password ?? "";
						_clientHttp.KeepAlive = false;

						_requestHttp.Header.SetHeaderValue("Content-Type", "application/json");

                        AuthorizationBase64 = EncodeBase64(_clientHttp.UserName, _clientHttp.Password);
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


						_clientHttps.UserName = controlConfig.TcpSshProperties.Username ?? "";
						_clientHttps.Password = controlConfig.TcpSshProperties.Password ?? "";
						_clientHttps.KeepAlive = false;
						_clientHttps.HostVerification = false;
						_clientHttps.PeerVerification = false;

						_requestHttps.Header.SetHeaderValue("Content-Type", "application/json");

                        AuthorizationBase64 = EncodeBase64(_clientHttps.UserName, _clientHttps.Password);
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
		    
			var url = request.StartsWith("http")
				? string.Format("{0}", request.Trim('/'))
				: string.Format("{0}://{1}/{2}", Method.ToString().ToLower(), Host, request.Trim('/'));

            Debug.Console(0, this, "SendRequest: {0}", url);

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
		private void DispatchHttpRequest(string request, Crestron.SimplSharp.Net.Http.RequestType type)
		{
			if (string.IsNullOrEmpty(request))
			{
				Debug.Console(0, this, "DispatchHttpRequest: request is null or empty, cannot dispatch request");
				return;
			}

		    try
		    {
                var uri = new Uri(request);
                Debug.Console(0, this, "DispatchHttpRequest: uri - {0}", uri);
                _requestHttp.Url.Parse(uri.AbsoluteUri);
                _requestHttp.RequestType = type;

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
		        Debug.Console(0, this, Debug.ErrorLogLevel.Error,"DispatchHttpRequest Exception: {0}", ex);
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
		        Debug.Console(0, this, "DispatchHttpsRequest: uri - {0}", uri);
		        _requestHttps.Url.Parse(uri.AbsoluteUri);
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
			var authorization = "";

			if (string.IsNullOrEmpty(username))
				return authorization;

			try
			{
				var base64String = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(string.Format("{0}:{1}", username, password)));
				authorization = string.Format("Basic {0}", base64String);
			}
			catch (Exception err)
			{
				Debug.Console(0, this, Debug.ErrorLogLevel.Error, "EncodeBase64 Exception:\r{0}", err);
			}

			return authorization;
		}
	}
}
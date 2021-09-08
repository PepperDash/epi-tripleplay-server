using Crestron.SimplSharpPro.DeviceSupport;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace EssentialsPluginTemplate
{
	/// <summary>
	/// Plugin device
	/// </summary>
	/// <remarks>
	/// Rename the class to match the device plugin being developed.
	/// </remarks>
	/// <example>
	/// "EssentialsPluginDeviceTemplate" renamed to "SamsungMdcDevice"
	/// </example>
	public class EssentialsPluginDeviceTemplate : EssentialsBridgeableDevice
	{
		/// <summary>
		/// It is often desirable to store the config
		/// </summary>
		private EssentialsPluginConfigObjectTemplate _config;

		#region IBasicCommunication Properties and Constructor.  Remove if not needed.

		// TODO [ ] Add, modify, remove properties and fields as needed for the plugin being developed
		private readonly IBasicCommunication _comms;
		private readonly GenericCommunicationMonitor _commsMonitor;
		
		// TODO [ ] Delete the properties below if using a HEX/byte based API		
		// _comms gather is commonly used for ASCII based API's with deelimiters
		private readonly CommunicationGather _commsGather;

		/// <summary>
		/// Set this value to that of the delimiter used by the API (if applicable)
		/// </summary>
		private const string CommsDelimiter = "\r";

		// TODO [ ] Delete the properties below if using an ASCII based API
		// _comms byte buffer is commonly used for HEX/byte based API's
		private byte[] _commsByteBuffer = { };

		// TODO [ ] If connection state is managed by Essentials, delete the following.  If connection is managed by SiMPL, uncomment the following
		/// <summary>
		/// Connects/disconnects the comms of the plugin device
		/// This property would only be needed if the plugin connection needs to be managed by SiMPL via the bridge
		/// </summary>
		/// <remarks>
		/// triggers the _comms.Connect/Disconnect as well as thee comms monitor start/stop
		/// </remarks>		
		//public bool Connect
		//{
		//    get { return _comms.IsConnected; }
		//    set
		//    {
		//        if (value)
		//        {
		//            _comms.Connect();
		//            _commsMonitor.Start();				
		//        }
		//        else
		//        {
		//            _comms.Disconnect();
		//            _commsMonitor.Stop();
		//        }
		//    }
		//}

		/// <summary>
		/// Reports connect feedback through the bridge
		/// </summary>
		public BoolFeedback ConnectFeedback { get; private set; }

		/// <summary>
		/// Reports online feedback through the bridge
		/// </summary>
		public BoolFeedback OnlineFeedback { get; private set; }

		/// <summary>
		/// Reports socket status feedback through the bridge
		/// </summary>
		public IntFeedback SocketStatusFeedback { get; private set; }

		/// <summary>
		/// Reports monitor status feedback through the bridge
		/// Typically used for Fusion status reporting and system status LED's
		/// </summary>
		public IntFeedback MonitorStatusFeedback { get; private set; }

		/// <summary>
		/// Plugin device constructor
		/// </summary>
		/// <param name="key">device key</param>
		/// <param name="name">device name</param>
		/// <param name="config">device configuration object</param>
		/// <param name="comms">device communication as IBasicCommunication</param>
		/// <see cref="PepperDash.Core.IBasicCommunication"/>
		/// <seealso cref="Crestron.SimplSharp.CrestronSockets.SocketStatus"/>
		public EssentialsPluginDeviceTemplate(string key, string name, EssentialsPluginConfigObjectTemplate config, IBasicCommunication comms)
			: base(key, name)
		{
			Debug.Console(0, this, "Constructing new {0} instance", name);

			// TODO [ ] Update the constructor as needed for the plugin device being developed

			_config = config;

			ConnectFeedback = new BoolFeedback(() => _comms.IsConnected);
			OnlineFeedback = new BoolFeedback(() => _commsMonitor.IsOnline);
			MonitorStatusFeedback = new IntFeedback(() => (int)_commsMonitor.Status);			

			_comms = comms;
			_commsMonitor = new GenericCommunicationMonitor(this, _comms, _config.PollTimeMs, _config.WarningTimeoutMs, _config.ErrorTimeoutMs, Poll);

			var socket = _comms as ISocketStatus;
			if (socket != null)
			{
				// device comms is IP **ELSE** device comms is RS232
				socket.ConnectionChange += socket_ConnectionChange;
				SocketStatusFeedback = new IntFeedback(() => (int)socket.ClientStatus);
			}

			#region Communication data event handlers.  Comment out any that don't apply to the API type			

			// TODO [ ] Delete the properties below if using a HEX/byte based API		
			// _comms gather is commonly used for ASCII based API's that have a defined delimiter
			_commsGather = new CommunicationGather(_comms, CommsDelimiter);			
			// Event fires when the defined delimter is found
			_commsGather.LineReceived += Handle_LineRecieved;

			// TODO [ ] Delete event if the device has a delimiter
			// Event fires as data is recieved
			_comms.TextReceived += Handle_TextReceived;
			
			// TODO [ ] Delete the properties below if using an ASCII based API
			// _comms byte buffer for HEX/byte based API's in raw format.  Commonly used for API's such as Samsung MDC
			// Event fires as data is recieved
			_comms.BytesReceived += Handle_BytesReceived;	

			#endregion Communication data event handlers.  Comment out any that don't apply to the API type

			Debug.Console(0, this, "Constructing new {0} instance complete", name);
			Debug.Console(0, new string('*', 80));
			Debug.Console(0, new string('*', 80));
		}

		/// <summary>
		/// Use the custom activiate to connect the device and start the comms monitor.
		/// This method will be called when the device is built.
		/// </summary>
		/// <returns></returns>
		public override bool CustomActivate()
		{
			// Essentials will handle the connect method to the device                       
			_comms.Connect();
			// Essentialss will handle starting the comms monitor
			_commsMonitor.Start();

			return base.CustomActivate();
		}

		private void socket_ConnectionChange(object sender, GenericSocketStatusChageEventArgs args)
		{
			if (ConnectFeedback != null)
				ConnectFeedback.FireUpdate();

			if (SocketStatusFeedback != null)
				SocketStatusFeedback.FireUpdate();
		}

		// TODO [ ] Delete the properties below if using a HEX/byte based API
		// commonly used with ASCII based API's with a defined delimiter				
		private void Handle_LineRecieved(object sender, GenericCommMethodReceiveTextArgs args)
		{
			// TODO [ ] Implement method 
			throw new System.NotImplementedException();
		}

		// TODO [ ] Delete the properties below if using a HEX/byte based API
		// commonly used with ASCII based API's without a delimiter
		void Handle_TextReceived(object sender, GenericCommMethodReceiveTextArgs e)
		{
			// TODO [ ] Implement method 
			throw new System.NotImplementedException();
		}

		// TODO [ ] Delete the properties below if using an ASCII based API		
		private void Handle_BytesReceived(object sender, GenericCommMethodReceiveBytesArgs args)
		{
			// TODO [ ] Implement method 
			throw new System.NotImplementedException();
		}

		// TODO [ ] Delete the properties below if using a HEX/byte based API
		/// <summary>
		/// Sends text to the device plugin comms
		/// </summary>
		/// <remarks>
		/// Can be used to test commands with the device plugin using the DEVPROPS and DEVJSON console commands
		/// </remarks>
		/// <param name="text">Command to be sent</param>		
		public void SendText(string text)
		{
			if (string.IsNullOrEmpty(text)) return;

			_comms.SendText(string.Format("{0}{1}", text, CommsDelimiter));
		}

		// TODO [ ] Delete the properties below if using an ASCII based API		
		/// <summary>
		/// Sends bytes to the device plugin comms
		/// </summary>
		/// <remarks>
		/// Can be used to test commands with the device plugin using the DEVPROPS and DEVJSON console commands
		/// </remarks>
		/// <param name="bytes">Bytes to be sent</param>		
		public void SendBytes(byte[] bytes)
		{
			if (bytes == null) return;

			_comms.SendBytes(bytes);
		}

		/// <summary>
		/// Polls the device
		/// </summary>
		/// <remarks>
		/// Poll method is used by the communication monitor.  Update the poll method as needed for the plugin being developed
		/// </remarks>
		public void Poll()
		{
			// TODO [ ] Update Poll method as needed for the plugin being developed
			// Example: SendText("getStatus");
			throw new System.NotImplementedException();
		}

		#endregion IBasicCommunication Properties and Constructor.  Remove if not needed.


		#region Overrides of EssentialsBridgeableDevice

		/// <summary>
		/// Links the plugin device to the EISC bridge
		/// </summary>
		/// <param name="trilist"></param>
		/// <param name="joinStart"></param>
		/// <param name="joinMapKey"></param>
		/// <param name="bridge"></param>
		public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
		{
			var joinMap = new EssentialsPluginBridgeJoinMapTemplate(joinStart);

			// This adds the join map to the collection on the bridge
			if (bridge != null)
			{
				bridge.AddJoinMap(Key, joinMap);
			}

			var customJoins = JoinMapHelper.TryGetJoinMapAdvancedForDevice(joinMapKey);

			if (customJoins != null)
			{
				joinMap.SetCustomJoinData(customJoins);
			}

			Debug.Console(1, "Linking to Trilist '{0}'", trilist.ID.ToString("X"));
			Debug.Console(0, "Linking to Bridge Type {0}", GetType().Name);

			// TODO [ ] Implement bridge links as needed

			// links to bridge
			trilist.SetString(joinMap.DeviceName.JoinNumber, Name);
			
			// TODO [ ] If connection state is managed by Essentials, delete the following.  If connection is managed by SiMPL, uncomment the following
			//trilist.SetBoolSigAction(joinMap.Connect.JoinNumber, sig => Connect = sig);
			//ConnectFeedback.LinkInputSig(trilist.BooleanInput[joinMap.Connect.JoinNumber]);

			SocketStatusFeedback.LinkInputSig(trilist.UShortInput[joinMap.SocketStatus.JoinNumber]);
			OnlineFeedback.LinkInputSig(trilist.BooleanInput[joinMap.IsOnline.JoinNumber]);

			UpdateFeedbacks();

			trilist.OnlineStatusChange += (o, a) =>
			{
				if (!a.DeviceOnLine) return;

				trilist.SetString(joinMap.DeviceName.JoinNumber, Name);
				UpdateFeedbacks();
			};
		}

		private void UpdateFeedbacks()
		{
			// TODO [ ] Update as needed for the plugin being developed
			ConnectFeedback.FireUpdate();
			OnlineFeedback.FireUpdate();
			SocketStatusFeedback.FireUpdate();
		}

		#endregion Overrides of EssentialsBridgeableDevice
	}
}


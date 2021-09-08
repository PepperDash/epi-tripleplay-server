using PepperDash.Essentials.Core;

namespace EssentialsPluginTemplate
{
	/// <summary>
	/// Plugin device Bridge Join Map
	/// </summary>
	/// <remarks>
	/// Rename the class to match the device plugin being developed.  Reference Essentials JoinMaps, if one exists for the device plugin being developed
	/// </remarks>
	/// <see cref="PepperDash.Essentials.Core.Bridges"/>
	/// <example>
	/// "EssentialsPluginBridgeJoinMapTemplate" renamed to "SamsungMdcBridgeJoinMap"
	/// </example>
	public class EssentialsPluginBridgeJoinMapTemplate : JoinMapBaseAdvanced
	{
		#region Digital

		// TODO [ ] Add digital joins below plugin being developed

		/// <summary>
		/// Plugin online join map
		/// </summary>
		/// <remarks>
		/// Reports the plugin online sate to SiMPL as a boolean value
		/// </remarks>
		[JoinName("IsOnline")]
		public JoinDataComplete IsOnline = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 1,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Is Online",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Digital
			});

		// TODO [ ] If connection state is managed by Essentials, delete the following.  If connection is managed by SiMPL, uncomment the following
		/// <summary>
		/// Plugin connect join map
		/// This property would only be needed if the plugin connection needs to be managed by SiMPL via the bridge
		/// </summary>
		/// <remarks>
		/// Typically used with socket based communications.  Connects (held) and disconnects (released) socket based communcations when triggered from SiMPL.
		/// Additionally, the connection state feedback will report to SiMP Las a boolean value.
		/// </remarks>
		//[JoinName("Connect")]
		//public JoinDataComplete Connect = new JoinDataComplete(
		//    new JoinData
		//    {
		//        JoinNumber = 2,
		//        JoinSpan = 1
		//    },
		//    new JoinMetadata
		//    {
		//        Description = "Connect (Held)/Disconnect (Release) & Connect state feedback",
		//        JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
		//        JoinType = eJoinType.Digital
		//    });		

		#endregion


		#region Analog

		// TODO [ ] Add analog joins below plugin being developed

		/// <summary>
		/// Plugin socket status join map
		/// </summary>
		/// <remarks>
		/// Typically used with socket based communications.  Reports the socket state to SiMPL as an analog value.
		/// </remarks>
		/// <see cref="Crestron.SimplSharp.CrestronSockets.SocketStatus"/>
		[JoinName("SocketStatus")]
		public JoinDataComplete SocketStatus = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 1,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Socket SocketStatus",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Analog
			});

		/// <summary>
		/// Plugin monitor status join map
		/// </summary>
		/// <remarks>
		/// Typically used with comms monitor to report plugin monitor state for system status page and Fusion monitor state.
		/// </remarks>
		/// <see cref="PepperDash.Essentials.Core.MonitorStatus"/>
		[JoinName("MonitorStatus")]
		public JoinDataComplete MonitorStatus = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 2,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Monitor Status",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Analog
			});

		#endregion


		#region Serial

		// TODO [ ] Add serial joins below plugin being developed

		/// <summary>
		/// Plugin device name
		/// </summary>
		/// <remarks>
		/// Reports the plugin name, as read from the configuration file, to SiMPL as a string value.
		/// </remarks>
		[JoinName("DeviceName")]
		public JoinDataComplete DeviceName = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 1,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Device Name",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Serial
			});

		#endregion

		/// <summary>
		/// Plugin device BridgeJoinMap constructor
		/// </summary>
		/// <param name="joinStart">This will be the join it starts on the EISC bridge</param>
		public EssentialsPluginBridgeJoinMapTemplate(uint joinStart)
			: base(joinStart, typeof(EssentialsPluginBridgeJoinMapTemplate))
		{
		}
	}
}

using Crestron.SimplSharpPro.DeviceSupport;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace EssentialsPluginTemplate
{
	/// <summary>
	/// Plugin device template for logic devices that don't communicate outside the program
	/// </summary>
	/// <remarks>
	/// Rename the class to match the device plugin being developed.
	/// </remarks>
	/// <example>
	/// "EssentialsPluginLogicDeviceTemplate" renamed to "SamsungMdcDevice"
	/// </example>
	public class EssentialsPluginLogicDeviceTemplate : EssentialsBridgeableDevice
	{
		/// <summary>
		/// It is often desirable to store the config
		/// </summary>
		private EssentialsPluginConfigObjectTemplate _config;


		/// <summary>
		/// Plugin device constructor
		/// </summary>
		/// <param name="key">A string</param>
		/// <param name="name">A string</param>
		/// <param name="config">An EssentialsPluginConfigObjectTemplate object</param>
		public EssentialsPluginLogicDeviceTemplate(string key, string name, EssentialsPluginConfigObjectTemplate config)
			: base(key, name)
		{
			Debug.Console(0, this, "Constructiong new {0} instance", name);

			// store the config
			_config = config;

			// TODO [ ] Update the constructor as needed for the plugin device being developed			
		}

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

			trilist.OnlineStatusChange += (o, a) =>
			{
				if (!a.DeviceOnLine) return;

				trilist.SetString(joinMap.DeviceName.JoinNumber, Name);
			};
		}

		#endregion
	}
}
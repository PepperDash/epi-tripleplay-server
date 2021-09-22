using System;
using System.Collections.Generic;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
	/// <summary>
	/// Plugin factory for devices that require communications using IBasicCommunications or custom communication methods
	/// </summary>
	public class TriplePlayIptvFactory : EssentialsPluginDeviceFactory<TriplePlayIptvDevice>
	{
		/// <summary>
		/// Plugin device factory constructor
		/// </summary>
		public TriplePlayIptvFactory()
		{
			// Set the minimum Essentials Framework Version
			MinimumEssentialsFrameworkVersion = "1.9.1";

			// In the constructor we initialize the list with the typenames that will build an instance of this device
			// only include unique typenames, when the constructur is used all the typenames will be evaluated in lower case.
			TypeNames = new List<string>() { "tripleplayiptv", "tripleplay" };
		}

		/// <summary>
		/// Builds and returns an instance of TriplePlayIptvDevice
		/// </summary>
		public override EssentialsDevice BuildDevice(DeviceConfig dc)
		{
			try
			{
				Debug.Console(0, new string('*', 80));
				Debug.Console(0, new string('*', 80));
				Debug.Console(0, "[{0}] Factory Attempting to create new device from type: {1}", dc.Key, dc.Type);				
				
				// get the plugin device properties configuration object & check for null 
				var propertiesConfig = dc.Properties.ToObject<TriplePlayIptvConfig>();

			    if (propertiesConfig == null)
			    {
			        Debug.Console(0, "[{0}] Unable to get configuration. Please check configuration.", dc.Key);
			        return null;
			    }

			    IRestfulComms client;

			    switch (propertiesConfig.Control.Method)
			    {
			        case eControlMethod.Http:
			            client = new GenericClientHttp(String.Format("{0}-http", dc.Key), propertiesConfig.Control);
			            break;
			        case eControlMethod.Https:
			            client = new GenericClientHttps(String.Format("{0}-https", dc.Key), propertiesConfig.Control);
			            break;
			        default:
			            Debug.Console(0, "[{0}] Control method {1} NOT supported. Please check configuration", dc.Key,
			                propertiesConfig.Control.Method);
			            return null;
			    }

			    return new TriplePlayIptvDevice(dc.Key, dc.Name, propertiesConfig, client);
			    
			}
			catch (Exception ex)
			{
				Debug.Console(0, "[{0}] Factory BuildDevice Exception: {1}", dc.Key, ex);
				return null;
			}
		}
	}
}
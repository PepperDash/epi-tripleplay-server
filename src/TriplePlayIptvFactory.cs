using System;
using System.Collections.Generic;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
	/// <summary>
	/// Plugin factory for TriplePlay IPTV devices
	/// </summary>
	public class TriplePlayIptvFactory : EssentialsPluginDeviceFactory<TriplePlayIptvDevice>
	{
		/// <summary>
		/// Plugin device factory constructor
		/// </summary>
		public TriplePlayIptvFactory()
		{
			// Numeric major.minor.patch only: a prerelease suffix makes Essentials skip loading the plugin.
			MinimumEssentialsFrameworkVersion = "3.0.0";

			// Type names are compared in lower case.
			TypeNames = new List<string>() { "tripleplayiptv", "tripleplay" };
		}

		/// <summary>
		/// Builds and returns an instance of TriplePlayIptvDevice
		/// </summary>
		public override EssentialsDevice BuildDevice(DeviceConfig dc)
		{
			try
			{
				var propertiesConfig = dc.Properties.ToObject<TriplePlayIptvConfig>();

				if (propertiesConfig == null || propertiesConfig.Control == null)
				{
					Debug.LogInformation("[{Key}] Unable to get configuration. Please check configuration.", dc.Key);
					return null;
				}

				var method = propertiesConfig.Control.Method;
				if (method != eControlMethod.Http && method != eControlMethod.Https)
				{
					Debug.LogInformation("[{Key}] Control method {Method} NOT supported. Please check configuration", dc.Key, method);
					return null;
				}

				var client = new RestClient(
					string.Format("{0}-{1}", dc.Key, method.ToString().ToLowerInvariant()),
					propertiesConfig.Control);

				return new TriplePlayIptvDevice(dc.Key, dc.Name, propertiesConfig, client);
			}
			catch (Exception ex)
			{
				Debug.LogInformation(ex, "[{Key}] Factory BuildDevice Exception: {Message}", dc.Key, ex.Message);
				return null;
			}
		}
	}
}

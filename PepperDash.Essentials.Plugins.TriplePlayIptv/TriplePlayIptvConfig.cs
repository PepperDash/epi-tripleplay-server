﻿using System.Collections.Generic;
using Newtonsoft.Json;
using PepperDash.Essentials.Core;

namespace TriplePlayIptvPlugin
{
	/// <summary>
	/// Plugin device configuration object
	/// </summary>
	/// <remarks>
	/// Rename the class to match the device plugin being created
	/// </remarks>
	/// <example>
	/// "TriplePlayIptvConfig" renamed to "SamsungMdcConfig"
	/// <code>
	/// {
	///		"devices": [
	///			{
	///				"key": "essentialsPluginKey",
	///				"name": "Essentia",
	///				"type": "tripleplayiptv",
	///				"group": "pluginDevices",
	///				"properties": {
	///					"control": {
	///						"method": "http-https",
	///						"tcpSshProperties": {
	///							"address": "172.22.0.101",
	///							"port": 23,
	///							"username": "admin",
	///							"password": "password"
	///						}
	///					},
	///					"pollTimeMs": 30000,
	///					"warningTimeoutMs": 180000,
	///					"errorTimeoutMs": 300000,
	///					"stbId": 1,
	///					"presets": {
	///						"1": {
	///							"name": "Preset 1",
	///							"icon": 1,
	///							"channel": 101
	///						}
	///						"2": {
	///							"name": "Item 2",
	///							"icon": 2,
	///							"channel": 102
	///						}
	///					}
	///				}
	///			}
	///		]
	/// }
	/// </code>
	/// </example>
	[ConfigSnippet("{\"devices\":[{\"key\":\"essentialsPluginKey\",\"name\":\"Essentia\",\"type\":\"tripleplayiptv\",\"group\":\"pluginDevices\",\"properties\":{\"control\":{\"method\":\"http-https\",\"tcpSshProperties\":{\"address\":\"172.22.0.101\",\"port\":23,\"username\":\"admin\",\"password\":\"password\",\"autoReconnect\":true,\"autoReconnectIntervalMs\":10000}},\"pollTimeMs\":30000,\"warningTimeoutMs\":180000,\"errorTimeoutMs\":300000,\"stbId\":1,\"presets\":{\"1\":{\"name\":\"Preset 1\",\"icon\":1,\"channel\":101},\"2\":{\"name\":\"Item 2\",\"icon\":2,\"channel\":102}}}}]}")]
	public class TriplePlayIptvConfig
	{
		/// <summary>
		/// JSON control object
		/// </summary>
		[JsonProperty("control")]
		public EssentialsControlPropertiesConfig Control { get; set; }

		/// <summary>
		/// Serializes the poll time value
		/// </summary>
		[JsonProperty("pollTimeMs")]
		public long PollTimeMs { get; set; }

		/// <summary>
		/// Serializes the warning timeout value
		/// </summary>
		[JsonProperty("warningTimeoutMs")]
		public long WarningTimeoutMs { get; set; }

		/// <summary>
		/// Serializes the error timeout value
		/// </summary>
		[JsonProperty("errorTimeoutMs")]
		public long ErrorTimeoutMs { get; set; }

		/// <summary>
		/// Settop box ID required for device control
		/// </summary>
		[JsonProperty("stbId")]
		public int StbId { get; set; }

		/// <summary>
		/// Preset dictionary
		/// </summary>
		[JsonProperty("presets")]
		public Dictionary<uint, TriplePlayPresetsConfig> Presets { get; set; }

		/// <summary>
		/// Constuctor
		/// </summary>
		public TriplePlayIptvConfig()
		{
			Presets = new Dictionary<uint, TriplePlayPresetsConfig>();
		}
	}

	/// <summary>
	/// Presets configuration 
	/// </summary>
	public class TriplePlayPresetsConfig
	{
		/// <summary>
		/// Serializes collection name property
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Serializes collection value property
		/// </summary>
		[JsonProperty("Icon")]
		public uint Icon { get; set; }

		/// <summary>
		/// Channel number
		/// </summary>
		[JsonProperty("channel")]
		public uint Channel { get; set; }

		/// <summary>
		/// Bridge index of preset (not implemented)
		/// </summary>
		[JsonProperty("bridgeIndex")]
		public uint BridgeIndex { get; set; }        
	}

    /// <summary>
    /// Services results config
    /// </summary>
    public class TriplePlayServicesPresetsConfig
    {
        /// <summary>
        /// Result ID (received from GetAllServices result response)
        /// </summary>
        [JsonProperty("id")]
        public uint  Id { get; set; }

        /// <summary>
        /// Channel number (received from GetAllServices result response)
        /// </summary>
        [JsonProperty("channelNumber")]
        public uint ChannelNumber { get; set; }

        /// <summary>
        /// Name (received from GetAllServices result response)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Is favorite flag (recieeved from GetAllServices result response)
        /// </summary>
        [JsonProperty("isFavorite")]
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Icon path (received from GetAllServices result response)
        /// </summary>
        [JsonProperty("iconPath")]
        public string IconPath { get; set; }

        /// <summary>
        /// Is watchable (recieeved from GetAllServices result response)
        /// </summary>
        [JsonProperty("isWatchable")]
        public bool IsWatchable { get; set; }
    }
}
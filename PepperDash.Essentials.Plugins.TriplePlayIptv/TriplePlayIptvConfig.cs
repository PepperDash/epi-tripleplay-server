using System.Collections.Generic;
using Newtonsoft.Json;
using PepperDash.Essentials.Core;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
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
	///                 "presetsMaxAllowed": 24,
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
		/// Input device id - vga
		/// </summary>
		[JsonProperty("inputVgaId")]
		public int? InputVgaId { get; set; }

		/// <summary>
		/// Input device id - hdmi
		/// </summary>
		[JsonProperty("inputHdmiId")]
		public int? InputHdmiId { get; set; }

		/// <summary>
		/// Input device id - hdmi 1
		/// </summary>
		[JsonProperty("inputHdmi1Id")]
		public int? InputHdmi1Id { get; set; }

		/// <summary>
		/// Input device id - hdmi 2
		/// </summary>
		[JsonProperty("inputHdmi2Id")]
		public int? InputHdmi2Id { get; set; }

		/// <summary>
		/// Input device id - hdmi 3
		/// </summary>
		[JsonProperty("inputHdmi3Id")]
		public int? InputHdmi3Id { get; set; }

		/// <summary>
		/// Input device id - hdmi 4
		/// </summary>
		[JsonProperty("inputHdmi4Id")]
		public int? InputHdmi4Id { get; set; }

		/// <summary>
		/// Input device id - dvi
		/// </summary>
		[JsonProperty("inputDviId")]
		public int? InputDviId { get; set; }

		/// <summary>
		/// Input device id - display port
		/// </summary>
		[JsonProperty("inputDisplayPortId")]
		public int? InputDisplayPortId { get; set; }

		/// <summary>
		/// Input device id - pc
		/// </summary>
		[JsonProperty("inputPcId")]
		public int? InputPcId { get; set; }

		/// <summary>
		/// Input device id - media
		/// </summary>
		[JsonProperty("inputMediaId")]
		public int? InputMediaId { get; set; }

        /// <summary>
        /// Allows configuration of maximum allowed presets used, defaults to 24 if not defined
        /// </summary>
        [JsonProperty("presetsMaxAllowed")]
        public int? PresetsMaxAllowed { get; set; }

		/// <summary>
		/// Preset dictionary
		/// </summary>
		[JsonProperty("presets", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<uint, TriplePlayServicesPresetsConfig> Presets { get; set; }

		/// <summary>
		/// Constuctor
		/// </summary>
		public TriplePlayIptvConfig()
		{
            Presets = new Dictionary<uint, TriplePlayServicesPresetsConfig>();
		}
	}

	/// <summary>
	/// Presets configuration 
	/// </summary>
	public class TriplePlayPresetsConfig
	{
        /// <summary>
        /// Preset ID
        /// </summary>
        [JsonProperty("Id")]
        public int Id { get; set; }

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
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public uint  Id { get; set; }

        /// <summary>
        /// Channel number (received from GetAllServices result response)
        /// </summary>
        [JsonProperty("channelNumber", NullValueHandling = NullValueHandling.Ignore)]
        public uint ChannelNumber { get; set; }

        /// <summary>
        /// Name (received from GetAllServices result response)
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Is favorite flag (recieeved from GetAllServices result response)
        /// </summary>
        [JsonProperty("isFavorite", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Icon path (received from GetAllServices result response)
        /// </summary>
        [JsonProperty("iconPath", NullValueHandling = NullValueHandling.Ignore)]
        public string IconPath { get; set; }

        /// <summary>
        /// Is watchable (recieeved from GetAllServices result response)
        /// </summary>
        [JsonProperty("isWatchable", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsWatchable { get; set; }
    }
}
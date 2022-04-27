using System.Collections.Generic;
using Newtonsoft.Json;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Root response object for services response
    /// </summary>
    public class ServicesResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }

        // "result": []
        [JsonProperty("result")]
        public List<ResultsObject> Results { get; set; }        
    }

    /// <summary>
    /// Results
    /// </summary>
    public class ResultsObject
    {
        [JsonProperty("id")]
        public uint Id { get; set; }

        [JsonProperty("channelNumber")]
        public uint ChannelNumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("isFavourite")]
        public bool IsFavourite { get; set; }

        [JsonProperty("iconPath")]
        public string IconPath { get; set; }

        [JsonProperty("accessControl")]
        public bool AccessControl { get; set; }

        [JsonProperty("serviceAccessControl")]        
        public string ServiceAccessControl { get; set; }

        [JsonProperty("recordingType")]
        public string RecordingType { get; set; }

        [JsonProperty("type")]
        public uint Type { get; set; }

        [JsonProperty("typeSpecificData")]
        public TypeSpecificDataObject TypeSpecificData { get; set; }

        [JsonProperty("multipleTypeData")]
        public MultipleTypeDataObject MultipleTypeData { get; set; }

        [JsonProperty("geographicalArea")]        
        public string GeographicalArea { get; set; }

        [JsonProperty("isWatchable")]
        public bool IsWatchable { get; set; }

        [JsonProperty("liveTrickplayRunning")]
        public bool LiveTrickplayRunning { get; set; }
    }

    /// <summary>
    /// Type specific data, a sub class of Result instance
    /// </summary>
    public class TypeSpecificDataObject
    {
        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("port")]
        public uint Port { get; set; }

        [JsonProperty("videoCodec")]
        public string VideoCodec { get; set; }

        [JsonProperty("encryptionType")]
        public string EncryptionType { get; set; }

        [JsonProperty("encryptionId")]
        public string EncryptionId { get; set; }
    }

    /// <summary>
    /// Multipe type data, a sub class of Result instance
    /// </summary>
    public class MultipleTypeDataObject
    {
        [JsonProperty("None")]
        public TypeSpecificDataObject None { get; set; }
    }
}
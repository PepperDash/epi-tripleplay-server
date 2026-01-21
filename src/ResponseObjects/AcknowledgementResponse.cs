using Newtonsoft.Json;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Root response object for acknowledgement response
    /// </summary>
    public class AcknowledgementResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }

        // "result": true
        [JsonProperty("result")]
        public bool Result { get; set; }
    }
}
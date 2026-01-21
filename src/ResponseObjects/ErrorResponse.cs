using Newtonsoft.Json;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Root response object for error responses
    /// </summary>
    public class ErrorResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }

        [JsonProperty("error")]
        public ErrorObject Error { get; set; }
    }

    /// <summary>
    /// Response error object
    /// </summary>
    public class ErrorObject
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
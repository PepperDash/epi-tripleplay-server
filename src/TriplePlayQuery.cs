using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Builds TriplePlay JSON-RPC queries: call={"jsonrpc":"2.0","method":...,"params":[stbId, ...]}
    /// </summary>
    public static class TriplePlayQuery
    {
        /// <summary>
        /// Builds the JSON-RPC call object. Parameters follow the set-top box id in the order
        /// string, int, bool; null or empty parameters are left out.
        /// </summary>
        /// <returns>the JSON, or null when stbId is not positive or method is empty</returns>
        public static string BuildCall(int stbId, string method, string strParam, int? intParam, bool? boolParam)
        {
            if (stbId <= 0 || string.IsNullOrEmpty(method)) return null;

            var array = new JArray { stbId };
            if (!string.IsNullOrEmpty(strParam))
                array.Add(strParam);
            if (intParam != null)
                array.Add(intParam);
            if (boolParam != null)
                array.Add(boolParam);

            var obj = new JObject
            {
                {"jsonrpc", "2.0"},
                {"method", method},
                {"params", array}
            };

            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        /// <summary>
        /// Builds the URL-encoded call= query string. HttpClient sends the URI as given, so the JSON is encoded here.
        /// </summary>
        /// <returns>the query, or null when stbId is not positive or method is empty</returns>
        public static string Build(int stbId, string method, string strParam, int? intParam, bool? boolParam)
        {
            var call = BuildCall(stbId, method, strParam, intParam, boolParam);
            return call == null ? null : "call=" + Uri.EscapeDataString(call);
        }
    }
}

using System;
using System.Net.Http.Headers;
using System.Text;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Base URI and authorization for the TriplePlay server
    /// </summary>
    public static class RestEndpoint
    {
        /// <summary>
        /// Builds the base URI. A port outside 1-65535 is left out, so the scheme's default is used.
        /// </summary>
        public static Uri BaseUri(bool https, string address, int port)
        {
            var scheme = https ? "https" : "http";
            return port >= 1 && port <= 65535
                ? new Uri(string.Format("{0}://{1}:{2}/", scheme, address, port))
                : new Uri(string.Format("{0}://{1}/", scheme, address));
        }

        /// <summary>
        /// Builds a Basic authorization header from Latin-1 credentials
        /// </summary>
        /// <returns>the header, or null when username is empty</returns>
        public static AuthenticationHeaderValue BasicAuthorization(string username, string password)
        {
            if (string.IsNullOrEmpty(username)) return null;

            var credentials = Encoding.Latin1.GetBytes(string.Format("{0}:{1}", username, password ?? ""));
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
        }
    }
}

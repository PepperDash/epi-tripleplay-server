using System;
using PepperDash.Core;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Interface that defines RESTFul communication methods
    /// </summary>
    public interface IRestfulComms :IKeyed
    {
        /// <summary>
        /// Send a request to the given path. Host parameters are configured when the client is created
        /// </summary>
        /// <param name="path">Path to send the request to. Will be appended to host set when client was created</param>
        /// <param name="content">If request type is POST, content will be sent as the content of the POST request</param>
        void SendRequest(string path, string content);

        /// <summary>
        /// Send a request to the given path using the given HTTP method. Host parameters are configured when the client is created
        /// </summary>
        /// <param name="requestType">Request type. Valid options are Get, Post, Put, Delete, Head, Patch</param>
        /// <param name="path">Path to send the request to. Will be appended to host set when client was created</param>
        /// <param name="content">If request type is POST, content will be sent as the content of the POST request</param>
        void SendRequest(string requestType, string path, string content);

        /// <summary>
        /// This event will fire when a response is received from the Host
        /// </summary>
        event EventHandler<GenericClientResponseEventArgs> ResponseReceived;
    }
}
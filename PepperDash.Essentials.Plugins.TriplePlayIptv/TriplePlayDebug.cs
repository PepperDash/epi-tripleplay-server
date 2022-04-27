using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Debug class used to identify and implement available debug levels
    /// </summary>
    static public class TriplePlayDebug
    {
        /// <summary>
        /// Debug info (level 0)
        /// </summary>
        public const int Info = 0;

        /// <summary>
        /// Debug debug (level 1)
        /// </summary>
        public const int Debug = 1;

        /// <summary>
        /// Debug verbose/silly (level 2)
        /// </summary>
        public const int Verbose = 2;
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using Alaris.Irc;
using NLog;

namespace Alaris.Administration
{
    ///<summary>
    /// A class used to remotely manage Alaris.
    ///</summary>
    [Serializable]
    public sealed class RemoteManager : MarshalByRefObject
    {
        private static HttpChannel _channel;
        private Connection _connection;
        private List<string> _channels;
        private bool _inited;

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        ///<summary>
        /// Run a remote notice method.
        ///</summary>
        ///<param name="module"></param>
        ///<param name="message"></param>
        public void RemoteNotice(string module, string message)
        {
            Log.Info("{0}: {1}", module, message);
        }

        ///<summary>
        /// Initialize the instance. Must be called before anything!
        ///</summary>
        public bool Initialize(string pass)
        {
            
            

            if(!pass.Equals(AlarisBot.GetBot().RemotePassword, StringComparison.InvariantCultureIgnoreCase))
            {
                Log.Warn("Remote connection attempt without valid password!");
                _inited = false;
                return false;
            }


            _inited = true;
            return true;
        }

        ///<summary>
        /// Sends a public message to the specified channel.
        ///</summary>
        ///<param name="chan">Channel to send to.</param>
        ///<param name="msg">Message to send.</param>
        public void PublicMessage(string chan, string msg)
        {
            AlarisBot.GetBot().SendMsg(chan, msg);
        }

        ///<summary>
        /// Starts the Remoting services.
        ///</summary>
        internal static void StartServives(int port, string name)
        {
            _channel = new HttpChannel(port);
            ChannelServices.RegisterChannel(_channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteManager), name,
                WellKnownObjectMode.SingleCall);
        }
    }
}

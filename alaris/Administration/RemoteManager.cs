using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Tcp;
using Alaris.Irc;

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
        ///<summary>
        /// Run a remote notice method.
        ///</summary>
        ///<param name="module"></param>
        ///<param name="message"></param>
        public void RemoteNotice(string module, string message)
        {
            Log.Notice(module,message);
        }

        ///<summary>
        /// Initialize the instance. Must be called before anything!
        ///</summary>
        public void Initialize()
        {
            var sBot = Singleton<AlarisBot>.Instance;
            _connection = sBot.Connection;
            _channels = sBot.Channels;
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

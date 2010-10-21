using System;
using System.Runtime.Remoting.Channels.Tcp;
using Alaris.Irc;

namespace Alaris.Administration
{
    ///<summary>
    /// A class used to remotely manage Alaris.
    ///</summary>
    [Serializable]
    public sealed class RemoteManager
    {
        private static TcpChannel _channel;
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
        /// Starts the Remoting services.
        ///</summary>
        internal static void StartServives(int port)
        {
            _channel = new TcpChannel(port);
        }
    }
}

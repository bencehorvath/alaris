using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Alaris.API;

namespace Alaris.Extras
{
    /// <summary>
    ///   Class for managing nodes in the cluster (like the database interface)
    /// </summary>
    public class ClusterManager
    {

        private ClusterManager()
        {
            RefreshDatabaseManager();
        }

        /// <summary>
        ///   Requests the database manager instance from the database server.
        /// </summary>
        public void RefreshDatabaseManager()
        {
            var channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);

           
        }

    }
}
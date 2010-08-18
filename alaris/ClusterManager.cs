using System;
using System.Runtime;
using System.Runtime.Remoting;
using MySql.Data.MySqlClient;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using Alaris.Core;

namespace Alaris.Extras
{
	/// <summary>
	/// Class for managing nodes in the cluster (like the database interface)
	/// </summary>
	public class ClusterManager
	{
		private DatabaseManager _manager;
		
		private ClusterManager ()
		{
			RefreshDatabaseManager();
		}
		
		/// <summary>
		/// Requests the database manager instance from the database server.
		/// </summary>
		public void RefreshDatabaseManager()
		{
			var channel = new TcpChannel();
			ChannelServices.RegisterChannel(channel, false);
			
			_manager = (DatabaseManager)Activator.GetObject(typeof(DatabaseManager), "tcp://localhost:9091/DatabaseManager");
			
			if(_manager.Equals(null))
				throw new InvalidOperationException("Couldn't request the DatabaseManager instance from the database server.");
			
			Log.Debug("ClusterManager", "DatabaseManager instance requested successfully.");
		}
		
	
        /// <summary>
        /// The Database Manager
        /// </summary>
        public DatabaseManager DatabaseManager { get
        {
            if (_manager.Equals(null))
                throw new InvalidOperationException("The requested manager instance is null!");

            return _manager;
        }
        }
	}
}


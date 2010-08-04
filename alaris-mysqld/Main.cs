using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;

namespace Alaris.Extras
{
	class MysqlCluster
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Initializing in cluster...");
			
			var channel = new TcpChannel(9091);
			ChannelServices.RegisterChannel(channel, true);
			
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(DatabaseManager), "DatabaseManager", WellKnownObjectMode.Singleton);
			
			Console.WriteLine("Database server up and ready.");
		}
	}
}


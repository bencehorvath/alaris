using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using Alaris.Administration;

namespace Alaris.Server
{
	public class Entry
	{
		public static void Main (string[] args)
		{
		    var chan = new HttpChannel();
            ChannelServices.RegisterChannel(chan, false);

		    var manager = (RemoteManager) Activator.GetObject(typeof (RemoteManager), "http://localhost:5564/RemoteManager");

            if(manager == null)
            {
                Console.WriteLine("Couldn't request instance.");
                return;
            }

            manager.Initialize("alaris00");
            manager.RemoteNotice("SERVER", "[SERVER]: REMOTE MESSAGE: ALERT!");
            manager.RemoteNotice("SERVER", "[SERVER]: REMOTE MESSAGE: ALERT!");
            manager.RemoteNotice("SERVER", "[SERVER]: REMOTE MESSAGE: ALERT!");
		}
		
		
	}
}


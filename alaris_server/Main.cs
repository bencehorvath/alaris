using System;
using System.Net;
using System.Net.Sockets;

namespace Alaris.Server
{
	public class Entry
	{
		public static void Main (string[] args)
		{
			var listener = new ServerListener(35220);
			
			listener.Listen();
		}
		
		
	}
}


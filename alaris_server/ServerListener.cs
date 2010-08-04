using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Mono.Math;
using System.Reflection;
using System.Threading;
using System.Timers;
using Alaris.Irc;
using Alaris.Extras;
using Alaris.Core;
using System.Net.Sockets;


namespace Alaris.Server
{

	public sealed class ServerListener : IDisposable
	{
		private readonly TcpListener _listener;
		//private readonly Thread _lThread;
		private readonly ServerPacketHandler sServerPacketHandler = Singleton<ServerPacketHandler>.Instance;
		/// <summary>
		/// Initializes a new instance of the <see cref="Alaris.Server.ServerListener"/> class.
		/// </summary>
		/// <param name='port'>
		/// Port to listen on.
		/// </param>
		public ServerListener(int port)
		{
			_listener = new TcpListener(IPAddress.Any, port);
			/*_lThread = new Thread(Listen);
			
			_lThread.Start();*/
			
			sServerPacketHandler.Init();
		}
		
		public void Listen()
		{
			_listener.Start();
			Log.Notice("AlarisListener", "Started...");
			
			while(true)
			{
				var client = _listener.AcceptTcpClient();
				
				Log.Notice("AlarisListener", "Client connection from: " + client.Client.RemoteEndPoint);
				
				var client_thread = new Thread(new ParameterizedThreadStart(ClientHandler));
				
				client_thread.Start(client);
				
				Thread.Sleep(50);
			}
		}
		
		public void ClientHandler(object ob)
		{
			var client = (ob as TcpClient);
			
			var stream = client.GetStream();
			
			byte[] message_buffer = new byte[262144];
			
			int bytes_read;
			
			Log.Notice("ClientHandler", "Handling client...");
			
			while(true)
			{
				bytes_read = 0;
				
				// read
				
				if(stream.DataAvailable && stream.CanRead)
				{
					Log.Debug("ClientHandler", "Stream data available, reading.");
					bytes_read = stream.Read(message_buffer, 0, message_buffer.Length);
					
					if(bytes_read == 0)
					{
						Log.Notice("ClientHandler", "Lost connection.");
						break;
					}
					
					var encoding = new UTF8Encoding();
					
					var msg = encoding.GetString(message_buffer, 0, bytes_read);
					
					var packet = new AlarisPacket(msg);
					
					sServerPacketHandler.HandlePacket(packet, client);
				}
				
				Thread.Sleep(50);
			}
		}
		
		public void Dispose()
		{
			/*_lThread.Join(1000);
			_lThread.Abort();*/
		}
	}
}

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using ICSharpCode.SharpZipLib;

using System.Reflection;
using System.Threading;
using System.Timers;
using Alaris.Irc;
using Alaris.Extras;
using System.Net.Sockets;


namespace Alaris.Core
{
	class Entry
	{
		private static void Main(string[] args)
		{
			// setup console.
			Console.ForegroundColor = ConsoleColor.Cyan;
			
			Console.WriteLine("Welcome to Alaris!");
			Console.WriteLine("Version: {0}", Utilities.GetBotVersion());
			Console.WriteLine("You can safely use <Ctrl+C> to terminate the process.\n");
			Thread.Sleep(2000);
			string conf = "alaris.conf";
			
			if(args.Length > 0)
				conf = args[0];
			
			if(!File.Exists(conf))
			{
				Log.LargeWarning("The required configuration file is not found!");
				Thread.Sleep(3000);
				Log.Notice("Config", "Generating default config file in " + conf + "");
				Thread.Sleep(1500);
				var writer = new StreamWriter(conf);
				writer.WriteLine("# The bot configuration file.\n# the server to connect to\nserver = irc.rizon.net\n# the nickname of the bot\nnick = alaris"
				                 + "\n# password to nickserv\n# use 'nothing' for no identification\nnickserv = nothing\n# channels to join to"
				                 + "\n# separate with commas\nchannels = #skullbot,#ascnhalf\n# User,Nick,Hostname\nadmin_data = Twl,Twl,evil.from.behind\n\n# mysql support enabled or not.\nmysql_enabled = 0\nmysql_data = localhost,root,pw,alaris");
				writer.Close();
			}
			
			var sBot = Singleton<AlarisBot>.Instance;
			var listener = new ClientListener(sBot.GetListenerPort());
			var lthread = new Thread(listener.Listen);
			lthread.Start();
			
			Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eargs) => {
				sBot.Disconnect("Daemon killed.");
				lthread.Join(100);
				lthread.Abort();
			};
			
			/*var exc = new List<Exception>();
			bot.GetCrashHandler().HandleReadConfig(bot.ReadConfig, conf, ref exc);
			exc.Clear();*/
			
			
			//bot.Connect();
			
			
			sBot.Pool.Enqueue(sBot);
			
			if(AlarisBot.AlarisServer)
			{
				Log.Debug("AlarisServer", "Initiating connection.");
				
				var packet = new AlarisPacket();
				
				packet.Write<int>((int)Opcode.CMSG_REQUEST_AUTH);
				packet.Write<string>(sBot.GetGuid().ToString());
				packet.Write<string>(Utilities.MD5String("twlbot"));
				packet.Write<int>(sBot.GetListenerPort());
				
				sBot.SendPacketToACS(packet);
			}
				
		}
	}
}
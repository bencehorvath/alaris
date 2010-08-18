using System;
using System.IO;
using Alaris.Core;
using System.Threading;
using Alaris.Extras;

namespace Alaris
{
    static class Entry
	{
		private static void Main(string[] args)
		{
			// setup console.
			Console.ForegroundColor = ConsoleColor.Cyan;
			
			Console.WriteLine("Welcome to Alaris!");
			Console.WriteLine("Version: {0}", Utilities.BotVersion);
			Console.WriteLine("You can safely use <Ctrl+C> to terminate the process.\n");
			Thread.Sleep(2000);
			string conf = "alaris.config.xml";
			
			if(args.Length > 0)
				conf = args[0];
			
			if(!File.Exists(conf))
			{
				Log.LargeWarning("The required configuration file is not found!");
				Thread.Sleep(3000);
				Log.Error("Alaris", "Terminating...");
			    return;
			}
			
			var sBot = Singleton<AlarisBot>.Instance;
		    ClientListener listener;
		    Thread lthread = null;

            if (AlarisBot.AlarisServer)
            {
                listener = new ClientListener(sBot.GetListenerPort());
                lthread = new Thread(listener.Listen);
                lthread.Start();
            }

		    Console.CancelKeyPress += (sender, e) => 
            {
				sBot.Disconnect("Daemon killed.");

                if (lthread != null)
                {
                    lthread.Join(100);
                    lthread.Abort();
                }
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
				
				packet.Write((int)Opcode.CMSG_REQUEST_AUTH);
				packet.Write(sBot.GetGuid().ToString());
				packet.Write(Utilities.MD5String("twlbot"));
				packet.Write(sBot.GetListenerPort());
				
				sBot.SendPacketToACS(packet);
                packet.Dispose();
			}
				
		}
	}
}
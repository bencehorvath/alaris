using System;
using System.IO;
using System.Threading;
using Alaris.API;
using Alaris.Irc;


namespace Alaris
{
    internal static class Entry
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

            if (args.Length > 0)
                conf = args[0];

            if (!File.Exists(conf))
            {
                Log.LargeWarning("The required configuration file is not found!");
                Thread.Sleep(3000);
                Log.Error("Alaris", "Terminating...");
                return;
            }
            
            var sBot = Singleton<AlarisBot>.Instance;

            Console.CancelKeyPress += (sender, e) => sBot.Disconnect("Daemon killed.");


            sBot.Pool.Enqueue(sBot);


            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                                                              {
                                                                  Log.LargeWarning("Unhandled Exception thrown.");
                                                                  Log.Error("UnhandledException", eventArgs.ExceptionObject.ToString());
                                                                  
                                                              };
        }
    }
}
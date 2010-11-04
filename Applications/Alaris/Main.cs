using System;
using System.IO;
using System.Net.Mime;
using System.Threading;
using Alaris.API;
using Alaris.Irc;
using NLog;


namespace Alaris
{
    internal static class Entry
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            ThreadPool.SetMaxThreads(40, 20);
            ThreadPool.SetMinThreads(4, 2);

            // setup console.
            //Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("Welcome to Alaris!");
            Console.WriteLine("Version: {0}", Utilities.BotVersion);
            Console.WriteLine("You can safely use <Ctrl+C> to terminate the process.\n");
            Thread.Sleep(2000);
            string conf = "alaris.config.xml";

            if (args.Length > 0)
                conf = args[0];

            if (!File.Exists(conf))
            {
                Log.Warn("The required configuration file is not found!");
                Thread.Sleep(3000);
                Log.Info("Terminating");
                return;
            }
            
            var sBot = Singleton<AlarisBot>.Instance;

            Console.CancelKeyPress += (sender, e) => sBot.Disconnect("Daemon killed.");


            //sBot.Run();
            ThreadPool.QueueUserWorkItem(b => sBot.Run());

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                                                              {
                                                                  Log.Warn("Unhandled Exception thrown.");
                                                                  Log.ErrorException("Unhandled exception has been thrown", eventArgs.ExceptionObject as Exception);
                                                                 
                                                              };

            
        }
    }
}
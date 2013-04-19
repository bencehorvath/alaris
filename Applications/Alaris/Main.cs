using System;
using System.IO;
using System.Threading;
using Alaris.Framework;
using Alaris.Framework.CommandLine;
using NLog;


namespace Alaris
{
    internal static class Entry
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.WriteLine("Welcome to Alaris!");
            Console.WriteLine("Version: {0}", Utility.BotVersion);
            Console.WriteLine("You can safely use <Ctrl+C> to terminate the process.\n");
            Thread.Sleep(2000);

            var conf = "alaris.config.xml";

            if (args.Length > 0)
                conf = args[0];

            if (!File.Exists(conf))
            {
                Log.Warn("The required configuration file is not found!");
                Thread.Sleep(3000);
                Log.Info("Terminating");
                return;
            }

            var bot = new AlarisBot("alaris.config.xml");

            Console.CancelKeyPress += (sender, e) => bot.Disconnect("Daemon killed.");


            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                                                              {
                                                                  Log.Error("An unnhandled exception has been thrown. ({0})", eventArgs.ExceptionObject as Exception);
                                                                  CrashDumper.CreateCrashDump();
                                                              };

            if (AlarisBase.Instance.CLIEnabled)
            {
                Log.Info("Starting CLI");
                CLI.Start();
            }


        }
    }
}
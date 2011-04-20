using System;
using System.Threading;
using Alaris.Framework.Commands;
using Alaris.Framework.Extensions;
using Alaris.Irc;
using NLog;

namespace Alaris.Framework.CommandLine
{
    /// <summary>
    /// Static class providing CLI functionality.
    /// </summary>
    public static class CLI
    {
        #region Properties
        /// <summary>
        /// Gets whether the CLI thread is running or not.
        /// </summary>
        public static bool IsRunning { get; private set; }

        #endregion

        #region Private Members

        private static readonly Thread CLIThread;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static string _currentChannel;

        private static string _cmd;

        #endregion

        static CLI()
        {
            _currentChannel = "#skullbot";
            CLIThread = new Thread(Run) {Name = "CLI Thread"};
        }

        private static void Run()
        {
            _cmd = string.Empty;

            while(_cmd != "stopcli")
            {
                _cmd = Console.ReadLine();
                if (string.IsNullOrEmpty(_cmd))
                    continue;

                if(_cmd.StartsWith("setchannel", StringComparison.InvariantCultureIgnoreCase))
                {
                    var channel = _cmd.Remove(0, 10);

                    if(channel.IsValidChannelName())
                    {
                        _currentChannel = channel;
                    }
                    else
                    {
                        Log.Error("Specified channel name ({0}) is invalid!", channel);
                    }

                    return;
                }

                if (!_cmd.StartsWith("@"))
                    _cmd = string.Format("@{0}", _cmd);

                CommandManager.HandleCommand(new UserInfo("SysOp", "SysOp", "SysOp.Local.Host"), _currentChannel, _cmd);

                Thread.Sleep(150);
            }
        }

        /// <summary>
        /// Starts the CLI thread.
        /// </summary>
        public static void Start()
        {
            if (IsRunning || CLIThread.ThreadState == ThreadState.Running)
                return;

            Log.Info("CLI is started");
            CLIThread.Start();
            IsRunning = true;
        }

        /// <summary>
        /// Stops the CLI thread.
        /// </summary>
        public static void Stop()
        {
            if (!IsRunning || CLIThread.ThreadState != ThreadState.Running)
                return;

            Log.Info("Stopping CLI");
            _cmd = "stopcli";
            CLIThread.Join(1500);
            CLIThread.Abort();

            IsRunning = false;
        }
    }
}

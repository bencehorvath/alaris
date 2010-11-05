using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;
using Alaris.Administration;
using Alaris.API;
using Alaris.API.Database;
using Alaris.CommandLine;
using Alaris.Commands;
using Alaris.Config;
using Alaris.Exceptions;
using Alaris.Irc;
using Alaris.Irc.Ctcp;
using Alaris.Xml;
using NLog;
using CLI = Alaris.Xml.CLI;

namespace Alaris
{
    /// <summary>
    ///   The main class for Alaris.
    /// </summary>
    public partial class AlarisBot : IDisposable
    {
        private Connection _connection;
        private ScriptManager _manager;
        private string _nick;
        private string _server;
        private bool _confdone;
        private bool _nickserv;
        private string _nspw = "";
        private readonly List<string> _channels = new List<string>();
        private readonly CrashHandler _sCrashHandler = Singleton<CrashHandler>.Instance;
        private readonly Guid _guid = Guid.NewGuid();
        private readonly string _configfile;
        private string _scriptsDir;

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        

        /// <summary>
        /// Database name.
        /// </summary>
        public string DBName { get; private set; }

        /// <summary>
        /// Gets whether the Lua engine is enabled or not.
        /// </summary>
        public bool LuaEnabled { get; private set; }

        /// <summary>
        /// Gets the remote's name.
        /// </summary>
        public string RemoteName { get; private set; }
        /// <summary>
        /// Gets the remote's port.
        /// </summary>
        public int RemotePort { get; private set; }

        /// <summary>
        /// Gets the remote's password.
        /// </summary>
        public string RemotePassword { get; private set; }

        /// <summary>
        ///   The bot's crash handler instance.
        /// </summary>
        public CrashHandler CrashHandler
        {
            get { return _sCrashHandler; }
        }

        /// <summary>
        /// Gets the current locale.
        /// </summary>
        public string Locale { get; private set; }

        /// <summary>
        ///   The bot's script manager instance.
        /// </summary>
        public ScriptManager ScriptManager
        {
            get { return _manager; }
        }

        /// <summary>
        /// Gets whether the Addons are enabled or not.
        /// </summary>
        public bool AddonsEnabled { get; private set; }

        /// <summary>
        /// Directory where the Addons are located
        /// </summary>
        public string AddonDirectory { get; private set; }

        /// <summary>
        /// Gets whether the CLI is enabled or not.
        /// </summary>
        public bool CLIEnabled { get; private set; }

        /// <summary>
        /// Gets the list of channels the bot is on.
        /// </summary>
        public List<string> Channels
        {
            get { return _channels; }
        }

        /// <summary>
        /// The configuration class.
        /// </summary>
        public AlarisConfig Config { get; private set; }

        /// <summary>
        /// Gets the IRC connection.
        /// </summary>
        public Connection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        /// The bot instance (singleton)
        /// </summary>
        public static AlarisBot Instance { get { return Singleton<AlarisBot>.Instance; } }

        /// <summary>
        ///   This is not an unused constructor. Called through singleton!
        /// </summary>
        private AlarisBot() : this("alaris.config.xml")
        {
        }


        /// <summary>
        ///   Creates a new instance of Alaris bot.
        /// </summary>
        private AlarisBot(string config)
        {
            Log.Info("Initializing");
            _configfile = config;
            CrashHandler.HandleReadConfig(ReadConfig, _configfile);
            var cargs = new ConnectionArgs(_nick, _server);
            
            Log.Info("Running huge amount of parallel tasks");
            
            try
            {
                
                ThreadPool.QueueUserWorkItem(o =>
                                                 {
                                                     Log.Info("Starting Identd service");
                                                     Identd.Start(_nick);
                                                 });

                ThreadPool.QueueUserWorkItem(obj =>
                                                 {
                                                     _connection = new Connection(cargs, true, false)
                                                     {
                                                         TextEncoding = Encoding.GetEncoding("Latin1")
                                                     };

                                                     var responder = new CtcpResponder(_connection)
                                                     {
                                                         VersionResponse = "Alaris " + Utilities.BotVersion,
                                                         SourceResponse = "http://www.wowemuf.org",
                                                         UserInfoResponse = "Alaris multi-functional bot."
                                                     };

                                                     Log.Info("Text encoding is {0}", _connection.TextEncoding.WebName);

                                                     _connection.CtcpResponder = responder;

                                                     Log.Info("CTCP is enabled");

                                                     _manager = new ScriptManager(ref _connection, _channels, _scriptsDir);
                                                     SetupHandlers();
                                                     _manager.Run();

                                                     

                                                    ThreadPool.QueueUserWorkItem(ps =>
                                                                                    {
                                                                                        if (AddonsEnabled)
                                                                                        {
                                                                                            Log.Info(
                                                                                                "Initializing Addon manager");
                                                                                            AddonManager.Initialize(
                                                                                                ref _connection,
                                                                                                Channels);

                                                                                            AddonManager.
                                                                                                LoadPluginsFromDirectory
                                                                                                (AddonDirectory);
                                                                                        }

                                                                                        
                                                                                        Log.Info("Setting up commands");

                                                                                        CommandManager.CommandPrefix = "@";
                                                                                        CommandManager.CreateMappings();
                                                                                    });
                                                     

                                                 });

            }
            catch(Exception x)
            {
                Log.FatalException("An exception has been thrown during one of the parallel executions.", x);          
            }

            //_connection = new Connection(cargs, true, false);

            Log.Info("Initializing Script manager");
           
            ThreadPool.QueueUserWorkItem(s => DatabaseManager.Initialize(DBName));

            //Log.Notice("Remoting", string.Format("Starting remoting channel on port {0} with name: {1}", RemotePort, RemoteName));
            Log.Info("Starting remoting service on port {0} with name {1}", RemotePort, RemoteName);
            RemoteManager.StartServives(RemotePort, RemoteName);

   
            Log.Info("Spawning another thread to continue startup.");
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the <see cref = "AlarisBot" />
        ///   is reclaimed by garbage collection.
        /// </summary>
        ~AlarisBot()
        {
            Log.Debug("Alaris", "~AlarisBot()");
        }

        /// <summary>
        ///   Gets the GUID.
        /// </summary>
        public Guid GetGuid()
        {
            return _guid;
        }

        /// <summary>
        ///   Run this instance.
        /// </summary>
        public void Run()
        {
            Connect(); 
        }


        private void SetupHandlers()
        {
            Log.Info("Registering event handlers");
            _manager.RegisterOnRegisteredHook(OnRegistered);
            _manager.RegisterOnPublicHook(OnPublicMessage);
            _connection.CtcpListener.OnCtcpRequest += OnCtcpRequest;
        }


        /// <summary>
        ///   Reads and parses the specified config file.
        /// </summary>
        /// <param name = "configfile">
        ///   The config file name.
        /// </param>
        /// <exception cref="ConfigFileInvalidException"></exception>
        private void ReadConfig(string configfile)
        {
            if (!File.Exists("./" + configfile))
                throw new FileNotFoundException(
                    "The config file specified could not be found. It is essential to have a configuration file in the directory of the bot. " +
                    configfile + " could not be found.");

            Log.Info("Reading configuration file");

            using(var reader = new StreamReader(configfile))
            {
                var serializer = new XmlSerializer(typeof (AlarisConfig));
                Config = (AlarisConfig)serializer.Deserialize(reader);
            }

            _server = Config.Config.Irc.Server;
            _nick = Config.Config.Irc.Nickname;

            _nickserv = Config.Config.Irc.NickServ.Enabled;

            if (_nickserv)
                _nspw = Config.Config.Irc.NickServ.Password;

            var chans = Config.Config.Irc.Channels;
            var clist = chans.Split(',');

            foreach (var chan in clist.Where(Rfc2812Util.IsValidChannelName).AsParallel())
                _channels.Add(chan);

            Utilities.AdminNick = Config.Config.Irc.Admin.Nick;
            Utilities.AdminUser = Config.Config.Irc.Admin.User;
            Utilities.AdminHost = Config.Config.Irc.Admin.Host;

            _scriptsDir = Config.Config.Scripts.Directory;

            LuaEnabled = Config.Config.Scripts.Lua;

            DBName = Config.Config.Database;

            AddonsEnabled = Config.Config.Addons.Enabled;

            if (AddonsEnabled)
                AddonDirectory = Config.Config.Addons.Directory;

            Locale = Config.Config.Localization.Locale;
            Log.Debug("Current locale is {0}", Locale);

            RemotePort = Config.Config.Remote.Port;
            RemoteName = Config.Config.Remote.Name;
            RemotePassword = Config.Config.Remote.Password;

            CLIEnabled = Config.Config.CLI.Enabled;

            Log.Info("Config file successfully loaded and validated");
            _confdone = true;

            
            Log.Info("Connecting to {0} with nick {1}", _server, _nick);
        }


        /// <summary>
        ///   Establishes the connection to the previously specified server.
        /// </summary>
        private void Connect()
        {
            if (!_confdone)
                throw new Exception("The config file has not been read before connecting.");

           Log.Info("Establishing connection");
            try
            {
                _connection.Connect();
            }
            catch (Exception x)
            {
                Log.FatalException("An exception has been thrown during the connection process.", x);
                Identd.Stop();
            }
        }

        /// <summary>
        ///   Disconnects the bot from the IRC server.
        /// </summary>
        /// <param name = "rsr">
        ///   Reason for disconnect.
        /// </param>
        public void Disconnect(string rsr)
        {
            Log.Info("Disconnecting");

            if (Identd.IsRunning())
            {
                Identd.Stop();
                Log.Info("Stopped Identd daemon");
            }

            //_manager.Lua.Free();

            try { _connection.Disconnect(rsr);}
            catch(InvalidOperationException)
            {
            }

            // stop cli

            CommandLine.CLI.Stop();

            Process.GetCurrentProcess().CloseMainWindow();
            Process.GetCurrentProcess().Close();
        }

        /// <summary>
        ///   Method run when the bot is registered to the IRC server.
        /// </summary>
        private void OnRegistered()
        {
            // Stop Identd, no need for it anymore.
            Identd.Stop();
            Log.Info("Bot is registered on the server");
            Log.Info("Stopped Identd service");

            // join channels here););

            foreach (var chan in _channels)
            {
                if (Rfc2812Util.IsValidChannelName(chan))
                    _connection.Sender.Join(chan);

                Log.Debug("Joined channel: {0}", chan);
            }

        }

        /// <summary>
        ///   Method run when the bot receives a CTCP request.
        /// </summary>
        /// <param name = "command">
        ///   The CTCP command.
        /// </param>
        /// <param name = "user">
        ///   Data about the user who sent it.
        /// </param>
        private static void OnCtcpRequest(string command, UserInfo user)
        {
            Log.Debug("Received CTCP command {0} from {1}", command, user.Nick);
        }

        /// <summary>
        ///   Releases all used resources.
        /// </summary>
        public void Dispose()
        {
            _connection.Dispose();
        }

    }
}
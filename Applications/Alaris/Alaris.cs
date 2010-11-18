using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Alaris.API;
using Alaris.API.Database;
using Alaris.Commands;
using Alaris.Exceptions;
using Alaris.Extensions;
using Alaris.Irc;
using Alaris.Irc.Ctcp;
using Alaris.Services;
using Alaris.Xml;
using NLog;

namespace Alaris
{
    /// <summary>
    ///   The main class for Alaris.
    /// </summary>
    [Serializable]
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
        private readonly CrashHandler _sCrashHandler;
        private readonly Guid _guid = Guid.NewGuid();
        private readonly string _configfile;
        private string _scriptsDir;
        private static bool _isInstantiated;

        [NonSerialized]
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
        public static AlarisBot Instance { get { return InstanceHolder<AlarisBot>.Instance; } }

        /// <summary>
        ///   This is not an unused constructor. Called through singleton!
        /// </summary>
        public AlarisBot() : this("alaris.config.xml")
        {
        }

 
        /// <summary>
        ///   Creates a new instance of Alaris bot.
        /// </summary>
        public AlarisBot(string config)
        {
            if (_isInstantiated)
                return;

            _isInstantiated = true;

            Log.Info("Initializing");

            _sCrashHandler = new CrashHandler();
            InstanceHolder<CrashHandler>.Set(_sCrashHandler);

            _configfile = config;
            CrashHandler.HandleReadConfig(ReadConfig, _configfile);
            var cargs = new ConnectionArgs(_nick, _server);
            
            Log.Info("Running huge amount of parallel tasks");
            
            try
            {
                ThreadPool.QueueUserWorkItem(i =>
                                                 {
                                                     Log.Info("Starting Identd service");
                                                     Identd.Start(_nick);
                                                 });


                _connection = new Connection(cargs, true, false)
                                {
                                    TextEncoding = Encoding.GetEncoding("Latin1")
                                };


                var responder = new CtcpResponder(_connection)
                                    {
                                        VersionResponse =
                                            "Alaris " + Utilities.BotVersion,
                                        SourceResponse = "http://www.wowemuf.org",
                                        UserInfoResponse =
                                            "Alaris multi-functional bot."
                                    };

                Log.Info("Text encoding is {0}", _connection.TextEncoding.WebName);

                _connection.CtcpResponder = responder;

                if (!this.SetAsInstance())
                    throw new Exception();

                Log.Info("CTCP is enabled");

                _manager = new ScriptManager(ref _connection, _channels, _scriptsDir);

                ThreadPool.QueueUserWorkItem(s => _manager.Run());
                                                                                                      
            }
            catch(Exception x)
            {
                Log.Fatal("An exception has been thrown during one of the parallel executions ({0})", x);          
            }

            //_connection = new Connection(cargs, true, false);

            ThreadPool.QueueUserWorkItem(d => DatabaseManager.Initialize(DBName));

           
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

            ThreadPool.QueueUserWorkItem(c =>
                                             {
                                                 Log.Info("Setting up commands");

                                                 CommandManager.CommandPrefix = "@";
                                                 CommandManager.CreateMappings();
                                             });

            SetupHandlers();
            Connect();
            
            ThreadPool.QueueUserWorkItem(s => ServiceManager.StartServices());
      
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

            _channels.GetChannelsFrom(clist);

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
                Log.Fatal("An exception has been thrown during the connection process. ({0})", x);
                Console.WriteLine(x);
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

            // stop services

            ServiceManager.StopServices();

            // stop cli

            CommandLine.CLI.Stop();

            Thread.Sleep(20000);

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
            
            _channels.JoinToChannels(_connection);

            Thread.Sleep(1000);

            GC.Collect(3, GCCollectionMode.Forced);
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
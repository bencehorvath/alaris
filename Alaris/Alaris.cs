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
using Alaris.Administration;
using Alaris.API;
using Alaris.API.Database;
using Alaris.Commands;
using Alaris.Config;
using Alaris.Exceptions;
using Alaris.Irc;
using Alaris.Irc.Ctcp;
using NLog;

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
        private const int ListenerPort = 35221;
        private const string ACSHost = "127.0.0.1";
        private const int ACSPort = 35220;
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
        /// Gets the list of channels the bot is on.
        /// </summary>
        public List<string> Channels
        {
            get { return _channels; }
        }

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
        ///   Creates a new instacne of Alaris bot.
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
                                                 });

            }
            catch(Exception x)
            {
                Log.FatalException("An exception has been thrown during one of the parallel executions.", x);          
            }

            //_connection = new Connection(cargs, true, false);

            ThreadPool.QueueUserWorkItem(s =>
                                             {
                                                 Log.Info("Setting up commands");

                                                 CommandManager.CommandPrefix = "@";
                                                 CommandManager.CreateMappings();
                                             });


            

            Log.Info("Initializing Script manager");
           
            ThreadPool.QueueUserWorkItem(s => DatabaseManager.Initialize(DBName));

            //Log.Notice("Remoting", string.Format("Starting remoting channel on port {0} with name: {1}", RemotePort, RemoteName));
            Log.Info("Starting remoting service on port {0} with name {1}", RemotePort, RemoteName);
            RemoteManager.StartServives(RemotePort, RemoteName);

   
            Log.Info("Spawning another thread to continue startup.");
        }

        /// <summary>
        ///   Gets the listener port.
        /// </summary>
        /// <returns>
        ///   The listener port.
        /// </returns>
        public static int GetListenerPort()
        {
            return ListenerPort;
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

            var config = new XmlSettings(configfile, "alaris");

            config.Document.Schemas.Add("http://github.com/Twl/alaris", "Alaris.xsd");

            try
            {

                config.Document.Validate((sender, args) =>
                                             {
                                                 Log.Debug("XML", args.Message);

                                                 if (args.Exception != null)
                                                 {
                                                     Log.Error("Config", args.Exception.Message);
                                                 }

                                             });
            }
            catch(XmlSchemaValidationException x)
            {
                Log.ErrorException("Config file is invalid!", x);
                throw new ConfigFileInvalidException(x.Message);
            }

            _server = config.GetSetting("config/irc/server", "irc.rizon.net");
            _nick = config.GetSetting("config/irc/nickname", "alaris");
            _nspw = config.GetSetting("config/irc/nickserv", "nothing");

            _nickserv = (_nspw != "nothing");

            var chans = config.GetSetting("config/irc/channels", "#skullbot,#hun_bot");
            var clist = chans.Split(',');

            foreach (var chan in clist.Where(Rfc2812Util.IsValidChannelName).AsParallel())
                _channels.Add(chan);

            Utilities.AdminNick = config.GetSetting("config/irc/admin/nick", "Twl");
            Utilities.AdminUser = config.GetSetting("config/irc/admin/user", "Twl");
            Utilities.AdminHost = config.GetSetting("config/irc/admin/host", "evil.from.behind");


            _scriptsDir = config.GetSetting("config/scripts/directory", "scripts");

            DBName = config.GetSetting("config/database", "Alaris").ToUpper(CultureInfo.InvariantCulture);

            LuaEnabled = config.GetSetting("config/scripts/LUA", "Disabled").Equals("Enabled");

            Locale = config.GetSetting("config/localization/locale", "enGB");


            Log.Debug("Current locale is {0}", Locale);

            RemotePort = Convert.ToInt32(config.GetSetting("config/remote/port", "5564"));
            RemoteName = config.GetSetting("config/remote/name", "RemoteManager");
            RemotePassword = config.GetSetting("config/remote/password", "alaris00");

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
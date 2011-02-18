using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Alaris.Framework.Commands;
using Alaris.Framework.Config;
using Alaris.Framework.Database;
using Alaris.Framework.Extensions;
using Alaris.Framework.Services;
using Alaris.Irc;
using Alaris.Irc.Ctcp;
using NLog;

namespace Alaris.Framework
{
    ///<summary>
    /// Base abstract class for Alaris-based bots.
    ///</summary>
    [Serializable]
    public abstract class AlarisBase
    {
        private readonly bool _isInstantiated;

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Connection _connection;
        private readonly ScriptManager _manager;
        private string _nick;
        private string _server;
        private bool _confdone;
        private bool _nickserv;
        private string _nspw = "";
        private readonly List<string> _channels = new List<string>();
        private readonly CrashHandler _sCrashHandler;
        protected readonly Guid Guid = Guid.NewGuid();
        private string _scriptsDir;

        /// <summary>
        /// Whether the bot instance is using parallelization (mainly on startup).
        /// </summary>
        public bool IsParallelized { get; protected set; }

        /// <summary>
        /// The configuration file's name
        /// </summary>
        public string ConfigFile { get; protected set; }

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
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static AlarisBase Instance
        {
            get
            {
                return InstanceHolder<AlarisBase>.Instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarisBase"/> class.
        /// </summary>
        /// <param name="configFile">The config file.</param>
        /// <param name="parallelized">if set to <c>true</c> [parallelized].</param>
        /// <param name="additionalCommandAssemblies">The additional command assemblies.</param>
        protected AlarisBase(string configFile = "alaris.config.xml", bool parallelized = true, params Assembly[] additionalCommandAssemblies)
        {
            if (_isInstantiated)
                return;

            _isInstantiated = true;

            IsParallelized = parallelized;
            ConfigFile = configFile;

            Log.Info("Initializing");

            var sw = new Stopwatch();
            sw.Start();

            _sCrashHandler = new CrashHandler();
            InstanceHolder<CrashHandler>.Set(_sCrashHandler);

            CrashHandler.HandleReadConfig(ReadConfig);
            var cargs = new ConnectionArgs(_nick, _server);

            if(IsParallelized)
            {
                Log.Info("Running huge amount of parallel tasks");

                Task itask = null, mtask = null, dtask, ctask;

                try
                {

                    itask = Task.Factory.StartNew(() =>
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

                    mtask = Task.Factory.StartNew(() => _manager.Run());

                }
                catch (Exception x)
                {
                    Log.Fatal("An exception has been thrown during one of the parallel executions ({0})", x);
                }

                dtask = Task.Factory.StartNew(() => DatabaseManager.Initialize(DBName));

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

                ctask = Task.Factory.StartNew(() =>
                {
                    Log.Info("Setting up commands");

                    CommandManager.CommandPrefix = "@";
                    CommandManager.CreateMappings();
                });

                SetupHandlers();
                Connect();

                var stask = Task.Factory.StartNew(ServiceManager.StartServices);

                sw.Stop();
                Log.Info("Startup took {0}ms", sw.ElapsedMilliseconds);

                Log.Info("Waiting for pending tasks to finish");

                Task.WaitAll(itask, mtask, dtask, ctask, stask);
            }
        }

        /// <summary>
        /// Connect to the IRC server.
        /// </summary>
        public void Connect()
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
        /// Disconnect the bot and free resources.
        /// </summary>
        public virtual void Disconnect(string reason)
        {
            Log.Info("Disconnecting");

            if (Identd.IsRunning())
            {
                Identd.Stop();
                Log.Info("Stopped Identd daemon");
            }

            //_manager.Lua.Free();

            try { _connection.Disconnect(reason); }
            catch (InvalidOperationException)
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
        /// Sets up event handlers.
        /// </summary>
        protected virtual void SetupHandlers()
        {
            Log.Info("Registering event handlers");
            _manager.RegisterOnRegisteredHook(OnRegistered);
            _manager.RegisterOnPublicHook(OnPublicMessage);
            _connection.CtcpListener.OnCtcpRequest += OnCtcpRequest;
        }

        /// <summary>
        /// Reloads everything possible (addons, commands etc.)
        /// </summary>
        protected virtual void ReloadAll()
        {
            UnloadAll();

            Log.Info("Loading things back...");

            LoadAll();
        }

        /// <summary>
        /// Loads everything possible (addons, commands etc.)
        /// </summary>
        protected virtual void LoadAll()
        {
            AddonManager.LoadPluginsFromDirectory(AddonDirectory);

            // here we can re-map commands since addons are back.
            // note: reloads from main assemblies (non-plugins) as well.

            CommandManager.CreateMappings();
        }

        /// <summary>
        /// Unloads everything possible (addons, commands etc.)
        /// </summary>
        protected virtual void UnloadAll()
        {
            Log.Info("Unloading everything possible...");
            CommandManager.DeleteMappings();
            AddonManager.UnloadPlugins();
        }

        /// <summary>
        /// Called when a CTCP command is received..
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="who">From whom.</param>
        protected virtual void OnCtcpRequest(string command, UserInfo who)
        {
             Log.Debug("Received CTCP command {0} from {1}", command, who.Nick);
        }

        /// <summary>
        /// This method is called when a public message occurs.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        protected virtual void OnPublicMessage(UserInfo user, string channel, string message)
        {
            Task.Factory.StartNew(() => CommandManager.HandleCommand(user, channel, message));
        }

        protected virtual void OnRegistered()
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
        /// Reads and parses the configuration file
        /// </summary>
        protected virtual void ReadConfig()
        {
            if (!File.Exists("./" + ConfigFile))
                throw new FileNotFoundException(
                    "The config file specified could not be found. It is essential to have a configuration file in the directory of the bot. " +
                    ConfigFile + " could not be found.");

            Log.Info("Reading configuration file");

            using (var reader = new StreamReader(ConfigFile))
            {
                var serializer = new XmlSerializer(typeof(AlarisConfig));
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
        ///   Sends the given message to the specified channel.
        /// </summary>
        /// <param name = "channel">
        ///   A <see cref = "System.String" />
        /// </param>
        /// <param name = "message">
        ///   A <see cref = "System.String" />
        /// </param>
        public virtual void SendMsg(string channel, string message)
        {
            _connection.Sender.PublicMessage(channel, message);
        }

        /// <summary>
        /// Sends the MSG.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        public void SendMsg(string channel, string message, params object[] args)
        {
            var msg = string.Format(message, args);
            SendMsg(channel, msg);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Alaris.Framework.Commands;
using Alaris.Framework.Config;
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
        private readonly ScriptManager _scriptManager;
        private bool _confdone;
        private readonly Server _server;
        private readonly byte _serverId;


        private readonly List<string> _channels = new List<string>();
        private readonly CrashHandler _sCrashHandler;

        /// <summary>
        /// Guid of this instance.
        /// </summary>
        protected readonly Guid Guid = Guid.NewGuid();

        private string _scriptsDir;

        /// <summary>
        /// List of server the bot connects to.
        /// </summary>
        public Server Server { get { return _server; } } 

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
            get { return _scriptManager; }
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
        /// The addon manager utility for this bot instance.
        /// </summary>
        public AddonManager AddonManager { get; private set; }

        /// <summary>
        /// The command manager utility for this bot instance.
        /// </summary>
        public CommandManager CommandManager { get; private set; }

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
        public Alaris.Framework.Config.Config Config { get; private set; }

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
        public static AlarisBase Instance { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarisBase"/> class.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="serverId"></param>
        /// <param name="parallelized">if set to <c>true</c> [parallelized].</param>
        /// <param name="additionalCommandAssemblies">The additional command assemblies.</param>
        protected AlarisBase(AlarisConfig configuration, byte serverId = 0, bool parallelized = true, params Assembly[] additionalCommandAssemblies)
        {
            if (_isInstantiated)
                return;

            _isInstantiated = true;

            IsParallelized = parallelized;
            Config = configuration.Config;
            _server = Config.Servers[serverId];
            _serverId = serverId;
            ProcessConfiguration();
            Instance = this;

            Log.Info("Initializing");



            var sw = new Stopwatch();
            sw.Start();

            _sCrashHandler = new CrashHandler();
            InstanceHolder<CrashHandler>.Set(_sCrashHandler);

            
            //var cargs = new ConnectionArgs(_nick, _server);

            Log.Info("Starting Identd");
            Identd.Start(_server.Nickname);

            var connectionArgs = new ConnectionArgs(_server.Nickname, _server.Address);

            _connection = new Connection(connectionArgs, true, false)
            {
                TextEncoding = Encoding.GetEncoding("Latin1")
            };


            _connection.CtcpResponder = new CtcpResponder(_connection)
            {
                VersionResponse =
                    "Alaris " + Utility.BotVersion,
                SourceResponse = "https://www.github.com/twl/alaris",
                UserInfoResponse =
                    "Alaris multi-functional bot."
            };

            Log.Info("Text encoding is {0}", _connection.TextEncoding.WebName);
            Log.Info("CTCP is enabled");

            _scriptManager = new ScriptManager(_scriptsDir);
            _scriptManager.Run();

            if (AddonsEnabled)
            {
                Log.Info("Initializing AddonManager");
                AddonManager = new AddonManager();
                AddonManager.Initialize();
                AddonManager.LoadPluginsFromDirectory(AddonDirectory);
            }
            
            Log.Info("Setting up commands");
            CommandManager = new CommandManager {CommandPrefix = "@"};
            CommandManager.CreateMappings();

            SetupHandlers();
            Connect();
        }

        /// <summary>
        /// Connect to the IRC server.
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

            //_scriptManager.Lua.Free();

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
        protected void SetupHandlers()
        {
            Log.Info("Registering event handlers");
            _scriptManager.RegisterOnRegisteredHook(OnRegistered);
            _scriptManager.RegisterOnPublicHook(OnPublicMessage);
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

        /// <summary>
        /// Called when registered.
        /// </summary>
        protected virtual void OnRegistered()
        {
            // Stop Identd, no need for it anymore.
            Identd.Stop();
            Log.Info("Bot is registered on the server");
            Log.Info("Stopped Identd service");

            // join channels here););

            _channels.JoinToChannels(_connection);

            Thread.Sleep(1000);

            GC.Collect(3, GCCollectionMode.Optimized);
        }

        /// <summary>
        /// Reads and parses the configuration file
        /// </summary>
        private void ProcessConfiguration()
        {

            Log.Info("Processing configuration");


            _channels.GetChannelsFrom(Config.Servers[_serverId].Channels.Split(','));

            Utility.Operator = Config.Servers[_serverId].BotOperator;

            _scriptsDir = Config.Scripts.Directory;

            LuaEnabled = Config.Scripts.Lua;

            DBName = Config.Database;

            AddonsEnabled = Config.Addons.Enabled;

            if (AddonsEnabled)
                AddonDirectory = Config.Addons.Directory;

            Locale = Config.Localization.Locale;
            Log.Debug("Current locale is {0}", Locale);

            RemotePort = Config.Remote.Port;
            RemoteName = Config.Remote.Name;
            RemotePassword = Config.Remote.Password;

            CLIEnabled = Config.CLI.Enabled;

            Log.Info("Config info successfully loaded and validated");
            _confdone = true;

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
            Contract.Assume(args != null);
            var msg = string.Format(message, args);
            SendMsg(channel, msg);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Alaris.Config;
using Alaris.Core;
using Alaris.Extras;
using Alaris.Irc;
using Alaris.Irc.Ctcp;
using Alaris.Threading;

namespace Alaris
{
    /// <summary>
    ///   The main class for Alaris.
    /// </summary>
    public partial class AlarisBot : IThreadContext, IDisposable
    {
        private Connection _connection;
        private readonly ScriptManager _manager;
        private string _nick;
        private string _server;
        private bool _confdone;
        private bool _nickserv;
        private string _nspw = "";
        private readonly List<string> _channels = new List<string>();
        private string _anick, _auser, _ahost;
        private readonly CrashHandler _sCrashHandler = Singleton<CrashHandler>.Instance;
        private readonly DatabaseManager _sDatabaseManager = Singleton<DatabaseManager>.Instance;
        private readonly Guid _guid = Guid.NewGuid();
        private readonly string _configfile;
        private const int ListenerPort = 35221;
        private const string ACSHost = "127.0.0.1";
        private const int ACSPort = 35220;

        /// <summary>
        ///   MySQL support enabled or not.
        /// </summary>
        public bool MysqlEnabled;

        /// <summary>
        ///   MySQL data (host, user etc.).
        ///   Size: 4 (DB is last)
        /// </summary>
        public readonly string[] MysqlData = new string[4];

        /// <summary>
        ///   The bot's crash handler instance.
        /// </summary>
        public CrashHandler CrashHandler
        {
            get { return _sCrashHandler; }
        }

        /// <summary>
        ///   The bot's script manager instance.
        /// </summary>
        public ScriptManager ScriptManager
        {
            get { return _manager; }
        }

        /// <summary>
        ///   Determines whether the communication to and dependance of alaris_server is set.
        /// </summary>
        public static readonly bool AlarisServer;

        /// <summary>
        ///   The acs_rand_request_channel.
        /// </summary>
        public string AcsRandRequestChannel;

        /// <summary>
        ///   Gets or sets the thread pool.
        /// </summary>
        /// <value>
        ///   The thread pool.
        /// </value>
        public CThreadPool Pool { get; private set; }

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
            Log.Notice("Alaris", "Initalizing...");
            _configfile = config;
            CrashHandler.HandleReadConfig(ReadConfig, _configfile);
            var cargs = new ConnectionArgs(_nick, _server);
            Log.Debug("Identd", "Starting service...");
            Identd.Start(_nick);
            Log.Success("Identd", "Service daemon running.");
            Log.Notice("Alaris", "Setting up connection...");
            Pool = new CThreadPool(4);

            _connection = new Connection(cargs, true, false);
            var responder = new CtcpResponder(_connection)
                                {
                                    VersionResponse = "Alaris " + Utilities.BotVersion,
                                    SourceResponse = "http://www.wowemuf.org",
                                    UserInfoResponse = "Alaris multi-functional bot."
                                };

            _connection.CtcpResponder = responder;
            Log.Notice("Alaris", "Text encoding: UTF-8");
            _connection.TextEncoding = Encoding.GetEncoding("Latin1");

            Log.Success("CTCP", "Enabled.");

            Log.Notice("ScriptManager", "Initalizing...");
            _manager = new ScriptManager(ref _connection, ref _channels);
            //_manager.LoadPlugins();
            //Thread.Sleep(2000);
            //Log.Success("ScriptManager", "Setup complete");

            Pool.Enqueue(_manager);

            SetupHandlers();
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

            // start database server.
            if (MysqlEnabled)
            {
                _sDatabaseManager.Initialize(MysqlData[0], MysqlData[1], MysqlData[2], MysqlData[3]);
            }

            Connect();
        }


        private void SetupHandlers()
        {
            Log.Notice("ScriptManager", "Setting up event handlers.");
            _manager.RegisterOnRegisteredHook(OnRegistered);
            _manager.RegisterOnPublicHook(OnPublicMessage);
            _connection.CtcpListener.OnCtcpRequest += OnCtcpRequest;
            Log.Success("ScriptManager", "Event handlers are properly setup.");
        }


        /// <summary>
        ///   Reads and parses the specified config file.
        /// </summary>
        /// <param name = "configfile">
        ///   The config file name.
        /// </param>
        private void ReadConfig(string configfile)
        {
            if (!File.Exists("./" + configfile))
                throw new FileNotFoundException(
                    "The config file specified could not be found. It is essential to have a configuration file in the directory of the bot. " +
                    configfile + " could not be found.");

            // read conf file.
            Log.Notice("Config", "Reading configuration file: " + configfile);

            var config = new XmlSettings(configfile, "alaris");

            _server = config.GetSetting("config/irc/server", "irc.rizon.net");
            _nick = config.GetSetting("config/irc/nickname", "alaris");
            _nspw = config.GetSetting("config/irc/nickserv", "nothing");

            _nickserv = (_nspw != "nothing");

            var chans = config.GetSetting("config/irc/channels", "#skullbot,#hun_bot");
            var clist = chans.Split(',');

            foreach (var chan in clist.Where(Rfc2812Util.IsValidChannelName))
                _channels.Add(chan);

            Utilities.AdminNick = config.GetSetting("config/irc/admin/nick", "Twl");
            Utilities.AdminUser = config.GetSetting("config/irc/admin/user", "Twl");
            Utilities.AdminHost = config.GetSetting("config/irc/admin/host", "evil.from.behind");

            MysqlEnabled = Convert.ToBoolean(config.GetSetting("config/mysql/enabled", "false"));

            if (MysqlEnabled)
            {
                Log.Notice("MySQL", "Enabled.");
                MysqlData[0] = config.GetSetting("config/mysql/hostname", "localhost");
                MysqlData[1] = config.GetSetting("config/mysql/username", "root");
                MysqlData[2] = config.GetSetting("config/mysql/password", "pw");
                MysqlData[3] = config.GetSetting("config/mysql/database", "alaris");
            }
            else
                Log.Notice("MySQL", "Disabled.");

            Log.Success("Config", "File read and validated successfully.");
            _confdone = true;

            Log.Notice("Config", string.Format("Connect to: {0} with nick {1}", _server, _nick));
        }

        /// <summary>
        ///   Sends the packet to ACS.
        /// </summary>
        /// <param name = 'packet'>
        ///   Packet.
        /// </param>
        public static void SendPacketToACS(AlarisPacket packet)
        {
            if (packet == null) throw new ArgumentNullException("packet");
            if (!AlarisServer)
                return;

            var client = new TcpClient();
            try
            {
                var ip = IPAddress.Parse(ACSHost);
                if (ip == null) return;

                var endp = new IPEndPoint(ip, ACSPort);
                client.Connect(endp);

                Thread.Sleep(300);


                if (client.Connected)
                {
                    var stream = client.GetStream();
                    Log.Debug("AlarisServer", "Connected. Sending packet.");

                    var encoder = new UTF8Encoding();

                    byte[] buffer = encoder.GetBytes(packet.GetNetMessage());

                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();

                    Log.Debug("AlarisServer", "Packet sent.");

                    stream.Close();

                    Log.Debug("AlarisServer", "Connection closed.");
                }
            }
            catch
            {
                Log.Error("Alaris", "Couldn't send ACS packet.");
            }
            finally
            {
                client.Close();
            }
        }

        /// <summary>
        ///   Establishes the connection to the previously specified server.
        /// </summary>
        private void Connect()
        {
            if (!_confdone)
                throw new Exception("The config file has not been read before connecting.");

            Log.Notice("Alaris", "Establishing connection...");
            try
            {
                _connection.Connect();
            }
            catch (Exception x)
            {
                Log.Error("Alaris", x.Message);
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
            Log.Notice("Alaris", "Disconnecting...");
            Pool.Free();

            if (Identd.IsRunning())
            {
                Identd.Stop();
                Log.Success("Identd", "Stopped service daemon");
            }

            try { _connection.Disconnect(rsr);}
            catch(InvalidOperationException)
            {
            }
        }

        /// <summary>
        ///   Method run when the bot is registered to the IRC server.
        /// </summary>
        private void OnRegistered()
        {
            // Stop Identd, no need for it anymore.
            Identd.Stop();
            Log.Success("Identd", "Stopped service daemon");
            Log.Success("Alaris", "Bot registered on server");

            // join channels here

            foreach (var chan in _channels)
            {
                if (Rfc2812Util.IsValidChannelName(chan))
                    _connection.Sender.Join(chan);

                Log.Notice("Alaris", "Joined channel: " + chan);
            }

            _manager.RunRegisteredHandlers();
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
            Log.Notice("CTCP", "Received command " + command + " from " + user.Nick);
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
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using ICSharpCode.SharpZipLib;
using System.Reflection;
using System.Threading;
using System.Timers;
using Alaris.Irc;
using Alaris.Extras;
using System.Net.Sockets;


namespace Alaris.Core
{
	/// <summary>
	/// The main class for Alaris.
	/// </summary>
	public partial class AlarisBot : IThreadContext
	{
		private Connection _connection;
		private readonly ScriptManager _manager;
		private string _nick = "alaris";
		private string _server = "irc.rizon.net";
		private bool _confdone = false;
		private bool _nickserv = false;
		private string _nspw = "";
		private List<string> _channels = new List<string>();
		private string _anick, _auser, _ahost;
		private CrashHandler sCrashHandler = Singleton<CrashHandler>.Instance;
		private DatabaseManager sDatabaseManager = Singleton<DatabaseManager>.Instance;
		private readonly Guid _guid = Guid.NewGuid();
		private readonly string _configfile;
		private const int listener_port = 35221;
		private const string ACS_HOST = "127.0.0.1";
		private const int ACS_PORT= 35220;
		/// <summary>
		/// MySQL support enabled or not.
		/// </summary>
		public bool MysqlEnabled = false;
		/// <summary>
		/// MySQL data (host, user etc.).
		/// Size: 4 (DB is last)
		/// </summary>
		public string[] MysqlData = new string[4];
		
		/// <summary>
		/// Determines whether the communication to and dependance of alaris_server is set.
		/// </summary>
		public readonly static bool AlarisServer = false;
		
		/// <summary>
		/// The acs_rand_request_channel.
		/// </summary>
		public string acs_rand_request_channel;
		
		/// <summary>
		/// Gets or sets the thread pool.
		/// </summary>
		/// <value>
		/// The thread pool.
		/// </value>
		public CThreadPool Pool { get; private set; }
		
		private AlarisBot() : this("alaris.conf") {}
		
		/// <summary>
		/// Creates a new instacne of Alaris bot.
		/// </summary>
		private AlarisBot (string config)
		{
			Log.Notice("Alaris", "Initalizing...");
			_configfile = config;
			var cargs = new ConnectionArgs(_nick, _server);
			Log.Debug("Identd", "Starting service...");
			Identd.Start(_nick);
			Log.Success("Identd", "Service daemon running.");
			Log.Notice("Alaris", "Setting up connection...");
			Pool = new CThreadPool(4);
			
			_connection = new Connection(cargs, true, false);
			var responder = new CtcpResponder(_connection);
			responder.VersionResponse = "Alaris " + Utilities.GetBotVersion();
			responder.SourceResponse = "http://www.wowemuf.org";
			responder.UserInfoResponse = "Alaris multi-functional bot.";
			
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
		/// Gets the listener port.
		/// </summary>
		/// <returns>
		/// The listener port.
		/// </returns>
		public int GetListenerPort() { return listener_port; }
		
		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="Alaris.Core.AlarisBot"/>
		/// is reclaimed by garbage collection.
		/// </summary>
		~AlarisBot()
		{
			Log.Debug("Alaris", "~AlarisBot()");
		}
		
		/// <summary>
		/// Gets the GUID.
		/// </summary>
		public Guid GetGuid() { return _guid; }
		
		/// <summary>
		/// Run this instance.
		/// </summary>
		public void Run()
		{
			var ls = new List<Exception>();
			sCrashHandler.HandleReadConfig(ReadConfig, _configfile, ref ls);
			ls.Clear();
			
			// start database server.
			if(MysqlEnabled)
			{	
				sDatabaseManager.Initialize(MysqlData[0], MysqlData[1], MysqlData[2], MysqlData[3]);
			}
			
			Connect();
		}
		
		/// <summary>
		/// Gets the class' current crash handler.
		/// </summary>
		/// <returns>
		/// The crash handler.
		/// </returns>
		public CrashHandler GetCrashHandler() { return sCrashHandler; }
		
		private void SetupHandlers()
		{
			Log.Notice("ScriptManager", "Setting up event handlers.");
			_manager.RegisterOnRegisteredHook(OnRegistered);
			_manager.RegisterOnPublicHook(OnPublicMessage);
			_connection.CtcpListener.OnCtcpRequest += OnCtcpRequest;
			Log.Success("ScriptManager", "Event handlers are properly setup.");
		}
		
		
		
		/// <summary>
		/// Reads and parses the specified config file.
		/// </summary>
		/// <param name="configfile">
		/// The config file name.
		/// </param>
		public void ReadConfig(string configfile)
		{
			if(!File.Exists("./" + configfile))
				throw new FileNotFoundException("The config file specified could not be found. It is essential to have a configuration file in the directory of the bot.");
			
			// read conf file.
			Log.Notice("Config", "Reading configuration file: " + configfile);
			var reader = new StreamReader("./"+configfile);
			string config = reader.ReadToEnd();
			reader.Close();
			reader.Dispose();
			
			
			var serverRegex = new Regex(@"server\s=\s(?<server>\S+)");
			if(!serverRegex.IsMatch(config))
				throw new ConfigFileInvalidException("The specified configuration file is invalid.");
			
			_server = serverRegex.Match(config).Groups["server"].ToString();
			
			var nickRegex = new Regex(@"nick\s=\s(?<nick>\S+)");
			if(!nickRegex.IsMatch(config))
				throw new ConfigFileInvalidException("The specified configuration file is invalid.");
			
			_nick = nickRegex.Match(config).Groups["nick"].ToString();
			
			
			var nickservRegex = new Regex(@"nickserv\s=\s(?<nickserv>\S+)");
			
			if(!nickservRegex.IsMatch(config))
				throw new ConfigFileInvalidException("The specified configuration file is invalid.");
			
			_nspw = nickservRegex.Match(config).Groups["nickserv"].ToString();
			_nickserv = (_nspw == "nothing") ? false : true;
			
			var chanRegex = new Regex(@"channels\s=\s(?<channels>\S+)");
			
			if(!chanRegex.IsMatch(config))
				throw new ConfigFileInvalidException("The specified configuration file is invalid.");
			
			string chn = chanRegex.Match(config).Groups["channels"].ToString();
			string[] chns = chn.Split(',');
			
			foreach(string cs in chns)
				if(!string.IsNullOrEmpty(cs))
					_channels.Add(cs);
			
			
			var admRegex = new Regex(@"admin_data\s=\s(?<adm>\S+)");
			if(!admRegex.IsMatch(config))
				throw new ConfigFileInvalidException("The specified configuration file is invalid.");
			
			string[] adms = admRegex.Match(config).Groups["adm"].ToString().Split(',');
			_auser = adms[0];
			_anick = adms[1];
			_ahost = adms[2];
			
			_confdone = true;
			Utilities.AdminUser = _auser;
			Utilities.AdminNick = _anick;
			Utilities.AdminHost = _ahost;
			
			var mysqlRegex = new Regex(@"mysql_enabled\s=\s(?<enabled>\d)\s*\n*\r*mysql_data=(?<host>\S+),(?<user>\S+),(?<pass>\S+),(?<db>\S+)");
			
			if(!mysqlRegex.IsMatch(config))
				throw new ConfigFileInvalidException("The specified configuration file is invalid.");
			
			var msmatch = mysqlRegex.Match(config);
			MysqlEnabled = (msmatch.Groups["enabled"].ToString() == "1");
			
			if(MysqlEnabled)
			{
				Log.Debug("Alaris", "MySQL support is ON.");
				MysqlData[0] = msmatch.Groups["host"].ToString();
				MysqlData[1] = msmatch.Groups["user"].ToString();
				MysqlData[2] = msmatch.Groups["pass"].ToString();
				MysqlData[3] = msmatch.Groups["db"].ToString();
			}
			else
				Log.Debug("Alaris", "MySQL support is OFF.");
			
			
			Log.Success("Config", "File read and validated successfully.");

			
		}
		
		/// <summary>
		/// Sends the packet to ACS.
		/// </summary>
		/// <param name='packet'>
		/// Packet.
		/// </param>
		public void SendPacketToACS(AlarisPacket packet)
		{
			if(!AlarisServer)
				return;
			
			var client = new TcpClient();
			try
			{
				
				var endp = new IPEndPoint(IPAddress.Parse(ACS_HOST), ACS_PORT);
				client.Connect(endp);
				
				Thread.Sleep(300);
				
				if(client.Connected)
				{
					
					var stream = client.GetStream();
					Log.Debug("AlarisServer", "Connected. Sending packet.");
	
					var encoder = new UTF8Encoding();
					
					byte[] buffer = encoder.GetBytes(packet.GetNetMessage());
					
					stream.Write(buffer, 0, buffer.Length);
					stream.Flush();
					
					Log.Debug("AlarisServer", "Packet sent.");
					
					stream.Close();
					
					client.Close();
					
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
		/// Establishes the connection to the previously specified server.
		/// </summary>
		public void Connect()
		{
			if(!_confdone)
				throw new Exception("The config file has not been read before connecting.");
			
			Log.Notice("Alaris", "Establishing connection...");
			try { _connection.Connect(); }
			catch(Exception x) { Log.Error("Alaris", x.Message); Identd.Stop(); }
		}
		
		/// <summary>
		/// Disconnects the bot from the IRC server.
		/// </summary>
		/// <param name="rsr">
		/// Reason for disconnect.
		/// </param>
		public void Disconnect(string rsr)
		{
			Log.Notice("Alaris", "Disconnecting...");
			Pool.Free();
			
			if(Identd.IsRunning())
			{
				Identd.Stop();
				Log.Success("Identd", "Stopped service daemon");
			}
			
			_connection.Disconnect(rsr);
		}
		
		/// <summary>
		/// Method run when the bot is registered to the IRC server.
		/// </summary>
		protected void OnRegistered()
		{
			// Stop Identd, no need for it anymore.
			Identd.Stop();
			Log.Success("Identd", "Stopped service daemon");
			Log.Success("Alaris", "Bot registered on server");
			
			// join channels here
			
			foreach(string chan in _channels)
			{
				if(Rfc2812Util.IsValidChannelName(chan))
					_connection.Sender.Join(chan);
				
				Log.Notice("Alaris", "Joined channel: " + chan);
			}
			
			_manager.RunRegisteredHandlers();
			
		}
		
		/// <summary>
		/// Method run when the bot receives a CTCP request.
		/// </summary>
		/// <param name="command">
		/// The CTCP command.
		/// </param>
		/// <param name="user">
		/// Data about the user who sent it.
		/// </param>
		protected void OnCtcpRequest(string command, UserInfo user)
		{
			Log.Notice("CTCP", "Received command " + command + " from " + user.Nick);
		}
	}
	

}
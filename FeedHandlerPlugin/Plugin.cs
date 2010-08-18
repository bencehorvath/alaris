using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Timers;
using Alaris.Irc;
using Alaris.Core;
using Alaris.Extras;
using Timer=System.Timers.Timer;

namespace Alaris.FeedHandlerPlugin
{
	public class AlarisPlugin : IAlarisBasic
	{
		private Timer _timer = new Timer();
		private Connection _connection;
		private List<string> _channels;
		private bool _running = false;
		

		
		/// <summary>
		/// Creates a new instance of MangosRss.
		/// </summary>
		/// <param name="conn">
		/// The IRC connection.
		/// </param>
		/// <param name="channels">
		/// The channels the bot is on.
		/// </param>
		public AlarisPlugin()
		{
			_channels = new List<string>();
		}
		
		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Alaris.FeedHandlerPlugin.AlarisPlugin"/> is reclaimed by garbage collection.
		/// </summary>
		~AlarisPlugin()
		{
			Log.Debug("FeedHandler", "~AlarisPlugin()");
		}
		
		public void Initialize(ref Connection con) {}
		
		public void Initialize(ref Connection con, ref List<string> chans)
		{
			_connection = con;
			_channels = chans;
		}
		
		public void Stop()
		{
			FeedFactory.StopRunners();
		}
	
		public void OnUnload()
		{
			Stop();
			_connection = null;
			_channels = null;
			_timer.Dispose();
			
		}
		
		public void Start()
		{
			var mang = FeedFactory.CreateFeedRunner(new Uri("http://github.com/mangos/mangos/commits/master.atom"), ref _connection, _channels, "mangos", 10000, "@mangos");
			
			var alar = FeedFactory.CreateFeedRunner(new Uri("http://github.com/Twl/alaris/commits/master.atom"), ref _connection, _channels, "alaris", 11000, "@alaris");
			FeedFactory.FeedRunners.Add(mang);
			FeedFactory.FeedRunners.Add(alar);
			
			FeedFactory.StartRunners();
	
				
		}
		
		public void OnLoad()
		{
			if(_connection != null && _channels != null && _timer != null && !_running)
			{
				Start();
			}
		}
		
		

		
		public void OnRegistered()
		{
			Log.Notice("FeedHandler", "Initizalizing...");
			
			//Start();
			
			Log.Success("FeedHandler", "RSS setup correctly.");
		}
		
		public void OnPublicMessage(UserInfo user, string chan, string msg)
		{
			if (msg == "@feeds")
			{
				string s = "";
				
				foreach(var runner in FeedFactory.FeedRunners)
				{
					s += runner.Name + " | ";
				}
				
				_connection.Sender.PublicMessage(chan, s);
			}
			
			if (msg.StartsWith("@feeds start"))
			{
				FeedFactory.StartRunners();
			}
			
			if(msg.StartsWith("@feeds stop"))
			{
				FeedFactory.StopRunners();
			}
		}
		

		public string GetName()
		{
			return "FeedHandlerPlugin";
		}
		
	}
}



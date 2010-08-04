using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Mono.Math;
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
		private string _latest;
		private Connection _connection;
		private List<string> _channels;
		private bool _firstrun = true;
		private bool _running = false;
		private string _lasttit;
		
		
		/// <summary>
		/// The latest hash.
		/// </summary>
		public string Latest { get { return _latest; } }
		/// <summary>
		/// Latest title.
		/// </summary>
		public string LatestTitle { get { return _lasttit; } }
		
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
			Log.Debug("MangosRss", "~AlarisPlugin()");
		}
		
		public void Initialize(ref Connection con) {}
		
		public void Initialize(ref Connection con, ref List<string> chans)
		{
			_connection = con;
			_channels = chans;
		}

		/// <summary>
		/// Starts the timer and all of the process.
		/// </summary>
		public void Start()
		{
			if(_running)
				return;
			
			_timer.Interval = 12000;
			_timer.Elapsed += Handle_timerElapsed;
			_timer.Enabled = true;
			_timer.Start();
			_running = true;
			
		}
		
		/// <summary>
		/// Stops everything.
		/// </summary>
		public void Stop()
		{
			if(!_running)
				return;
			
			_timer.Enabled = false;
			_timer.Elapsed -= Handle_timerElapsed;;
			_timer.Stop();
			_running = false;
		}
		
		public void OnUnload()
		{
			Stop();
			_connection = null;
			_channels = null;
			_timer.Dispose();
			
		}
		
		public void OnLoad()
		{
			if(_connection != null && _channels != null && _timer != null && !_running)
			{
				Start();
			}
		}

		private void Handle_timerElapsed (object sender, ElapsedEventArgs e)
		{
			MangosUpdate();
		}
		
		public void OnRegistered()
		{
			Log.Notice("MangosRSS", "Initizalizing...");
			
			Start();
			
			Log.Success("MangosRSS", "RSS setup correctly.");
		}
		
		public void OnPublicMessage(UserInfo user, string chan, string msg)
		{
			if(msg == "@mangos stop" && Utilities.IsAdmin(user))
			{
				Stop();
				Log.Notice("MangosRSS", "Stopped listening.");
				_connection.Sender.PublicMessage(chan, "Mangos monitor: " + IrcConstants.Red + "stopped.");
				return;
			}
			
			if(msg == "@mangos start" && Utilities.IsAdmin(user))
			{
				Start();
				Log.Notice("MangosRSS", "Started listening.");
				_connection.Sender.PublicMessage(chan, "Mangos monitor: " + IrcConstants.Green + "started.");
				return;
			}
			
			if(msg == "@mangos")
			{
				_connection.Sender.PublicMessage(chan, "Available sub-commands: start | stop | current");
				return;
			}
			
			if(msg == "@mangos current")
			{
				string hash = Latest;
				string href = "http://github.com/mangos/mangos/commit/" + hash;
				string title = LatestTitle;
				_connection.Sender.PublicMessage(chan, IrcConstants.Bold + 
							                                 "MaNGOS: " + 
							                                 IrcConstants.Normal + 
							                                 IrcConstants.DarkGreen + 
							                                 "new commit " + 
							                                 IrcConstants.Normal +
							                                 IrcConstants.Bold + hash);
				
				_connection.Sender.PublicMessage(chan, IrcConstants.Bold + "Info: " + IrcConstants.Normal + title);
				_connection.Sender.PublicMessage(chan, IrcConstants.Bold + "More Info: " + IrcConstants.Normal + href);
				return;
			}
			
		}
		
		/// <summary>
		/// Runs the periodic MaNGOS RSS update.
		/// </summary>
		public void MangosUpdate()
		{
			Log.Notice("MangosRSS", "Running periodic check...");
			var rssUrl = new Uri("http://github.com/mangos/mangos/commits/master.atom");
			var client = new WebClient();
			client.Encoding = Encoding.UTF8;
			var rss = client.DownloadString(rssUrl);
			client.Dispose();
			
			var getDataRegex = new Regex(@"<link\stype=.text/html.\srel=.\S*.\shref=.(?<url>\S+)./>\s*<title>(?<ttl>.+)</title>", RegexOptions.IgnoreCase);
			
			if(getDataRegex.IsMatch(rss))
			{
				var match = getDataRegex.Matches(rss)[0];
				
				var href = match.Groups["url"].ToString();
				//Log.Notice("MangosRSS", "Href is: " + href);
				var title = match.Groups["ttl"].ToString();
				
				//var getHashRegex = new Regex(@"http://github.com/mangos/mangos/commit/(?<hash>\S+)", RegexOptions.IgnoreCase);
				
				var hash = href.Replace("http://github.com/mangos/mangos/commit/", string.Empty);
				
				Log.Success("MangosRSS", "Got hash: " + hash);
				
				if(hash != _latest && !_firstrun)
				{
					foreach(string chan in _channels)
					{
						_connection.Sender.PublicMessage(chan, IrcConstants.Bold + 
						                                 "MaNGOS: " + 
						                                 IrcConstants.Normal + 
						                                 IrcConstants.DarkGreen + 
						                                 "new commit " + 
						                                 IrcConstants.Normal +
						                                 IrcConstants.Bold + hash);
						
						_connection.Sender.PublicMessage(chan, IrcConstants.Bold + "Info: " + IrcConstants.Normal + title);
						Thread.Sleep(20);
					}
				}
				
				_latest = hash;
				_lasttit = title;
				_firstrun = false;
			}
		}
		
		public string GetName()
		{
			return "MangosRssPlugin";
		}
		
	}
}



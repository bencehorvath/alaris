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
using Atom.Core;



namespace Alaris.FeedHandlerPlugin
{
	public class FeedRunner
	{
		private readonly Uri _feedUrl;
		private Timer _timer = new Timer();
		private string _latest;
		private readonly Connection _connection;
		private readonly List<string> _channels;
		private bool _firstrun = true;
		private bool _running = false;
		private string _lasttit;
		private readonly string _module;
		private readonly int _interval;
		private readonly string _commandPrefix;
		
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get { return _module; } }
		
		/// <summary>
		/// The latest hash.
		/// </summary>
		public string Latest { get { return _latest; } }
		/// <summary>
		/// Latest title.
		/// </summary>
		public string LatestTitle { get { return _lasttit; } }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Alaris.FeedHandlerPlugin.FeedRunner"/> class.
		/// </summary>
		/// <param name='feedurl'>
		/// Feed url.
		/// </param>
		/// <param name='con'>
		/// Connection.
		/// </param>
		/// <param name='chans'>
		/// Channels.
		/// </param>
		/// <param name='moduleName'>
		/// Module name.
		/// </param>
		/// <param name='interval'>
		/// Check interval.
		/// </param>
		public FeedRunner (Uri feedurl, ref Connection con, IList<string> chans, string moduleName, int interval, string commandName)
		{
			_feedUrl = feedurl;
			_connection = con;
			_channels = (List<string>)chans;
			_module = moduleName;
			_interval = interval;
			_commandPrefix = commandName;
		}
		
		/// <summary>
		/// Starts the timer and all of the process.
		/// </summary>
		public void Start()
		{
			if(_running)
				return;
			
			_timer.Interval = _interval;
			_timer.Elapsed += Handle_timerElapsed;
			_timer.Enabled = true;
			_timer.Start();
			_running = true;
			
		
		}
		
		/// <summary>
		/// Stop this instance.
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
		
		private void Handle_timerElapsed (object sender, ElapsedEventArgs e)
		{
			FeedUpdate();
		}
		
		/// <summary>
		/// Feed updater procedure.
		/// </summary>
		public void FeedUpdate()
		{
			Log.Debug(_module, "Running feed check...");
			
			var feed = AtomFeed.Load(_feedUrl);
			
			var entry = feed.Entries[0]; // the first entry.
			
			var tm = entry.Links[0].HRef.ToString().Split('/');
			var hash = tm[(tm.Length-1)];
			
			Log.Debug(_module, "Got hash: " + hash);
			
			if(_firstrun)
			{
				_lasttit = entry.Title.Content;
				_latest = hash;
				_firstrun = false;
				return;
			}
			
			if(_lasttit != entry.Title.Content)
			{
				var title = entry.Title.Content;
				foreach(string chan in _channels)
				{
					_connection.Sender.PublicMessage(chan, IrcConstants.Bold + 
					                                 _module + ": " + 
					                                 IrcConstants.Normal + 
					                                 IrcConstants.DarkGreen + 
					                                 "new commit " + 
					                                 IrcConstants.Normal +
					                                 IrcConstants.Bold + hash);
					
					_connection.Sender.PublicMessage(chan, IrcConstants.Bold + "Info: " + IrcConstants.Normal + title);
					Thread.Sleep(20);
				}
				
				_lasttit = title;
				_latest = hash;
			}
			
		
		}
		
	}
}


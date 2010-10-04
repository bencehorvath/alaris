using System;
using System.Text;
using Alaris.Irc.Delegates.Ctcp;

namespace Alaris.Irc.Ctcp
{
	/// <summary>
	/// A class which automatically responds to CTCP queries. The
	/// replies it sends are configurable by the client.
	/// </summary>
	public sealed class CtcpResponder
	{
		private readonly Connection _connection;
		private long _nextTime;
		private double _floodDelay;
		private string _fingerMessage;
		private string _userInfoMessage;
		private string _versionMessage;
		private string _sourceMessage;
		private string _clientInfoMessage;

		/// <summary>
		/// Create an instance and register handlers for
		/// CTCP events. The Connection's CtcpEnable property must
		/// be set to true or the connection will not send CTCP events
		/// to this responder.
		/// </summary>
		/// <param name="connection">The containing connection.</param>
		public CtcpResponder( Connection connection )
		{
			_connection = connection;
			_nextTime = DateTime.Now.ToFileTime();
			//Wait at least 2 second in between automatic CTCP responses
			_floodDelay = 2000;
			//Send back user nick by default for finger requests.
			_userInfoMessage = "Alaris CTCP";		
			_fingerMessage = _userInfoMessage;
			_versionMessage = "Alaris IRC 2.5 (based on Thresher 1.1)";
            _sourceMessage = "http://github.com/twl/alaris/";
			_clientInfoMessage = "This client supports: UserInfo, Finger, Version, Source, Ping, Time and ClientInfo";
			if( connection.EnableCtcp ) 
			{
				connection.CtcpListener.OnCtcpRequest += OnCtcpRequest;
				connection.CtcpListener.OnCtcpPingRequest += OnCtcpPingRequest;
			}
		}

		/// <summary>
		/// How long the responder should wait before
		/// replying to a query. Queries coming before this
		/// time has passed will be droppped.
		/// </summary>
		/// <value>The delay in milliseconds. The default is 2000 (2 seconds).</value>
		public double ResponseDelay 
		{
			get 
			{
				return _floodDelay;
			}
			set
			{
				_floodDelay = value;
			}
		}
		/// <summary>
		/// Finger responses normally consist of a message
		/// and the idle time.
		/// </summary>
		/// <value>The Idle time will be automatically appended
		/// to the finger response. This default to the UserInfo message.</value>
		public string FingerResponse
		{
			get
			{
				return _fingerMessage + " Idle time " + FormatIdleTime() + ".";
			}
			set
			{
				_fingerMessage = value;
			}
		}
		/// <summary>
		/// A message about the user.
		/// </summary>
		/// <value>Any string which does not exceed the IRC max length.
		/// This defaults to "Thresher Auto-Responder".</value>
		public string UserInfoResponse
		{
			get
			{
				return _userInfoMessage;
			}
			set
			{
				_userInfoMessage = value;
			}
		}
		/// <summary>
		/// The version of the client software.
		/// </summary>
		/// <value>This defaults to "Thresher IRC library 1.0".</value>
		public string VersionResponse
		{
			get
			{
				return _versionMessage;
			}
			set
			{
				_versionMessage = value;
			}
		}
		/// <summary>
		/// Tell others what CTCP commands this client supports.
		/// </summary>
		/// <value>By default it sends a list of all the CTCP commands.</value>
		public string ClientInfoResponse
		{
			get
			{
				return _clientInfoMessage;
			}
			set
			{
				_clientInfoMessage = value;
			}
		}
		/// <summary>
		/// Where to get this client.
		/// </summary>
		/// <value>This can be a complex set of FTP instructions or just a
		/// URL to the client's homepage.</value>
		public string SourceResponse
		{
			get
			{
				return _sourceMessage;
			}
			set
			{
				_sourceMessage = value;
			}
		}

		/// <summary>
		/// For a TimeSpan to show only hours,minutes, and seconds.
		/// </summary>
		/// <returns>A beautified TimeSpan</returns>
		private string FormatIdleTime() 
		{
			StringBuilder builder = new StringBuilder();
			builder.Append( _connection.IdleTime.Hours + " Hours, " );
			builder.Append( _connection.IdleTime.Minutes + " Minutes, " );
			builder.Append( _connection.IdleTime.Seconds + " Seconds" );
			return builder.ToString();
		}
		/// <summary>
		/// Format the current date into date, time, and time zone. Used
		/// by Time replies.
		/// </summary>
		/// <returns>A beautified DateTime</returns>
		private static string FormatDateTime() 
		{
			var time = DateTime.Now;
			var builder = new StringBuilder();
			builder.Append( time.ToLongDateString() + " ");
			builder.Append( time.ToLongTimeString() + " " );
			builder.Append( "(" + TimeZone.CurrentTimeZone.StandardName + ")" );
			return builder.ToString();
		}
		/// <summary>
		/// Create the next time period and adding the correct number
		/// of ticks. No Ctcp replies will be sent if the current time is not later
		/// than this value.
		/// </summary>
		private void UpdateTime() 
		{
			_nextTime = DateTime.Now.ToFileTime() + (long)( _floodDelay * TimeSpan.TicksPerMillisecond );
		}
		private void OnCtcpRequest( string command, UserInfo who ) 
		{
			if( DateTime.Now.ToFileTime() > _nextTime ) 
			{
				switch( command ) 
				{
					case CtcpUtil.Finger:
						_connection.CtcpSender.CtcpReply( command, who.Nick, _fingerMessage + " Idle time: " + FormatIdleTime() );
						break;
					case CtcpUtil.Time:
						_connection.CtcpSender.CtcpReply( command, who.Nick, FormatDateTime() );
						break;
					case CtcpUtil.UserInfo:
						_connection.CtcpSender.CtcpReply( command, who.Nick, _userInfoMessage );
						break;
					case CtcpUtil.Version:
						_connection.CtcpSender.CtcpReply( command, who.Nick, _versionMessage );
						break;
					case CtcpUtil.Source:
						_connection.CtcpSender.CtcpReply( command, who.Nick, _sourceMessage );
						break;
					case CtcpUtil.ClientInfo:
						_connection.CtcpSender.CtcpReply( command, who.Nick, _clientInfoMessage );
						break;
					default:
						string error = command + " is not a supported Ctcp query.";
						_connection.CtcpSender.CtcpReply( command, who.Nick, error );
						break;
				}
				UpdateTime();
			}
		}
		private void OnCtcpPingRequest( UserInfo who, string timestamp ) 
		{
			_connection.CtcpSender.CtcpPingReply( who.Nick, timestamp );
		}

		/// <summary>
		/// Stop listening to the CtcpListener.
		/// </summary>
		internal void Disable() 
		{
			_connection.CtcpListener.OnCtcpRequest -= OnCtcpRequest;
			_connection.CtcpListener.OnCtcpPingRequest -= OnCtcpPingRequest;
		}

	}
}

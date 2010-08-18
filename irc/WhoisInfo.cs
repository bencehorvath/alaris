using System;
using System.Globalization;


namespace Alaris.Irc
{
	/// <summary>
	/// The collection of information about a user 
	/// returned by a Whois query. Instances of this class
	/// are created internally.
	/// </summary>
	public sealed class WhoisInfo
	{
		internal UserInfo userInfo;
		internal string realName;
		internal string[] channels;
		internal string ircServer;
		internal string serverDescription;
		internal long idleTime;
		internal bool isOperator;

		/// <summary>
		/// Create an empty instance where the operator
		/// property defaults to false.
		/// </summary>
		internal WhoisInfo() 
		{
			isOperator = false;
		}

		/// <summary>
		/// A user's nick, logon, and hostname.
		/// </summary>
		/// <value>A UserInfo instance.</value>
		public UserInfo User
		{
			get
			{
				return userInfo;
			}
		}
		/// <summary>
		/// A user's real name.
		/// </summary>
		/// <value>A string</value>
		public string RealName
		{
			get
			{
				return realName;
			}
		}
		/// <summary>
		/// The name of IRC server.
		/// </summary>
		/// <value>The IRC server FQDN hostname string.</value>
		public string Server
		{
			get
			{
				return ircServer;
			}
		}
		/// <summary>
		/// Text describing the IRC server.
		/// </summary>
		/// <value>A string describing the IRC network this server is a member of.</value>
		public string ServerDescription
		{
			get
			{
				return serverDescription;
			}
		}
		/// <summary>
		/// User's idle time in seconds.
		/// </summary>
		/// <value>Seconds as a long.</value>
		public long IdleTime
		{
			get
			{
				return idleTime;
			}
		}
		/// <summary>
		/// Whether the user is an operator or not.
		/// </summary>
		/// <value>True if the user is an IRC operator.</value>
		public bool Operator
		{
			get
			{
				return isOperator;
			}
		}

		internal void SetChannels( string[] channels ) 
		{
			this.channels = channels;
		}

		/// <summary>
		/// An array of channel names. Names may have =,@, or + prefixed to them.
		/// </summary>
		/// <returns>A string array.</returns>
		public string[] GetChannels()
		{
			return channels;
		}

	}
}

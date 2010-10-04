namespace Alaris.Irc
{
	/// <summary>
	/// A collection of parameters necessary to establish
	/// an IRC connection.
	/// </summary>
	
	public struct ConnectionArgs
	{
		private string _realName;
		private string _nickName;
		private string _userName;
		private string _modeMask;
		private string _hostname;
		private int _port;
		private string _serverPassword;

		/// <summary>
		/// Create a new instance initialized with the default values:
		/// TCP/IP port 6667, no server password, and user mode
		/// invisible.
		/// </summary>
		/// <param name="name">The nick, user name, and real name are 
		/// all set to this value.</param>
		/// <param name="hostname">The hostname of the IRC server.</param>
		public ConnectionArgs( string name, string hostname ) 
		{
			_realName = name;
			_nickName = name;
			_userName = name;
			_modeMask = "4";
			_hostname = hostname;
			_port = 6667;
			_serverPassword = "*";
		}

		/// <summary>
		/// The IRC server hostname
		/// </summary>
		/// <value>The full hostname such as irc.gamesnet.net</value>
		public string Hostname
		{
			get
			{
				return _hostname;
			}
			set
			{
				_hostname = value;
			}
		}
		/// <summary>
		/// Set's the user's initial IRC mode mask. Set to 0 to recieve wallops
		/// and be invisible. Set to 4 to be invisible and not receive wallops.
		/// </summary>
		/// <value>A number mask such as 0 or 4.</value>
		public string ModeMask
		{
			get
			{
				return _modeMask;
			}
			set
			{
				_modeMask = value;
			}
		}
		/// <summary>
		/// The user's nick name.
		/// </summary>
		/// <value>A string which conforms to the IRC nick standard.</value>
		public string Nick
		{
			get
			{
				return _nickName;
			}
			set
			{
				_nickName = value;
			}
		}
		/// <summary>
		/// The TCP/IP port the IRC listens server listens on.
		/// </summary>
		/// <value> Normally should be set to 6667. </value>
		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}
		/// <summary>
		/// The user's 'real' name.
		/// </summary>
		/// <value>A short string with any legal characters.</value>
		public string RealName
		{
			get
			{
				return _realName;
			}
			set
			{
				_realName = value;
			}
		}
		/// <summary>
		/// The user's machine logon name.
		/// </summary>
		/// <value>A short string with any legal characters.</value>
		public string UserName
		{
			get
			{
				return _userName;
			}
			set
			{
				_userName = value;
			}
		}
		/// <summary>
		/// The password for this server. These are seldomly used. Set to '*' 
		/// </summary>
		/// <value>A short string with any legal characters.</value>
		public string ServerPassword
		{
			get
			{
				return _serverPassword;
			}
			set
			{
				_serverPassword = value;
			}
		}
	}
}

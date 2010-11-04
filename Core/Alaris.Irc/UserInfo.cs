namespace Alaris.Irc
{
	/// <summary>
	/// A convenient holder of user information. Instances of this class
	/// are created internally.
	/// </summary>
	public class UserInfo 
	{
		/// <summary>The user's handle.</summary>
		private readonly string _nickName;
		/// <summary>The user's username on the local machine</summary>
		private readonly string _userName;
		/// <summary>The user's fully qualified host name</summary>
		private readonly string _hostName;
		private static readonly UserInfo EmptyInstance = new UserInfo();

		/// <summary>
		/// Creat an empty instance
		/// </summary>
		private UserInfo() 
		{
			_nickName = "";
			_userName = "";
			_hostName = "";
		}
		/// <summary>
		/// Create a new UserInfo and set all its values.
		/// </summary>
		public UserInfo(string nick, string name, string host) 
		{
			_nickName = nick;
			_userName = name;
			_hostName = host;
		}

		/// <summary>
		/// An IRC user's nick name.
		/// </summary>
		public string Nick
		{
			get 
			{
				return _nickName;
			}
		}
		/// <summary>
		/// An IRC user's system username.
		/// </summary>
		public string User
		{
			get
			{
				return _userName;
			}
		}
		/// <summary>
		/// The hostname of the IRC user's machine.
		/// </summary>
		public string Hostname 
		{
			get 
			{
				return _hostName;
			}
		}
		/// <summary>
		/// A singleton blank instance of UserInfo used when an instance is required
		/// by a method signature but no infomation is available, e.g. the last reply
		/// from a Who request.
		/// </summary>
		public static UserInfo Empty 
		{
			get
			{
				return EmptyInstance;
			}
		}

		/// <summary>
		/// A string representation of this object which
		/// shows all its values.
		/// </summary>
		public override string ToString() 
		{
			return string.Format("Nick={0} User={1} Host={2}", Nick, User, Hostname ); 
		}
	}
}

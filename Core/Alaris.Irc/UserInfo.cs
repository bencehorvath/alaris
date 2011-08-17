using System;

namespace Alaris.Irc
{
	/// <summary>
	/// A convenient holder of user information. Instances of this class
	/// are created internally.
	/// </summary>
	public class UserInfo : IEquatable<UserInfo>
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
	    /// Indicates whether the current object is equal to another object of the same type.
	    /// </summary>
	    /// <returns>
	    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
	    /// </returns>
	    /// <param name="other">An object to compare with this object.</param>
	    public bool Equals(UserInfo other)
	    {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return Equals(other._nickName, _nickName) && Equals(other._userName, _userName) && Equals(other._hostName, _hostName);
	    }

        public static bool operator==(UserInfo one, UserInfo another)
        {
            return one.Equals(another);
        }

	    public static bool operator !=(UserInfo one, UserInfo another)
	    {
	        return !(one == another);
	    }

	    /// <summary>
		/// A string representation of this object which
		/// shows all its values.
		/// </summary>
		public override string ToString() 
		{
			return string.Format("Nick={0} User={1} Host={2}", Nick, User, Hostname ); 
		}

	    /// <summary>
	    /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
	    /// </summary>
	    /// <returns>
	    /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
	    /// </returns>
	    /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != typeof (UserInfo)) return false;
	        return Equals((UserInfo) obj);
	    }

	    /// <summary>
	    /// Serves as a hash function for a particular type. 
	    /// </summary>
	    /// <returns>
	    /// A hash code for the current <see cref="T:System.Object"/>.
	    /// </returns>
	    /// <filterpriority>2</filterpriority>
	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            int result = (_nickName != null ? _nickName.GetHashCode() : 0);
	            result = (result*397) ^ (_userName != null ? _userName.GetHashCode() : 0);
	            result = (result*397) ^ (_hostName != null ? _hostName.GetHashCode() : 0);
	            return result;
	        }
	    }
	}
}

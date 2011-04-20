using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Alaris.Irc
{

	/// <summary>
	/// RFC 2812 Utility methods.
	/// </summary>
	public static class Rfc2812Util 
	{
		// Regex that matches the standard IRC 'nick!user@host' 
	    // Regex that matches a legal IRC nick 
		private static readonly Regex NickRegex;
		private static readonly Regex EmailRegex;
		//Regex to create a UserInfo from a string
		private static readonly Regex NameSplitterRegex;
		private const string ChannelPrefix = "#!+&";
	    private const string UserModes = "awiorOs";
		private const string ChannelModes = "OohvaimnqpsrtklbeI";
		private const string Space = " ";

		internal static readonly TraceSwitch IrcTrace = new TraceSwitch("IrcTraceSwitch", "Debug level for RFC2812 classes.");
		// Odd chars that IRC allows in nicknames 
		internal const string Special = "\\[\\]\\`_\\^\\{\\|\\}";
		internal const string Nick = "[" + Special + "a-zA-Z][\\w\\-" + Special + "]{0,8}";
		internal const string User = "(" + Nick+ ")!([\\~\\w]+)@([\\w\\.\\-]+)";

		/// <summary>
		/// Static initializer 
		/// </summary>
		static Rfc2812Util() 
		{
		    NickRegex = new Regex( Nick ); 
			EmailRegex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|hu|gov|mil|biz|info|mobi|name|aero|jobs|museum)\b");
			NameSplitterRegex = new Regex("[!@]",RegexOptions.Compiled | RegexOptions.Singleline );
		}

		//Should never be instantiated

	    /// <summary>
		/// Converts the user string sent by the IRC server
		/// into a UserInfo object.
		/// </summary>
		/// <param name="fullUserName">The user in nick!user@host form.</param>
		/// <returns>A UserInfo object.</returns>
		public static UserInfo UserInfoFromString( string fullUserName )
	    {
	        var parts = ParseUserInfoLine( fullUserName );
	        return parts == null ? UserInfo.Empty : new UserInfo(parts[0], parts[1], parts[2]);
	    }

	    /// <summary>
		/// Break up an IRC user string into its component
		/// parts. 
		/// </summary>
		/// <param name="fullUserName">The user in nick!user@host form</param>
		/// <returns>A string array with the first item being nick, then user, and then host.</returns>
		public static string[] ParseUserInfoLine( string fullUserName ) 
		{
			if( fullUserName == null || fullUserName.Trim().Length == 0 ) 
			{
				return null;
			}
			var match = NameSplitterRegex.Match( fullUserName );
			if( match.Success ) 
			{
				var parts = NameSplitterRegex.Split( fullUserName );
				return parts;
			}

	        return new[] { fullUserName, "","" };
		}

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// an array of channel names is valid.
		/// </summary>
		/// <returns>True if the channel names are all valid.</returns>
		public static bool IsValidChannelList( string[] channels )
		{
		    return channels != null && channels.Length != 0 && channels.All(IsValidChannelName);
		}

	    /// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// the channel name is valid.
		/// </summary>
		/// <returns>True if the channel name is valid.</returns>
		public static bool IsValidChannelName(string channel) 
		{
			if (channel == null || channel.Trim().Length == 0 ) 
			{
				return false;
			}

			if( ContainsSpace(channel) ) 
			{
				return false;
			}
			if (ChannelPrefix.IndexOf( channel[0] ) != -1) 
			{
				if (channel.Length <= 50) 
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// the nickname is valid.
		/// </summary>
		/// <returns>True is the nickname is valid</returns>
		public static bool IsValidNick( string nick) 
		{
			if( nick == null || nick.Trim().Length == 0 ) 
			{
				return false;
			}

			return !ContainsSpace( nick ) && NickRegex.IsMatch( nick );
		}
		
		/// <summary>
		/// Check if whether the provided e-mail address is considered valid. 
		/// </summary>
		/// <param name="email">
		/// The e-mail address to check.
		/// </param>
		/// <returns>
		/// True if valid, otherwise False.
		/// </returns>
		public static bool IsValidEmailAddress(string email)
		{
			return (EmailRegex.IsMatch(email));
		}

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// an array of nicknames names is valid.
		/// </summary>
		/// <returns>True if the channel names are all valid.</returns>
		public static bool IsValidNicklList( string[] nicks ) 
		{
			if( nicks == null || nicks.Length == 0 ) 
			{
				return false;
			}

		    return nicks.All(IsValidNick);
		}	

		/// <summary>
		/// Convert a ModeAction into its RFC2812 character.
		/// </summary>
		/// <param name="action">The action enum.</param>
		/// <returns>Either '+' or '-'.</returns>
		public static char ModeActionToChar( ModeAction action ) 
		{
			return Convert.ToChar( (byte) action, CultureInfo.InvariantCulture ) ;
		}

		/// <summary>
		/// Converts the char received from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="action">Either '+' or '-'.</param>
		/// <returns>An action enum.</returns>
		public static ModeAction CharToModeAction( char action ) 
		{
			byte b = Convert.ToByte( action, CultureInfo.InvariantCulture );
			return (ModeAction) Enum.Parse( typeof( ModeAction), b.ToString( CultureInfo.InvariantCulture), false );
		}

		/// <summary>
		/// Converts a UserMode into its RFC2812 character.
		/// </summary>
		/// <param name="mode">The mode enum.</param>
		/// <returns>The corresponding char.</returns>
		public static char UserModeToChar( UserMode mode ) 
		{
			return Convert.ToChar( (byte) mode, CultureInfo.InvariantCulture ) ;
		}

		/// <summary>
		/// Convert a string of UserModes characters to
		/// an array of UserMode enums.
		/// </summary>
		/// <param name="modes">A string of UserMode chars from the IRC server.</param>
		/// <returns>An array of UserMode enums. Charactres that are not from RFC2812 will be droppped.</returns>
		public static UserMode[] UserModesToArray( IEnumerable<char> modes ) 
		{
		    return (from t in modes where IsValidModeChar(t, UserModes) select CharToUserMode(t)).ToArray();
		}

		/// <summary>
		/// Converts the char recived from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="mode">One of the IRC mode characters, e.g. 'a','i', etc...</param>
		/// <returns>An mode enum.</returns>
		public static UserMode CharToUserMode( char mode ) 
		{
			byte b = Convert.ToByte( mode, CultureInfo.InvariantCulture );
			return (UserMode) Enum.Parse( typeof( UserMode), b.ToString(CultureInfo.InvariantCulture), false );
		}

		/// <summary>
		/// Convert a string of ChannelModes characters to
		/// an array of ChannelMode enums.
		/// </summary>
		/// <param name="modes">A string of ChannelMode chars from the IRC server.</param>
		/// <returns>An array of ChannelMode enums. Charactres that are not from RFC2812 will be droppped.</returns>
		public static ChannelMode[] ChannelModesToArray( IEnumerable<char> modes ) 
		{
		    return (from t in modes where IsValidModeChar(t, ChannelModes) select CharToChannelMode(t)).ToArray();
		}

		/// <summary>
		/// Converts a ChannelMode into its RFC2812 character.
		/// </summary>
		/// <param name="mode">The mode enum.</param>
		/// <returns>The corresponding char.</returns>
		public static char ChannelModeToChar( ChannelMode mode ) 
		{
			return Convert.ToChar( (byte) mode, CultureInfo.InvariantCulture ) ;
		}
		/// <summary>
		/// Converts the char recived from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="mode">One of the IRC mode characters, e.g. 'O','i', etc...</param>
		/// <returns>An mode enum.</returns>
		public static ChannelMode CharToChannelMode( char mode ) 
		{
			byte b = Convert.ToByte( mode, CultureInfo.InvariantCulture );
			return (ChannelMode) Enum.Parse( typeof( ChannelMode), b.ToString( CultureInfo.InvariantCulture), false );
		}

		/// <summary>
		/// Converts a StatQuery enum value to its RFC2812 character.
		/// </summary>
		/// <param name="query">The query enum.</param>
		/// <returns>The corresponding char.</returns>
		public static char StatsQueryToChar( StatsQuery query ) 
		{
			return Convert.ToChar( (byte) query, CultureInfo.InvariantCulture ) ;
		}

		/// <summary>
		/// Converts the char recived from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="queryType">One of the IRC stats query characters, e.g. 'u','l', etc...</param>
		/// <returns>An StatsQuery enum.</returns>
		public static StatsQuery CharToStatsQuery( char queryType ) 
		{
			byte b = Convert.ToByte( queryType, CultureInfo.InvariantCulture );
			return (StatsQuery) Enum.Parse( typeof( StatsQuery), b.ToString(CultureInfo.InvariantCulture), false );
		}

		private static bool IsValidModeChar( char c, string validList ) 
		{
			return validList.IndexOf( c ) != -1;
		}

		private static bool ContainsSpace( string text ) 
		{
			return text.IndexOf( Space, 0, text.Length ) != -1;
		}
	}
}

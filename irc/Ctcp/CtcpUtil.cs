using System;
using System.Diagnostics;
using System.Globalization;

namespace Alaris.Irc
{
	/// <summary>
	/// Constants and utility methods to support CTCP.
	/// </summary>
	/// <remarks>The CTCP constants should be used to test incoming
	/// CTCP queries for their type and as the CTCP command
	/// for outgoing ones.</remarks>
	public sealed class CtcpUtil
	{
		/// <summary>CTCP Finger.</summary>
		public const string Finger = "FINGER";
		/// <summary>CTCP USERINFO.</summary>
		public const string UserInfo = "USERINFO";
		/// <summary>CTCP VERSION.</summary>
		public const string Version = "VERSION";
		/// <summary>CTCP SOURCE.</summary>
		public const string Source = "SOURCE";
		/// <summary>CTCP CLIENTINFO.</summary>
		public const string ClientInfo = "CLIENTINFO";
		/// <summary>CTCP ERRMSG.</summary>
		public const string ErrorMessage = "ERRMSG";
		/// <summary>CTCP PING.</summary>
		public const string Ping = "PING";
		/// <summary>CTCP TIME.</summary>
		public const string Time = "TIME";

		internal static TraceSwitch CtcpTrace = new TraceSwitch("CtcpTraceSwitch", "Debug level for CTCP classes.");

		//Should never be called so make it private
		private CtcpUtil(){}

		/// <summary>
		/// Generate a timestamp string suitable for the CTCP Ping command.
		/// </summary>
		/// <returns>The current time as a string.</returns>
		public static string CreateTimestamp() 
		{
			return DateTime.Now.ToFileTime().ToString( CultureInfo.InvariantCulture );
		}

	}
}

using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;
using Alaris.Irc.Delegates.Ctcp;

namespace Alaris.Irc.Ctcp
{
	/// <summary>
	/// This class is used to send CTCP specific events. Once registered with this object 
	/// the client can receive notification of all CTCP requests, pings, and replies. Instances 
	/// of this class are not created directly but are retrieved as a property from a 
	/// <see cref="Connection"/> object.
	/// </summary>
	/// <remarks>All CTCP messages come in Request/Reply pairs. Each event
	/// signals either the Request or the Response. All CTCP queries (with the exception of
	/// CTCP Ping) are very similiar so they are all handled by the same set of events.</remarks>
	///<example><code>
	/// //Create a Connection object which will support CTCP (second bool param).
	/// Connection connection = new Connection( args, true, false );	
	/// //Register a delegate on this CtcpListener.
	/// connection.CtcpListener.OnCtcpRequest += new CtcpRequestEventHandler( MyOnCtcpRequest );
	/// //If the Connection was created without CTCP support then this property will return null.
	/// //However, CTCP can be turned on and off dynamically. To enable it at a later time call:
	/// connection.EnableCtcp = true;
	/// //Now you can register listeners as above.
	/// //Setting EnableCtcp to false will delete the instance of CtcpListener and no more
	/// //CTCP events will be raised.
	///</code></example>
	public class CtcpListener
	{
		/// <summary>
		/// Listens for replies to CTCP queries sent by this client.
		/// </summary>
		public event CtcpReplyEventHandler OnCtcpReply;
		/// <summary>
		/// Listens for CTCP requests.
		/// </summary>
		public event CtcpRequestEventHandler OnCtcpRequest;
		/// <summary>
		/// Listens for a reply to CTCP Ping query sent by this client.
		/// </summary>
		public event CtcpPingReplyEventHandler OnCtcpPingReply;
		/// <summary>
		/// Listens for CTCP Ping requests.
		/// </summary>
		public event CtcpPingRequestEventHandler OnCtcpPingRequest;

        private const string CtcpTypes = "(FINGER|USERINFO|VERSION|SOURCE|CLIENTINFO|ERRMSG|PING|TIME)";
		private static readonly Regex CtcpRegex = new Regex(string.Format(":([^ ]+) [A-Z]+ [^:]+:\u0001{0}([^\u0001]*)\u0001", CtcpTypes), RegexOptions.Compiled | RegexOptions.Singleline );	
		private readonly Connection _connection;
		private const int Name = 0;
		private const int Command = 1;
		private const int Text = 2;

		/// <summary>
		/// Create a new listener using a specific connection.
		/// </summary>
		/// <param name="connection">The connection to the IRC server.</param>
		internal CtcpListener( Connection connection )
		{
			_connection = connection;
		}

		private static bool IsReply( string[] tokens ) 
		{
			if( tokens[ Text ].Length == 0 ) 
			{
				return false;
			}
			return true;
		}

		private static string[] TokenizeMessage( string message ) 
		{
			try 
			{
				Match match = CtcpRegex.Match( message );
				return new string[] { match.Groups[1].ToString(), match.Groups[2].ToString(),match.Groups[3].ToString().Trim()  };
			}
			catch( Exception e ) 
			{
				return null;
			}
		}

		internal void Parse( string line ) 
		{
			string[] ctcpTokens = TokenizeMessage( line );
			if( ctcpTokens != null ) 
			{
				if( ctcpTokens[ Command ].ToUpper( CultureInfo.CurrentCulture ) == CtcpUtil.Ping ) 
				{
					if( _connection.CtcpSender.IsMyRequest( ctcpTokens[ Text] ) ) 
					{
						_connection.CtcpSender.ReplyReceived( ctcpTokens[ Text] );
						if( OnCtcpPingReply != null ) 
						{
							OnCtcpPingReply( Rfc2812Util.UserInfoFromString( ctcpTokens[ Name ] ), ctcpTokens[ Text] ) ;
						}
					}
					else 
					{
						//Ignore PING's with now parameters
						if( ctcpTokens[ Text] != null && ctcpTokens[ Text].TrimEnd().Length != 0  ) 
						{
							if( OnCtcpPingRequest != null ) 
							{
								OnCtcpPingRequest( Rfc2812Util.UserInfoFromString( ctcpTokens[ Name ] ), ctcpTokens[ Text] ) ;
							}
						}
					}
				}
				else 
				{
					if( IsReply( ctcpTokens ) ) 
					{
						if( OnCtcpReply != null ) 
						{
							OnCtcpReply( ctcpTokens[ Command].ToUpper( CultureInfo.CurrentCulture ) ,  Rfc2812Util.UserInfoFromString( ctcpTokens[ Name ] ), ctcpTokens[ Text] );
						}
					}
					else 
					{
						if( OnCtcpRequest != null ) 
						{
							OnCtcpRequest( ctcpTokens[ Command].ToUpper( CultureInfo.CurrentCulture ) , Rfc2812Util.UserInfoFromString( ctcpTokens[ Name ] ));
						}	
					}
				}
			}
			else
			{
				_connection.Listener.Error( ReplyCode.UnparseableMessage, line );
				Debug.WriteLineIf( CtcpUtil.CtcpTrace.TraceWarning, "Unknown CTCP command '" + line + "' recieved by CtcpListener" );
			}
		}

		/// <summary>
		/// Test if the message contains CTCP commands.
		/// </summary>
		/// <param name="message">The raw message from the IRC server</param>
		/// <returns>True if this is a Ctcp request or reply.</returns>
		public static bool IsCtcpMessage( string message ) 
		{
			return CtcpRegex.IsMatch( message );
		}

	}
}

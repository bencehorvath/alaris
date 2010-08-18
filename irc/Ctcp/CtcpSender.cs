using System;
using System.Collections;
using System.Globalization;

namespace Alaris.Irc.Ctcp
{
	/// <summary>
	/// This class allows the client to send CTCP commands. There is no limit
	/// on what commands can actually be sent, however, the accepted CTCP 
	/// commands are: UserInfo, Finger, Version, Source, ClientInfo, ErrMsg, and Time.
	/// 
	/// <remarks>Action, though it is also a CTCP command, is so common that it is handled 
	/// by the normal sender class. </remarks> 
	/// </summary>
	public sealed class CtcpSender : CommandBuilder
	{
		private ArrayList pingList;

		/// <summary>
		/// Create an instance using a specific connection.
		/// </summary>
		/// <param name="connection">The connection to an IRC server.</param>
		internal CtcpSender( Connection connection ) : base( connection )
		{
			pingList = new ArrayList();
		}

		/// <summary>
		/// Test if the timestamp is one that this client sent out.
		/// Needed to distinguish betwen a Ping reply and
		/// query.
		/// </summary>
		/// <param name="timestamp">The timstamp</param>
		/// <returns>True if the timestamp was sent by this client.</returns>
		internal bool IsMyRequest( string timestamp ) 
		{
			return pingList.Contains( timestamp );
		}
		/// <summary>
		/// Remove a timstamp from the list
		/// we are maintaining.
		/// </summary>
		/// <param name="timestamp">The timestamp that was sent back.</param>
		internal void ReplyReceived( string timestamp ) 
		{
			pingList.Remove( timestamp );
		}

		/// <summary>
		/// Send a reply in response to a CTCP request. Replies that exceed
		/// the IRC max length will be truncated.
		/// </summary>
		/// <param name="nick">The target's nick name.</param>
		/// <param name="command">The CTCP command. Should be a string constant
		/// from <see cref="CtcpUtil"/>.</param>
		/// <param name="reply">The text of the response.</param>
		/// <exception cref="ArgumentException">If the nick is invalid, the command 
		/// is empty, or the reply is empty.</exception> 
		/// <see cref="CtcpListener.OnCtcpReply"/>
		public void CtcpReply( string command, string nick, string reply )
		{
		    if (nick == null) throw new ArgumentNullException("nick");
		    lock( this )
			{
				if (!Rfc2812Util.IsValidNick(nick) ) 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nick.");
				}
				if( reply == null || reply.Trim().Length == 0 ) 
				{
					ClearBuffer();
					throw new ArgumentException("Reply cannot be null or empty.");
				}
				if( command == null || command.Trim().Length == 0 ) 
				{
					ClearBuffer();
					throw new ArgumentException("The Ctcp command cannot be null or empty.");
				}
				// 14 is NOTICE + 3 x Spaces + : + CR + LF + 2xCtcpQuote
				int max = MAX_COMMAND_SIZE - 14 - nick.Length - command.Length;
				if (reply.Length > max) 
				{
					reply = reply.Substring(0, max);
				}
				SendMessage("NOTICE", nick, CtcpQuote + command.ToUpper( CultureInfo.InvariantCulture ) + " " + reply + CtcpQuote );
			}
		}

	    /// <summary>
		/// Send a CTCP query to another user.
		/// </summary>
		/// <remarks>The target may also respond with an error
		/// or nothing at all if it cannot or does not want to answer.
		/// </remarks>
		/// <param name="nick">The target's nick name.</param>
		/// <param name="command">The CTCP command. Should be a string constant
		/// from <see cref="CtcpUtil"/>.</param>
		/// <exception cref="ArgumentException">If the nick is invalid or the command is empty.</exception> 
		/// <see cref="CtcpListener.OnCtcpRequest"/>
		public void CtcpRequest( string command, string nick) 
		{
			lock( this )
			{
				if (!Rfc2812Util.IsValidNick(nick) ) 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nick.");
				}
				if( command == null || command.Trim().Length == 0 ) 
				{
					ClearBuffer();
					throw new ArgumentException("The Ctcp command cannot be null or empty.");
				}
				SendMessage("PRIVMSG", nick, CtcpQuote + command.ToUpper( CultureInfo.InvariantCulture ) + CtcpQuote );
			}
		}
		/// <summary>
		/// Send back a timestamp so the requester can calculate his
		/// ping to this client.
		/// </summary>
		/// <param name="nick">The target's nick name.</param>
		/// <param name="timestamp">The timestamp sent by the requester.</param>
		/// <exception cref="ArgumentException">If the nick is invalid or the timestamp is empty.</exception> 
		/// <see cref="CtcpListener.OnCtcpPingReply"/>
		public void CtcpPingReply( string nick, string timestamp ) 
		{
			lock( this )
			{
				if (!Rfc2812Util.IsValidNick(nick) ) 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nick.");
				}
				if( timestamp == null || timestamp.Trim().Length == 0 ) 
				{
					ClearBuffer();
					throw new ArgumentException("Timestamp cannot be null or empty.");
				}
				SendMessage("NOTICE", nick, CtcpQuote + CtcpUtil.Ping + " " + timestamp + CtcpQuote );
			}
		}
		/// <summary>
		/// Send a CTCP Ping request to another user.
		/// </summary>
		/// <remarks>The target may also respond with an error
		/// or nothing at all if it cannot or does not want to answer.
		/// </remarks>
		/// <param name="nick">The target's nick name.</param>
		/// <param name="timestamp">The timestamp to send to the target user. These
		/// can be generated by Thresher (<see cref="CtcpUtil.CreateTimestamp"/>) or
		/// by the client application.</param>
		/// <exception cref="ArgumentException">If the nick is invalid.</exception> 
		/// <see cref="CtcpListener.OnCtcpPingRequest"/>
		public void CtcpPingRequest( string nick, string timestamp ) 
		{
			lock( this )
			{
				if (!Rfc2812Util.IsValidNick(nick) ) 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nick.");
				}
				pingList.Add( timestamp );
				SendMessage("PRIVMSG", nick, CtcpQuote + CtcpUtil.Ping + " " + timestamp + CtcpQuote );
			}
		}

	}
}

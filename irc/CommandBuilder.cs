using System;
using System.Text;


namespace Alaris.Irc
{
	/// <summary>
	/// CommandBuilder provides the support methods needed
	/// by its subclasses to build correctly formatted messages for
	/// the IRC server. It is never itself instantiated.
	/// </summary>
	public abstract class CommandBuilder
	{
		// Buffer to hold commands 
		private StringBuilder commandBuffer;
		//Containing conenction instance
		private Connection connection;

		internal const char SPACE = ' ';
		internal const string SPACE_COLON = " :";
		internal const int MAX_COMMAND_SIZE = 512;
		internal const char CtcpQuote = '\u0001';

		internal CommandBuilder(Connection connection )
		{
			this.connection = connection;
			commandBuffer = new StringBuilder(MAX_COMMAND_SIZE);
		}

		internal Connection Connection 
		{
			get
			{
				return connection;
			}
		}
		internal StringBuilder Buffer
		{
			get
			{
				return commandBuffer;
			}
		}

		/// <summary>
		/// This methods actually sends the notice and privmsg commands.
		/// It assumes that the message has already been broken up
		/// and has a valid target.
		/// </summary>
		internal void SendMessage(string type, string target, string message) 
		{
			commandBuffer.Append(type);
			commandBuffer.Append(SPACE);
			commandBuffer.Append(target);
			commandBuffer.Append(SPACE_COLON);
			commandBuffer.Append(message);
			connection.SendCommand( commandBuffer );
		}
		/// <summary>
		/// Clear the contents of the string buffer.
		/// </summary>
		internal void ClearBuffer() 
		{
			commandBuffer.Remove(0, commandBuffer.Length );
		}
		/// <summary>
		/// Break up a large message into smaller peices that will fit within the IRC
		/// max message size.
		/// </summary>
		/// <param name="message">The text to be broken up</param>
		/// <param name="maxSize">The largest size a piece can be</param>
		/// <returns>A string array holding the correctly sized messages.</returns>
		internal string[] BreakUpMessage(string message, int maxSize) 
		{
			int pieces = (int) Math.Ceiling( (float)message.Length / (float)maxSize );
			string[] parts = new string[ pieces ];
			for( int i = 0; i < pieces; i++ ) 
			{
				int start = i * maxSize;
				if( i == pieces - 1 ) 
				{
					parts[i] = message.Substring( start );	
				}
				else 
				{
					parts[i] = message.Substring( start , maxSize );	
				}
			}
			return parts;
		}
	}
}

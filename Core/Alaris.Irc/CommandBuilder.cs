using System;
using System.Collections.Generic;
using System.Text;

namespace Alaris.Irc
{
    /// <summary>
    ///   CommandBuilder provides the support methods needed
    ///   by its subclasses to build correctly formatted messages for
    ///   the IRC server. It is never itself instantiated.
    /// </summary>
    [Serializable]
    public abstract class CommandBuilder
    {
        // Buffer to hold commands 
        private readonly StringBuilder _commandBuffer;
        //Containing conenction instance
        private readonly Connection _connection;

        internal const char Space = ' ';
        internal const string SpaceColon = " :";
        internal const int MaxCommandSize = 512;
        internal const char CtcpQuote = '\u0001';

        internal CommandBuilder(Connection connection)
        {
            _connection = connection;
            _commandBuffer = new StringBuilder(MaxCommandSize);
        }

        internal Connection Connection
        {
            get { return _connection; }
        }

        internal StringBuilder Buffer
        {
            get { return _commandBuffer; }
        }

        /// <summary>
        ///   This methods actually sends the notice and privmsg commands.
        ///   It assumes that the message has already been broken up
        ///   and has a valid target.
        /// </summary>
        internal void SendMessage(string type, string target, string message)
        {
            _commandBuffer.Append(type);
            _commandBuffer.Append(Space);
            _commandBuffer.Append(target);
            _commandBuffer.Append(SpaceColon);
            _commandBuffer.Append(message);
            _connection.SendCommand(_commandBuffer);
        }

        /// <summary>
        ///   Clear the contents of the string buffer.
        /// </summary>
        internal void ClearBuffer()
        {
            _commandBuffer.Remove(0, _commandBuffer.Length);
        }

        /// <summary>
        ///   Break up a large message into smaller peices that will fit within the IRC
        ///   max message size.
        /// </summary>
        /// <param name = "message">The text to be broken up</param>
        /// <param name = "maxSize">The largest size a piece can be</param>
        /// <returns>A string array holding the correctly sized messages.</returns>
        internal static IEnumerable<string> BreakUpMessage(string message, int maxSize)
        {
            var pieces = (int) Math.Ceiling(message.Length/(float) maxSize);
            var parts = new string[pieces];
            for (int i = 0; i < pieces; i++)
            {
                int start = i*maxSize;
                if (i == pieces - 1)
                {
                    parts[i] = message.Substring(start);
                }
                else
                {
                    parts[i] = message.Substring(start, maxSize);
                }
            }
            return parts;
        }
    }
}
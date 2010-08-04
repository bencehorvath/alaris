using System;

namespace Alaris.Irc
{
	/// <summary>
	/// Classes should implement this Interface in order to act as custom
	/// parsers for the raw messages received from IRC servers.
	/// </summary>
	public interface IParser
	{
		/// <summary>
		/// Before a message is passed to the custom parser
		/// the Connection will check if the IRC message is the right kind.
		/// </summary>
		/// <param name="line">The raw message from the IRC server.</param>
		/// <returns>True if this parser can process the message.</returns>
		bool CanParse( string line );

		/// <summary>
		/// Send the raw IRC message to this custom parser. <i>This
		/// consumes the message and it will not be processed by the default
		/// or any other custom parsers after this one.</i>
		/// </summary>
		/// <param name="message">The raw message from the IRC server.</param>
		void Parse( string message );
	}
}

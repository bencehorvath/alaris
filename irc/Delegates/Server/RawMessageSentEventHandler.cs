namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// The full unparsed text messages sent to the IRC server. It
    /// includes all messages sent except for those exchanged during a DCC chat.
    /// </summary>
    /// <param name="message">The text sent.</param>
    /// <see cref="Connection.OnRawMessageSent"/>
    public delegate void RawMessageSentEventHandler( string message );
}
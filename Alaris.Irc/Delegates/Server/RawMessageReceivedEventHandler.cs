namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// The full unparsed text messages received from the IRC server. It
    /// includes all messages received except for those exchanged during a DCC chat.
    /// </summary>
    /// <param name="message">The text received.</param>
    /// <see cref="Connection.OnRawMessageReceived"/>
    public delegate void RawMessageReceivedEventHandler( string message );
}
namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// The server's "Message of the Day" if any.
    /// </summary>
    /// <param name="message">An information string.</param>
    /// <param name="last">True if this is the last in the set of messages.</param>
    /// <seealso cref="Listener.OnMotd"/>
    public delegate void MotdEventHandler( string message, bool last );
}
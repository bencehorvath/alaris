namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// Error messages from the IRC server.
    /// </summary>
    /// <param name="code">The RFC 2812 or custom numeric code.</param>
    /// <param name="message">The error message text.</param>
    /// <seealso cref="Listener.OnError"/>
    public delegate void ErrorMessageEventHandler( ReplyCode code, string message );
}
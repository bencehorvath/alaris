namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// Messages that are not handled by other events and are not errors.
    /// </summary>
    /// <param name="code">The RFC 2812 numeric code.</param>
    /// <param name="message">The unparsed message text.</param>
    /// <seealso cref="Listener.OnReply"/>
    public delegate void ReplyEventHandler( ReplyCode code, string message );
}
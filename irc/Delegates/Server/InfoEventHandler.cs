namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// The response to an <see cref="Sender.Info"/> request.
    /// </summary>
    /// <param name="message">An information string.</param>
    /// <param name="last">True if this is the last in the set of messages.</param>
    /// <seealso cref="Listener.OnInfo"/>
    public delegate void InfoEventHandler( string message, bool last );
}
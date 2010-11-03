namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// The response to an <see cref="Sender.Admin"/> request.
    /// </summary>
    /// <param name="message">An information string.</param>
    /// <seealso cref="Listener.OnAdmin"/>
    public delegate void AdminEventHandler( string message );
}
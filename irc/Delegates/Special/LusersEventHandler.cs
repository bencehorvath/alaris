namespace Alaris.Irc.Delegates.Special
{
    /// <summary>
    /// The response to a <see cref="Sender.Lusers"/> request.
    /// </summary>
    /// <param name="message">An information string.</param>
    /// <seealso cref="Listener.OnLusers"/>
    public delegate void LusersEventHandler( string message );
}
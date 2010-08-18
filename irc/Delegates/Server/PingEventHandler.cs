namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// Called when a server sends a keep-alive Ping.
    /// </summary>
    /// <param name="message">The message that the IRC server wants echoed back to it.</param>
    /// <seealso cref="Listener.OnPing"/>
    public delegate void PingEventHandler( string message );
}
namespace Alaris.Irc.Delegates.Special
{
    /// <summary>
    /// The response to a <see cref="Sender.Links"/> request.
    /// </summary>
    /// <param name="mask">The hostname as it appears in IRC queries.</param>
    /// <param name="hostname">The actual hostname.</param>
    /// <param name="hopCount">The number of hops from this server to the target server.</param>
    /// <param name="serverInfo">Information about the server, usually the network name.</param>
    /// <param name="done">True if this is the last message in the series. If it is the
    /// last it will not contain any server information.</param>
    /// <seealso cref="Listener.OnLinks"/>
    public delegate void LinksEventHandler( string mask, string hostname, int hopCount, string serverInfo, bool done );
}
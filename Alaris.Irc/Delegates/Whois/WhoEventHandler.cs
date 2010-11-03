namespace Alaris.Irc.Delegates.Whois
{
    /// <summary>
    /// The response to a <see cref="Sender.Who"/> request.
    /// </summary>
    /// <param name="user">The subject of the query</param>
    /// <param name="channel">The channel the user is on</param>
    /// <param name="ircServer">The name of the user's IRC server</param>
    /// <param name="mask">The user's mode mask</param>
    /// <param name="hopCount">Number of network hops to the user</param>
    /// <param name="realName">The user's real name</param>
    /// <param name="last">True if this is the last response</param>
    /// <seealso cref="Listener.OnWho"/>
    public delegate void WhoEventHandler( UserInfo user, string channel, string ircServer, string mask, 
                                          int hopCount, string realName, bool last );
}
namespace Alaris.Irc.Delegates.Channel
{
    /// <summary>
    /// Someone has joined a channel.
    /// </summary>
    /// <param name="user">Who joined.</param>
    /// <param name="channel">The channel name.</param>
    /// <seealso cref="Listener.OnJoin"/>
    public delegate void JoinEventHandler( UserInfo user, string channel );
}
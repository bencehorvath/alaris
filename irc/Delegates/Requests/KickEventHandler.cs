namespace Alaris.Irc.Delegates.Requests
{
    /// <summary>
    /// Someone has been kicked from a channel. 
    /// </summary>
    /// <param name="user">Who did the kicking.</param>
    /// <param name="channel">The channel that the person was kicked from.</param>
    /// <param name="kickee">Who was kicked.</param>
    /// <param name="reason">Why the person was kicked.</param>
    /// <seealso cref="Listener.OnKick"/>
    public delegate void KickEventHandler( UserInfo user, string channel, string kickee, string reason );
}
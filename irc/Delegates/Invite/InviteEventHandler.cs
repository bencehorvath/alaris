namespace Alaris.Irc.Delegates.Invite
{
    /// <summary>
    /// The user has been invited to a channel.
    /// </summary>
    /// <param name="user">Who sent the invite.</param>
    /// <param name="channel">The target channel.</param>
    /// <seealso cref="Listener.OnInvite"/>
    public delegate void InviteEventHandler( UserInfo user, string channel );
}
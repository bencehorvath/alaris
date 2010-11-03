namespace Alaris.Irc.Delegates.Whois
{
    /// <summary>
    /// The response to a <see cref="Sender.Whowas"/> request.
    /// </summary>
    /// <param name="user">Information on the user.</param>
    /// <param name="realName">The user's real name.</param>
    /// <param name="last">True if this is the final reply.</param>
    /// <seealso cref="Listener.OnWhowas"/>
    public delegate void WhowasEventHandler( UserInfo user, string realName, bool last );
}
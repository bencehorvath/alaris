namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// A user changed his nickname.
    /// </summary>
    /// <param name="user">The user who is changing his nick.</param>
    /// <param name="newNick">The new nickname.</param>
    /// <seealso cref="Listener.OnNick"/>
    public delegate void NickEventHandler( UserInfo user, string newNick );
}
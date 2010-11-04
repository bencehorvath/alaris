namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// Someone has quit IRC.
    /// </summary>
    /// <param name="user">The user who quit.</param>
    /// <param name="reason">A goodbye message.</param>
    /// <seealso cref="Listener.OnQuit"/>
    public delegate void QuitEventHandler( UserInfo user, string reason);
}
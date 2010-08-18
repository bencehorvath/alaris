namespace Alaris.Irc.Delegates.Messages
{
    /// <summary>
    /// A private Notice type message was sent to the user.
    /// </summary>
    /// <param name="user">The user who sent the message.</param>
    /// <param name="notice">A message.</param>
    /// <seealso cref="Listener.OnPrivateNotice"/>
    public delegate void PrivateNoticeEventHandler( UserInfo user, string notice );
}
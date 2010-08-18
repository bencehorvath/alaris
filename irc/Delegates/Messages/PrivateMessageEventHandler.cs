namespace Alaris.Irc.Delegates.Messages
{
    /// <summary>
    /// A private message was sent to the user.
    /// </summary>
    /// <param name="user">Who sent the message.</param>
    /// <param name="message">The message.</param>
    /// <seealso cref="Listener.OnPrivate"/>
    public delegate void PrivateMessageEventHandler( UserInfo user, string message );
}
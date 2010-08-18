namespace Alaris.Irc.Delegates.Messages
{
    /// <summary>
    /// A private action message was sent to the user.
    /// </summary>
    /// <param name="user">The user who expresses the action.</param>
    /// <param name="description">An action.</param>
    /// <seealso cref="Listener.OnPrivateAction"/>
    public delegate void PrivateActionEventHandler( UserInfo user, string description );
}
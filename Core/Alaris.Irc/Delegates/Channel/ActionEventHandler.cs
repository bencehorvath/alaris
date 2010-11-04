namespace Alaris.Irc.Delegates.Channel
{
    /// <summary>
    /// An action message was sent to a channel.
    /// </summary>
    /// <param name="user">The user who expresses the action.</param>
    /// <param name="channel">The target channel.</param>
    /// <param name="description">An action.</param>
    /// <seealso cref="Listener.OnAction"/>
    public delegate void ActionEventHandler( UserInfo user, string channel, string description );
}
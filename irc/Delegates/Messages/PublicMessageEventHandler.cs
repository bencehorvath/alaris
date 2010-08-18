namespace Alaris.Irc.Delegates.Messages
{
    /// <summary>
    /// A public message was sent to a channel.
    /// </summary>
    /// <param name="user">The user who sent the message.</param>
    /// <param name="channel">The taregt channel.</param>
    /// <param name="message">A message.</param>
    /// <seealso cref="Listener.OnPublic"/>
    public delegate void PublicMessageEventHandler( UserInfo user, string channel, string message );
}
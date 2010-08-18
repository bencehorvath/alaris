namespace Alaris.Irc.Delegates.Messages
{
    /// <summary>
    /// A Notice type message was sent to a channel.
    /// </summary>
    /// <param name="user">The user who sent the message.</param>
    /// <param name="channel">The target channel.</param>
    /// <param name="notice">A message.</param>
    /// <seealso cref="Listener.OnPublicNotice"/>
    public delegate void PublicNoticeEventHandler( UserInfo user, string channel, string notice );
}
namespace Alaris.Irc.Delegates.Channel
{
    /// <summary>
    /// The response to a <see cref="Sender.RequestTopic"/> command.
    /// </summary>
    /// <param name="channel">The channel who topic was requested.</param>
    /// <param name="topic">The topic.</param>
    /// <seealso cref="Listener.OnTopicRequest"/>
    public delegate void TopicRequestEventHandler( string channel, string topic);
}
namespace Alaris.Irc.Delegates.Channel
{
    /// <summary>
    /// A channel's topic has changed.
    /// </summary>
    /// <param name="user">Who changed the topic.</param>
    /// <param name="channel">Which channel had its topic changed.</param>
    /// <param name="newTopic">The new topic.</param>
    /// <seealso cref="Listener.OnTopicChanged"/>
    public delegate void TopicEventHandler( UserInfo user, string channel, string newTopic);
}
namespace Alaris.Irc.Delegates.Requests
{
    /// <summary>
    /// The response to a <see cref="Sender.List"/> request.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    /// <param name="visibleNickCount">The number of visible users on that channel.</param>
    /// <param name="topic">The channel's topic.</param>
    /// <param name="last">True if this is the last reply.</param>
    /// <seealso cref="Listener.OnList"/>
    public delegate void ListEventHandler( string channel, int visibleNickCount, string topic, bool last );
}
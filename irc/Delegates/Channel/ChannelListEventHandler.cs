namespace Alaris.Irc.Delegates.Channel
{
    /// <summary>
    /// Response to a <see cref="Sender.RequestChannelList"/> command.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    /// <param name="mode">What type is this a list? For example bans, invitation masks, etc..</param>
    /// <param name="item">A mask or nick (in the case of ChannelCreator).</param>
    /// <param name="last">Is this the last item. If its the last then the item paramter
    /// will be empty unless the mode is ChannelCreator.</param>
    /// <param name="who">Who set the mask (not for ChannelCreator).</param>
    /// <param name="whenSet">When was it set (not for ChannelCreator).</param>
    /// <seealso cref="Listener.OnChannelList"/>
    public delegate void ChannelListEventHandler( string channel, ChannelMode mode, string item, UserInfo who, long whenSet, bool last );
}
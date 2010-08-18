namespace Alaris.Irc.Delegates.Channel
{
    /// <summary>
    /// The response to a <see cref="Sender.RequestChannelModes"/> command.
    /// </summary>
    /// <param name="channel">The name of the channel.</param>
    /// <param name="modes">Objects which hold all the information about a channel's modes.</param>
    /// <seealso cref="Listener.OnChannelModeRequest"/>
    public delegate void ChannelModeRequestEventHandler( string channel, ChannelModeInfo[] modes);
}
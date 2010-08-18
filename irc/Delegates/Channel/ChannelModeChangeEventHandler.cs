namespace Alaris.Irc.Delegates.Channel
{
    /// <summary>
    /// A channel's mode has changed.
    /// </summary>
    /// <param name="who">Who changed the mode.</param>
    /// <param name="channel">The name of the channel.</param>
    /// <param name="modes">Objects which hold all the information about 1 or more mode changes.</param>
    /// <seealso cref="Listener.OnChannelModeChange"/>
    public delegate void ChannelModeChangeEventHandler( UserInfo who, string channel, ChannelModeInfo[] modes );
}
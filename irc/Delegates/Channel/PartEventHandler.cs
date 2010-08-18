namespace Alaris.Irc.Delegates.Channel
{
    /// <summary>
    /// Someone has left a channel. 
    /// </summary>
    /// <param name="user">The user who left.</param>
    /// <param name="channel">The channel he left.</param>
    /// <param name="reason">The reason for leaving.</param>
    /// <seealso cref="Listener.OnPart"/>
    public delegate void PartEventHandler( UserInfo user, string channel, string reason);
}
namespace Alaris.Irc.Delegates.Special
{
    /// <summary>
    /// The response to a <see cref="Sender.Time"/> request.
    /// </summary>
    /// <param name="time">The name of the server and
    /// its local time</param>
    /// <seealso cref="Listener.OnTime"/>
    public delegate void TimeEventHandler( string time );
}
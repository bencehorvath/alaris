namespace Alaris.Irc.Delegates.Special
{
    /// <summary>
    /// The response to a <see cref="Sender.Stats"/> request.
    /// </summary>
    /// <param name="queryType">What kind of query this is in response to.</param>
    /// <param name="message">The actual response.</param>
    /// <param name="done">True if this is the last message in the series.</param>
    /// <seealso cref="Listener.OnStats"/>
    public delegate void StatsEventHandler( StatsQuery queryType, string message, bool done );
}
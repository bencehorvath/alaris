namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// A Notice or Private message was sent to someone
    /// whose status is away.
    /// </summary>
    /// <param name="nick">The nick of the user who is away.</param>
    /// <param name="awayMessage">An away message, if any, set by the user. </param>
    /// <seealso cref="Listener.OnAway"/>
    public delegate void AwayEventHandler( string nick, string awayMessage );
}
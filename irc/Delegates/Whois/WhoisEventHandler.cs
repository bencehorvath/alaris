namespace Alaris.Irc.Delegates.Whois
{
    /// <summary>
    /// The response to a <see cref="Sender.Whois"/> request.
    /// </summary>
    /// <param name="whoisInfo">The data associated with the nick queried.</param>
    /// <seealso cref="Listener.OnWho"/>
    public delegate void WhoisEventHandler( WhoisInfo whoisInfo );
}
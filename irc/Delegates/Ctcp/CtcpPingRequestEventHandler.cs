using Alaris.Irc.Ctcp;

namespace Alaris.Irc.Delegates.Ctcp
{
    /// <summary>
    /// Someone has sent a Ctcp Ping request.
    /// </summary>
    /// <param name="who">Who sent the request.</param>
    /// <param name="timestamp">The timestamp which should be sent 
    /// back."</param>
    /// <see cref="CtcpListener.OnCtcpPingRequest"/>
    public delegate void CtcpPingRequestEventHandler( UserInfo who, string timestamp );
}
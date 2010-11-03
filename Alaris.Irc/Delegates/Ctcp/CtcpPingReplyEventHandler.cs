using Alaris.Irc.Ctcp;

namespace Alaris.Irc.Delegates.Ctcp
{
    /// <summary>
    /// Someone has replied to a Ctcp ping request sent by this client.
    /// </summary>
    /// <param name="who">Who sent the reply.</param>
    /// <param name="timestamp">The timestamp originally sent in the request."</param>
    /// <see cref="CtcpListener.OnCtcpPingReply"/>
    public delegate void CtcpPingReplyEventHandler( UserInfo who, string timestamp );
}
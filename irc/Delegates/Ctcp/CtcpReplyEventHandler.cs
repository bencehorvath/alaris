using Alaris.Irc.Ctcp;

namespace Alaris.Irc.Delegates.Ctcp
{
    /// <summary>
    /// Someone has replied to a Ctcp request sent by this client.
    /// </summary>
    /// <param name="who">Who sent the reply.</param>
    /// <param name="command">The Ctcp command this replies to."</param>
    /// <param name="reply">The text of the reply.</param>
    /// <see cref="CtcpListener.OnCtcpReply"/>
    public delegate void CtcpReplyEventHandler( string command, UserInfo who, string reply );
}
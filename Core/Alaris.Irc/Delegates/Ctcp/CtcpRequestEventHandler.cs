using Alaris.Irc.Ctcp;

namespace Alaris.Irc.Delegates.Ctcp
{
    /// <summary>
    /// Someone has sent a Ctcp request.
    /// </summary>
    /// <param name="who">Who sent the request.</param>
    /// <param name="command">The Ctcp command to send to IRC."</param>
    /// <see cref="CtcpListener.OnCtcpRequest"/>
    public delegate void CtcpRequestEventHandler( string command, UserInfo who );
}
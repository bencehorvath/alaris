using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc
{
    /// <summary>
    /// When trying to initiate a DCC chat request the remote user
    /// did not respond within the alotted time.
    /// </summary>
    /// <param name="session">Which session is the source of the event.</param>
    /// <see cref="DccChatSession.OnChatRequestTimeout"/>
    public delegate void ChatRequestTimeoutEventHandler( DccChatSession session );
}
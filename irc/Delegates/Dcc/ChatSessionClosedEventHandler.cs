using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc
{
    /// <summary>
    /// A DCC chat session has been closed.
    /// </summary>
    /// <param name="session">Which session is the source of the event.</param>
    /// <see cref="DccChatSession.OnChatSessionClosed"/>
    public delegate void ChatSessionClosedEventHandler( DccChatSession session );
}
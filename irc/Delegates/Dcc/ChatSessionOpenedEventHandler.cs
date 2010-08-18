using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc
{
    /// <summary>
    /// A DCC chat session has been opened with a remote user.
    /// </summary>
    /// <param name="session">Which session is the source of the event.</param>
    /// <see cref="DccChatSession.OnChatSessionOpened"/>
    public delegate void ChatSessionOpenedEventHandler( DccChatSession session );
}
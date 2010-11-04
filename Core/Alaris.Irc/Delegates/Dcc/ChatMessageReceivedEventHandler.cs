using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc
{
    /// <summary>
    /// A DCC chat message was received from the remote user.
    /// </summary>
    /// <param name="session">Which session is the source of the event.</param>
    /// <param name="message">A string message.</param>
    /// <see cref="DccChatSession.OnChatMessageReceived"/>
    public delegate void ChatMessageReceivedEventHandler( DccChatSession session , string message );
}
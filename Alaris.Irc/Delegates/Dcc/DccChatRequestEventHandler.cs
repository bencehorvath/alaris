using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc
{
    /// <summary>
    /// Someone has requested a DCC chat session.
    /// </summary>
    /// <param name="dccUserInfo">The collection of information about the remote user.</param>
    /// <see cref="DccListener.OnDccChatRequest"/>
    public delegate void DccChatRequestEventHandler( DccUserInfo dccUserInfo );
}
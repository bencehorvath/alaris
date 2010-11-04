using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc
{
    /// <summary>
    /// A remote user has requested a file. To respond
    /// use <see cref="DccFileSession.Send"/> with the relevant information.
    /// </summary>
    /// <param name="dccUserInfo">The requestor's information.</param>
    /// <param name="fileName">The name of the requested file.</param>
    /// <param name="turbo">True to use send-ahead mode for transfers.</param>
    /// <see cref="DccListener.OnDccGetRequest"/>
    public delegate void DccGetRequestEventHandler( DccUserInfo dccUserInfo, string fileName, bool turbo);
}
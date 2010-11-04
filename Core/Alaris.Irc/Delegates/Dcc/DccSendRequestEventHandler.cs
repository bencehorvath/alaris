using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc
{
    /// <summary>
    /// Another user has offered to send a file.
    /// </summary>
    /// <param name="dccUserInfo">The collection of information about the remote user.</param>
    /// <param name="fileName">The name of the file to be sent.</param>
    /// <param name="size">The size in bytes of the offered file.</param>
    /// <param name="turbo">True if the sender will use send-ahead mode.</param>
    /// <see cref="DccListener.OnDccSendRequest"/>
    public delegate void DccSendRequestEventHandler( DccUserInfo dccUserInfo, string fileName, int size, bool turbo );
}
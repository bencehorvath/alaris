using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc.Files
{
    /// <summary>
    /// The file transfer connection has been successfully opened and the data
    /// transfer has begun.
    /// </summary>
    /// <param name="session">The session in which the transfer has started.</param> 
    /// <see cref="DccFileSession.OnFileTransferStarted"/>
    public delegate void FileTransferStartedEventHandler( DccFileSession session );
}
using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc.Files
{
    /// <summary>
    /// A file was succefully transfered.
    /// </summary>
    /// <param name="session">The session in which the transfer was successfully completed.</param> 
    /// <see cref="DccFileSession.OnFileTransferCompleted"/>
    public delegate void FileTransferCompletedEventHandler( DccFileSession session );
}
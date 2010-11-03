using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc.Files
{
    /// <summary>
    /// Something happened to stop the transfer before it was completed. Normally
    /// this will be due to one of the sides canceling the transfer.
    /// </summary>
    /// <param name="session">The session in which the transfer was interrupted.</param> 
    /// <see cref="DccFileSession.OnFileTransferInterrupted"/>
    public delegate void FileTransferInterruptedEventHandler( DccFileSession session );
}
using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc.Files
{
    /// <summary>
    /// Called for each successful data block transfer. This allows the developer
    /// to show a transfer progress display of some kind.
    /// </summary>
    /// <param name="session">The session in which data was transfered.</param> 
    /// <param name="bytesSent">The number of bytes sent in this block. The DccFileSession
    /// contains the cumulative number of bytes sent/received and the total number
    /// the will be processed.</param> 
    /// <see cref="DccFileSession.OnFileTransferProgress"/>
    public delegate void FileTransferProgressEventHandler( DccFileSession session , int bytesSent );
}
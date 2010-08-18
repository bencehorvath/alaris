using Alaris.Irc.Dcc;

namespace Alaris.Irc.Delegates.Dcc.Files
{
    /// <summary>
    /// There has been no activity in this session for the timeout period. The 
    /// session is automatically closed and this event is raised. 
    /// </summary>
    /// <param name="session">The session in which the timeout occurred.</param> 
    /// <see cref="DccFileSession.OnFileTransferTimeout"/>
    public delegate void FileTransferTimeoutEventHandler( DccFileSession session );
}
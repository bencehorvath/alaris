namespace Alaris.Irc.Delegates.Special
{
    /// <summary>
    /// The response to a <see cref="Sender.Version"/> request.
    /// </summary>
    /// <param name="versionInfo">The information string in the form 
    /// IRC: [version].[debuglevel] [server] :[comments]</param>
    /// <seealso cref="Listener.OnVersion"/>
    public delegate void VersionEventHandler( string versionInfo );
}
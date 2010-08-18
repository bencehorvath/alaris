namespace Alaris.Irc.Delegates.Server
{
    /// <summary>
    /// Called when a nick change fails.
    /// </summary>
    /// <remarks>
    /// <para>This method can be called under 2 conditions:
    /// It can arise when the user is already 
    /// registered with the IRC server and is trying change his nick.
    /// Or when the user is trying to register for the first time with 
    /// the IRC server and it fails.</para>
    /// <para>Note that if the later arises then you will have to manually
    /// complete the regsitration process.</para> 
    /// </remarks>
    /// <param name="badNick">The nick which caused the problem</param>
    /// <param name="reason">A message explaining the error</param>
    /// <seealso cref="Listener.OnNickError"/>
    public delegate void NickErrorEventHandler( string badNick, string reason ) ;
}
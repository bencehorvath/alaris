namespace Alaris.Irc.Delegates.Special
{
    /// <summary>
	/// Someone was disconnected from the server via a Kill.
	/// </summary>
	/// <param name="user">Which Operator send teh Kill command</param>
	/// <param name="nick">Who was Killed.</param>
	/// <param name="reason">Why the nick was disconnected.</param>
	/// <seealso cref="Listener.OnKill"/>
	public delegate void KillEventHandler( UserInfo user, string nick, string reason );
}

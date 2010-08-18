namespace Alaris.Irc.Delegates.User
{
    /// <summary>
    /// This user's mode has changed.
    /// </summary>
    /// <param name="action">Whether a mode was added or removed.</param>
    /// <param name="mode">The mode that was changed.</param>
    /// <seealso cref="Listener.OnUserModeChange"/>
    public delegate void UserModeChangeEventHandler( ModeAction action, UserMode mode );
}
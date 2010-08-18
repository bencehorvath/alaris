namespace Alaris.Irc.Delegates.User
{
    /// <summary>
    /// The response to a <see cref="Sender.RequestUserModes"/> command for this user.
    /// </summary>
    /// <param name="modes">The complete list of user modes as an array.</param>
    /// <seealso cref="Listener.OnUserModeRequest"/>
    public delegate void UserModeRequestEventHandler( UserMode[] modes );
}
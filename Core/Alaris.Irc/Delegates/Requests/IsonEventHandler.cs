namespace Alaris.Irc.Delegates.Requests
{
    /// <summary>
    /// The response to a <see cref="Sender.Ison"/> request.
    /// </summary>
    /// <param name="nicks">If someone with this nick is on the same IRC network their nick
    /// will be returned here. Otherwise nick will be an empty string.</param>
    /// <seealso cref="Listener.OnIson"/>
    public delegate void IsonEventHandler( string nicks );
}
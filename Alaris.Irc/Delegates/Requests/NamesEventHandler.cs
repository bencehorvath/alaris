namespace Alaris.Irc.Delegates.Requests
{
    /// <summary>
    /// The response to a <see cref="Sender.Names"/> request.
    /// </summary>
    /// <param name="channel">The channel the user is on. "@" is used for secret channels, "*" for private
    /// channels, and "=" for public channels.</param>
    /// <param name="nicks">A list of nicks on the channel. If this is the last reply
    /// then it will be empty. Nicks prefixed with a '@' are channel
    /// operators. Nicks prefixed with a '+' have voice privileges on
    /// a moderated channel, i.e. they are allowed to send public messages.</param>
    /// <param name="last">True if this is the last names reply.</param>
    /// <seealso cref="Listener.OnNames"/>
    public delegate void NamesEventHandler( string channel, string[] nicks, bool last );
}
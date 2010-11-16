namespace Alaris
{
    public partial class AlarisBot
    {
        /// <summary>
        ///   Sends the given message to the specified channel.
        /// </summary>
        /// <param name = "channel">
        ///   A <see cref = "System.String" />
        /// </param>
        /// <param name = "message">
        ///   A <see cref = "System.String" />
        /// </param>
        public void SendMsg(string channel, string message)
        {
            _connection.Sender.PublicMessage(channel, message);
        }


        /// <summary>
        /// Sends the MSG.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        public void SendMsg(string channel, string message, params object[] args)
        {
            var msg = string.Format(message, args);
            SendMsg(channel, msg);
        }
    }
}
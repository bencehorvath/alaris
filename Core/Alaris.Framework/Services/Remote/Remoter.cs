using System;

namespace Alaris.Framework.Services.Remote
{
    /// <summary>
    /// Class used by clients to remotely control Alaris.
    /// </summary>
    public sealed class Remoter : IRemote
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Authorizes the client.
        /// </summary>
        /// <param name="passHash">Hash of the required password.</param>
        /// <returns>True if successfully authenticated otherwise false.</returns>
        public bool Authorize(string passHash)
        {
            if (passHash.Equals(Utility.MD5String(AlarisBase.Instance.Config.Remote.Password), StringComparison.InvariantCultureIgnoreCase))
            {
                //TODO: use this to check in later methods if the client is authorized or not.
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sends the specified to message to the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        public void PublicMessage(string channel, string message)
        {
            Log.Info("Sending message: {0}: {1}", channel, message);
            AlarisBase.Instance.SendMsg(channel, message);
        }
    }
}

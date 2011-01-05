using System.ServiceModel;

namespace Alaris.Framework.Services.Remote
{
    /// <summary>
    /// Interface which should be implemented by the remoting service class.
    /// </summary>
    [ServiceContract(Namespace = "http://github.com/Twl/alaris")]
    public interface IRemote
    {
        /// <summary>
        /// Authorizes the client.
        /// </summary>
        /// <param name="passHash">Hash of the required password.</param>
        /// <returns>True if successfully authenticated otherwise false.</returns>
        [OperationContract]
        bool Authorize(string passHash);

        /// <summary>
        /// Sends the specified to message to the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        [OperationContract]
        void PublicMessage(string channel, string message);
    }
}
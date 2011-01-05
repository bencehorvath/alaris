using System.Collections.Generic;
using Alaris.Irc;

namespace Alaris.Framework.Commands
{
    /// <summary>
    /// Main param passed to command methods.
    /// </summary>
    public sealed class AlarisMainParameter
    {
        #region Properties

        /// <summary>
        /// Gets the Alaris instance.
        /// </summary>
        public AlarisBase Bot { get
        {
            return AlarisBase.Instance;
        }}
        /// <summary>
        ///  The IRC connection.
        /// </summary>
        public Connection IrcConnection { get; set; }
        /// <summary>
        /// Channels the bot is on.
        /// </summary>
        public List<string> Channels { get; set; }
        /// <summary>
        /// Channels where the command was given.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// User of the action.
        /// </summary>
        public UserInfo User { get; set; }

        #endregion
    }
}

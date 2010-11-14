using System;
using System.Collections.Generic;
using Alaris.API;
using Alaris.Irc;

namespace Alaris.DefaultAddon
{
    public class DefaultAddon : IAlarisAddon
    {
        private Connection _connection;
        private List<string> _channels;

        #region Implementation of IAlarisAddon

        /// <summary>
        /// Creates the addon.
        /// </summary>
        /// <param name="conn">IRC connection.</param>
        /// <param name="channels">Channel list.</param>
        public void Setup(ref Connection conn, List<string> channels)
        {
            _connection = conn;
            _channels = channels;

            AlarisBot.Instance.ScriptManager.RegisterOnPublicHook((usr, chn, msg) =>
                                                                      {
                                                                          if(msg.Equals("@sayhd", StringComparison.InvariantCultureIgnoreCase))
                                                                          {
                                                                              _connection.Sender.PublicMessage(chn, "Hello from DefaultAddon!");
                                                                          }

                                                                      });
        }

        #endregion

        #region Implementation of IAlarisAddon

        /// <summary>
        /// Destroys the addon, releasing all resources.
        /// </summary>
        public void Destroy()
        {
            
        }

        /// <summary>
        /// Name of the addon
        /// </summary>
        public string Name
        {
            get { return "DefaultAddon"; }
        }

        /// <summary>
        /// Author of the addon.
        /// </summary>
        public string Author
        {
            get { return "Twl"; }
        }

        /// <summary>
        /// Website where the addon is available.
        /// </summary>
        public string Website
        {
            get { return "http://www.github.com/twl/alaris"; }
        }

        #endregion
    }
}

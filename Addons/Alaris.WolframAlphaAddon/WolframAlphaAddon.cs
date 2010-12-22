using System.Collections.Generic;
using Alaris.API;
using Alaris.Irc;

namespace Alaris.WolframAlphaAddon
{
    public class WolframAlphaAddon : IAlarisAddon
    {
        #region Implementation of IAlarisAddon

        /// <summary>
        /// Creates the addon.
        /// </summary>
        /// <param name="conn">IRC connection.</param>
        /// <param name="channels">Channel list.</param>
        public void Setup(ref Connection conn, List<string> channels)
        {
            // nothing
        }

        #endregion

        #region Implementation of IAlarisAddon

        /// <summary>
        /// Destroys the addon, releasing all resources.
        /// </summary>
        public void Destroy()
        {
            // nothing
        }

        /// <summary>
        /// Name of the addon
        /// </summary>
        public string Name
        {
            get { return "WolframAlphaAddon"; }
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

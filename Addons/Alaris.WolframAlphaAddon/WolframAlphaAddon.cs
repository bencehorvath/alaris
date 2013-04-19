using System;
using Alaris.Framework;


// appid: 557QYQ-UUUWTKX95V

namespace Alaris.WolframAlphaAddon
{
    public class WolframAlphaAddon : IAlarisAddon
    {
        private readonly Guid _guid;
        private AlarisBase _alaris;

        public WolframAlphaAddon()
        {
            _guid = new Guid("{6EFBC75A-BD57-43C2-B84A-AD58376FB5C6}");
        }

        #region Implementation of IAlarisAddon

        /// <summary>
        /// Creates the addon.
        /// </summary>
        /// <param name="conn">IRC connection.</param>
        /// <param name="channels">Channel list.</param>
        public void Setup(AlarisBase alaris)
        {
            _alaris = alaris;
            // nothing
        }

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

        public Guid GetGuid()
        {
            return _guid;
        }
    }
}

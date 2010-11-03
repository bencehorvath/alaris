using System.Collections.Generic;
using Alaris.Irc;

namespace Alaris.API
{
    /// <summary>
    /// Another attempt to implement a plugin interface.
    /// </summary>
    public interface IAlarisPlugin
    {
        /// <summary>
        /// Creates the plugin.
        /// </summary>
        /// <param name="conn">IRC connection.</param>
        /// <param name="channels">Channel list.</param>
        void Setup(ref Connection conn, List<string> channels);
        /// <summary>
        /// Destroys the plugin, releasing all resources.
        /// </summary>
        void Destroy();

        /// <summary>
        /// Name of the plugin
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Author of the plugin.
        /// </summary>
        string Author { get; }
    }
}

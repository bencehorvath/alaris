using Alaris.Irc;
using System.Collections.Generic;

namespace Alaris.API
{
	public interface IAlarisBasic : IAlarisComponent
	{
	    void Initialize(ref Connection con, List<string> channels);
        /// <summary>
        /// On public message hook.
        /// </summary>
        /// <param name="user">Userinfo of sender.</param>
        /// <param name="channel">Channel where happened.</param>
        /// <param name="msg">Message sent</param>
		void OnPublicMessage(UserInfo user, string channel, string msg);
        /// <summary>
        /// On registered hook.
        /// </summary>
		void OnRegistered();
        /// <summary>
        /// Function called when the plugin is loaded.
        /// </summary>
		void OnLoad();
        /// <summary>
        /// Function called when the plugin is unloaded.
        /// </summary>
		void OnUnload();
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Author of the plugin.
        /// </summary>
        string Author { get; }
	}
}
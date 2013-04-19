using System.Collections.Generic;
using Alaris.Irc.Delegates.Messages;

namespace Alaris.Framework.Lua
{
    /// <summary>
    /// Class containing most of the exported Lua functions.
    /// It as also a wrapper to the Main API.
    /// </summary>
    public sealed class LuaFunctions
    {
        private readonly LuaInterface.Lua _lua;
        private readonly List<PublicMessageEventHandler> _registeredOnPM = new List<PublicMessageEventHandler>();

        #region Properites

        /// <summary>
        /// Events registered by Lua on Public Message.
        /// </summary>
        public IEnumerable<PublicMessageEventHandler> RegisteredOnPM
        {
            get
            {
                return _registeredOnPM;
            }
        }

        #endregion

        /// <summary>
        /// Creates a new instance of <c>LuaFunctions</c>
        /// </summary>
        /// <param name="vm">Lua VM</param>
        public LuaFunctions(ref LuaInterface.Lua vm)
        {
            _lua = vm;
        }

        /// <summary>
        /// Registers a function hook.
        /// </summary>
        /// <param name="eventName">Event to listen to.</param>
        /// <param name="luaName">Lua function to call.</param>
        [LuaFunction("RegisterHook", "Registers a function hook.")]
        public void RegisterFunctionHook(string eventName, string luaName)
        {
            if(eventName == "OnPublicMessage")
            {
                var func = _lua.GetFunction(typeof (PublicMessageEventHandler), luaName);

                if (func == null)
                    return;

                var handler = func as PublicMessageEventHandler;

                //_connection.Listener.OnPublic += handler;
                //foreach(var server in AlarisBase.Instance.Servers)

                _registeredOnPM.Add(handler);
            }
            
        }

        /// <summary>
        /// Sends a message to the IRC server.
        /// </summary>
        /// <param name="chan">Channel to send to.</param>
        /// <param name="message">Message.</param>
        [LuaFunction("SendMsg", "Sends a message to the IRC server.")]
        public void SendMessage(string chan, string message)
        {
            AlarisBase.Instance.Connection.Sender.PublicMessage(chan, message);
        }
    }
}

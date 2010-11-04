using System.Collections.Generic;
using Alaris.Irc;
using Alaris.Irc.Delegates.Messages;
using LuaInterface;

namespace Alaris.LuaEngine
{
    /// <summary>
    /// Class containing most of the exported Lua functions.
    /// It as also a wrapper to the Main API.
    /// </summary>
    public sealed class LuaFunctions
    {
        private readonly Connection _connection;
        private readonly Lua _lua;
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
        /// <param name="conn">IRC connection</param>
        public LuaFunctions(ref Lua vm, ref Connection conn)
        {
            _connection = conn;
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

                _connection.Listener.OnPublic += handler;
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
            _connection.Sender.PublicMessage(chan, message);
        }
    }
}

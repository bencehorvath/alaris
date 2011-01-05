using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alaris.API;
using Alaris.Commands;
using Alaris.Framework;
using Alaris.Framework.Commands;
using Alaris.Framework.Database;
using Alaris.Framework.Extensions;
using Alaris.Framework.Services;
using Alaris.Irc;
using Alaris.Irc.Ctcp;
using NLog;
using CLI = Alaris.Framework.CommandLine.CLI;

namespace Alaris
{
    /// <summary>
    ///   The main class for Alaris.
    /// </summary>
    [Serializable]
    public partial class AlarisBot : AlarisBase, IDisposable
    {
        [NonSerialized]
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        

        /// <summary>
        ///   This is not an unused constructor. Called through singleton!
        /// </summary>
        public AlarisBot() : this("alaris.config.xml")
        {
        }

 
        /// <summary>
        ///   Creates a new instance of Alaris bot.
        /// </summary>
        public AlarisBot(string config)
            : base(config)
        {

        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the <see cref = "AlarisBot" />
        ///   is reclaimed by garbage collection.
        /// </summary>
        ~AlarisBot()
        {
            Log.Debug("Alaris", "~AlarisBot()");
        }

        /// <summary>
        ///   Gets the GUID.
        /// </summary>
        public Guid GetGuid()
        {
            return _guid;
        }


        /// <summary>
        /// Sets up event handlers.
        /// </summary>
        protected override void SetupHandlers()
        {
            base.SetupHandlers();
        }


        /// <summary>
        ///   Disconnects the bot from the IRC server.
        /// </summary>
        /// <param name = "rsr">
        ///   Reason for disconnect.
        /// </param>
        public override void Disconnect(string rsr)
        {
            base.Disconnect(rsr);
        }

        /// <summary>
        ///   Method run when the bot is registered to the IRC server.
        /// </summary>
        protected override void OnRegistered()
        {
            base.OnRegistered();
        }



        /// <summary>ddd
        ///   Releases all used resources.
        /// </summary>
        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
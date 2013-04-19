using System;
using System.IO;
using Alaris.Framework.Exceptions;
using NLog;

namespace Alaris.Framework
{
    /// <summary>
    ///   A class providing functions to run specific method in an exception-handled environment.
    /// </summary>
    [Serializable]
    public class CrashHandler : IAlarisComponent
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Guid _guid;
        /// <summary>
        ///   Creates a new instance of <see cref = "CrashHandler" />
        /// </summary>
        public CrashHandler()
        {
            Log.Info("Initializing crash handler");
            _guid = Guid.NewGuid();
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref = "CrashHandler" /> is reclaimed by garbage collection.
        /// </summary>
        ~CrashHandler()
        {
            Log.Trace("~CrashHandler()");
            
        }


        /// <summary>
        /// Gets the current instance's GUID.
        /// </summary>
        /// <returns>The GUID</returns>
        public Guid GetGuid()
        {
            return _guid;
        }
    }
}
using System;
using System.IO;
using Alaris.API;
using Alaris.Exceptions;
using Alaris.Irc;
using NLog;

namespace Alaris
{
    /// <summary>
    ///   The delegate used for <see cref = "AlarisBot.ReadConfig" />
    /// </summary>
    public delegate void ReadConfigDelegate(string configfile);

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

        /// <summary>
        ///   Executes the specified <see cref = "ReadConfigDelegate">ReadConfig</see> method in an exception-safe environment.
        /// </summary>
        /// <param name = "confread">
        ///   The ReadConfig method.
        /// </param>
        /// <param name = "param">
        ///   The parameter passed to the ReadConfig method.
        /// </param>
        public static void HandleReadConfig(ReadConfigDelegate confread, string param)
        {
            try
            {
                confread(param);
            }
            catch (FileNotFoundException ex)
            {
                Log.ErrorException("FileNotFoundException thrown inside Crash Handler", ex);
            }
            catch (ConfigFileInvalidException ex)
            {
                Log.ErrorException("ConfigFileInvalidException thrown inside Crash Handler ", ex);
            }
            catch (Exception x)
            {
                Log.ErrorException("Exception thrown inside Crash Handler: " + x, x);
            }
        }
    }
}
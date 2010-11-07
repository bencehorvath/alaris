using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NLog;

namespace Alaris
{
    /// <summary>
    /// Used to create crash dumps which are very useful for investigation of problems.
    /// </summary>
    public static class CrashDumper
    {
        private readonly static Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Creates the crash dump.
        /// </summary>
        public static void CreateCrashDump()
        {
            if (!Directory.Exists("Dumps"))
                Directory.CreateDirectory("Dumps");

            Log.Info("Creating crash dump...");

            try
            {
                using (var fs =
                    File.Open(
                        Path.Combine(Environment.CurrentDirectory, "Dumps",
                                     string.Format("{0}.acd", DateTime.Now.ToString("yyyy_MM_dd_HH_mm"))),
                        FileMode.Create))
                {

                    var formatter = new BinaryFormatter();

                    var bot = AlarisBot.Instance;

                    formatter.Serialize(fs, bot);

                }
            }
            catch(Exception x)
            {
                Log.Fatal("Failed to write crash dump. ({0})", x);
                return;
            }


            Log.Info("Crash dump created.");
        }
    }
}

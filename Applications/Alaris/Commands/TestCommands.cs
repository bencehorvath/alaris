using System;
using Alaris.Framework.Commands;

namespace Alaris.Commands
{
    /// <summary>
    /// Test commands.
    /// </summary>
    public static class TestCommands
    {
        /// <summary>
        /// test handler
        /// </summary>
        /// <param name="mp"></param>
        [AlarisCommand("testm", CommandPermission.Normal)]
        public static void HandleTestCommand(AlarisMainParameter mp)
        {
            mp.IrcConnection.Sender.PublicMessage(mp.Channel, "Test command called.");
        }

        /// <summary>
        /// test add
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [ParameterizedAlarisCommand("add", CommandPermission.Admin, 2)]
        public static void TestAddHandler(AlarisMainParameter mp, string a, string b)
        {
            var ar = Convert.ToInt32(a);
            var br = Convert.ToInt32(b);

            mp.IrcConnection.Sender.PublicMessage(mp.Channel, string.Format("Result: {0}", ar + br));
        }
    }
    
}

using Alaris.Commands;
using Alaris.Framework.Commands;

namespace Alaris.DefaultAddon.Commands
{
    public static class TestCommands
    {
        [AlarisCommand("default")]
        public static void TestDefaultCmd(AlarisMainParameter mp)
        {
            mp.Bot.SendMsg(mp.Channel, "Hello from DefaultAddon with new command sys!");
        }
    }
}

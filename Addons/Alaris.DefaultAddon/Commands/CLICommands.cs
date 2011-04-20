using Alaris.Framework.Commands;

namespace Alaris.DefaultAddon.Commands
{
    public static class CLICommands
    {
        [ParameterizedAlarisCommand("say", CommandPermission.Admin)]
        public static void HandleSayCommand(AlarisMainParameter mp, string what)
        {
            mp.Bot.SendMsg(mp.Channel, what);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alaris.API;
using Alaris.Irc;

namespace Alaris.Commands
{
    /// <summary>
    /// Class used to manage and load Alaris commands.
    /// </summary>
    public static class CommandManager
    {
        /// <summary>
        /// Prefix of IRC commands.
        /// </summary>
        public static string CommandPrefix { get; set; }

        private readonly static Dictionary<AlarisCommand, AlarisMethod> CommandMethodMap = new Dictionary<AlarisCommand, AlarisMethod>();

        /// <summary>
        /// This methods loads every method marked as a command and maps it to the specified command.
        /// </summary>
        public static void CreateMappings()
        {
            var asm = Assembly.GetExecutingAssembly();

            var types = asm.GetTypes();
            
            foreach(var type in types)
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

                foreach(var method in methods)
                {
                    foreach(var attribute in Attribute.GetCustomAttributes(method))
                    {
                        if(attribute.GetType() == typeof(AlarisCommandAttribute) || attribute.GetType() == typeof(ParameterizedAlarisCommand))
                        {
                            if (attribute.GetType() == typeof(ParameterizedAlarisCommand) && method.GetParameters().Length != (((ParameterizedAlarisCommand)attribute).ParameterCount+1))
                                continue;

                            var attr = (AlarisCommandAttribute) attribute;

                            CommandMethodMap.Add(new AlarisCommand
                                                     {
                                                         Command = attr.Command,
                                                         Permission = attr.Permission
                                                     }, new AlarisMethod(method, attr, attr is ParameterizedAlarisCommand));

                        }
                    }
                }
            }

            Log.Success("CommandManager", string.Format("Created {0} mapping(s).", CommandMethodMap.Count));
        }

        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="user">The irc user</param>
        /// <param name="channel">IRC channel</param>
        /// <param name="message">IRC msg.</param>
        public static void HandleCommand(UserInfo user, string channel, string message)
        {
            try
            {

                if (!message.StartsWith(CommandPrefix))
                    return;

                var commandText = message.Remove(0, 1); // remove prefix
                Log.Notice("CommandManager", string.Format("Checking for command: {0}", commandText));

                if(CommandMethodMap.Count <= 5)
                    foreach (var entry in CommandMethodMap)
                        Log.Notice("CommandManager", "Mapping contains: " + entry.Key.Command);

                var wr = commandText.Split(' ');
                var command = wr[0];

                var parameters = new List<string>();

                for (var i = 1; i < wr.Length; ++i)
                {
                    parameters.Add(wr[i]);
                }

                var perm = CommandPermission.Normal;
                AlarisMethod handler = null;

                foreach (var entry in CommandMethodMap.Where(entry => entry.Key.Command.Equals(command, StringComparison.InvariantCultureIgnoreCase)))
                {
                    perm = entry.Key.Permission;
                    handler = entry.Value;
                }

                if (handler == null)
                    return;

                if (parameters.Count != 0 && !handler.IsParameterized)
                    return;

                if (perm == CommandPermission.Admin && !Utilities.IsAdmin(user))
                    return;

                Log.Notice("CommandManager",
                           string.Format("It {0} parameterized. It is about to be called with {1} params",
                                         (handler.IsParameterized ? "is" : "is not"), parameters.Count));

                var mp = new AlarisMainParameter
                             {
                                 Channel = channel,
                                 Channels = AlarisBot.GetBot().Channels,
                                 IrcConnection = AlarisBot.GetBot().Connection
                             };

                var parl = new List<object>();

                if(handler.IsParameterized)
                {
                    parl.Add(mp);

                    parl.AddRange(parameters);

                }
                else
                    parl.Add(mp);

                /*object[] passParams = handler.IsParameterized
                                          ? new object[] {mp, parameters.ToArray()}
                                          : new object[] {mp};*/

                Log.Notice("CommandManager", string.Format("Invoking handler method ({0}). ({1})", handler.Method.Name, parl.Count));

                

                handler.Method.Invoke(null, parl.ToArray());

            }
            catch(Exception x)
            {
                Log.Error("CommandManager", x.Message);
                return;
            }
        }
    }
}

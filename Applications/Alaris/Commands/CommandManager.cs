using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alaris.API;
using Alaris.Irc;
using NLog;

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

        private readonly static Dictionary<AlarisCommandWrapper, AlarisMethod> CommandMethodMap = new Dictionary<AlarisCommandWrapper, AlarisMethod>();
        private readonly static Dictionary<AlarisCommandWrapper, AlarisMethod> SubCommandMethodMap = new Dictionary<AlarisCommandWrapper, AlarisMethod>();

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// This methods loads every method marked as a command and maps it to the specified command.
        /// </summary>
        public static void CreateMappings()
        {
            CommandMethodMap.Clear();

            var tasm = Assembly.GetExecutingAssembly();

            var asms = AddonManager.Assemblies.ToList();
            asms.Add(tasm);

            foreach (var asm in asms)
            {
                var types = asm.GetTypes();

                foreach (var type in types)
                {
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

                    foreach (var method in methods)
                    {
                        var passEverything = false;

                        foreach (var attribute in Attribute.GetCustomAttributes(method))
                        {
                            if (attribute.GetType() == typeof (AlarisCommandAttribute) ||
                                attribute.GetType() == typeof (ParameterizedAlarisCommand))
                            {
                                if (attribute.GetType() == typeof (ParameterizedAlarisCommand))
                                {
                                    var patt = (ParameterizedAlarisCommand) attribute;

                                    if (patt.IsParameterCountUnspecified)
                                        passEverything = true;

                                    else if(method.GetParameters().Length != patt.ParameterCount + 1)
                                        continue;
                                }
                                    

                                var attr = (AlarisCommandAttribute) attribute;

                                CommandMethodMap.Add(new AlarisCommandWrapper
                                                         {
                                                             Command = attr.Command,
                                                             Permission = attr.Permission,
                                                             IsParameterCountUnspecified = passEverything
                                                         },
                                                     new AlarisMethod(method, attr, attr is ParameterizedAlarisCommand));

                            }
                            else if(attribute.GetType() == typeof(AlarisSubCommandAttribute) ||
                                attribute.GetType() == typeof(ParameterizedAlarisSubCommand))
                            {
                                if(attribute.GetType() == typeof(ParameterizedAlarisSubCommand))
                                {
                                    var patt = (ParameterizedAlarisSubCommand) attribute;

                                    if (patt.IsParameterCountUnspecified)
                                        passEverything = true;
                                    else if (method.GetParameters().Length != patt.ParameterCount + 1)
                                        continue;
                                }

                                var attr = (AlarisSubCommandAttribute)attribute;

                                CommandMethodMap.Add(new AlarisCommandWrapper
                                {
                                    Command = attr.Command,
                                    Permission = attr.Permission,
                                    IsParameterCountUnspecified = passEverything
                                },
                                                     new AlarisMethod(method, attr, attr is ParameterizedAlarisSubCommand));
                            }
                        }
                    }
                }
            }

            Log.Info("Created {0} command mapping(s) and {1} sub-command mapping(s)", CommandMethodMap.Count, SubCommandMethodMap.Count);
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
                Log.Info("Checking for command: {0}", commandText);

                if(CommandMethodMap.Count <= 5)
                    foreach (var entry in CommandMethodMap)
                        Log.Info("Mapping contains command: {0}", entry.Key.Command);


                var wr = commandText.Split(' ');
                var command = wr[0];

                var parameters = new List<string>();

                for (var i = 1; i < wr.Length; ++i)
                {
                    parameters.Add(wr[i]);
                }

                var perm = CommandPermission.Normal;
                AlarisMethod handler = null;
                AlarisCommandWrapper cmd = null;

                foreach (var entry in CommandMethodMap.Where(entry => entry.Key.Command.Equals(command, StringComparison.InvariantCultureIgnoreCase)))
                {
                    perm = entry.Key.Permission;
                    handler = entry.Value;
                    cmd = entry.Key;
                }

                if (handler == null)
                    return;

                if (parameters.Count != 0 && !handler.IsParameterized)
                    return;

                if (perm == CommandPermission.Admin && !Utilities.IsAdmin(user))
                    return;

                Log.Info("The handler {0} parameterized. It is about to be called with {1} params",
                         (handler.IsParameterized ? "is" : "is not"), parameters.Count);

                var mp = new AlarisMainParameter
                             {
                                 Channel = channel,
                                 Channels = AlarisBot.Instance.Channels,
                                 IrcConnection = AlarisBot.Instance.Connection,
                                 User = user
                                 
                             };

                var parl = new List<object>();

                if(handler.IsParameterized)
                {
                    parl.Add(mp);

                    

                    if (!cmd.IsParameterCountUnspecified)
                    {
                        parl.AddRange(parameters);
                        var pdiff = handler.Method.GetParameters().Length - (parl.Count + 1);

                        if (pdiff > 0)
                        {
                            for (var i = 0; i <= pdiff; ++i)
                                parl.Add(null);
                        }
                    }

                }
                else
                    parl.Add(mp);

                /*object[] passParams = handler.IsParameterized
                                          ? new object[] {mp, parameters.ToArray()}
                                          : new object[] {mp};*/


                
                
                if(handler.IsParameterized && cmd.IsParameterCountUnspecified)
                {
                    var prms = new ArrayList {mp, parameters.ToArray()};
                    Log.Info("Invoking command handler method ({0}) ({1})", handler.Method.Name, prms.Count);
                    handler.Method.Invoke(null, prms.ToArray());
                    return;
                }

                Log.Info("Invoking command handler method ({0}) ({1})", handler.Method.Name, parl.Count);

                handler.Method.Invoke(null, parl.ToArray());

            }
            catch(Exception x)
            {
                Log.ErrorException(string.Format("Exception thrown during command recognition ({0})", x), x);
                return;
            }
        }

    }
}

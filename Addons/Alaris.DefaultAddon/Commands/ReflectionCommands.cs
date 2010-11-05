using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Alaris.Commands;
using NLog;

namespace Alaris.DefaultAddon.Commands
{
    public static class ReflectionCommands
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [ParameterizedAlarisCommand("call", CommandPermission.Admin, 0, true)]
        public static void HandleCallCommand(AlarisMainParameter mp, params string[] parameters)
        {
            var methodName = parameters[0];

            var pars = new ArrayList();

            for(var i = 1; i < parameters.Length; ++i)
            {
                pars.Add(parameters[i]);
            }

            var atype = AlarisBot.Instance.GetType();

            var method = atype.GetMethods().FirstOrDefault(minfo => minfo.Name.Equals(methodName, 
                StringComparison.InvariantCultureIgnoreCase) 
                && minfo.GetParameters().Length == pars.Count);

            if(method == null)
            {
                Log.Info("Specified method {0} is not found inside AlarisBot matching parameter count {1}", methodName, pars.Count);
            }
            else
            {
                try
                {
                    var ret = method.Invoke(AlarisBot.Instance, pars.ToArray());


                    mp.IrcConnection.Sender.PublicMessage(mp.Channel, ret.ToString());
                }
                catch(Exception x)
                {
                    Log.ErrorException(string.Format("Exception thrown while calling the specified method ({0})", x.Message), x);
                }

                return;
            }

            Log.Debug("Searching for method in all available assemblies. (Parameter count is {0})", pars.Count);

            var asms = AddonManager.Assemblies.ToList();
            asms.Add(AlarisBot.Instance.GetType().Assembly);
            asms.AddRange(AppDomain.CurrentDomain.GetAssemblies());

            foreach(var asm in asms)
            {
                foreach(var type in asm.GetTypes())
                {
                    foreach(var mi in type.GetMethods(BindingFlags.Static | BindingFlags.InvokeMethod))
                    {
                        if(mi.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase) && mi.GetParameters().Length == pars.Count)
                        {
                            method = mi;
                            break;
                        }
                    }
                }
            }

            if(method != null)
            {
                try
                {
                    var ret = method.Invoke(null, pars.ToArray());

                    if (method.ReturnType == typeof(string))
                        mp.IrcConnection.Sender.PublicMessage(mp.Channel, ret as string);
                }
                catch (Exception x)
                {
                    Log.ErrorException(string.Format("Exception thrown while calling the specified method ({0})", x.Message), x);
                }

            }

        }

        [AlarisCommand("ListMethods", CommandPermission.Admin)]
        public static void HandleListMethodsCommand(AlarisMainParameter mp)
        {
            var sb = new StringBuilder();

            foreach(var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach(var mi in type.GetMethods(BindingFlags.Public | BindingFlags.InvokeMethod))
                {
                    sb.AppendFormat("{0}.{1}({2})", type.Name, mi.Name, mi.GetParameters().Length);
                }
            }

            var fl = sb.ToString();
            
            if(!string.IsNullOrEmpty(fl))
                mp.IrcConnection.Sender.PublicMessage(mp.Channel, fl);
        }

        [AlarisCommand("ListLoadedAssemblies", CommandPermission.Admin)]
        public static void HandleListAssembliesCommand(AlarisMainParameter mp)
        {
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
                mp.IrcConnection.Sender.PublicMessage(mp.Channel, asm.GetName().Name);
        }
    }
}

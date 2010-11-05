using System.Reflection;

namespace Alaris.Commands
{
    /// <summary>
    /// A method inside Alaris.
    /// </summary>
    public sealed class AlarisMethod
    {
        /// <summary>
        /// The method contained in this instance.
        /// </summary>
        public MethodInfo Method { get; private set; }
        /// <summary>
        /// The command's attribute.
        /// </summary>
        public AlarisCommandAttribute CommandAttribute { get; private set; }

        /// <summary>
        /// The subcommand's attribute.
        /// </summary>
        public AlarisSubCommandAttribute SubCommandAttribute { get; private set; }

        /// <summary>
        /// Gets whether the method and command has params or not.
        /// </summary>
        public bool IsParameterized { get; private set; }

        /// <summary>
        /// Creates a new instance of AlarisMethod.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="attr"></param>
        /// <param name="isParameterized"></param>
        public AlarisMethod(MethodInfo method, AlarisCommandAttribute attr, bool isParameterized)
        {
            Method = method;
            CommandAttribute = attr;
            IsParameterized = isParameterized;
        }

        /// <summary>
        /// Creates a new instance of AlarisMethod.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="attr"></param>
        /// <param name="isParameterized"></param>
        public AlarisMethod(MethodInfo method, AlarisSubCommandAttribute attr, bool isParameterized)
        {
            Method = method;
            SubCommandAttribute = attr;
            IsParameterized = isParameterized;
        }
      
    }
}

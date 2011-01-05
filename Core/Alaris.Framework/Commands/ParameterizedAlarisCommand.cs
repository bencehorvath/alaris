using System;

namespace Alaris.Framework.Commands
{
    /// <summary>
    /// Marks a method as a parameterized Alaris command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ParameterizedAlarisCommand : AlarisCommandAttribute
    {
        #region Properties

        /// <summary>
        /// Number of parameters for the command.
        /// </summary>
        public int ParameterCount { get; private set; }

        /// <summary>
        /// Gets whether the parameter count is unspecified or not.
        /// </summary>
        public bool IsParameterCountUnspecified { get; private set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Marks a method as a parameterized Alaris command.
        /// </summary>
        /// <param name="command">Alaris command.</param>
        /// <param name="permission">Access permission.</param>
        /// <param name="numParams">Number of parameters.</param>
        /// <param name="isParamCountUnknown">Specifies whether the parameter count for this command unknown or not.</param>
        public ParameterizedAlarisCommand(string command, CommandPermission permission = CommandPermission.Normal, int numParams = 1, bool isParamCountUnknown = false) : base(command, permission)
        {
            Command = command;
            Permission = permission;
            ParameterCount = numParams;
            IsParameterCountUnspecified = (isParamCountUnknown && numParams == 0);
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized Alaris sub-command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ParameterizedAlarisSubCommand : AlarisSubCommandAttribute
    {
        #region Properties

        /// <summary>
        /// Number of parameters for the command.
        /// </summary>
        public int ParameterCount { get; private set; }

        /// <summary>
        /// Gets whether the parameter count is unspecified or not.
        /// </summary>
        public bool IsParameterCountUnspecified { get; private set; }

        #endregion 

        #region Construtor
        /// <summary>
        /// Marks a method as a parameterized Alaris sub-command.
        /// </summary>
        /// <param name="command">Alaris command.</param>
        /// <param name="permission">Access permission.</param>
        /// <param name="numParams">Number of parameters.</param>
        /// <param name="isParamCountUnknown">Specifies whether the parameter count for this command unknown or not.</param>
        public ParameterizedAlarisSubCommand(string command, CommandPermission permission = CommandPermission.Normal, int numParams = 1, bool isParamCountUnknown = false)
            : base(command, permission)
        {
            Command = command;
            Permission = permission;
            ParameterCount = numParams;
            IsParameterCountUnspecified = (isParamCountUnknown && numParams == 0);
        }

        #endregion
    }
}

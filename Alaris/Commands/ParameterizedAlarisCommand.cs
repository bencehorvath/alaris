using System;

namespace Alaris.Commands
{
    /// <summary>
    /// Marks a method as a parameterized Alaris command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ParameterizedAlarisCommand : AlarisCommandAttribute
    {
        /// <summary>
        /// Number of parameters for the command.
        /// </summary>
        public int ParameterCount { get; private set; }

        /// <summary>
        /// Marks a method as a parameterized Alaris command.
        /// </summary>
        /// <param name="command">Alaris command.</param>
        /// <param name="permission">Access permission.</param>
        /// <param name="numParams">Number of parameters.</param>
        public ParameterizedAlarisCommand(string command, CommandPermission permission = CommandPermission.Normal, int numParams = 1) : base(command, permission)
        {
            Command = command;
            Permission = permission;
            ParameterCount = numParams;
        }
    }
}

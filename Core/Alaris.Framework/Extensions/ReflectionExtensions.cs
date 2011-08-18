using System;
using System.Diagnostics.Contracts;

namespace Alaris.Framework.Extensions
{
    /// <summary>
    /// Reflection related extension methods.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Checks if the type is a simple type.
        /// 
        /// Simple types are primitive types and strings.
        /// </summary>
        [Pure]
        public static bool IsSimple(this Type type)
        {
            Contract.Requires(type != null);

            return type.IsEnum || type.IsNumeric() || type == typeof(string) || type == typeof(char) ||
                type == typeof(bool);
        }

        /// <summary>
        /// Determines whether the specified type is numeric.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is numeric; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsNumeric(this Type type)
        {
            Contract.Requires(type != null);

            return type.IsInteger() || type.IsFloatingPoint();
        }

        /// <summary>
        /// Determines whether the specified type is floating point.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if floating point; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsFloatingPoint(this Type type)
        {
            Contract.Requires(type != null);

            return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        }

        /// <summary>
        /// Determines whether the specified type is integer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is integer; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsInteger(this Type type)
        {
            Contract.Requires(type != null);

            return type == typeof(int) || type == typeof(uint) || type == typeof(short) || type == typeof(ushort) ||
                type == typeof(byte) || type == typeof(sbyte) || type == typeof(long) || type == typeof(ulong);
        }
    }
}

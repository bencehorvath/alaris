﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Linq;

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

        /// <summary>
        /// Gets the custom attributes of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of attribute to check</typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [Pure]
        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider type)
            where T : Attribute
        {
            Contract.Requires(type != null);
            Contract.Ensures(Contract.Result<T[]>() != null);

            var attribs = type.GetCustomAttributes(typeof(T), false) as T[];
            Contract.Assume(attribs != null);
            return attribs;
        }

        /// <summary>
        /// Gets the custom attribute of the specified type.
        /// </summary>
        /// <typeparam name="T">The attribute to check</typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [Pure]
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider type)
            where T : Attribute
        {
            Contract.Requires(type != null);

            return type.GetCustomAttributes<T>().TryGet(0);
        }

        /// <summary>
        /// Gets the types that implement the specified interface in the assembly.
        /// </summary>
        /// <param name="asm">The assembly to check in.</param>
        /// <param name="interfaceType">The type to check.</param>
        /// <returns></returns>
        public static List<Type> GetTypesWithInterface(this Assembly asm, Type interfaceType)
        {
            Contract.Requires(interfaceType != null);

            var types = (from t in asm.GetTypes().AsParallel()
                          where t.GetInterfaces().Contains(interfaceType)
                          select t).ToList();

            return types;
        }
    }
}

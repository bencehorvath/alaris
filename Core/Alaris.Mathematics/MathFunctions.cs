using System;
using System.Collections.Generic;
using Alaris.Mathematics.Algorithms;

namespace Alaris.Mathematics
{
    /// <summary>
    /// Provides simple mathematic methods.
    /// </summary>
    public static class MathFunctions
    {
        /// <summary>
        /// Applies the quicksort algorithm to a specified list.
        /// </summary>
        /// <typeparam name="T">Type of the list</typeparam>
        /// <param name="list">The list to process.</param>
        /// <returns>The sorted list.</returns>
        public static List<T> QuickSort<T>(IList<T> list) where T: IComparable
        {
            var qs = new QuickSort<T>(list);
            qs.Sort();

            return new List<T>(qs.Output);
        }

        /// <summary>
        /// Applies the quicksort algorithm to a specified array.
        /// </summary>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <param name="arr">The array to process.</param>
        /// <returns>The sorted array</returns>
        public static T[] QuickSort<T>(T[] arr) where T: IComparable
        {
            var qs = new QuickSort<T>(arr);
            qs.Sort();

            return qs.Output;
        }
    }
}

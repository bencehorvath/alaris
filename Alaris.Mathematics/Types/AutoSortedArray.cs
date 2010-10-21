﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alaris.Mathematics.Types
{
    public sealed class AutoSortedArray<T>
        where T: IComparable
    {
        private List<T> _array;

        /// <summary>
        /// Gets the sorted array.
        /// </summary>
        public List<T> Array { get; private set; }

        public AutoSortedArray(IEnumerable<T> list)
        {
            _array = list as List<T>;
            //sort here.

            Sort();
           
        }

        public AutoSortedArray()
        {
            _array = new List<T>();
        }

        /// <summary>
        /// Add the specified item to the sorted array.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            _array.Add(item);
            Sort();
        }

        /// <summary>
        /// Adds the specified items to the sorted array.
        /// </summary>
        /// <param name="items">Items to add.</param>
        public void Add(params T[] items)
        {
            foreach (var item in items)
                _array.Add(item);

            Sort();
        }

        /// <summary>
        /// Adds the specified item to the array without running sort.
        /// </summary>
        /// <param name="item">The item to add</param>
        public void SimpleAdd(T item)
        {
            _array.Add(item);
        }

        /// <summary>
        /// Runs the sort algorithm manually.
        /// </summary>
        public void Sort()
        {
            _array = MathFunctions.QuickSort(_array);    
        }

        /// <summary>
        /// Converts the array to a readable string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append('{');

            for (var index = 0; index < _array.Count; index++)
            {
                var item = _array[index];

                builder.AppendFormat(index == _array.Count - 1 ? "{0}" : "{0}, ", Convert.ToString(item));
            }

            builder.Append('}');

            return builder.ToString();
        }

    }
}
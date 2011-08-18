using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Alaris.Framework.Extensions
{
    /// <summary>
    /// Collection related extension methods.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns the entry in this list at the given index, or the default value of the element
        /// type if the index was out of bounds.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="list">The list to retrieve from.</param>
        /// <param name="index">The index to try to retrieve at.</param>
        /// <returns>The value, or the default value of the element type.</returns>
        public static T TryGet<T>(this IList<T> list, int index)
        {
            Contract.Requires(list != null);
            Contract.Requires(index >= 0);

            return index >= list.Count ? default(T) : list[index];
        }

        /// <summary>
        /// Returns the entry in this dictionary at the given key, or the default value of the key
        /// if none.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dict">The dictionary to operate on.</param>
        /// <param name="key">The key of the element to retrieve.</param>
        /// <returns>The value (if any).</returns>
        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            Contract.Requires(dict != null);
            Contract.Requires(key != null);

            TValue val;
            return dict.TryGetValue(key, out val) ? val : default(TValue);
        }
    }
}

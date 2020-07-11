using System;
using System.Collections.Generic;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Represents an in memory cache and provides methods and properties for accessing the items in the cache.
    /// </summary>
    /// <typeparam name="T">The type of the payload carried by the cache item.</typeparam>
    public interface ICache<T>
        : IEnumerable<KeyValuePair<string, ICacheItem<T>>>,
          System.Collections.IEnumerable,
          System.Collections.Specialized.INotifyCollectionChanged
    {
        /// <summary>
        /// Adds the specified item to the cache using the specified key.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <param name="item">The item to be added to the cache.</param>
        void AddItem(string key, ICacheItem<T> item);
        /// <summary>
        /// Adds the specified item to the cache using the <see cref="dodSON.Core.Cache.ICacheItem{T}"/>.Id as the key.
        /// </summary>
        /// <param name="item">The item to be added to the cache.</param>
        void AddItem(ICacheItem<T> item);
        /// <summary>
        /// Retrieves a specified cache entry.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <returns>The item retrieved from the cache. If the value of the key parameter is not found, returns null.</returns>
        ICacheItem<T> GetItem(string key);
        /// <summary>
        /// Remove the specified item from the cache.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <returns>The item removed from the cache. If the value of the key parameter is not found, returns null.</returns>
        ICacheItem<T> RemoveItem(string key);
        /// <summary>
        /// Determines if the specified cache entry exists.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <returns>True if the cache contains the key; otherwise false.</returns>
        bool ContainsKey(string key);
        /// <summary>
        /// Processes all items in the cache and removes any which are not valid.
        /// </summary>
        /// <returns>The items removed.</returns>
        IEnumerable<ICacheItem<T>> Purge();
        /// <summary>
        /// Processes all items in the cache and removes any which are not valid. Returns <b>true</b> if any items were purged from the cache; otherwise it will return <b>false</b>. The <paramref name="purgedItems"/> list will contain any purged items or will be an empty array, respectively.
        /// </summary>
        /// <param name="purgedItems">If any items were purged from the cache will be contained in this list.</param>
        /// <returns>Returns <b>true</b> if any items were purged from the cache; otherwise it will return <b>false</b>. The <paramref name="purgedItems"/> list will contain any purged items or will be an empty array, respectively.</returns>
        bool TryPurge(out List<ICacheItem<T>> purgedItems);
        /// <summary>
        /// Clears all items in the cache.
        /// </summary>
        /// <returns>The items removed.</returns>
        IEnumerable<ICacheItem<T>> Clear();
        /// <summary>
        /// The number of items in the cache.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Returns an enumerator that can iterate through the keys in the cache.
        /// </summary>
        System.Collections.Generic.IEnumerable<string> Keys { get; }
        /// <summary>
        /// Returns an enumerator that can iterate through the items in the cache.
        /// </summary>
        System.Collections.Generic.IEnumerable<ICacheItem<T>> Items { get; }
        /// <summary>
        /// Retrieves a specified cache entry.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <returns>The item retrieved from the cache. If the value of the key parameter is not found, returns null.</returns>
        ICacheItem<T> this[string key] { get; set; }
    }
}

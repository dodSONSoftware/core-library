using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Represents a cache item validater and provides a property to test the validity of the cache item.
    /// </summary>
    /// <typeparam name="T">The type of the payload carried by the cache item.</typeparam>
    public interface ICacheValidater<T>
    {
        /// <summary>
        /// When implemented, should get a value indicating whether the given cache item is valid or not. <b>True</b> indicates it is valid; otherwise <b>false</b>.
        /// </summary>
        /// <param name="cacheItem">The ICacheItem whose validity is being tested.</param>
        /// <returns>Returns a value indicating whether the given cache item is valid or not. <b>True</b> indicates it is valid; otherwise <b>false</b>.</returns>
        bool IsValid(ICacheItem<T> cacheItem);
        /// <summary>
        /// When implemented, should restore the <see cref="ICacheValidater{T}"/> to it initial start state.
        /// </summary>
        void Reset();
        /// <summary>
        /// When implemented, should make the <see cref="ICacheItem{T}"/> invalid; this will cause it to be purged at the very next purge cycle.
        /// </summary>
        void MarkInvalid();
    }
}

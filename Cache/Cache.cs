using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Represents an in memory cache and provides methods and properties for accessing the items in the cache.
    /// </summary>
    /// <typeparam name="T">The type of the payload carried by the cache item.</typeparam>
    /// <example>
    /// The following code example will populate a cache with items carrying different validaters and exercise the cache over a few second.
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// [Serializable]
    /// public class DataHolder
    /// {
    ///     public DataHolder() { }
    ///
    ///     public string Alpha { get; set; }
    ///     public int Beta { get; set; }
    ///     public decimal Gamma { get; set; }
    ///     public byte[] Delta { get; set; }
    /// }
    /// 
    /// static void Main(string[] args)
    /// {
    ///     Console.WriteLine("---- creating and populating cache");
    ///     // create validaters
    ///     var validater1 = new dodSON.Core.Cache.ValidateTime&lt;CodeExamplesForXMLCommentsSharedAssembly.DataHolder&gt;(DateTime.Now.AddSeconds(3));
    ///     var validater2 = new dodSON.Core.Cache.ValidateTime&lt;CodeExamplesForXMLCommentsSharedAssembly.DataHolder&gt;(DateTime.Now.AddSeconds(6));
    ///     var validater3 = new dodSON.Core.Cache.ValidateTime&lt;CodeExamplesForXMLCommentsSharedAssembly.DataHolder&gt;(DateTime.Now.AddSeconds(9));
    ///     
    ///     // create cache
    ///     var cache = new dodSON.Core.Cache.Cache&lt;CodeExamplesForXMLCommentsSharedAssembly.DataHolder&gt;();
    ///     
    ///     // add items to cache
    ///     for (int i = 0; i &lt; 12; i++)
    ///     {
    ///         var payload = new CodeExamplesForXMLCommentsSharedAssembly.DataHolder()
    ///                           {
    ///                               Alpha = "Randy",
    ///                               Beta = 1,
    ///                               Gamma = (decimal)(i * 3.1415926),
    ///                               Delta = new byte[] { 0x00, 0x80, 0xff }
    ///                           };
    ///         var validater = ((i % 3) == 0) ? validater1 : ((i % 3) == 1) ? validater2 : validater3;
    ///         var item = new dodSON.Core.Cache.CacheItem&lt;CodeExamplesForXMLCommentsSharedAssembly.DataHolder&gt;(payload, validater);
    ///         cache.AddItem(item);
    ///     }
    ///     
    ///     // start a stop watch
    ///     var stWatch = System.Diagnostics.Stopwatch.StartNew();
    ///     
    ///     // just to be sure the fractions-of-a-second in the output are clear
    ///     dodSON.Core.Threading.ThreadingHelper.Sleep(256);
    ///     for (int i = 0; i &lt; 12; i++)
    ///     {
    ///         Console.WriteLine();
    ///         Console.WriteLine(string.Format("Seconds= {0}", stWatch.Elapsed));
    ///         
    ///         // purge
    ///         var purgedCount = cache.Purge().Count();
    ///         Console.WriteLine(string.Format("Items Purged= {0}", purgedCount));
    ///         
    ///         // display cache count
    ///         Console.WriteLine(string.Format("cache.Count= {0}", cache.Count));
    ///         
    ///         // wait 1 second
    ///         Console.WriteLine("---- waiting");
    ///         dodSON.Core.Threading.ThreadingHelper.Sleep(1000);
    ///     }
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    ///
    /// // ---- creating and populating cache
    /// // 
    /// // Seconds= 00:00:00.2556092
    /// // Items Purged= 0
    /// // cache.Count= 12
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:01.2577121
    /// // Items Purged= 0
    /// // cache.Count= 12
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:02.2577628
    /// // Items Purged= 0
    /// // cache.Count= 12
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:03.2578153
    /// // Items Purged= 4
    /// // cache.Count= 8
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:04.2588807
    /// // Items Purged= 0
    /// // cache.Count= 8
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:05.2588871
    /// // Items Purged= 0
    /// // cache.Count= 8
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:06.2589297
    /// // Items Purged= 4
    /// // cache.Count= 4
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:07.2589792
    /// // Items Purged= 0
    /// // cache.Count= 4
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:08.2590827
    /// // Items Purged= 0
    /// // cache.Count= 4
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:09.2590942
    /// // Items Purged= 4
    /// // cache.Count= 0
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:10.2591126
    /// // Items Purged= 0
    /// // cache.Count= 0
    /// // ---- waiting
    /// // 
    /// // Seconds= 00:00:11.2591824
    /// // Items Purged= 0
    /// // cache.Count= 0
    /// // ---- waiting
    /// // 
    /// // press anykey...
    /// </code>
    /// </example>    
    [Serializable]
    public class Cache<T>
        : ICache<T>
    {
        #region System.Collections.Specialized.INotifyCollectionChanged Methods
        /// <summary>
        /// Occurs when an item is added, replaced, removed or the entire list is cleared.
        /// </summary>
        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>
        /// Will raise the collection changed event.
        /// </summary>
        /// <param name="action">The action that is being performed.</param>
        /// <param name="newOrChangedItem">The new, changed or deleted item.</param>
        /// <param name="oldItem">If an item is being replaced, this is the original item.</param>
        protected void RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction action,
                                                   ICacheItem<T> newOrChangedItem,
                                                   ICacheItem<T> oldItem)
        {
            var handler = CollectionChanged;
            if (handler != null)
            {
                if (action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    handler(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(action));
                }
                else if (action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                {
                    handler(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(action, newOrChangedItem, oldItem));
                }
                else if (action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    handler(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(action, oldItem));
                }
                else
                {
                    handler(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(action, newOrChangedItem));
                }
            }
        }
        #endregion
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the cache class.
        /// </summary>
        public Cache()
        {
        }
        #endregion
        #region Private Fields
        private Dictionary<string, ICacheItem<T>> _InternalCache = new Dictionary<string, ICacheItem<T>>();
        #endregion
        #region IEnumerable<KeyValuePair<,>> Methods
        /// <summary>
        /// Provides an enumerator to iterate over all of the keys and values in the cache.
        /// </summary>
        /// <returns>An enumerator to iterate over all of the keys and values in this cache.</returns>
        public IEnumerator<KeyValuePair<string, ICacheItem<T>>> GetEnumerator() => _InternalCache.GetEnumerator() as IEnumerator<KeyValuePair<string, ICacheItem<T>>>;
        /// <summary>
        /// Provide enumerator to iterate over the cache.
        /// </summary>
        /// <returns>An enumerator to iterate over the cache.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _InternalCache.GetEnumerator() as System.Collections.IEnumerator;
        #endregion
        #region Public Methods
        /// <summary>
        /// Adds the specified item to the cache using the specified key.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <param name="item">The item to be added to the cache.</param>
        public void AddItem(string key,
                            ICacheItem<T> item)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _InternalCache.Add(key, item);
            RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction.Add, item, null);
        }
        /// <summary>
        /// Adds the specified item to the cache using the <see cref="dodSON.Core.Cache.ICacheItem{T}"/>.Id as the key.
        /// </summary>
        /// <param name="item">The item to be added to the cache.</param>
        public void AddItem(ICacheItem<T> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            AddItem(item.Id, item);
        }
        /// <summary>
        /// Determines if the specified cache entry exists.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <returns>True if the cache contains the key; otherwise false.</returns>
        public bool ContainsKey(string key) => _InternalCache.ContainsKey(key);
        /// <summary>
        /// Retrieves a specified cache item by key.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <returns>The item retrieved from the cache. If the value of the key parameter is not found, returns null.</returns>
        public ICacheItem<T> GetItem(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return this[key];
        }
        /// <summary>
        /// Remove the specified item from the cache.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <returns>The item removed from the cache. If the value of the key parameter is not found, returns null.</returns>
        public ICacheItem<T> RemoveItem(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            ICacheItem<T> item = null;
            if (_InternalCache.ContainsKey(key))
            {
                // save entry 
                item = _InternalCache[key];
                // remove the entry
                _InternalCache.Remove(key);
                // raise 'item removed' event
                RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, null, item);
            }
            // return removed entry
            return item;
        }
        /// <summary>
        /// Processes all items in the cache and removes any which are not valid.
        /// </summary>
        /// <returns>The number of items removed.</returns>
        public IEnumerable<ICacheItem<T>> Purge()
        {
            if (TryPurge(out List<ICacheItem<T>> purgedItems))
            {
                return purgedItems;
            }
            return new List<ICacheItem<T>>();
        }
        /// <summary>
        /// Processes all items in the cache and removes any which are not valid. Returns <b>true</b> if any items were purged from the cache; otherwise it will return <b>false</b>. The <paramref name="purgedItems"/> list will contain any purged items or will be an empty array, respectively.
        /// </summary>
        /// <param name="purgedItems">If any items were purged from the cache will be contained in this list.</param>
        /// <returns>Returns <b>true</b> if any items were purged from the cache; otherwise it will return <b>false</b>. The <paramref name="purgedItems"/> list will contain any purged items or will be an empty array, respectively.</returns>
        public bool TryPurge(out List<ICacheItem<T>> purgedItems)
        {
            purgedItems = new List<ICacheItem<T>>();
            var deleteCandidates = new List<string>();
            // process each item in the list
            foreach (var item in from x in _InternalCache
                                 where !x.Value.IsValid
                                 select x)
            {
                deleteCandidates.Add(item.Key);
            }
            if (deleteCandidates.Count > 0)
            {
                // delete all 'delete candidates'
                foreach (var key in deleteCandidates)
                {
                    purgedItems.Add(RemoveItem(key));
                }
                return true;
            }
            //
            return false;

        }
        /// <summary>
        /// Clears all items in the cache.
        /// </summary>
        /// <returns>The items removed.</returns>
        public IEnumerable<ICacheItem<T>> Clear()
        {
            var list = new List<ICacheItem<T>>();
            // process each item in the list
            var keyList = new List<string>();
            foreach (var item in _InternalCache.Keys)
            {
                keyList.Add(item);
            }
            foreach (var key in keyList)
            {
                var item = RemoveItem(key);
                if (item != null)
                {
                    list.Add(RemoveItem(key));
                }
            }
            // raise 'item removed' event
            RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction.Reset, null, null);
            //
            return list;
        }
        /// <summary>
        /// Gets the number of items in the cache.
        /// </summary>
        public int Count => _InternalCache.Count;
        /// <summary>
        /// Returns an enumerator that can iterate through the keys in the cache.
        /// </summary>
        public IEnumerable<string> Keys => _InternalCache.Keys;
        /// <summary>
        /// Returns an enumerator that can iterate through the items in the cache.
        /// </summary>
        public IEnumerable<ICacheItem<T>> Items => _InternalCache.Values;
        /// <summary>
        /// Retrieves a specified cache entry.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <returns>The item retrieved from the cache. If the value of the key parameter is not found, returns null.</returns>
        public ICacheItem<T> this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }
                ICacheItem<T> item = null;
                if (_InternalCache.ContainsKey(key))
                {
                    item = _InternalCache[key];
                }
                return item;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }
                if (!_InternalCache.ContainsKey(key))
                {
                    throw new IndexOutOfRangeException($"The specified key cannot be found. Key={key}");
                }
                var oldValue = _InternalCache[key];
                _InternalCache[key] = value;
                RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction.Replace, value, oldValue);
            }
        }
        #endregion
    }
}

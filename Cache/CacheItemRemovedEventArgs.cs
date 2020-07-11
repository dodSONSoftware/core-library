using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Provides data when a cache item is removed from the cache.
    /// </summary>
    /// <typeparam name="T">The type of the payload carried by the cache item.</typeparam>
    /// <example>
    /// The following code example will populate a cache with items carrying a validater with random expiration times and exercise the cache over a few second.
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
    ///     // create random number generator and a stop watch
    ///     var rnd = new Random();
    ///     var stWatch = new System.Diagnostics.Stopwatch();
    ///     
    ///     // create cache
    ///     Console.WriteLine();
    ///     Console.WriteLine("---- creating and populating cache");
    ///     var cache = new dodSON.Core.Cache.Cache&lt;DataHolder&gt;();
    ///     
    ///     // register with the cache's CollectionChanged event
    ///     cache.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(
    ///           (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =&gt;
    ///           {
    ///               System.Collections.IList list = null;
    ///               if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
    ///               {
    ///                   list = e.NewItems;
    ///               }
    ///               else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
    ///               {
    ///                   list = e.OldItems;
    ///               }
    ///               else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
    ///               {
    ///                   for (int i = 0; i &lt; e.NewItems.Count; i++)
    ///                   {
    ///                       var newItem = e.NewItems[i] as dodSON.Core.Cache.ICacheItem&lt;DataHolder&gt;;
    ///                       var oldItem = e.OldItems[i] as dodSON.Core.Cache.ICacheItem&lt;DataHolder&gt;;
    ///                       if (newItem != null &amp;&amp; oldItem != null)
    ///                       {
    ///                           Console.WriteLine(string.Format("{0}; {1} Item: oldItem.Beta= {2}; newItem.Beta= {3}; Cache.Count= {4}",
    ///                                                                  stWatch.Elapsed,
    ///                                                                  e.Action,
    ///                                                                  oldItem.Payload.Beta,
    ///                                                                  newItem.Payload.Beta,
    ///                                                                  cache.Count));
    ///                       }
    ///                   }
    ///               }
    ///               else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
    ///               {
    ///                   // both lists will be null
    ///               }
    ///               // process the list, if any (add/remove)
    ///               if (list != null)
    ///               {
    ///                   foreach (dodSON.Core.Cache.ICacheItem&lt;DataHolder&gt; item in list)
    ///                   {
    ///                       Console.WriteLine(string.Format("{0}; {1} Item: Beta= {2}; Cache.Count= {3}",
    ///                                                           stWatch.Elapsed,
    ///                                                           e.Action,
    ///                                                           item.Payload.Beta,
    ///                                                           cache.Count));
    ///                   }
    ///               }
    ///           });
    ///           
    ///     // add items to cache
    ///     for (int i = 0; i &lt; 12; i++)
    ///     {
    ///         var payload = new DataHolder()
    ///                           {
    ///                               Alpha = "Randy",
    ///                               Beta = i,
    ///                               Gamma = (decimal)(i * 3.1415926),
    ///                               Delta = new byte[] { 0x00, 0x80, 0xff }
    ///                           };
    ///         var secondsAhead = (double)rnd.Next(1, 20) / 2.0;
    ///         var expirationDateTime = DateTime.Now.AddSeconds(secondsAhead);
    ///         var validater = new dodSON.Core.Cache.ValidateTime&lt;DataHolder&gt;(expirationDateTime);
    ///         var item = new dodSON.Core.Cache.CacheItem&lt;DataHolder&gt;(payload, validater);
    ///         cache.AddItem(item);
    ///     }
    ///     
    ///     // remove item
    ///     Console.WriteLine();
    ///     Console.WriteLine("---- removing last item");
    ///     var key = cache.Last().Key;
    ///     cache.RemoveItem(key);
    ///     
    ///     // replace item
    ///     Console.WriteLine();
    ///     Console.WriteLine("---- replacing first item");
    ///     var payload2 = new DataHolder()
    ///                           {
    ///                               Alpha = "Randy",
    ///                               Beta = 1024,
    ///                               Gamma = 3.1415926M,
    ///                               Delta = new byte[] { 0x00, 0x80, 0xff }
    ///                           };
    ///     var validater2 = new dodSON.Core.Cache.ValidateTime&lt;DataHolder&gt;(DateTime.Now.AddSeconds(2.75));
    ///     cache[cache.First().Key] = new dodSON.Core.Cache.CacheItem&lt;DataHolder&gt;(payload2, validater2);
    ///     
    ///     // display information
    ///     Console.WriteLine();
    ///     Console.WriteLine(string.Format("Cache.Count= {0}", cache.Count));
    ///     Console.WriteLine();
    ///     Console.WriteLine("---- press anykey to stop purging the cache");
    ///     Console.WriteLine("---- or wait until all items have been purged...");
    ///     Console.WriteLine();
    ///     
    ///     // start the stop watch
    ///     stWatch.Start();
    ///     do
    ///     {
    ///         cache.Purge();
    ///         dodSON.Core.Threading.ThreadingHelper.Sleep(100);
    ///     } while (!Console.KeyAvailable &amp;&amp; cache.Count &gt; 0);
    ///     if (Console.KeyAvailable) { Console.ReadKey(true); }
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    ///
    /// // This code produces output similar to the following:
    ///
    /// // ---- creating and populating cache
    /// // 00:00:00; Add Item: Beta= 0; Cache.Count= 1
    /// // 00:00:00; Add Item: Beta= 1; Cache.Count= 2
    /// // 00:00:00; Add Item: Beta= 2; Cache.Count= 3
    /// // 00:00:00; Add Item: Beta= 3; Cache.Count= 4
    /// // 00:00:00; Add Item: Beta= 4; Cache.Count= 5
    /// // 00:00:00; Add Item: Beta= 5; Cache.Count= 6
    /// // 00:00:00; Add Item: Beta= 6; Cache.Count= 7
    /// // 00:00:00; Add Item: Beta= 7; Cache.Count= 8
    /// // 00:00:00; Add Item: Beta= 8; Cache.Count= 9
    /// // 00:00:00; Add Item: Beta= 9; Cache.Count= 10
    /// // 00:00:00; Add Item: Beta= 10; Cache.Count= 11
    /// // 00:00:00; Add Item: Beta= 11; Cache.Count= 12
    /// // 
    /// // ---- removing last item
    /// // 00:00:00; Remove Item: Beta= 11; Cache.Count= 11
    /// // 
    /// // ---- replacing first item
    /// // 00:00:00; Replace Item: oldItem.Beta= 0; newItem.Beta= 1024; Cache.Count= 11
    /// // 
    /// // Cache.Count= 11
    /// // 
    /// // ---- press anykey to stop purging the cache
    /// // ---- or wait until all items have been purged...
    /// // 
    /// // 00:00:01.0011492; Remove Item: Beta= 7; Cache.Count= 10
    /// // 00:00:01.6012902; Remove Item: Beta= 4; Cache.Count= 9
    /// // 00:00:01.6015259; Remove Item: Beta= 6; Cache.Count= 8
    /// // 00:00:02.8013138; Remove Item: Beta= 1024; Cache.Count= 7
    /// // 00:00:03.0013348; Remove Item: Beta= 1; Cache.Count= 6
    /// // 00:00:03.0015193; Remove Item: Beta= 5; Cache.Count= 5
    /// // 00:00:06.0014489; Remove Item: Beta= 2; Cache.Count= 4
    /// // 00:00:08.0015996; Remove Item: Beta= 3; Cache.Count= 3
    /// // 00:00:09.6016861; Remove Item: Beta= 8; Cache.Count= 2
    /// // 00:00:09.6018661; Remove Item: Beta= 9; Cache.Count= 1
    /// // 00:00:09.6020312; Remove Item: Beta= 10; Cache.Count= 0
    /// // 
    /// // press anykey...
    /// </code>
    /// </example>    
    [Serializable]
    public class CacheCollectionChangedEventArgs<T>
        : EventArgs
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the CacheItemRemovedEventArgs class.
        /// </summary>
        private CacheCollectionChangedEventArgs()
            : base() { }
        /// <summary>
        /// Initializes a new instance of the CacheItemRemovedEventArgs class.
        /// </summary>
        /// <param name="key">The key associated with the ICacheItem when is was added to the cache.</param>
        /// <param name="item">The ICacheItem being removed.</param>
        public CacheCollectionChangedEventArgs(string key,
                                         ICacheItem<T> item)
            : this()
        {
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentNullException(nameof(key)); }
            Key = key;
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }

        #endregion
        #region Public Methods
        /// <summary>
        /// Gets the key associated with the ICacheItem when is was added to the cache.
        /// </summary>
        public string Key { get; } = "";
        /// <summary>
        /// Gets the ICacheItem being removed.
        /// </summary>
        public ICacheItem<T> Item { get; } = null;
        #endregion
    }
}

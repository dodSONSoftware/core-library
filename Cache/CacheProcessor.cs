using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Provides a monitored cache which will cache and process <see cref="ICacheProcessorItem"/>s. 
    /// Each item will remain in the cache until it's <see cref="ICacheValidater{T}"/> renders it invalid; as each item is purged the <see cref="ICacheProcessorItem"/>'s <see cref="ICacheProcessorItem.Process"/> will be executed in its own thread.
    /// If an item is removed, its <see cref="ICacheProcessorItem.Process"/>, an <see cref="Action{ICacheProcessorItem}"/>, will not be executed.
    /// </summary>
    /// <example>
    /// The following code example will create a <see cref="dodSON.Core.Cache.CacheProcessor"/>, start it and add two <see cref="ICacheProcessorItem"/>s in the form of the custom class MyItem.
    /// <br/><br/>
    /// Each MyItem contains a list of strings which will be added to, while a MyItem with the proper <see cref="ICacheProcessorItem.Key"/> is in the cache; this demonstrates access to custom data in a cached item.
    /// <br/><br/>
    /// The 'Key2' cache item's <see cref="ICacheProcessorItem.Process"/>, when it's purged and executed, will create and add another cache item to the cache, using the same <see cref="ICacheProcessorItem.Key"/> and MyItem.List; this demonstrates passing data to the next process.
    /// <br/>
    /// The first process will then wait for a period of time, simulating work, before terminating.
    /// <br/><br/>
    /// The second process, when it's executed, will create and add a third cache item to the cache, using the same <see cref="ICacheProcessorItem.Key"/> and MyItem.List. 
    /// <br/>
    /// The second process will then wait for a period of time, simulating work, before terminating.
    /// <br/><br/>
    /// The third process, when it's executed, will simply wait for a period of time, simulating work, before terminating.
    /// <br/><br/>
    /// The cache will continue to purge and process cached items as long as there are cached items to purge.
    /// <br/>
    /// When done using the <see cref="CacheProcessor"/> it should be <see cref="CacheProcessor.Stop"/>ped.
    /// <br/>
    /// Note: just because the <see cref="CacheProcessor"/> has been stopped does not mean all of the background threads have completed.
    /// <br/><br/>
    /// The program will monitor the <see cref="CacheProcessor"/>, if more than 1 second passes without any items in the cache, it will terminate the forever loop and wait for a key press to exit.
    /// However, even when it has determined there are no items in the list, there may still be threads running in the background; this demonstrates each <see cref="ICacheProcessorItem.Process"/> is run on its own thread.
    /// <br/><br/>
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    ///class Program
    /// {
    ///     static void Main(string[] args)
    ///     {
    ///         Console.WriteLine($"#### Main: CurrentDomain={System.AppDomain.CurrentDomain.Id}, ManagedThreadId={System.Threading.Thread.CurrentThread.ManagedThreadId}");
    ///
    ///         // ######## create validaters wait times
    ///         TimeSpan key1_CacheWaitTime = TimeSpan.FromSeconds(20);
    ///
    ///         TimeSpan key2_CacheWaitTime = TimeSpan.FromSeconds(10);
    ///         TimeSpan key2_step2_CacheWaitTime = TimeSpan.FromSeconds(20);
    ///
    ///         // ######## create step wait times
    ///         TimeSpan step1_WaitingTime = TimeSpan.FromSeconds(20);
    ///         TimeSpan step2_WaitingTime = TimeSpan.FromSeconds(25);
    ///         TimeSpan step3_WaitingTime = TimeSpan.FromSeconds(5);
    ///
    ///         // ######## create a cache processor
    ///         dodSON.Core.Cache.CacheProcessor cacheProcessor = new dodSON.Core.Cache.CacheProcessor();
    ///         cacheProcessor.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =&gt;
    ///                                             {
    ///                                                 // any changes in the collection of items in the cache will appear here. (add item, remove item, replace item and clear cache)
    ///                                                 var text = "";
    ///                                                 switch (e.Action)
    ///                                                 {
    ///                                                     case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
    ///                                                         text = $"Key={(e.NewItems[0] as dodSON.Core.Cache.ICacheItem&lt;dodSON.Core.Cache.ICacheProcessorItem&gt;).Payload.Key}, Created={(e.NewItems[0] as dodSON.Core.Cache.ICacheItem&lt;dodSON.Core.Cache.ICacheProcessorItem&gt;).Payload.CreatedTimeUtc}, Cache Count={cacheProcessor.Count}";
    ///                                                         break;
    ///                                                     case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
    ///                                                         text = $"Key={(e.OldItems[0] as dodSON.Core.Cache.ICacheItem&lt;dodSON.Core.Cache.ICacheProcessorItem&gt;).Payload.Key}, Created={(e.OldItems[0] as dodSON.Core.Cache.ICacheItem&lt;dodSON.Core.Cache.ICacheProcessorItem&gt;).Payload.CreatedTimeUtc}, Cache Count={cacheProcessor.Count}";
    ///                                                         break;
    ///                                                     case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
    ///                                                         text = $"Key={(e.NewItems[0] as dodSON.Core.Cache.ICacheItem&lt;dodSON.Core.Cache.ICacheProcessorItem&gt;).Payload.Key}, Old Item Created={(e.NewItems[0] as dodSON.Core.Cache.ICacheItem&lt;dodSON.Core.Cache.ICacheProcessorItem&gt;).Payload.CreatedTimeUtc}, New Item Created={(e.NewItems[0] as dodSON.Core.Cache.ICacheItem&lt;dodSON.Core.Cache.ICacheProcessorItem&gt;).Payload.CreatedTimeUtc}, Cache Count={cacheProcessor.Count}";
    ///                                                         break;
    ///                                                     case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
    ///                                                         text = $"NOT SUPPORTED";
    ///                                                         break;
    ///                                                     case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
    ///                                                         text = $"Cache Cleared, Cache Count={cacheProcessor.Count}";
    ///                                                         break;
    ///                                                     default:
    ///                                                         break;
    ///                                                 }
    ///                                                 // output any changes to the cache
    ///                                                 Console.WriteLine($"{Environment.NewLine}[- {e.Action}: {text} -]");
    ///                                             };
    ///
    ///         // start the cache processor
    ///         TimeSpan cachePurgeRequestsExecutionInterval = TimeSpan.FromSeconds(0.5);
    ///         cacheProcessor.Start(cachePurgeRequestsExecutionInterval);
    ///
    ///         // ######## add an item to the cache processor
    ///         string key1 = "Key1";
    ///         dodSON.Core.Cache.ValidateTime&lt;dodSON.Core.Cache.ICacheProcessorItem&gt; validater1 = new dodSON.Core.Cache.ValidateTime&lt;dodSON.Core.Cache.ICacheProcessorItem&gt;(DateTime.Now.Add(key1_CacheWaitTime));
    ///         List&lt;string&gt; list1 = new List&lt;string&gt;();
    ///         cacheProcessor.Add(new MyItem(key1,
    ///                                       validater1,
    ///                                       (myItem1) =&gt;
    ///                                       {
    ///                                           // output information
    ///                                           Console.WriteLine($"{Environment.NewLine}#### Key1--&gt;Step #1:  CurrentDomain={System.AppDomain.CurrentDomain.Id}, ManagedThreadId={System.Threading.Thread.CurrentThread.ManagedThreadId}, CreatedTime={myItem1.CreatedTimeUtc.ToLocalTime()}, HasProcessExecuted={myItem1.HasProcessExecuted}");
    ///                                       },
    ///                                       list1));
    ///
    ///         // ######## add another item to the cache processor
    ///         string key = "Key2";
    ///         dodSON.Core.Cache.ValidateTime&lt;dodSON.Core.Cache.ICacheProcessorItem&gt; validater2 = new dodSON.Core.Cache.ValidateTime&lt;dodSON.Core.Cache.ICacheProcessorItem&gt;(DateTime.Now.Add(key2_CacheWaitTime));
    ///         List&lt;string&gt; list2 = new List&lt;string&gt;();
    ///         cacheProcessor.Add(new MyItem(key,
    ///                                       validater2,
    ///                                       (myItem1) =&gt;
    ///                                       {
    ///                                           // cast the argument myItem1 (an ICacheProcessorItem) into MyItem
    ///                                           MyItem myItemAlpha = (myItem1 as MyItem);
    ///                                           // output information
    ///                                           Console.WriteLine($"{Environment.NewLine}#### Key2--&gt;Step #1:  CurrentDomain={System.AppDomain.CurrentDomain.Id}, ManagedThreadId={System.Threading.Thread.CurrentThread.ManagedThreadId}, CreatedTime={myItemAlpha.CreatedTimeUtc.ToLocalTime()}, HasProcessExecuted={myItemAlpha.HasProcessExecuted}");
    ///                                           myItemAlpha.List.Add("---------------- Step #1 ----------------");
    ///                                           foreach (string item in myItemAlpha.List)
    ///                                           {
    ///                                               Console.WriteLine($"        {item}");
    ///                                           }
    ///                                           // create another MyItem and add to the cache.
    ///                                           // This allows for cascading sets of cached items; i.e. performing work, caching new items until other work is ready to be performed.
    ///                                           (myItemAlpha as dodSON.Core.Cache.ICacheProcessorItemAdvanced).Validater = new dodSON.Core.Cache.ValidateTime&lt;dodSON.Core.Cache.ICacheProcessorItem&gt;(DateTime.Now.Add(key2_step2_CacheWaitTime));
    ///                                           MyItem nextItem = new MyItem(myItemAlpha.Key,
    ///                                                                        myItemAlpha.Validater,
    ///                                                                        (myItem2) =&gt;
    ///                                                                        {
    ///                                                                            // cast the argument myItem2 (an ICacheProcessorItem) into MyItem
    ///                                                                            MyItem myItemBeta = (myItem2 as MyItem);
    ///                                                                            // output information
    ///                                                                            Console.WriteLine($"{Environment.NewLine}#### Key2--&gt;Step #2:  CurrentDomain={System.AppDomain.CurrentDomain.Id}, ManagedThreadId={System.Threading.Thread.CurrentThread.ManagedThreadId}, CreatedTime={myItemBeta.CreatedTimeUtc.ToLocalTime()}, HasProcessExecuted={myItemBeta.HasProcessExecuted}");
    ///                                                                            myItemBeta.List.Add("---------------- Step #2 ----------------");
    ///                                                                            foreach (string item in myItemBeta.List)
    ///                                                                            {
    ///                                                                                Console.WriteLine($"        {item}");
    ///                                                                            }
    ///                                                                            // create another MyItem and add to the cache.
    ///                                                                            myItemBeta.Validater.Reset();
    ///                                                                            MyItem myGammaItem = new MyItem(myItemBeta.Key,
    ///                                                                                                            myItemBeta.Validater,
    ///                                                                                                            (myItem3) =&gt;
    ///                                                                                                            {
    ///                                                                                                                // cast the argument myItem3 (an ICacheProcessorItem) into MyItem
    ///                                                                                                                MyItem myItemGamma = (myItem3 as MyItem);
    ///                                                                                                                // output information
    ///                                                                                                                Console.WriteLine($"{Environment.NewLine}#### Key2--&gt;Step #3:  CurrentDomain={System.AppDomain.CurrentDomain.Id}, ManagedThreadId={System.Threading.Thread.CurrentThread.ManagedThreadId}, CreatedTime={myItemGamma.CreatedTimeUtc.ToLocalTime()}, HasProcessExecuted={myItemGamma.HasProcessExecuted}");
    ///                                                                                                                myItemGamma.List.Add("---------------- Step #3 ----------------");
    ///                                                                                                                foreach (string item in myItemGamma.List)
    ///                                                                                                                {
    ///                                                                                                                    Console.WriteLine($"        {item}");
    ///                                                                                                                }
    ///                                                                                                                // %%%%%%%% put in an artificial wait
    ///                                                                                                                Console.WriteLine($"{Environment.NewLine}Key2--&gt;Step #3 Waiting...");
    ///                                                                                                                System.Diagnostics.Stopwatch timer3 = System.Diagnostics.Stopwatch.StartNew();
    ///                                                                                                                dodSON.Core.Threading.ThreadingHelper.Sleep(step3_WaitingTime);
    ///                                                                                                                Console.WriteLine($"Key2--&gt;Step #3 Waiting Completed. {timer3.Elapsed}");
    ///                                                                                                                // %%%%%%%%
    ///                                                                                                            },
    ///                                                                                                            myItemBeta.List);
    ///                                                                            cacheProcessor.Add(myGammaItem);
    ///                                                                            // %%%%%%%% put in an artificial wait
    ///                                                                            Console.WriteLine($"{Environment.NewLine}Key2--&gt;Step #2 Waiting...");
    ///                                                                            System.Diagnostics.Stopwatch timer2 = System.Diagnostics.Stopwatch.StartNew();
    ///                                                                            dodSON.Core.Threading.ThreadingHelper.Sleep(step2_WaitingTime);
    ///                                                                            Console.WriteLine($"Key2--&gt;Step #2 Waiting Completed. {timer2.Elapsed}");
    ///                                                                            // %%%%%%%%
    ///                                                                        },
    ///                                                                        myItemAlpha.List);
    ///                                           cacheProcessor.Add(nextItem);
    ///                                           // %%%%%%%% put in an artificial wait
    ///                                           Console.WriteLine($"{Environment.NewLine}Key2--&gt;Step #1 Waiting...");
    ///                                           System.Diagnostics.Stopwatch timer1 = System.Diagnostics.Stopwatch.StartNew();
    ///                                           dodSON.Core.Threading.ThreadingHelper.Sleep(step1_WaitingTime);
    ///                                           Console.WriteLine($"Key2--&gt;Step #1 Waiting Completed. {timer1.Elapsed}");
    ///                                           // %%%%%%%%
    ///                                       },
    ///                                       list2));
    ///
    ///         // ######## wait until the cache has been clear for at least 1 second
    ///         TimeSpan loopTimeoutLimit = TimeSpan.FromSeconds(1);
    ///         TimeSpan timeoutLimit = TimeSpan.FromSeconds(1);
    ///         DateTimeOffset timeout = DateTimeOffset.Now.Add(timeoutLimit);
    ///         while (true)
    ///         {
    ///             // if no items in the cache and more than 1 second has passed: break out of the loop
    ///             if (cacheProcessor.Count == 0)
    ///             {
    ///                 if (DateTimeOffset.Now &gt; timeout)
    ///                 {
    ///                     break;
    ///                 }
    ///             }
    ///             else
    ///             {
    ///                 // reset timeout
    ///                 timeout = DateTimeOffset.Now.Add(timeoutLimit);
    ///                 // find cached MyItem by key
    ///                 MyItem dude = cacheProcessor.Find&lt;MyItem&gt;("Key2");
    ///                 if (dude != null)
    ///                 {
    ///                     // found cached MyItem
    ///                     dude.List.Add($"Key={dude.Key}, CreatedTime={dude.CreatedTimeUtc.ToLocalTime()}, HasProcessExecuted={dude.HasProcessExecuted}");
    ///                 }
    ///             }
    ///             // sleep for a bit...
    ///             dodSON.Core.Threading.ThreadingHelper.Sleep(loopTimeoutLimit);
    ///         }
    ///
    ///         // ######## stop the Cache Processor
    ///         Console.WriteLine($"Cache Processor: IsRunning={cacheProcessor.IsRunning}");
    ///         Console.WriteLine($"Cache Processor: StartedTime={cacheProcessor.StartedTimeUtc.ToLocalTime()}");
    ///         Console.WriteLine($"Cache Processor: RunningTime={cacheProcessor.RunningTime}");
    ///         Console.WriteLine($"Stopping Cache Processor");
    ///         cacheProcessor.Stop();
    ///         Console.WriteLine($"Cache Processor: IsRunning={cacheProcessor.IsRunning}");
    ///         Console.WriteLine($"Cache Processor: StartedTime={cacheProcessor.StartedTimeUtc.ToLocalTime()}");
    ///         Console.WriteLine($"Cache Processor: RunningTime={cacheProcessor.RunningTime}");
    ///         Console.WriteLine();
    ///         Console.WriteLine($"Total Received Requests={cacheProcessor.TotalReceivedRequests}");
    ///         Console.WriteLine($"Total Processed Requests={cacheProcessor.TotalProcessedRequests}");
    ///
    ///         // ######## wait for key press to terminate program
    ///         Console.WriteLine();
    ///         Console.WriteLine($"Press anykey to terminate program. However, there still may be some threads running.{Environment.NewLine}");
    ///         Console.ReadKey(true);
    ///     }
    /// }
    ///
    /// // ######## My Custom Cache Item: implements the CacheProcessorItemBase
    ///
    /// class MyItem :
    ///     dodSON.Core.Cache.CacheProcessorItemBase
    /// {
    ///     private MyItem() : base() { }
    ///
    ///     public MyItem(string key,
    ///                   dodSON.Core.Cache.ICacheValidater&lt;dodSON.Core.Cache.ICacheProcessorItem&gt; validater,
    ///                   Action&lt;dodSON.Core.Cache.ICacheProcessorItem&gt; process,
    ///                   List&lt;string&gt; list) : base(key, validater, process)
    ///     {
    ///         List = list ?? throw new ArgumentNullException(nameof(list));
    ///     }
    ///
    ///     // a list of strings
    ///     public List&lt;string&gt; List
    ///     {
    ///         get;
    ///     }
    /// }
    ///
    /// // This code produces output similar to the following:
    /// //
    /// // #### Main: CurrentDomain=1, ManagedThreadId=1
    /// // 
    /// // [- Add: Key=Key1, Created=2018-08-17 12:51:59 AM +00:00, Cache Count=1 -]
    /// // 
    /// // [- Add: Key=Key2, Created=2018-08-17 12:51:59 AM +00:00, Cache Count=2 -]
    /// // 
    /// // [- Remove: Key=Key2, Created=2018-08-17 12:51:59 AM +00:00, Cache Count=1 -]
    /// // 
    /// // #### Key2--&gt;Step #1:  CurrentDomain=1, ManagedThreadId=4, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=True
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         ---------------- Step #1 ----------------
    /// // 
    /// // [- Add: Key=Key2, Created=2018-08-17 12:52:09 AM +00:00, Cache Count=2 -]
    /// // 
    /// // Key2--&gt;Step #1 Waiting...
    /// // 
    /// // [- Remove: Key=Key1, Created=2018-08-17 12:51:59 AM +00:00, Cache Count=1 -]
    /// // 
    /// // #### Key1--&gt;Step #1:  CurrentDomain=1, ManagedThreadId=5, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=True
    /// // Key2--&gt;Step #1 Waiting Completed. 00:00:19.9997035
    /// // 
    /// // [- Remove: Key=Key2, Created=2018-08-17 12:52:09 AM +00:00, Cache Count=0 -]
    /// // 
    /// // #### Key2--&gt;Step #2:  CurrentDomain=1, ManagedThreadId=5, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=True
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         ---------------- Step #1 ----------------
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         ---------------- Step #2 ----------------
    /// // 
    /// // [- Add: Key=Key2, Created=2018-08-17 12:52:29 AM +00:00, Cache Count=1 -]
    /// // 
    /// // Key2--&gt;Step #2 Waiting...
    /// // 
    /// // [- Remove: Key=Key2, Created=2018-08-17 12:52:29 AM +00:00, Cache Count=0 -]
    /// // 
    /// // #### Key2--&gt;Step #3:  CurrentDomain=1, ManagedThreadId=6, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=True
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:51:59 PM -05:00, HasProcessExecuted=False
    /// //         ---------------- Step #1 ----------------
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:09 PM -05:00, HasProcessExecuted=False
    /// //         ---------------- Step #2 ----------------
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         Key=Key2, CreatedTime=2018-08-16 7:52:29 PM -05:00, HasProcessExecuted=False
    /// //         ---------------- Step #3 ----------------
    /// // 
    /// // Key2--&gt;Step #3 Waiting...
    /// // Cache Processor: IsRunning=True
    /// // Cache Processor: StartedTime=2018-08-16 7:51:59 PM -05:00
    /// // Cache Processor: RunningTime=00:00:51.0333381
    /// // Stopping Cache Processor
    /// // Cache Processor: IsRunning=False
    /// // Cache Processor: StartedTime=0001-01-01 12:00:00 AM -06:00
    /// // Cache Processor: RunningTime=00:00:00
    /// // 
    /// // Total Received Requests=4
    /// // Total Processed Requests=4
    /// // 
    /// // Press anykey to terminate program. However, there still may be some threads running.
    /// // 
    /// // Key2--&gt;Step #2 Waiting Completed. 00:00:25.0006092
    /// // Key2--&gt;Step #3 Waiting Completed. 00:00:05.0000163
    /// </code>
    /// </example>

    // TODO: should implement ICacheProcessor : ICollection<ICacheProcessorItem>

    public class CacheProcessor
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
                                                   ICacheItem<ICacheProcessorItem> newOrChangedItem,
                                                   ICacheItem<ICacheProcessorItem> oldItem)
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
        /// <summary>
        /// Will raise the collection changed event.
        /// </summary>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> to send to the <see cref="CollectionChanged"/> event.</param>
        protected void RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);
        #endregion
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="CacheProcessor"/>.
        /// </summary>
        public CacheProcessor()
        {
            _Cache.CollectionChanged += Cache_CollectionChanged;
        }
        #endregion
        #region Private Fields
        private object _CacheSyncObject = new object();
        private ICache<ICacheProcessorItem> _Cache = new Cache<ICacheProcessorItem>();
        // TODO: should modify this to NOT use threading, but just purge the cache (before/after) each use.
        private Threading.ThreadWorker _CacheWorker = null;
        private volatile bool _ExecuteRemainingRequestsOnStop = false;
        #endregion
        #region Public Methods
        /// <summary>
        /// The date and time this <see cref="CacheProcessor"/> was started in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTimeOffset StartedTimeUtc
        {
            get; private set;
        }
        /// <summary>
        /// Gets the amount of time this <see cref="CacheProcessor"/> has been running.
        /// </summary>
        public TimeSpan RunningTime
        {
            get
            {
                if (StartedTimeUtc == DateTimeOffset.MinValue)
                {
                    return TimeSpan.Zero;
                }
                return DateTimeOffset.UtcNow - StartedTimeUtc;
            }
        }
        /// <summary>
        /// Returns whether this <see cref="CacheProcessor"/> is running. <b>True</b> indicates a running state; otherwise, <b>false</b> indicates the <see cref="CacheProcessor"/> is not running.
        /// </summary>
        public bool IsRunning
        {
            get => _CacheWorker?.IsAlive ?? false;
        }
        /// <summary>
        /// The total number of requests received and put into the cache.
        /// </summary>
        public int TotalReceivedRequests
        {
            get;
            private set;
        }
        /// <summary>
        /// Starts the <see cref="CacheProcessor"/> by starting a <see cref="Threading.ThreadWorker"/> to monitor and periodically purge the cache.
        /// </summary>
        /// <param name="cachePurgeRequestsExecutionInterval">The amount of time to wait before checking the cache for any items that can be purged.</param>
        public void Start(TimeSpan cachePurgeRequestsExecutionInterval)
        {
            if (cachePurgeRequestsExecutionInterval == null)
            {
                throw new ArgumentNullException(nameof(cachePurgeRequestsExecutionInterval));
            }
            if (_CacheWorker == null)
            {
                _CacheWorker = new Threading.ThreadWorker(cachePurgeRequestsExecutionInterval,
                                                          (e) =>
                                                          {
                                                              // #### EXECUTE FUNCTION

                                                              // purge any requests
                                                              IEnumerable<ICacheItem<ICacheProcessorItem>> requests;
                                                              lock (_CacheSyncObject)
                                                              {
                                                                  requests = _Cache.Purge();
                                                              }
                                                              // process any purged requests
                                                              ProcessRequests(requests);
                                                          },
                                                          () =>
                                                          {
                                                              // #### ONSTART FUNCTION 
                                                          },
                                                          () =>
                                                          {
                                                              // #### ONSTOP FUNCTION

                                                              // process any remaining requests
                                                              if (_ExecuteRemainingRequestsOnStop)
                                                              {
                                                                  lock (_CacheSyncObject)
                                                                  {
                                                                      ProcessRequests(_Cache.Items);
                                                                      _Cache.Clear();
                                                                  }
                                                              }
                                                          });
                _CacheWorker.Start();
                StartedTimeUtc = DateTimeOffset.UtcNow;
            }
        }
        /// <summary>
        /// If the <see cref="CacheProcessor"/> is running, this method will stop the cache monitoring <see cref="dodSON.Core.Threading.ThreadWorker"/> and clear the cache.
        /// </summary>
        /// <param name="executeRemainingRequestsOnStop">If <b>true</b> all remaining cached requests will have their <see cref="ICacheProcessorItem.Process"/> executed; otherwise, setting this to <b>false</b> will terminate all remaining cached requests without executing their <see cref="ICacheProcessorItem.Process"/>.</param>
        public void Stop(bool executeRemainingRequestsOnStop)
        {
            _ExecuteRemainingRequestsOnStop = executeRemainingRequestsOnStop;
            if (_CacheWorker != null)
            {
                if (_CacheWorker.IsAlive)
                {
                    _CacheWorker.Stop();
                }
                _CacheWorker = null;
                StartedTimeUtc = DateTimeOffset.MinValue;
            }
        }
        /// <summary>
        /// Returns the number of items currently in the cache.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_CacheSyncObject)
                {
                    return _Cache.Count;
                }
            }
        }
        /// <summary>
        /// Adds an <see cref="ICacheProcessorItem"/> to the cache with the given <see cref="ICacheValidater{T}"/>.
        /// </summary>
        /// <param name="item">The <see cref="ICacheProcessorItem"/> to cache.</param>
        public void Add(ICacheProcessorItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            lock (_CacheSyncObject)
            {
                if (_Cache.ContainsKey(item.Key))
                {
                    throw new Exception($"Duplicate request Key. request.Key={item.Key}");
                }
                _Cache.AddItem(item.Key, new CacheItem<ICacheProcessorItem>(item, item.Validater));
                ++TotalReceivedRequests;
            }
        }
        /// <summary>
        /// Removes the specified <see cref="ICacheProcessorItem"/> from the cache.
        /// </summary>
        /// <param name="item">The <see cref="ICacheProcessorItem"/> to remove from the cache.</param>
        /// <returns>The <see cref="ICacheProcessorItem"/> removed; or null, if the <paramref name="item"/> does not exists.</returns>
        public ICacheProcessorItem Remove(ICacheProcessorItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            lock (_CacheSyncObject)
            {
                if (_Cache.ContainsKey(item.Key))
                {
                    return _Cache.RemoveItem(item.Key).Payload;
                }
                return null;
            }
        }
        /// <summary>
        /// Determines if the specified cache entry exists.
        /// </summary>
        /// <param name="key">The string value used to reference the item.</param>
        /// <returns><b>True</b> if the cache contains the key; otherwise <b>false</b>.</returns>
        public bool Contains(string key)
        {
            lock (_CacheSyncObject)
            {
                return _Cache.ContainsKey(key);
            }
        }
        /// <summary>
        /// Searches the cache for an <see cref="ICacheValidater{T}"/> with the matching <see cref="ICacheProcessorItem.Key"/>.
        /// </summary>
        /// <param name="key">The string to match with the <see cref="ICacheProcessorItem.Key"/>.</param>
        /// <returns>If found, the <see cref="ICacheProcessorItem"/> with the matching <see cref="ICacheProcessorItem.Key"/>. If not found, <b>null</b>.</returns>
        public ICacheProcessorItem Find(string key)
        {
            lock (_CacheSyncObject)
            {
                if (_Cache.ContainsKey(key))
                {
                    return _Cache[key].Payload;
                }
                return null;
            }
        }
        /// <summary>
        /// Searches the cache for an <see cref="ICacheValidater{T}"/> with the matching <see cref="ICacheProcessorItem.Key"/>.
        /// </summary>
        /// <typeparam name="T">The type to cast the matching <see cref="ICacheProcessorItem.Key"/> into.</typeparam>
        /// <param name="key">The string to match with the <see cref="ICacheProcessorItem.Key"/>.</param>
        /// <returns>If found, the <see cref="ICacheProcessorItem"/> with the matching <see cref="ICacheProcessorItem.Key"/> cast into the type <typeparamref name="T"/>. If not found, <b>null</b>.</returns>
        public T Find<T>(string key) where T : class, ICacheProcessorItem => (Find(key) as T);
        /// <summary>
        /// Searches the cache for an <see cref="ICacheValidater{T}"/> with the matching <see cref="ICacheProcessorItem.Key"/>.
        /// If found will return <b>true</b> with the corresponding <see cref="ICacheProcessorItem"/> in the <paramref name="item"/>; otherwise, it will return <b>false</b> with <paramref name="item"/> set to <b>null</b>.
        /// </summary>
        /// <param name="key">The string to match with the <see cref="ICacheProcessorItem.Key"/>.</param>
        /// <param name="item">The matching <see cref="ICacheProcessorItem.Key"/> if found, <b>null</b> if not found.</param>
        /// <returns><b>True</b> if the <see cref="ICacheValidater{T}"/> with the matching <see cref="ICacheProcessorItem.Key"/> is found; otherwise, <b>false</b>.</returns>
        public bool TryFind(string key, out ICacheProcessorItem item)
        {
            item = Find(key);
            return (item != null);
        }
        /// <summary>
        /// Searches the cache for an <see cref="ICacheValidater{T}"/> with the matching <see cref="ICacheProcessorItem.Key"/>.
        /// </summary>
        /// <typeparam name="T">The type to cast the matching <see cref="ICacheProcessorItem.Key"/> into.</typeparam>
        /// <param name="key">The string to match with the <see cref="ICacheProcessorItem.Key"/>.</param>
        /// <param name="item">The matching <see cref="ICacheProcessorItem.Key"/> if found, <b>null</b> if not found.</param>
        /// <returns><b>True</b> if the <see cref="ICacheValidater{T}"/> with the matching <see cref="ICacheProcessorItem.Key"/> is found; otherwise, <b>false</b>.</returns>
        public bool TryFind<T>(string key, out T item)
            where T : class, ICacheProcessorItem
        {
            item = Find<T>(key);
            return (item != null);
        }
        /// <summary>
        /// Will flush the cache.
        /// </summary>
        public void Flush() => _CacheWorker?.ExecuteNow(true);
        /// <summary>
        /// Returns all of the <see cref="ICache{ICacheProcessorItem}"/>s in the cache.
        /// </summary>
        public IEnumerable<ICacheItem<ICacheProcessorItem>> Items
        {
            get
            {
                lock (_CacheSyncObject)
                {
                    foreach (var item in _Cache)
                    {
                        yield return item.Value;
                    }
                }
            }
        }
        #endregion
        #region Private Methods
        private void Cache_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaiseCollectionChangedEvent(e);
        }

        private void ProcessRequests(IEnumerable<ICacheItem<ICacheProcessorItem>> requests)
        {
            foreach (var request in requests)
            {
                Task.Run(() =>
                {
                    (request.Payload as ICacheProcessorItemAdvanced).HasProcessExecuted = true;
                    request.Payload.Process(request.Payload);
                });
            }
        }
        #endregion
    }
}

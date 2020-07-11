using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Threading
{
    /// <summary>
    /// Simplifies the creation of <see cref="ThreadBase"/>-derived types.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <example>
    /// <para>
    /// The following example will create a simple logging class which will create a <see cref="ThreadWorker"/> to monitor a list of cached log entries; 
    /// if the oldest log expires or the number of log entries exceeds maximum then the <see cref="ThreadWorker"/> will write all the log entries to a file.
    /// </para>
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// class Program
    /// {
    /// 	static void Main(string[] args)
    ///     {
    ///         var rnd = new Random();
    /// 
    ///         // #### Be sure to change these to proper values
    ///         var filename = @"C:\(WORKING)\Dev\ThreadingTests\SimpleLog.txt";
    ///         var flushTimeLimit = TimeSpan.FromSeconds(20);
    ///         var entryMaxLimit = 50;
    /// 
    ///         // start the logger
    ///         using (var logger = new SimpleLogger(filename, flushTimeLimit, entryMaxLimit))
    ///         {
    ///             Console.WriteLine("Logging Started.");
    ///             Console.WriteLine("press anykey to stop...");
    ///             Console.WriteLine();
    ///             // loop until a key is pressed
    ///             while (!Console.KeyAvailable)
    ///             {
    ///                 // wait for a random time
    ///                 var waitFor = TimeSpan.FromMilliseconds(rnd.Next(200, 700));
    ///                 dodSON.Core.Threading.ThreadingHelper.Sleep(waitFor);
    ///                 // generate log
    ///                 var entry = new SimpleLogEntry(Guid.NewGuid().ToString());
    ///                 logger.Add(entry);
    ///             }
    ///             Console.ReadKey(true);
    ///         }
    ///         //
    ///         Console.Write($"{Environment.NewLine}press anykey&gt;");
    ///         Console.ReadKey(true);
    ///     }
    /// }
    /// 
    /// public class SimpleLogEntry
    /// {
    ///     public SimpleLogEntry(string message)
    ///     {
    ///         EntryDate = DateTimeOffset.Now;
    ///         Message = message;
    ///     }
    /// 
    ///     public DateTimeOffset EntryDate { get; private set; }
    ///     public string Message { get; private set; }
    /// 
    ///     public override string ToString() { return $"{EntryDate} {Message}"; }
    /// }
    /// 
    /// public class SimpleLogger
    ///     : IDisposable
    /// {
    ///     // ######## PRIVATE FIELDS
    ///     private object _SyncObject = new object();
    ///     private string _Filename;
    ///     private TimeSpan _FlushTimeLimit;
    ///     private int _EntryMaxLimit;
    ///     private List&lt;SimpleLogEntry&gt; _LogEntries;
    ///     private dodSON.Core.Threading.ThreadWorker _WorkerThread;
    /// 
    ///     // ######## CTOR
    ///     public SimpleLogger(string filename, TimeSpan flushTimeLimit, int entryMaxLimit)
    ///     {
    ///         if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException(nameof(filename)); }
    ///         if (flushTimeLimit &lt; TimeSpan.FromSeconds(10)) { throw new ArgumentOutOfRangeException(nameof(flushTimeLimit), $"Parameter {nameof(flushTimeLimit)} must be greater than or equal to 10 seconds. ({nameof(flushTimeLimit)} &gt;= 10 seconds)"); }
    ///         if (entryMaxLimit &lt; 10) { throw new ArgumentOutOfRangeException(nameof(entryMaxLimit), $"Parameter {nameof(entryMaxLimit)} must be greater than or equal to 10. ({nameof(entryMaxLimit)} &gt;= 10)"); }
    ///         _Filename = filename;
    ///         _FlushTimeLimit = flushTimeLimit;
    ///         _EntryMaxLimit = entryMaxLimit;
    ///         _LogEntries = new List&lt;SimpleLogEntry&gt;();
    ///         _WorkerThread = new dodSON.Core.Threading.ThreadWorker(
    ///                             TimeSpan.FromSeconds(1),
    ///                             (w) =&gt;
    ///                             {
    ///                                 // #### QUERY FUNCTION
    ///                                 lock (_SyncObject)
    ///                                 {
    ///                                     // check if logs should be persisted
    ///                                     if (_LogEntries.Count &gt; 0)
    ///                                     {
    ///                                         if (_LogEntries.Count &gt;= _EntryMaxLimit)
    ///                                         {
    ///                                             Console.WriteLine("        ######## Maxed Out.");
    ///                                             return true;
    ///                                         }
    ///                                         if ((DateTimeOffset.Now - _LogEntries[0].EntryDate) &gt; _FlushTimeLimit)
    ///                                         {
    ///                                             Console.WriteLine("        ######## Timed Out.");
    ///                                             return true;
    ///                                         }
    ///                                     }
    ///                                     return false;
    ///                                 }
    ///                             },
    ///                             (e) =&gt;
    ///                             {
    ///                                 // #### EXECUTE FUNCTION
    ///                                 // persist logs
    ///                                 Flush();
    ///                             },
    ///                             () =&gt;
    ///                             {
    ///                                 // #### ONSTART FUNCTION
    ///                                 Console.WriteLine($"{Environment.NewLine}        ######## On Start");
    ///                                 // update the log file with a notification stating the start of a new log session
    ///                                 var line = $"{Environment.NewLine}################################{Environment.NewLine}";
    ///                                 System.IO.File.AppendAllText(filename, $"{line}Log Started: {DateTimeOffset.Now}{line}");
    ///                             },
    ///                             () =&gt;
    ///                             {
    ///                                 // #### ONSTOP FUNCTION
    ///                                 Console.WriteLine($"{Environment.NewLine}        ######## On Stop");
    ///                                 // persist all remaining logs
    ///                                 Flush();
    ///                             });
    ///         _WorkerThread.Start();
    ///     }
    /// 
    ///     // ######## PUBLIC METHODS
    ///     public void Add(SimpleLogEntry entry)
    ///     {
    ///         if (entry == null) { throw new ArgumentNullException(nameof(entry)); }
    ///         // add entry to the log list
    ///         lock (_SyncObject) { _LogEntries.Add(entry); }
    ///         Console.WriteLine($"Cached Entries: {_LogEntries.Count:N0}   Entry Added: {entry}");
    ///     }
    /// 
    ///     public void Flush()
    ///     {
    ///         lock (_SyncObject)
    ///         {
    ///             if (_LogEntries.Count &gt; 0)
    ///             {
    ///                 Console.WriteLine($"        ######## {_LogEntries.Count:N0} LOGS FLUSHED ########");
    ///                 // write all log entries to the log file
    ///                 using (var sw = new System.IO.StreamWriter(_Filename, true))
    ///                 {
    ///                     foreach (var entry in _LogEntries) { sw.WriteLine(entry.ToString()); }
    ///                 }
    ///                 // clear the log list
    ///                 _LogEntries.Clear();
    ///             }
    ///         }
    ///     }
    /// 
    ///     // ######## IDISPOSE INTERFACE METHODS
    ///     public void Dispose()
    ///     {
    ///         Dispose(true);
    ///         GC.SuppressFinalize(this);
    ///     }
    /// 
    ///     protected virtual void Dispose(bool disposing)
    ///     {
    ///         if (disposing)
    ///         {
    ///             // stop the thread worker
    ///             if (_WorkerThread.IsAlive) { _WorkerThread.Stop(); }
    ///             Console.WriteLine($"{Environment.NewLine}WorkerThread.IsAlive= {_WorkerThread.IsAlive}");
    ///         }
    ///     }
    /// }
    /// 
    /// // This code produces output similar to the following:
    ///
    /// //         ######## On Start
    /// // Logging Started.
    /// // press anykey to stop...
    /// // 
    /// // Cached Entries: 1   Entry Added: 2017-03-02 10:15:06 PM -06:00 71123ef9-67f7-4bf2-a46e-1ee07c5d475d
    /// // Cached Entries: 2   Entry Added: 2017-03-02 10:15:06 PM -06:00 520f1146-c4a3-4daa-bc13-622bbf54c93c
    /// // Cached Entries: 3   Entry Added: 2017-03-02 10:15:07 PM -06:00 53c30ab8-b501-4d78-bade-b8f749c1bac7
    /// // Cached Entries: 4   Entry Added: 2017-03-02 10:15:07 PM -06:00 63c12577-e6b8-4939-8b34-15769718d9f8
    /// // Cached Entries: 5   Entry Added: 2017-03-02 10:15:07 PM -06:00 865a0d6f-39d9-419f-bc02-dd331a89b9e7
    /// // Cached Entries: 6   Entry Added: 2017-03-02 10:15:08 PM -06:00 2be26f31-ba43-4232-a88f-603934eed9a4
    /// // Cached Entries: 7   Entry Added: 2017-03-02 10:15:08 PM -06:00 d6ea9bac-56c6-4763-85af-f141e8d89ba7
    /// // Cached Entries: 8   Entry Added: 2017-03-02 10:15:09 PM -06:00 368aa1a2-b9ba-420a-b7e5-4421ba8ffdc2
    /// // Cached Entries: 9   Entry Added: 2017-03-02 10:15:09 PM -06:00 c9857309-de62-4206-a096-ce8364b67f29
    /// // Cached Entries: 10   Entry Added: 2017-03-02 10:15:10 PM -06:00 bfb9e7a7-c90a-4f30-9793-6448f49783e0
    /// // Cached Entries: 11   Entry Added: 2017-03-02 10:15:10 PM -06:00 ce282d13-27d7-4d15-80f1-0d2827b35701
    /// // Cached Entries: 12   Entry Added: 2017-03-02 10:15:11 PM -06:00 50de2bb4-f551-46ec-b4d3-e6b97ae22623
    /// // Cached Entries: 13   Entry Added: 2017-03-02 10:15:11 PM -06:00 4450f2c6-55d8-4bdc-a262-6d60706fc089
    /// // Cached Entries: 14   Entry Added: 2017-03-02 10:15:12 PM -06:00 6bd4b130-16dc-4313-851c-9aa90a179034
    /// // Cached Entries: 15   Entry Added: 2017-03-02 10:15:12 PM -06:00 cda13821-8120-47a6-a0ee-f6e22b4c5c04
    /// // Cached Entries: 16   Entry Added: 2017-03-02 10:15:13 PM -06:00 7a328391-e3e2-4749-82b3-2cd40f77e2c1
    /// // Cached Entries: 17   Entry Added: 2017-03-02 10:15:13 PM -06:00 e432b8bd-436a-49b0-ac0f-b9d366cbacf4
    /// // Cached Entries: 18   Entry Added: 2017-03-02 10:15:14 PM -06:00 da0534a7-14e4-4685-8cb7-49ee6078d965
    /// // Cached Entries: 19   Entry Added: 2017-03-02 10:15:14 PM -06:00 b0e2246d-af22-46d4-820f-512997d32c37
    /// // Cached Entries: 20   Entry Added: 2017-03-02 10:15:15 PM -06:00 dd3c72dc-81a5-4d1a-9969-d87ca973ae22
    /// // Cached Entries: 21   Entry Added: 2017-03-02 10:15:15 PM -06:00 c2d55b6e-34e9-4e24-a998-a7b6021efb8e
    /// // Cached Entries: 22   Entry Added: 2017-03-02 10:15:15 PM -06:00 ee5e344e-7647-4516-8d97-08522da04abb
    /// // Cached Entries: 23   Entry Added: 2017-03-02 10:15:16 PM -06:00 c1c307b4-fddb-4975-a6e8-b22e986ab33a
    /// // Cached Entries: 24   Entry Added: 2017-03-02 10:15:16 PM -06:00 014cb5f7-863a-4df0-874b-b41edad201ad
    /// // Cached Entries: 25   Entry Added: 2017-03-02 10:15:17 PM -06:00 37666833-28d5-4d72-9fef-c7fd5d65c2ab
    /// // Cached Entries: 26   Entry Added: 2017-03-02 10:15:17 PM -06:00 40d7d4f4-69be-4846-84f5-14efc7865454
    /// // Cached Entries: 27   Entry Added: 2017-03-02 10:15:18 PM -06:00 98615e65-b9f0-49c5-886d-3fac95f3e86b
    /// // Cached Entries: 28   Entry Added: 2017-03-02 10:15:18 PM -06:00 148aef42-310e-4693-b50d-9aaa118cfe24
    /// // Cached Entries: 29   Entry Added: 2017-03-02 10:15:18 PM -06:00 30221098-06d1-4234-852c-aee3ef382a3a
    /// // Cached Entries: 30   Entry Added: 2017-03-02 10:15:19 PM -06:00 8e30bd93-188e-46bb-a698-94970644a5e0
    /// // Cached Entries: 31   Entry Added: 2017-03-02 10:15:19 PM -06:00 89475674-9bd7-4a2a-bf0e-b8b342251134
    /// // Cached Entries: 32   Entry Added: 2017-03-02 10:15:20 PM -06:00 e5e07c7d-e412-444f-81ec-af557d679ebd
    /// // Cached Entries: 33   Entry Added: 2017-03-02 10:15:20 PM -06:00 c888eb2c-934a-4b08-ba56-947952d19928
    /// // Cached Entries: 34   Entry Added: 2017-03-02 10:15:21 PM -06:00 e4b5d476-b195-4bcb-b417-3f2211ac5f11
    /// // Cached Entries: 35   Entry Added: 2017-03-02 10:15:21 PM -06:00 09abffc0-d1f0-4c11-90cd-169c862b6485
    /// // Cached Entries: 36   Entry Added: 2017-03-02 10:15:22 PM -06:00 9d8d576a-1bda-45da-959d-ad97fdc4e3c9
    /// // Cached Entries: 37   Entry Added: 2017-03-02 10:15:22 PM -06:00 f0a309aa-bada-4af1-bf2f-fc9596e83cf0
    /// // Cached Entries: 38   Entry Added: 2017-03-02 10:15:23 PM -06:00 62ccfdc6-eb16-482a-8365-29a03ed556f0
    /// // Cached Entries: 39   Entry Added: 2017-03-02 10:15:23 PM -06:00 f6bd5490-6540-4a85-9ed1-c2328790c11e
    /// // Cached Entries: 40   Entry Added: 2017-03-02 10:15:24 PM -06:00 cab0cb03-9356-4ddf-9c8b-bca97c56069e
    /// // Cached Entries: 41   Entry Added: 2017-03-02 10:15:24 PM -06:00 00346fa9-a9e8-4832-b801-bb5cecba716f
    /// // Cached Entries: 42   Entry Added: 2017-03-02 10:15:25 PM -06:00 660f42b5-de29-4393-9c2c-9b952a54124a
    /// // Cached Entries: 43   Entry Added: 2017-03-02 10:15:25 PM -06:00 66e05c5c-a9c7-44f6-8879-30cd5ede93c0
    /// // Cached Entries: 44   Entry Added: 2017-03-02 10:15:25 PM -06:00 85137e91-9136-423d-a7d7-da5fbd5cef7a
    /// // Cached Entries: 45   Entry Added: 2017-03-02 10:15:26 PM -06:00 1d614fdf-82b0-4eb5-928d-096b24938a6a
    /// // Cached Entries: 46   Entry Added: 2017-03-02 10:15:26 PM -06:00 394431a4-48db-4edb-89ee-0512c7b7cd94
    /// //         ######## Timed Out.
    /// //         ######## 46 LOGS FLUSHED ########
    /// // Cached Entries: 1   Entry Added: 2017-03-02 10:15:26 PM -06:00 af143356-9fc5-4819-bd69-428024bedf2b
    /// // Cached Entries: 2   Entry Added: 2017-03-02 10:15:27 PM -06:00 4add5ebf-3d9f-4e27-a948-32870220aa74
    /// // Cached Entries: 3   Entry Added: 2017-03-02 10:15:27 PM -06:00 29e50744-c582-4c79-8466-12e1d0b23e6b
    /// // Cached Entries: 4   Entry Added: 2017-03-02 10:15:27 PM -06:00 0e9eef1f-1b80-4e26-b0ed-66eb1f9709bf
    /// // Cached Entries: 5   Entry Added: 2017-03-02 10:15:28 PM -06:00 26730b16-b893-4f4d-a7fa-81cd7a078cc1
    /// // Cached Entries: 6   Entry Added: 2017-03-02 10:15:28 PM -06:00 a42a9ced-45ab-4ac8-88f4-231b536a48e5
    /// // Cached Entries: 7   Entry Added: 2017-03-02 10:15:29 PM -06:00 39d008f3-b7bc-4bb6-a2e5-0c0f208db364
    /// // Cached Entries: 8   Entry Added: 2017-03-02 10:15:29 PM -06:00 8650578c-81b8-4cde-ae90-d5ef1705652a
    /// // Cached Entries: 9   Entry Added: 2017-03-02 10:15:30 PM -06:00 dd534c53-d94b-41b8-8854-bff1cd5c9851
    /// // Cached Entries: 10   Entry Added: 2017-03-02 10:15:30 PM -06:00 dea8a565-5db5-4df9-9adf-0050d16e77ac
    /// // Cached Entries: 11   Entry Added: 2017-03-02 10:15:30 PM -06:00 eef6a738-b7b6-49a4-821d-9f04c978cba8
    /// // Cached Entries: 12   Entry Added: 2017-03-02 10:15:31 PM -06:00 fece1847-fff1-464f-a031-dbe340b45201
    /// // Cached Entries: 13   Entry Added: 2017-03-02 10:15:31 PM -06:00 225acb93-f77c-4e0f-9e9b-60ac1da3078c
    /// // Cached Entries: 14   Entry Added: 2017-03-02 10:15:32 PM -06:00 46939878-22ba-4cdc-b6a0-ff0c53cd8370
    /// // Cached Entries: 15   Entry Added: 2017-03-02 10:15:32 PM -06:00 c0d48d16-f130-46ea-9aaf-41e93debb4e7
    /// // Cached Entries: 16   Entry Added: 2017-03-02 10:15:32 PM -06:00 6f6e38b2-bc9c-427f-8bda-da7bc9e4a92a
    /// // Cached Entries: 17   Entry Added: 2017-03-02 10:15:33 PM -06:00 af230ee2-46dd-458c-b902-568549abd379
    /// // Cached Entries: 18   Entry Added: 2017-03-02 10:15:33 PM -06:00 6cd478c5-b0a5-4040-b1a4-cc0e29da482e
    /// // Cached Entries: 19   Entry Added: 2017-03-02 10:15:34 PM -06:00 a46661f3-04c5-48d9-b129-57a58a1ea8e6
    /// // Cached Entries: 20   Entry Added: 2017-03-02 10:15:34 PM -06:00 0656dcf5-fcfe-41f9-a482-214e9e4ce9a2
    /// // Cached Entries: 21   Entry Added: 2017-03-02 10:15:34 PM -06:00 2215896c-a433-4eea-9896-f74707e968af
    /// // Cached Entries: 22   Entry Added: 2017-03-02 10:15:35 PM -06:00 de583083-f0b5-4c71-a8ef-5b26130933c6
    /// // Cached Entries: 23   Entry Added: 2017-03-02 10:15:35 PM -06:00 2a3109fd-f946-4637-8073-94c380d724f4
    /// // Cached Entries: 24   Entry Added: 2017-03-02 10:15:35 PM -06:00 a5fffae9-2926-4d11-9634-b953f0ec9e62
    /// // Cached Entries: 25   Entry Added: 2017-03-02 10:15:36 PM -06:00 7d635803-8c3f-4c6a-9652-8d4878b4064a
    /// // Cached Entries: 26   Entry Added: 2017-03-02 10:15:36 PM -06:00 b007c10f-ca50-4c07-bf4c-78cbea190541
    /// // Cached Entries: 27   Entry Added: 2017-03-02 10:15:37 PM -06:00 b4ff8ae0-fe88-42d5-a48a-1ae2d3bb8bc0
    /// // Cached Entries: 28   Entry Added: 2017-03-02 10:15:37 PM -06:00 14ab81d2-a103-446f-a1b2-22b9582fa978
    /// // Cached Entries: 29   Entry Added: 2017-03-02 10:15:38 PM -06:00 97ec393b-b6b5-4256-95e6-d446a88b40c6
    /// // Cached Entries: 30   Entry Added: 2017-03-02 10:15:38 PM -06:00 0349f41c-0f76-4f27-b047-654f1f8c0ae6
    /// // Cached Entries: 31   Entry Added: 2017-03-02 10:15:38 PM -06:00 57847192-0fd8-46a6-99c1-a2d0cd8ff310
    /// // Cached Entries: 32   Entry Added: 2017-03-02 10:15:39 PM -06:00 8759a4be-0511-4721-83f8-def4984e5193
    /// // Cached Entries: 33   Entry Added: 2017-03-02 10:15:39 PM -06:00 c81a11e7-6d7a-4b93-a72c-e00785e91dbf
    /// // Cached Entries: 34   Entry Added: 2017-03-02 10:15:39 PM -06:00 1cf45c57-64a7-4b93-90c5-0157fdd581e3
    /// // Cached Entries: 35   Entry Added: 2017-03-02 10:15:40 PM -06:00 4e5f43a4-6413-4a2e-add2-5811b948252f
    /// // Cached Entries: 36   Entry Added: 2017-03-02 10:15:40 PM -06:00 2484bc05-9bec-43b0-a194-ff42e4948511
    /// // Cached Entries: 37   Entry Added: 2017-03-02 10:15:41 PM -06:00 ecd5251d-a92a-453d-8b5f-dad9a9bed697
    /// // Cached Entries: 38   Entry Added: 2017-03-02 10:15:41 PM -06:00 21b7bf79-cf14-48de-b80c-1e5c78ee1a90
    /// // Cached Entries: 39   Entry Added: 2017-03-02 10:15:41 PM -06:00 4175a32e-4380-4a76-a9bc-ba52582e9390
    /// // Cached Entries: 40   Entry Added: 2017-03-02 10:15:42 PM -06:00 0ebed068-779f-449f-9a31-bcb39edd5970
    /// // Cached Entries: 41   Entry Added: 2017-03-02 10:15:42 PM -06:00 1644505b-c272-414e-8a26-87af5ee5c963
    /// // Cached Entries: 42   Entry Added: 2017-03-02 10:15:43 PM -06:00 b633be41-8529-43dc-853e-d32d7d871627
    /// // Cached Entries: 43   Entry Added: 2017-03-02 10:15:43 PM -06:00 b736a5cb-6ea2-41e5-8c04-1bc168c16508
    /// // Cached Entries: 44   Entry Added: 2017-03-02 10:15:44 PM -06:00 081b1586-071c-4c7e-b77d-101303b7ef4a
    /// // Cached Entries: 45   Entry Added: 2017-03-02 10:15:44 PM -06:00 add4613f-24dd-46f5-98b3-3d9d9a31b0a7
    /// // Cached Entries: 46   Entry Added: 2017-03-02 10:15:45 PM -06:00 ea63cb1b-6a9d-47ba-9866-ea633ba636e2
    /// // Cached Entries: 47   Entry Added: 2017-03-02 10:15:45 PM -06:00 7f62a276-9832-492d-a9cb-2eeb5647b80f
    /// // Cached Entries: 48   Entry Added: 2017-03-02 10:15:45 PM -06:00 4d012336-c9a5-48fe-9bfc-90d3bcd75754
    /// // Cached Entries: 49   Entry Added: 2017-03-02 10:15:46 PM -06:00 677fdd63-47c2-47f6-84bb-9651c10ed5d8
    /// // Cached Entries: 50   Entry Added: 2017-03-02 10:15:46 PM -06:00 ca78b6ca-3d57-4930-8d0f-5e546d1b275c
    /// //         ######## Maxed Out.
    /// //         ######## 50 LOGS FLUSHED ########
    /// // Cached Entries: 1   Entry Added: 2017-03-02 10:15:47 PM -06:00 f78ad761-17f8-404f-b867-52fffdac6f72
    /// // Cached Entries: 2   Entry Added: 2017-03-02 10:15:47 PM -06:00 ec32a5bd-96a5-4d6a-803e-f0e58fbbfdbb
    /// // Cached Entries: 3   Entry Added: 2017-03-02 10:15:48 PM -06:00 b0845701-ac28-4489-beae-e953f18b94bf
    /// // Cached Entries: 4   Entry Added: 2017-03-02 10:15:48 PM -06:00 6bb0ae70-6419-43fd-b183-f16f039bebec
    /// // Cached Entries: 5   Entry Added: 2017-03-02 10:15:49 PM -06:00 52b9594d-ef5d-4b78-907c-9825670a4556
    /// // Cached Entries: 6   Entry Added: 2017-03-02 10:15:49 PM -06:00 beee67c4-1e8e-4a94-8c9d-bf125a115615
    /// // Cached Entries: 7   Entry Added: 2017-03-02 10:15:49 PM -06:00 6cb317e9-4287-4a51-bb4c-c08f4273db35
    /// // Cached Entries: 8   Entry Added: 2017-03-02 10:15:50 PM -06:00 16f2bbb5-edda-49be-a3e3-6e78629dbf6c
    /// // Cached Entries: 9   Entry Added: 2017-03-02 10:15:51 PM -06:00 1ab722bb-afab-4dd2-bef8-43b771eb4f65
    /// // Cached Entries: 10   Entry Added: 2017-03-02 10:15:51 PM -06:00 58daab28-4ffa-4580-bd88-6c5bfc11109a
    /// // Cached Entries: 11   Entry Added: 2017-03-02 10:15:51 PM -06:00 2ce20df4-fa32-4f0a-a7ff-42c21f15cd0b
    /// // Cached Entries: 12   Entry Added: 2017-03-02 10:15:52 PM -06:00 f8f5dab5-8e40-4748-8296-902d5572a449
    /// // Cached Entries: 13   Entry Added: 2017-03-02 10:15:52 PM -06:00 1ce7784a-a461-42bf-bf1b-6afaf6504542
    /// // Cached Entries: 14   Entry Added: 2017-03-02 10:15:53 PM -06:00 2db65075-620e-4d92-b517-90d3e753327a
    /// // Cached Entries: 15   Entry Added: 2017-03-02 10:15:53 PM -06:00 04806eb9-547e-46a6-9929-1614bcce2190
    /// // Cached Entries: 16   Entry Added: 2017-03-02 10:15:54 PM -06:00 bd19ad94-7297-4f3d-bd30-ded25a8b8e8c
    /// // 
    /// //         ######## On Stop
    /// //         ######## 16 LOGS FLUSHED ########
    /// // 
    /// // WorkerThread.IsAlive= False
    /// // 
    /// // press anykey&gt;
    /// </code>
    /// </example>
    public class ThreadWorker
            : ThreadBase
    {
        #region Ctor
        private ThreadWorker() : base() { }

        /// <summary>
        /// Initializes the ThreadWorker class to execute the <paramref name="executeEvent"/> every <paramref name="executionInterval"/>.
        /// </summary>
        /// <param name="executionInterval">The amount of time to wait between running the <see cref="OnExecute"/> method.</param>
        /// <param name="executeEvent">The <see cref="Action{ThreadCancelToken}"/> to execute.</param>
        public ThreadWorker(TimeSpan executionInterval,
                            Action<ThreadCancelToken> executeEvent)
            : this(executionInterval, (w) => { return true; }, executeEvent, () => { }, () => { }) { }
        /// <summary>
        /// Initializes the ThreadWorker class to execute the <paramref name="executeEvent"/> every <paramref name="executionInterval"/>.
        /// </summary>
        /// <param name="executionInterval">The amount of time to wait between running the <see cref="OnExecute"/> method.</param>
        /// <param name="executeEvent">The <see cref="Action{ThreadCancelToken}"/> to execute.</param>
        /// <param name="onStartEvent">The <see cref="Action"/> called just before the thread is started.</param>
        /// <param name="onStopEvent">The <see cref="Action"/> called just after the thread is stopped.</param>
        public ThreadWorker(TimeSpan executionInterval,
                            Action<ThreadCancelToken> executeEvent,
                            Action onStartEvent,
                            Action onStopEvent)
            : this(executionInterval, (w) => { return true; }, executeEvent, onStartEvent, onStopEvent) { }
        /// <summary>
        /// Initializes the ThreadWorker class to execute the <paramref name="executeEvent"/> every <paramref name="executionInterval"/> if the <paramref name="queryFunc"/> returns <b>true</b>.
        /// </summary>
        /// <param name="executionInterval">The amount of time to wait between checking the <see cref="CanExecute"/> property to see if it should run the <see cref="OnExecute"/> method.</param>
        /// <param name="queryFunc">The <see cref="Func{ThreadWorker, Boolean}"/> used to determine if the thread can execute. Return <b>true</b> to execute the thread; otherwise, <b>false</b>.</param>
        /// <param name="executeEvent">The <see cref="Action{ThreadCancelToken}"/> to execute.</param>
        public ThreadWorker(TimeSpan executionInterval,
                            Func<ThreadWorker, bool> queryFunc,
                            Action<ThreadCancelToken> executeEvent)
            : this(executionInterval, queryFunc, executeEvent, () => { }, () => { }) { }
        /// <summary>
        /// Initializes the ThreadWorker class to execute the <paramref name="executeEvent"/> every <paramref name="executionInterval"/> if the <paramref name="queryFunc"/> returns <b>true</b>.
        /// </summary>
        /// <param name="executionInterval">The amount of time to wait between checking the <see cref="CanExecute"/> property to see if it should run the <see cref="OnExecute"/> method.</param>
        /// <param name="queryFunc">The <see cref="Func{ThreadWorker, Boolean}"/> used to determine if the thread can execute. Return <b>true</b> to execute the thread; otherwise, <b>false</b>.</param>
        /// <param name="executeEvent">The <see cref="Action{ThreadCancelToken}"/> to execute.</param>
        /// <param name="onStartEvent">The <see cref="Action"/> called just before the thread is started.</param>
        /// <param name="onStopEvent">The <see cref="Action"/> called just after the thread is stopped.</param>
        public ThreadWorker(TimeSpan executionInterval,
                            Func<ThreadWorker, bool> queryFunc,
                            Action<ThreadCancelToken> executeEvent,
                            Action onStartEvent,
                            Action onStopEvent)
            : this()
        {
            if (executionInterval < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(executionInterval), $"Parameter {nameof(executionInterval)} must be greater then zero. ({nameof(executionInterval)} > 0)");
            }
            _ExecutionInterval = executionInterval;
            _CanExecuteFunc = queryFunc ?? throw new ArgumentNullException(nameof(queryFunc));
            _OnExecuteEvent = executeEvent ?? throw new ArgumentNullException(nameof(executeEvent));
            _OnStartEvent = onStartEvent ?? throw new ArgumentNullException(nameof(onStartEvent));
            _OnStopEvent = onStopEvent ?? throw new ArgumentNullException(nameof(onStopEvent));
        }
        #endregion
        #region Private Fields
        private readonly TimeSpan _ExecutionInterval;
        private readonly Func<ThreadWorker, bool> _CanExecuteFunc;
        private readonly Action<ThreadCancelToken> _OnExecuteEvent;
        private readonly Action _OnStartEvent;
        private readonly Action _OnStopEvent;
        #endregion
        #region dodSON.Core.Threading.ThreadBase Abstract Methods
        /// <summary>
        /// Gets a value indicating the amount of time to wait before checking the <see cref="CanExecute"/> method to determine if the <see cref="OnExecute(ThreadCancelToken)"/> should be executed.
        /// </summary>
        protected override TimeSpan ExecutionInterval => _ExecutionInterval;
        /// <summary>
        /// Gets a value indicating whether the <see cref="OnExecute"/> methods should be executed.
        /// </summary>
        protected override bool CanExecute => _CanExecuteFunc(this);
        /// <summary>
        /// The method called to perform work every <see cref="ExecutionInterval"/> when the <see cref="CanExecute"/> method returns <b>true</b>.
        /// </summary>
        /// <param name="cancelToken">A <see cref="ThreadCancelToken"/> object which should be monitored by the <see cref="OnExecute"/> method. If the <see cref="ThreadCancelToken.CancelThread"/> becomes <b>True</b> the <see cref="OnExecute"/> method should, gracefully, terminate execution as quickly as possible.</param>
        protected override void OnExecute(ThreadCancelToken cancelToken) => _OnExecuteEvent(cancelToken);
        /// <summary>
        /// Called just before the thread is started.
        /// </summary>
        protected override void OnStart() => _OnStartEvent();
        /// <summary>
        /// Called just after the thread is stopped.
        /// </summary>
        protected override void OnStop() => _OnStopEvent();
        #endregion
    }
}

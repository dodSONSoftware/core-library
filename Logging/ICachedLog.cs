using System;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines functionality to add caching to a log.
    /// </summary>
    public interface ICachedLog
            : ILog
    {
        /// <summary>
        /// The <see cref="ILog"/> used to read and write log entries.
        /// </summary>
        ILog InnerLog { get; }
        /// <summary>
        /// The number of cache flushes have been executed.
        /// </summary>
        int FlushGeneration { get; }
        /// <summary>
        /// The date when the cache was last flushed.
        /// </summary>        
        DateTime LastFlushDate { get; }
        /// <summary>
        /// The interval since the cache was last flushed.
        /// </summary>
        TimeSpan LastFlushed { get; }
        /// <summary>
        /// The number of log entries written to the log during the last cache flush.
        /// </summary>
        int LastFlushCount { get; }
        /// <summary>
        /// The largest number of log entries written to the log during a cache flush.
        /// </summary>
        int LargestFlushCount { get; }
        /// <summary>
        /// The number of log entires received.
        /// </summary>
        int IncomingLogCount { get; }
        /// <summary>
        /// Gets the number of log entries in the cache. These are log entries, being held in memory, that have not been written to the actual log, yet.
        /// </summary>
        int CachedLogCount { get; }
        /// <summary>
        /// Gets the number of log entries in the hosted log plus the number of <see cref="CachedLogCount"/>.
        /// </summary>
        int TotalLogCount { get; }
        /// <summary>
        /// Gets, or sets, a value indicating whether auto-cache flushing is on or off.
        /// <b>True</b> should cause the cache to be flushed, if the cache is flushable, before writing a <see cref="ILogEntry"/> to the cache; otherwise, <b>false</b> should not automatically flush the cache.
        /// </summary>
        bool AutoFlushEnabled { get; set; }
        /// <summary>
        /// The maximum number of log entries allowed in the log before the log should be flushed.
        /// </summary>
        int FlushMaximumLogs { get; set; }
        /// <summary>
        /// The maximum amount of time allowed to pass before the log should be flushed.
        /// </summary>
        TimeSpan FlushTimeLimit { get; set; }
        /// <summary>
        /// Returns whether the cache has meet the requirements to flush.
        /// </summary>
        bool IsFlushable { get; }
        /// <summary>
        /// This will write all log entries in the cache to the hosted log, clearing the cached log entries.
        /// </summary>
        void FlushLogs();
    }
}

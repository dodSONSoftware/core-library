using System;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Statistical information about the logging system.
    /// </summary>
    public interface ILogStatistics
    {
        // -------- Log Statistics
        /// <summary>
        /// The date the log was opened.
        /// </summary>
        DateTime DateStarted { get; }
        /// <summary>
        /// How long the log has been opened.
        /// </summary>
        TimeSpan RunTime { get; }
        /// <summary>
        /// The number of log entries in the log.
        /// <seealso cref="CachedLogCount"/>
        /// <seealso cref="ArchivedLogCount"/>
        /// </summary>
        int LogCount { get; set; }
        /// <summary>
        /// The number of log entries written to the log since startup. 
        /// </summary>
        int LogWrites { get; set; }
        // -------- Cached Log Statistics
        /// <summary>
        /// Gets or sets whether the automatic cache flushing mechanism is on or off.
        /// </summary>
        bool AutoFlushEnabled { get; set; }
        /// <summary>
        /// The number of cache flushes have been performed since startup.
        /// </summary>
        int FlushGeneration { get; set; }
        /// <summary>
        /// The date the last cache flush occurred.
        /// </summary>
        DateTime LastFlushDate { get; set; }
        /// <summary>
        /// How long since the cache was flushed.
        /// </summary>
        TimeSpan LastFlushed { get; }
        /// <summary>
        ///  The number of log entries written to the log during the last cache flush.
        /// </summary>
        int LastFlushCount { get; set; }
        /// <summary>
        /// The largest number of log entries written to the log during a log flush.
        /// </summary>
        int LargestFlushCount { get; set; }
        /// <summary>
        /// The maximum amount of time the system will wait before automatically flushing the cache.
        /// </summary>
        TimeSpan AutoFlushTimeLimit { get; set; }
        /// <summary>
        /// The amount of time remaining before the system will automatically flush the cache.
        /// </summary>
        TimeSpan AutoFlushTimeRemaining { get; set; }
        /// <summary>
        /// The ratio of time remaining before the system will automatically flush the cache.
        /// </summary>
        double AutoFlushTimeoutRatio { get; }
        /// <summary>
        /// The maximum number of log entries written to the log before the system automatically flushes the cache.
        /// </summary>
        int AutoFlushMaximumLogs { get; set; }
        /// <summary>
        /// The ratio of log entries written to the log before the system will automatically flush the cache.
        /// </summary>
        double AutoFlushLogsRatio { get; }
        /// <summary>
        /// The number of log entires received.
        /// </summary>
        int IncomingLogCount { get; set; }
        /// <summary>
        /// Returns the average number of log entires per second for the given duration.
        /// </summary>
        /// <param name="lastIncomingLogsReceived">The number of incoming log entries received for the duration specified.</param>
        /// <param name="forDuration">The amount of time to average for.</param>
        /// <returns>The average number of log entires per second for the given duration.</returns>
        double IncomingAverageLogsPerSecond(long lastIncomingLogsReceived, TimeSpan forDuration);
        /// <summary>
        /// Gets the number of log entries in the cache. These are log entries, being held in memory, that have not been written to the actual log, yet.
        /// </summary>
        /// <seealso cref="LogCount"/>
        /// <seealso cref="ArchivedLogCount"/>
        int CachedLogCount { get; set; }
        /// <summary>
        /// Gets the number of log entries.
        /// </summary>
        int TotalLogCount { get; set; }
        // -------- Archived Log Statistics
        /// <summary>
        /// Gets or sets whether the automatic archive mechanism is on or off.
        /// </summary>
        bool AutoArchiveEnabled { get; set; }
        /// <summary>
        /// The number of archive cycles have been performed since start up.
        /// </summary>
        int ArchiveGeneration { get; set; }
        /// <summary>
        /// Gets the number of log entries in the archive. These <i>do not</i> include the files in the actual log.
        /// <seealso cref="LogCount"/>
        /// <seealso cref="CachedLogCount"/>
        /// </summary>
        int ArchivedLogCount { get; set; }
        /// <summary>
        /// The date the log was last archived.
        /// </summary>
        DateTime LastArchiveDate { get; set; }
        /// <summary>
        /// How long since the log was last archived.
        /// </summary>
        TimeSpan LastArchived { get; }
        /// <summary>
        /// The number of log entries archived during the last archive cycle.
        /// </summary>
        int LastArchiveCount { get; set; }
        /// <summary>
        /// The largest number of log entries archived during an archive cycle.
        /// </summary>
        int LargestArchiveCount { get; set; }
        /// <summary>
        /// The maximum number of log entries allowed in the log before the system automatically archives the log.
        /// </summary>
        int AutoArchiveMaximumLogs { get; set; }
        /// <summary>
        /// The ratio of log entries in the log before the system automatically archives the log. 
        /// </summary>
        double AutoArchiveLogRatio { get; }
    }
}

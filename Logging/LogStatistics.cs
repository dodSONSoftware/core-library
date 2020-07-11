using System;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Statistical information about the logging system.
    /// </summary>
    [Serializable]
    public class LogStatistics
        : ILogStatistics
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the LogStatistics class.
        /// </summary>
        public LogStatistics()
        {
        }
        #endregion
        #region ILogStatistics Methods
        // Log Statistics
        /// <summary>
        /// The date the log was opened.
        /// </summary>
        public DateTime DateStarted { get; } = DateTime.Now;
        /// <summary>
        /// How long the log has been opened.
        /// </summary>
        public TimeSpan RunTime => DateTime.Now - DateStarted;
        /// <summary>
        /// Gets the number of log entries in the actual log.
        /// </summary>
        public int LogCount
        {
            get; set;
        }
        /// <summary>
        /// The number of log entries written to the log since startup. 
        /// </summary>
        public int LogWrites
        {
            get; set;
        }
        // Cached Log Statistics
        /// <summary>
        /// Gets or sets whether the automatic cache flushing mechanism is on or off.
        /// </summary>
        public bool AutoFlushEnabled
        {
            get; set;
        }
        /// <summary>
        /// The number of cache flushes have been performed since startup.
        /// </summary>
        public int FlushGeneration
        {
            get; set;
        }
        /// <summary>
        /// The date the last cache flush occurred.
        /// </summary>
        public DateTime LastFlushDate { get; set; } = DateTime.MinValue;
        /// <summary>
        /// How long since the cache was flushed.
        /// </summary>
        public TimeSpan LastFlushed
        {
            get
            {
                if (LastFlushDate == DateTime.MinValue)
                {
                    return TimeSpan.Zero;
                }
                else
                {
                    return DateTime.Now - LastFlushDate;
                }
            }
        }
        /// <summary>
        ///  The number of log entries written to the log during the last cache flush.
        /// </summary>
        public int LastFlushCount
        {
            get; set;
        }
        /// <summary>
        /// The maximum amount of time the system will wait before automatically flushing the cache.
        /// </summary>
        public TimeSpan AutoFlushTimeLimit
        {
            get; set;
        }
        /// <summary>
        /// The amount of time remaining before the system will automatically flush the cache.
        /// </summary>
        public TimeSpan AutoFlushTimeRemaining
        {
            get; set;
        }
        /// <summary>
        /// The ratio of time remaining before the system will automatically flush the cache.
        /// </summary>
        public double AutoFlushTimeoutRatio=> (1.0 - (AutoFlushTimeRemaining.TotalMilliseconds / AutoFlushTimeLimit.TotalMilliseconds));
        /// <summary>
        /// The maximum number of log entries written to the log before the system automatically flushes the cache.
        /// </summary>
        public int AutoFlushMaximumLogs
        {
            get; set;
        }
        /// <summary>
        /// The ratio of log entries written to the log before the system will automatically flush the cache.
        /// </summary>
        public double AutoFlushLogsRatio=> ((double)CachedLogCount / (double)AutoFlushMaximumLogs);
        /// <summary>
        /// Returns the average number of log entires per second for the given duration.
        /// </summary>
        /// <param name="lastIncomingLogsReceived">The number of incoming log entries received for the duration specified.</param>
        /// <param name="forDuration">The amount of time to average for.</param>
        /// <returns>The average number of log entires per second for the given duration.</returns>
        public double IncomingAverageLogsPerSecond(long lastIncomingLogsReceived, TimeSpan forDuration)
        {
            var value = (((double)IncomingLogCount - (double)lastIncomingLogsReceived) / forDuration.TotalSeconds);
            if (value < 0)
            {
                value = 0;
            }
            return value;
        }
        /// <summary>
        /// The largest number of log entries written to the log during a log flush.
        /// </summary>
        public int LargestFlushCount
        {
            get; set;
        }
        /// <summary>
        /// The number of log entires received.
        /// </summary>
        public int IncomingLogCount
        {
            get; set;
        }
        /// <summary>
        /// Gets the number of log entries in the cache. These are log entries, being held in memory, that have not been written to the actual log, yet.
        /// </summary>
        public int CachedLogCount
        {
            get; set;
        }
        /// <summary>
        /// The number of log entries in the actual log plus the number of log entries in the archives.
        /// </summary>
        public int TotalLogCount
        {
            get; set;
        }
        // Archived Log Statistics
        /// <summary>
        /// The number of archive cycles have been performed since start up.
        /// </summary>
        public int ArchiveGeneration
        {
            get; set;
        }
        /// <summary>
        /// The date the log was last archived.
        /// </summary>
        public DateTime LastArchiveDate { get; set; } = DateTime.MinValue;
        /// <summary>
        /// How long since the log was last archived.
        /// </summary>
        public TimeSpan LastArchived
        {
            get
            {
                if (LastArchiveDate == DateTime.MinValue)
                {
                    return TimeSpan.Zero;
                }
                else
                {
                    return DateTime.Now - LastArchiveDate;
                }
            }
        }
        /// <summary>
        /// The maximum number of log entries allowed in the log before the system automatically archives the log.
        /// </summary>
        public int AutoArchiveMaximumLogs
        {
            get; set;
        }
        /// <summary>
        /// The ratio of log entries in the log before the system automatically archives the log. 
        /// </summary>
        public double AutoArchiveLogRatio
        {
            get
            {
                if (AutoArchiveMaximumLogs == 0)
                {
                    return 0;
                }
                return ((double)LogCount / (double)AutoArchiveMaximumLogs);
            }
        }
        /// <summary>
        /// The number of log entries archived during the last archive cycle.
        /// </summary>
        public int LastArchiveCount
        {
            get; set;
        }
        /// <summary>
        /// The largest number of log entries archived during an archive cycle.
        /// </summary>
        public int LargestArchiveCount
        {
            get; set;
        }
        /// <summary>
        /// Gets the number of log entries in the archive. These <i>do not</i> include the files in the actual log.
        /// <seealso cref="LogCount"/>
        /// <seealso cref="CachedLogCount"/>
        /// </summary>
        public int ArchivedLogCount
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets whether the automatic archive mechanism is on or off.
        /// </summary>
        public bool AutoArchiveEnabled
        {
            get; set;
        }
        // -------- 
        /// <summary>
        /// The number of log entries that have been broadcast into the communication system.
        /// </summary>
        public int BroadcastCount
        {
            get; set;
        }
    }
    #endregion
}


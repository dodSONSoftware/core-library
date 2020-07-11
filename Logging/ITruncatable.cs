using System;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines functionality to add truncation to a log.
    /// </summary>
    public interface ITruncatable
    {
        /// <summary>
        /// Gets or sets whether the log will be automatically truncated; <b>true</b> will automatically truncate log entires, <b>false</b> will not.
        /// </summary>
        bool AutoTruncateEnabled { get; set; }
        /// <summary>
        /// The number of times the log has been truncated since startup.
        /// </summary>
        int AutoTruncateGeneration { get; set; } 
        /// <summary>
        /// The date the log was last truncated.
        /// </summary>                       
        DateTime LastTruncatedDate { get; }
        /// <summary>
        /// How long since the log was last truncated.
        /// </summary>
        TimeSpan LastTruncated { get; }
        /// <summary>
        /// The number of log entries removed when the log was last truncated.
        /// </summary>
        int LastTruncatedCount { get; }
        /// <summary>
        /// The largest number of log entries truncated during a truncating cycle since startup.
        /// </summary>
        int LargestTruncatedCount { get; }
        /// <summary>
        /// A value showing the relationship between the number of log entires and the threshold for automatic archiving.
        /// </summary>
        double TruncatabilityRatio { get; }
        /// <summary>
        /// The number of log entries to leave in the log after archiving the log.
        /// </summary>
        int LogsToRetain { get; set; }
        /// <summary>
        /// The number of log entries that have been archive since startup.
        /// </summary>
        int TotalTruncatedLogs { get; set; }
        /// <summary>
        /// Will truncate log entries from the log.
        /// </summary>
        /// <returns>The number of log entries truncated.</returns>
        int TruncateLogs();
    }
}

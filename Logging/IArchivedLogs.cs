using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines functionality to add archiving to the <see cref="ILog"/> messaging system.
    /// </summary>
    public interface IArchivedLogs
            : IReadOnlyCollection<ILogEntry>
    {
        /// <summary>
        /// The file store containing the log files.
        /// </summary>
        FileStorage.IFileStore ArchiveFileStore { get; }
        /// <summary>
        /// The number of archive cycles have been performed since start up.
        /// </summary>
        int ArchiveGeneration { get; }
        /// <summary>
        /// The date the log was last archived.
        /// </summary>
        DateTime LastArchiveDate { get; }
        /// <summary>
        /// How long since the log was last archived.
        /// </summary>
        TimeSpan LastArchived { get; }
        /// <summary>
        /// The number of log entries archived during the last archive cycle.
        /// </summary>
        int LastArchiveCount { get; }
        /// <summary>
        /// The largest number of log entries archived during an archive cycle.
        /// </summary>
        int LargestArchiveCount { get; }
        /// <summary>
        /// Will take an <see cref="ILogStatistics"/> class and add archive statistics to it.
        /// </summary>
        /// <param name="sourceStatistics">The pre-populated <see cref="ILogStatistics"/> class which will be populated with archive statistics.</param>
        /// <returns>The pre-populated <see cref="ILogStatistics"/> class with the archive statistics added.</returns>
        ILogStatistics PopulateStatistics(ILogStatistics sourceStatistics);
        // --------
        /// <summary>
        /// Will write all log entries in the <paramref name="log"/> to a file in the <see cref="ArchiveFileStore"/>, clearing the log entries.
        /// </summary>
        /// <param name="log">The log to read log entries from.</param>
        /// <param name="filename">The name of the log file to create in the <see cref="ArchiveFileStore"/>.</param>
        /// <param name="writeLogEntriesUsingLocalTime">Sets whether the date is written to the log relative to UTC or the local computer's timezone.</param>
        /// <param name="truncateLogArchive">Sets whether the ArchiveFileStore will be automatically truncated.</param>
        /// <param name="truncateLogArchiveMaximumFiles">The total number of files allowed in the ArchiveFileStore before auto truncation is performed.</param>
        /// <param name="truncateLogArchivePercentageToRemove">The percentage of ArchiveFileStore files to remove once <paramref name="truncateLogArchiveMaximumFiles"/> has been exceeded. Value must be between 0.0 and 1.0.</param>
        /// <returns>The number of log entries archived.</returns>
        int Archive(ILog log,
                    string filename,
                    bool writeLogEntriesUsingLocalTime,
                    bool truncateLogArchive,
                    int truncateLogArchiveMaximumFiles,
                    double truncateLogArchivePercentageToRemove);
        /// <summary>
        /// Will write all log entries in the <paramref name="log"/>, that return <b>true</b> from the <paramref name="LogEntryFilterPredicate"/> function, to a file in the <see cref="ArchiveFileStore"/>, clearing the log entries.
        /// </summary>
        /// <param name="log">The log to read log entries from.</param>
        /// <param name="filename">The name of the log file to create in the <see cref="ArchiveFileStore"/>.</param>
        /// <param name="writeLogEntriesUsingLocalTime">Sets whether the date is written to the log relative to UTC or the local computer's timezone.</param>
        /// <param name="truncateLogArchive">Sets whether the ArchiveFileStore will be automatically truncated.</param>
        /// <param name="truncateLogArchiveMaximumFiles">The total number of files allowed in the ArchiveFileStore before auto truncation is performed.</param>
        /// <param name="truncateLogArchivePercentageToRemove">The percentage of ArchiveFileStore files to remove once <paramref name="truncateLogArchiveMaximumFiles"/> has been exceeded. Value must be between 0.0 and 1.0.</param>
        /// <param name="LogEntryFilterPredicate">A function which takes an <see cref="ILogEntry"/> and returns <b>true</b> to include the log entry, or <b>false</b> to skip the log entry.</param>
        /// <returns>The number of log entries archived.</returns>
        int Archive(ILog log,
                    string filename,
                    bool writeLogEntriesUsingLocalTime,
                    bool truncateLogArchive,
                    int truncateLogArchiveMaximumFiles,
                    double truncateLogArchivePercentageToRemove,
                    Func<ILogEntry, bool> LogEntryFilterPredicate);
        /// <summary>
        /// Reads each log entry from all the log files in the log archive.
        /// </summary>
        /// <returns>Returns all log entries from all the log files in the log archive.</returns>
        IEnumerable<ILogEntry> Read();
        /// <summary>
        /// Returns each log entry from all the log files in the log archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate">A function which takes an <see cref="ILogEntry"/> and returns <b>true</b> to include the log entry, or false to skip the log entry.</param>
        /// <returns>Every log entry from all the log files in the log archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate);
        /// <summary>
        /// Returns each log entry in the log and the archive which returns <b>true</b> from the <paramref name="logFilenameFilterPredicate"/>.
        /// </summary>
        /// <param name="logFilenameFilterPredicate">A function which takes the filename about to be processed and returns <b>true</b> to include the log file, or false to skip the log file.</param>
        /// <returns>Every log entry from the log and the archive which returns <b>true</b> from the <paramref name="logFilenameFilterPredicate"/>.</returns>
        IEnumerable<ILogEntry> Read(Func<string, bool> logFilenameFilterPredicate);
        /// <summary>
        /// Returns each log entry in the log and the archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate">A function which takes an <see cref="ILogEntry"/> and returns <b>true</b> to include the log entry, or false to skip the log entry.</param>
        /// <param name="logFilenameFilterPredicate">A function which takes the filename about to be processed and returns <b>true</b> to include the log file, or false to skip the log file.</param>
        /// <returns>Every log entry from the log and the archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate,
                                    Func<string, bool> logFilenameFilterPredicate);
    }
}

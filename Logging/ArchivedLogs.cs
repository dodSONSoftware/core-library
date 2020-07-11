using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Provides unified access to logs in a log archive.
    /// </summary>
    public class ArchivedLogs
        : IArchivedLogs
    {
        #region Ctor
        private ArchivedLogs()
        {
        }
        /// <summary>
        /// Initializes an instance of the ArchivedLogs using the <paramref name="archiveFileStore"/> as the source of log files.
        /// </summary>
        /// <param name="archiveFileStore">The file store which provides the log files.</param>
        public ArchivedLogs(FileStorage.IFileStore archiveFileStore)
            : this()
        {
            ArchiveFileStore = archiveFileStore ?? throw new ArgumentNullException(nameof(archiveFileStore));
        }
        #endregion
        #region IArchivedLogs Methods
        /// <summary>
        /// The file store containing the log files.
        /// </summary>
        public FileStorage.IFileStore ArchiveFileStore
        {
            get; private set;
        }
        /// <summary>
        /// The number of archive cycles have been performed since start up.
        /// </summary>
        public int ArchiveGeneration
        {
            get; private set;
        }
        /// <summary>
        /// The date the log was last archived.
        /// </summary>
        public DateTime LastArchiveDate { get; private set; } = DateTime.MinValue;
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
        /// The number of log entries archived during the last archive cycle.
        /// </summary>
        public int LastArchiveCount
        {
            get; private set;
        }
        /// <summary>
        /// The largest number of log entries archived during an archive cycle.
        /// </summary>
        public int LargestArchiveCount
        {
            get; private set;
        }
        /// <summary>
        /// Returns the number of log entries in all the archive log files.
        /// </summary>
        public int Count
        {
            get
            {
                return ForEachFile((e) => { return true; }, (f) => { return true; }).Count();
            }
        }
        /// <summary>
        /// Will take an <see cref="ILogStatistics"/> class and add archive statistics to it.
        /// </summary>
        /// <param name="sourceStatistics">The pre-populated <see cref="ILogStatistics"/> class which will be populated with archive statistics.</param>
        /// <returns>The pre-populated <see cref="ILogStatistics"/> class with the archive statistics added.</returns>
        public ILogStatistics PopulateStatistics(ILogStatistics sourceStatistics)
        {
            if (sourceStatistics == null)
            {
                throw new ArgumentNullException(nameof(sourceStatistics));
            }
            sourceStatistics.ArchivedLogCount = Count;
            sourceStatistics.ArchiveGeneration = ArchiveGeneration;
            sourceStatistics.AutoArchiveEnabled = true;
            sourceStatistics.LargestArchiveCount = LargestArchiveCount;
            sourceStatistics.LastArchiveCount = LastArchiveCount;
            sourceStatistics.LastArchiveDate = LastArchiveDate;
            return sourceStatistics;
        }
        /// <summary>
        /// Will write all log entries in the <paramref name="log"/> to a file in the <see cref="ArchiveFileStore"/>, clearing the log entries.
        /// </summary>
        /// <param name="log">The log to read log entries from.</param>
        /// <param name="filename">The name of the log file to create in the <see cref="ArchiveFileStore"/>.</param>
        /// <param name="writeLogEntriesUsingLocalTime">Sets whether the date is written to the log relative to UTC or the local computer's timezone.</param>
        /// <param name="truncateLogArchive">Sets whether the ArchiveFileStore will be automatically truncated.</param>
        /// <param name="truncateLogArchiveMaximumFiles">The total number of files allowed in the ArchiveFileStore before auto truncation is performed.</param>
        /// <param name="truncateLogArchivePercentageToRemove">The percentage of log archive files to remove once <paramref name="truncateLogArchiveMaximumFiles"/> has been exceeded. Value must be between 0.0 and 1.0.</param>
        /// <returns>The number of log entries archived.</returns>
        public int Archive(ILog log,
                           string filename,
                           bool writeLogEntriesUsingLocalTime,
                           bool truncateLogArchive,
                           int truncateLogArchiveMaximumFiles,
                           double truncateLogArchivePercentageToRemove) => Archive(log, filename, writeLogEntriesUsingLocalTime, truncateLogArchive, truncateLogArchiveMaximumFiles, truncateLogArchivePercentageToRemove, (e) => { return true; });
        /// <summary>
        /// Will write all log entries in the <paramref name="log"/>, that return <b>true</b> from the <paramref name="LogEntryFilterPredicate"/> function, to a file in the <see cref="ArchiveFileStore"/>, clearing the log entries.
        /// </summary>
        /// <param name="log">The log to read log entries from.</param>
        /// <param name="filename">The name of the log file to create in the <see cref="ArchiveFileStore"/>.</param>
        /// <param name="writeLogEntriesUsingLocalTime">Sets whether the date is written to the log relative to UTC or the local computer's timezone.</param>
        /// <param name="truncateLogArchive">Sets whether the ArchiveFileStore will be automatically truncated.</param>
        /// <param name="truncateLogArchiveMaximumFiles">The total number of files allowed in the ArchiveFileStore before auto truncation is performed.</param>
        /// <param name="truncateLogArchivePercentageToRemove">The percentage of log archive files to remove once <paramref name="truncateLogArchiveMaximumFiles"/> has been exceeded. Value must be between 0.0 and 1.0.</param>
        /// <param name="LogEntryFilterPredicate">A function which takes an <see cref="ILogEntry"/> and returns <b>true</b> to include the log entry, or <b>false</b> to skip the log entry.</param>
        /// <returns>The number of log entries archived.</returns>
        public int Archive(ILog log,
                           string filename,
                           bool writeLogEntriesUsingLocalTime,
                           bool truncateLogArchive,
                           int truncateLogArchiveMaximumFiles,
                           double truncateLogArchivePercentageToRemove,
                           Func<ILogEntry, bool> LogEntryFilterPredicate)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            var count = LoggingHelper.ArchiveLogs(log, ArchiveFileStore, filename, writeLogEntriesUsingLocalTime, truncateLogArchive, truncateLogArchiveMaximumFiles, truncateLogArchivePercentageToRemove, LogEntryFilterPredicate);
            if (count > 0)
            {
                ++ArchiveGeneration;
                LastArchiveDate = DateTime.Now;
                LastArchiveCount = count;
                if (count > LargestArchiveCount)
                {
                    LargestArchiveCount = count;
                }

            }
            return count;
        }
        /// <summary>
        /// Reads each log entry from all the log files in the log archive.
        /// </summary>
        /// <returns>Returns all log entries from all the log files in the log archive.</returns>
        public IEnumerable<ILogEntry> Read() => Read((e) => { return true; }, (f) => { return true; });
        /// <summary>
        /// Returns each log entry from all the log files in the log archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate">A function which takes an <see cref="ILogEntry"/> and returns <b>true</b> to include the log entry, or false to skip the log entry.</param>
        /// <returns>Every log entry from all the log files in the log archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate) => Read(logEntryFilterPredicate, (f) => { return true; });
        /// <summary>
        /// Returns each log entry in the log and the archive which returns <b>true</b> from the <paramref name="logFilenameFilterPredicate"/>.
        /// </summary>
        /// <param name="logFilenameFilterPredicate">A function which takes the filename about to be processed and returns <b>true</b> to include the log file, or false to skip the log file.</param>
        /// <returns>Every log entry from the log and the archive which returns <b>true</b> from the <paramref name="logFilenameFilterPredicate"/>.</returns>
        public IEnumerable<ILogEntry> Read(Func<string, bool> logFilenameFilterPredicate) => Read((e) => { return true; }, logFilenameFilterPredicate);
        /// <summary>
        /// Returns each log entry in the log and the archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate">A function which takes an <see cref="ILogEntry"/> and returns <b>true</b> to include the log entry, or false to skip the log entry.</param>
        /// <param name="logFilenameFilterPredicate">A function which takes the filename about to be processed and returns <b>true</b> to include the log file, or false to skip the log file.</param>
        /// <returns>Every log entry from the log and the archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate, Func<string, bool> logFilenameFilterPredicate) => ForEachFile(logEntryFilterPredicate, logFilenameFilterPredicate);
        /// <summary>
        /// Returns an enumerator that iterates through all the log entries in all the archive log files.
        /// </summary>
        /// <returns>An enumerator that iterates through all the log entries in all the archive log files.</returns>
        public IEnumerator<ILogEntry> GetEnumerator() => Read().GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through all the log entries in all the archive log files.
        /// </summary>
        /// <returns>An enumerator that iterates through all the log entries in all the archive log files.</returns>
        IEnumerator IEnumerable.GetEnumerator() => Read().GetEnumerator();
        #endregion
        #region Private Methods
        private IEnumerable<ILogEntry> ForEachFile(Func<ILogEntry, bool> logEntryFilterPredicate,
                                                   Func<string, bool> logFilenameFilterPredicate)
        {
            var tempPath = System.IO.Path.GetTempPath();
            foreach (var storeItem in from x in ArchiveFileStore.GetAll()
                                      where logFilenameFilterPredicate(x.RootFilename)
                                      orderby x.RootFileLastModifiedTimeUtc ascending
                                      select x)
            {
                var extractedName = storeItem.Extract(tempPath, true);
                if (System.IO.File.Exists(extractedName))
                {
                    var log = new FileEventLog.Log(extractedName, false);
                    log.Open();
                    try
                    {
                        foreach (var entry in log.Read(logEntryFilterPredicate))
                        {
                            yield return entry;
                        }
                    }
                    finally
                    {
                        log.Close();
                        try
                        {
                            System.IO.File.Delete(extractedName);
                        }
                        catch { }
                    }
                }
            }
        }
        #endregion
    }
}

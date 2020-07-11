using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Common, and standardizing, logging methods used throughout the dodSON.Core.Logging namespace.
    /// </summary>
    /// <seealso cref="Logging.ArchivedLogs"/>
    /// <seealso cref="LogEntry.Parse(string)"/>
    /// <seealso cref="LogEntry.TryParse(string, out LogEntry)"/>
    /// <seealso cref="LogEntry.ToString()"/>
    public static class LoggingHelper
    {
        #region Private Constants
        private static readonly char _DefaultLogLineElementSeparator_ = ';';
        #endregion
        #region Public Methods
        /// <summary>
        /// Will copy all logs, which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/> function, from the <paramref name="log"/> and write them to a text file with the <paramref name="filename"/> in the given <paramref name="fileStore"/>.
        /// </summary>
        /// <remarks>
        /// <code>
        /// // The following characters are encoded into, and decoded from, the following:
        /// //      Percent        (%)  %&lt;25&gt;
        /// //      Semicolon      (;)  %&lt;3B&gt;
        /// //      Double-Quote   (")  %&lt;22&gt;
        /// //      Newline        ( )  %&lt;0D&gt;
        /// </code>
        /// </remarks>
        /// <param name="log">The logger to read log from.</param>
        /// <param name="fileStore">The file store to write the log file to.</param>
        /// <param name="filename">The name of the file in the file store to write the logs to.</param>
        /// <param name="writeLogEntriesUsingLocalTime">Sets whether the date is written to the log relative to UTC or the local computer's timezone.</param>
        /// <param name="logEntryFilterPredicate">The <see cref="Func{T, TResult}"/> should return <b>true</b> to write the log to the archive file; return <b>false</b> to skip writing the log.</param>
        /// <returns>The number of logs written to the log file.</returns>
        public static int ArchiveLogs(ILogReader log,
                                      FileStorage.IFileStore fileStore,
                                      string filename,
                                      bool writeLogEntriesUsingLocalTime,
                                      Func<ILogEntry, bool> logEntryFilterPredicate)
        {
            return ArchiveLogs(log,
                               fileStore,
                               filename,
                               writeLogEntriesUsingLocalTime,
                               false,
                               0,
                               0,
                               logEntryFilterPredicate);
        }
        /// <summary>
        /// Will copy all logs from the <paramref name="log"/> and write them to a text file with the <paramref name="filename"/> in the given <paramref name="fileStore"/>.
        /// </summary>
        /// <remarks>
        /// <code>
        /// // The following characters are encoded into, and decoded from, the following:
        /// //      Percent        (%)  %&lt;25&gt;
        /// //      Semicolon      (;)  %&lt;3B&gt;
        /// //      Double-Quote   (")  %&lt;22&gt;
        /// //      Newline        ( )  %&lt;0D&gt;
        /// </code>
        /// </remarks>
        /// <param name="log">The logger to read log from.</param>
        /// <param name="fileStore">The file store to write the log file to.</param>
        /// <param name="filename">The name of the file in the file store to write the logs to.</param>
        /// <param name="writeLogEntriesUsingLocalTime">Sets whether the date is written to the log relative to UTC or the local computer's timezone.</param>
        /// <param name="truncateLogArchive">Sets whether the <paramref name="fileStore"/> will be automatically truncated.</param>
        /// <param name="truncateLogArchiveMaximumFiles">The total number of files allowed in the <paramref name="fileStore"/> before auto truncation is performed.</param>
        /// <param name="truncateLogArchivePercentageToRemove">The percentage of log archive files to remove once <paramref name="truncateLogArchiveMaximumFiles"/> has been exceeded.</param>
        /// <returns>The number of logs written to the log file.</returns>
        public static int ArchiveLogs(ILogReader log,
                                      FileStorage.IFileStore fileStore,
                                      string filename,
                                      bool writeLogEntriesUsingLocalTime,
                                      bool truncateLogArchive,
                                      int truncateLogArchiveMaximumFiles,
                                      double truncateLogArchivePercentageToRemove)
        {
            return ArchiveLogs(log,
                               fileStore,
                               filename,
                               writeLogEntriesUsingLocalTime,
                               truncateLogArchive,
                               truncateLogArchiveMaximumFiles,
                               truncateLogArchivePercentageToRemove,
                               new Func<ILogEntry, bool>((e) => { return true; }));
        }
        /// <summary>
        /// Will copy all logs, which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/> function, from the <paramref name="log"/> and write them to a text file with the <paramref name="filename"/> in the given <paramref name="fileStore"/>.
        /// </summary>
        /// <remarks>
        /// <code>
        /// // The following characters are encoded into, and decoded from, the following:
        /// //      Percent        (%)  %&lt;25&gt;
        /// //      Semicolon      (;)  %&lt;3B&gt;
        /// //      Double-Quote   (")  %&lt;22&gt;
        /// //      Newline        ( )  %&lt;0D&gt;
        /// </code>
        /// </remarks>
        /// <param name="log">The logger to read log from.</param>
        /// <param name="fileStore">The file store to write the log file to.</param>
        /// <param name="filename">The name of the file in the file store to write the logs to.</param>
        /// <param name="writeLogEntriesUsingLocalTime">Sets whether the date is written to the log relative to UTC or the local computer's timezone.</param>
        /// <param name="truncateLogArchive">Sets whether the <paramref name="fileStore"/> will be automatically truncated.</param>
        /// <param name="truncateLogArchiveMaximumFiles">The total number of files allowed in the <paramref name="fileStore"/> before auto truncation is performed.</param>
        /// <param name="truncateLogArchivePercentageToRemove">The percentage of log archive files to remove once <paramref name="truncateLogArchiveMaximumFiles"/> has been exceeded.</param>
        /// <param name="logEntryFilterPredicate">The <see cref="Func{T, TResult}"/> should return <b>true</b> to write the log to the archive file; return <b>false</b> to skip writing the log.</param>
        /// <returns>The number of logs written to the log file.</returns>
        public static int ArchiveLogs(ILogReader log,
                                      FileStorage.IFileStore fileStore,
                                      string filename,
                                      bool writeLogEntriesUsingLocalTime,
                                      bool truncateLogArchive,
                                      int truncateLogArchiveMaximumFiles,
                                      double truncateLogArchivePercentageToRemove,
                                      Func<ILogEntry, bool> logEntryFilterPredicate)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (fileStore == null)
            {
                throw new ArgumentNullException(nameof(fileStore));
            }
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }
            if ((truncateLogArchivePercentageToRemove < 0) || (truncateLogArchivePercentageToRemove > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(truncateLogArchivePercentageToRemove), $"Parameter truncateLogArchivePercentageToRemove must be between 0.0 and 1.0. Value={truncateLogArchivePercentageToRemove}");
            }
            if (logEntryFilterPredicate == null)
            {
                throw new ArgumentNullException(nameof(logEntryFilterPredicate));
            }
            //
            var archivedLogs = 0;
            // copy logs from the logger to the temp file
            var tempFilename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            lock (log.SyncObject)
            {
                using (var sw = new System.IO.StreamWriter(tempFilename, false))
                {
                    foreach (var entry in from x in log.Read()
                                          orderby x.Timestamp ascending
                                          select x)
                    {
                        if ((entry != null) && (logEntryFilterPredicate(entry)))
                        {
                            sw.WriteLine(FormatLogLine(entry, writeLogEntriesUsingLocalTime));
                            ++archivedLogs;
                        }
                    }
                }
            }
            if (archivedLogs > 0)
            {
                // copy temp file into FileStore
                fileStore.Add(filename, tempFilename, DateTime.UtcNow, 0);
                if (truncateLogArchive)
                {
                    TruncateArchiveFileStore(fileStore, truncateLogArchiveMaximumFiles, truncateLogArchivePercentageToRemove);
                }
                fileStore.Save(true);
            }
            // delete temp file
            System.IO.File.Delete(tempFilename);
            // return number of archived logs
            return archivedLogs;
        }
        /// <summary>
        /// Returns a string representation of a <see cref="ILogEntry"/>.
        /// </summary>
        /// <remarks>
        /// This method will encode certain characters into short character sequences in order to allow the entry to exist in a single line and to avoid characters interfering with the operation of parsing the log entry from the line.
        /// This method is the companion method of <see cref="ParseLogLine(string)"/>.
        /// <para>
        /// <code>
        /// // The following characters are encoded into, and decoded from, the following:
        /// //      Percent        (%)  %&lt;25&gt;
        /// //      Semicolon      (;)  %&lt;3B&gt;
        /// //      Double-Quote   (")  %&lt;22&gt;
        /// //      Newline        ( )  %&lt;0D&gt;
        /// </code>
        /// </para>
        /// </remarks>
        /// <seealso cref="ParseLogLine(string)"/>
        /// <param name="logEntry">The log entry to format.</param>
        /// <param name="toLocalTime">Sets whether the time stamp is formatted relative to UTC or the local computer's timezone.</param>
        /// <returns>A representation of a <see cref="ILogEntry"/>.</returns>
        public static string FormatLogLine(ILogEntry logEntry, bool toLocalTime)
        {
            if (logEntry == null)
            {
                throw new ArgumentNullException(nameof(logEntry));
            }
            // generate time stamp
            var timeStamp = logEntry.Timestamp.ToUniversalTime();
            if (toLocalTime)
            {
                timeStamp = logEntry.Timestamp.ToLocalTime();
            }
            // return the formatted log line
            // the timeStamp takes the first two columns: time index (ticks) and the human-readable time (1 entry, 2 columns)
            return $"{timeStamp.Ticks}{_DefaultLogLineElementSeparator_} " +
                   $"{timeStamp}{_DefaultLogLineElementSeparator_} " +
                   $"{logEntry.EntryType}{_DefaultLogLineElementSeparator_} " +
                   $"{logEntry.EventId}{_DefaultLogLineElementSeparator_} " +
                   $"{logEntry.Category}{_DefaultLogLineElementSeparator_} " +
                   $"{Common.StringHelper.TokenizeString(logEntry.SourceId, Common.StringHelper.DefaultTokenReplacementPairs)}{_DefaultLogLineElementSeparator_} " +
                   $"{Common.StringHelper.TokenizeString(logEntry.Message, Common.StringHelper.DefaultTokenReplacementPairs)}";
        }
        /// <summary>
        /// Returns a <see cref="ILogEntry"/> created from the supplied <paramref name="line"/>.
        /// </summary>
        /// <remarks>
        /// This method will decode certain short string sequences into characters. These characters are encoded in order to allow the entry to exist in a single line and to avoid characters interfering with the operation of parsing the log entry from the line.
        /// This method is the companion method of <see cref="FormatLogLine(ILogEntry,bool)"/>.
        /// <para>
        /// <code>
        /// // The following characters are encoded into, and decoded from, the following:
        /// //      Percent        (%)  %&lt;25&gt;
        /// //      Semicolon      (;)  %&lt;3B&gt;
        /// //      Double-Quote   (")  %&lt;22&gt;
        /// //      Newline        ( )  %&lt;0D&gt;
        /// </code>
        /// </para>
        /// </remarks>
        /// <seealso cref="FormatLogLine(ILogEntry, bool)"/>
        /// <param name="line">The string to parse the <see cref="ILogEntry"/> from.</param>
        /// <returns>A <see cref="ILogEntry"/> created from the supplied <paramref name="line"/>, or null if the line could not be parsed.</returns>
        public static LogEntry ParseLogLine(string line)
        {
            // this method swallows any problems with parsing the line. (BAD LINE returns NULL)
            if (!string.IsNullOrWhiteSpace(line))
            {
                // split the line and only process if there are 7 entries
                var parts = line.Split(_DefaultLogLineElementSeparator_);
                if (parts?.Length >= 7)
                {
                    try
                    {
                        // read human-readable time entry, this is to get the time stamp's Offset
                        var timeStamp = DateTimeOffset.Parse(parts[1].Trim());
                        // read time index (ticks); using the human-readable time entry's Offset
                        var timeStampActual = new DateTimeOffset(Convert.ToInt64(parts[0].Trim()), timeStamp.Offset);
                        // get the rest of the log line as the message
                        var message = new StringBuilder(255);
                        for (int i = 6; i < parts.Length; i++) { message.Append(parts[i] + _DefaultLogLineElementSeparator_); }
                        --message.Length;
                        // create a new log entry using the time index
                        return new LogEntry((LogEntryType)Enum.Parse(typeof(LogEntryType), parts[2].Trim()),
                                            Common.StringHelper.UnTokenizeString(parts[5].Trim(), Common.StringHelper.DefaultTokenReplacementPairs),
                                            Common.StringHelper.UnTokenizeString(message.ToString().Trim(), Common.StringHelper.DefaultTokenReplacementPairs),
                                            Convert.ToInt32(parts[3].Trim()),
                                            Convert.ToUInt16(parts[4].Trim()),
                                            timeStampActual);
                    }
                    catch
                    {
                        // return null if ANY errors occur
                        return null;
                    }
                }
            }
            // return null if blank line
            return null;
        }
        /// <summary>
        /// Combines any number of <see cref="Logs"/> together into a single <see cref="Logs"/>.
        /// </summary>
        /// <param name="args">An array of <see cref="Logs"/>.</param>
        /// <returns>A single <see cref="Logs"/> containing all of the specified <see cref="Logs"/> together.</returns>
        public static Logs Combine(params Logs[] args)
        {
            Logs results = new Logs();
            foreach (var log in args)
            {
                results.Add(log);
            }
            return results;
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Will remove files from the <paramref name="fileStore"/> based on the parameters given.
        /// </summary>
        /// <param name="fileStore">The <paramref name="fileStore"/> to remove files from.</param>
        /// <param name="truncateLogArchiveMaximumFiles">The maximum files allowed to remain in the <paramref name="fileStore"/>.</param>
        /// <param name="truncateLogArchivePercentageToRemove">The percentage of files to remove from the <paramref name="fileStore"/>.</param>
        private static void TruncateArchiveFileStore(FileStorage.IFileStore fileStore, int truncateLogArchiveMaximumFiles, double truncateLogArchivePercentageToRemove)
        {
            if (fileStore.Count >= truncateLogArchiveMaximumFiles)
            {
                var removeCount = (int)(fileStore.Count * truncateLogArchivePercentageToRemove);
                if (removeCount > 0)
                {
                    foreach (var item in from x in fileStore.GetAll()
                                         orderby x.RootFileLastModifiedTimeUtc ascending
                                         select x)
                    {
                        fileStore.Delete(item.RootFilename);
                        if (--removeCount <= 0)
                        {
                            break;
                        }
                    }
                }
            }
        }
        #endregion
    }
}

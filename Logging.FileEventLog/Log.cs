using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging.FileEventLog
{
    /// <summary>
    /// Provides a file-based implementation of the <see cref="ILog"/> interface. This allows the creation and management of logs in a file.
    /// </summary>
    /// <remarks>
    /// To keep the size of the log under control, the FileEventLog.Log can automatically truncate the log file. 
    /// Be sure to set the <see cref="AutoTruncateEnabled"/>, <see cref="LogFileSizeMaximumBytes"/> and the <see cref="LogsToRetain"/> values to appropriate values.
    /// </remarks>
    public class Log
          : LogBase, ITruncatable
    {
        #region Public Constants
        /// <summary>
        /// The minimum size, in bytes, that the <see cref="LogFileSizeMaximumBytes"/> can be set to.
        /// </summary>
        public static readonly long MinimumLogSizeBytes = Common.ByteCountHelper.FromKilobytes(10);
        #endregion
        #region Ctor
        private Log() : base() { }
        /// <summary>
        /// Initializes a new instance of the EventLog class using the supplied filename to read and write logs.
        /// </summary>
        /// <param name="logFilename">The name of the file to read and write <see cref="ILogEntry"/>s to.</param>
        /// <param name="writeLogEntriesUsingLocalTime">Sets whether the date is written to the log relative to UTC or the local computer's timezone.</param>
        public Log(string logFilename,
                   bool writeLogEntriesUsingLocalTime) : this(logFilename, writeLogEntriesUsingLocalTime, false, MinimumLogSizeBytes, 0) { }
        /// <summary>
        /// Initializes a new instance of the EventLog class using the supplied information.
        /// </summary>
        /// <param name="logFilename">The name of the file to read and write <see cref="ILogEntry"/>s to.</param>
        /// <param name="writeLogEntriesUsingLocalTime">Sets whether the date is written to the log relative to UTC or the local computer's timezone.</param>
        /// <param name="autoTruncateLogFile">Sets whether the EventLog will automatically remove logs from the log file. <b>True</b> indicates that the EventLog should automatically truncate the log file; <b>false</b> will not monitor the size of the log file or truncate it.</param>
        /// <param name="maxLogSizeBytes">The maximum size, in bytes, the log file can be before triggering an automatic call to <see cref="TruncateLogs()"/>.</param>
        /// <param name="logsToRetain">The number of logs to retain when truncating file.</param>
        public Log(string logFilename,
                   bool writeLogEntriesUsingLocalTime,
                   bool autoTruncateLogFile,
                   long maxLogSizeBytes,
                   int logsToRetain)
            : this()
        {
            if (string.IsNullOrWhiteSpace(logFilename)) { throw new ArgumentNullException(nameof(logFilename)); }
            if (maxLogSizeBytes < MinimumLogSizeBytes) { throw new ArgumentOutOfRangeException(nameof(maxLogSizeBytes), $"Parameter {nameof(maxLogSizeBytes)} must be greater than {MinimumLogSizeBytes}."); }
            if (logsToRetain < 0) { throw new ArgumentOutOfRangeException(nameof(logsToRetain), $"Parameter {nameof(logsToRetain)} must be greater than, or equal to, 0."); }
            LogFilename = logFilename;
            _WriteLogEntriesUsingLocalTime = writeLogEntriesUsingLocalTime;
            AutoTruncateEnabled = autoTruncateLogFile;
            _MaxLogSizeBytes = maxLogSizeBytes;
            _LogsToRetain = logsToRetain;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public Log(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "Log") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Log\". Configuration Key={configuration.Key}", nameof(configuration)); }


            // _LogFilename 

            //LogFilename = Core.Configuration.ConfigurationHelper.FindConfigurationItemValue<string>(configuration, "LogFilename");
            if (!Core.Configuration.ConfigurationHelper.TryFindConfigurationItemValue(configuration, "LogFilename", out string _logfilename, out Exception logFilenameException))
            {
                throw logFilenameException;
            }
            LogFilename = _logfilename;
            if (string.IsNullOrWhiteSpace(LogFilename)) { throw new Exception($"Configuration item missing information. Configuration item \"LogFilename\" must contain a value."); }
            // _WriteLogEntriesUsingLocalTime
            _WriteLogEntriesUsingLocalTime = Core.Configuration.ConfigurationHelper.FindConfigurationItemValue<bool>(configuration, "WriteLogEntriesUsingLocalTime");
            // _AutoTruncateLogFile
            AutoTruncateEnabled = Core.Configuration.ConfigurationHelper.FindConfigurationItemValue<bool>(configuration, "AutoTruncateLogFile");
            // _MaxLogSizeBytes
            LogFileSizeMaximumBytes = Core.Configuration.ConfigurationHelper.FindConfigurationItemValue<long>(configuration, "MaxLogSizeBytes");
            // _LogsToRetain
            LogsToRetain = Core.Configuration.ConfigurationHelper.FindConfigurationItemValue<int>(configuration, "LogsToRetain");
        }
        #endregion
        #region Private Fields
        private bool _IsOpen = false;
        private bool _WriteLogEntriesUsingLocalTime = false;
        private long _MaxLogSizeBytes;
        private int _LogsToRetain;
        private int _TotalTruncatedLogs;
        //
        private ILogStatistics _LogStats = new LogStatistics();
        #endregion
        #region Public Methods
        /// <summary>
        /// The name of the file to read and write log entries.
        /// </summary>
        public string LogFilename { get; }
        /// <summary>
        /// The maximum size, in bytes, the log file can be before triggering an automatic call to <see cref="TruncateLogs()"/>.
        /// </summary>
        /// <seealso cref="AutoTruncateEnabled"/>
        /// <seealso cref="LogsToRetain"/>
        public long LogFileSizeMaximumBytes
        {
            get { return _MaxLogSizeBytes; }
            set
            {
                if (value < MinimumLogSizeBytes) { throw new ArgumentOutOfRangeException(nameof(value), $"Parameter {nameof(value)} must be greater than {MinimumLogSizeBytes}."); }
                _MaxLogSizeBytes = value;
            }
        }
        /// <summary>
        /// The size, in bytes, of the log file.
        /// </summary>
        public long LogFileSizeBytes
        {
            get { return (new System.IO.FileInfo(LogFilename)).Length; }
        }
        #endregion
        #region ITruncatable Methods
        /// <summary>
        /// Gets or sets whether the EventLog will automatically remove logs from the log file. <b>True</b> indicates that the EventLog should automatically truncate the log file; <b>false</b> will not monitor the size of the log file or truncate it.
        /// </summary>
        /// <seealso cref="LogFileSizeMaximumBytes"/>
        /// <seealso cref="LogsToRetain"/>
        public bool AutoTruncateEnabled { get; set; } = false;
        /// <summary>
        /// The number of times the log entires have be truncated from the log.
        /// </summary>
        public int AutoTruncateGeneration { get; set; } = 0;
        /// <summary>
        /// The number of logs to retain when truncating file.
        /// </summary>
        /// <seealso cref="AutoTruncateEnabled"/>
        /// <seealso cref="LogFileSizeMaximumBytes"/>
        public int LogsToRetain
        {
            get { return _LogsToRetain; }
            set
            {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value), $"Parameter {nameof(value)} must be greater than, or equal to, 0."); }
                _LogsToRetain = value;
            }
        }
        /// <summary>
        /// The number of logs that have been removed from the file.
        /// </summary>
        public int TotalTruncatedLogs
        {
            get { return _TotalTruncatedLogs; }
            set { _TotalTruncatedLogs = value; ; }
        }
        /// <summary>
        /// The ratio of data in the log file in relation to the <see cref="LogFileSizeMaximumBytes"/>.
        /// </summary>
        public double TruncatabilityRatio
        {
            get { return ((double)LogFileSizeBytes / (double)LogFileSizeMaximumBytes); }
        }
        /// <summary>
        /// The date the log file was last truncated.
        /// </summary>
        public DateTime LastTruncatedDate { get; private set; } = DateTime.MinValue;
        /// <summary>
        /// The interval since the log file was last truncated.
        /// </summary>
        public TimeSpan LastTruncated
        {
            get
            {
                if (LastTruncatedDate == DateTime.MinValue) { return TimeSpan.Zero; }
                else { return (DateTime.Now - LastTruncatedDate); }
            }
        }
        /// <summary>
        /// The number of log entries removed when the log was last truncated.
        /// </summary>
        public int LastTruncatedCount { get; private set; } = 0;
        /// <summary>
        /// Gets the largest number of logs truncated at any one time.
        /// </summary>
        public int LargestTruncatedCount { get; private set; } = 0;
        /// <summary>
        /// Will truncate log entries from the beginning of the log file. 
        /// </summary>
        /// <returns>The number of logs removed.</returns>
        public int TruncateLogs()
        {
            if (Exists)
            {
                if (_LogsToRetain <= 0)
                {
                    LastTruncatedCount = LogCount;
                    Create();
                    LastTruncatedDate = DateTime.Now;
                    ++AutoTruncateGeneration;
                }
                else
                {
                    lock (SyncObject)
                    {
                        var fileInfo = new System.IO.FileInfo(LogFilename);
                        var totalLines = LogCount;
                        if (totalLines > _LogsToRetain)
                        {
                            int skipLines = totalLines - _LogsToRetain;
                            LastTruncatedCount = skipLines;
                            _TotalTruncatedLogs += LastTruncatedCount;
                            var tempFilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(LogFilename), Guid.NewGuid().ToString());
                            using (var sw = new System.IO.StreamWriter(tempFilename, false))
                            {
                                foreach (var entry in Read())
                                {
                                    if (--skipLines < 0)
                                    {
                                        if (entry != null) { sw.WriteLine(LoggingHelper.FormatLogLine(entry, _WriteLogEntriesUsingLocalTime)); }
                                    }
                                }
                            }
                            Delete();
                            System.IO.File.Move(tempFilename, LogFilename);
                        }
                        LastTruncatedDate = DateTime.Now;
                        ++AutoTruncateGeneration;
                    }
                }
            }
            if (LastTruncatedCount > LargestTruncatedCount) { LargestTruncatedCount = LastTruncatedCount; }
            return LastTruncatedCount;
        }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public override Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("Log");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // _LogFilename
                result.Items.Add("LogFilename", LogFilename, LogFilename.GetType());
                // _WriteLogEntriesUsingLocalTime
                result.Items.Add("WriteLogEntriesUsingLocalTime", _WriteLogEntriesUsingLocalTime, _WriteLogEntriesUsingLocalTime.GetType());
                // _AutoTruncateLogFile
                result.Items.Add("AutoTruncateLogFile", AutoTruncateEnabled, AutoTruncateEnabled.GetType());
                // _MaxLogSizeBytes
                result.Items.Add("MaxLogSizeBytes", _MaxLogSizeBytes, _MaxLogSizeBytes.GetType());
                // _LogsToRetain
                result.Items.Add("LogsToRetain", _LogsToRetain, _LogsToRetain.GetType());
                // 
                return result;
            }
        }
        #endregion
        #region ILog Methods
        /// <summary>
        /// Returns the unique id for this <see cref="ILog"/>.
        /// </summary>
        public override string Id => LogFilename;
        /// <summary>
        /// Gets a value indicating whether the log exists. <b>True</b> indicates it was found, <b>false</b> indicates it was <i>not</i> found.
        /// </summary>
        public override bool Exists
        {
            get
            {
                lock (SyncObject) { return System.IO.File.Exists(LogFilename); }
            }
        }
        /// <summary>
        /// Returns whether the log has been opened. <b>True</b> if the log is open, otherwise, <b>false</b>.
        /// </summary>
        public override bool IsOpen
        {
            get { return _IsOpen; }
        }
        /// <summary>
        /// Opens, or creates, the log file.
        /// </summary>
        public override void Open()
        {
            if (!Exists) { Create(); }
            _IsOpen = true;
        }
        /// <summary>
        /// Will close the log.
        /// </summary>
        public override void Close()
        {
            _IsOpen = false;
        }
        /// <summary>
        /// Will create an empty log.
        /// </summary>
        public override void Create()
        {
            if (Exists)
            {
                lock (SyncObject)
                {
                    System.IO.File.Delete(LogFilename);
                    dodSON.Core.Threading.ThreadingHelper.Sleep(250);
                    System.IO.File.Create(LogFilename).Close();
                }
            }
            else
            {
                lock (SyncObject)
                {
                    System.IO.File.Create(LogFilename).Close();
                }
            }
        }
        /// <summary>
        /// Will delete the log.
        /// </summary>
        /// <remarks>This will permanently delete all log entires; this action <i>cannot</i> be undone.</remarks>
        public override void Delete()
        {
            if (Exists)
            {
                lock (SyncObject)
                {
                    System.IO.File.Delete(LogFilename);
                }
            }
        }
        /// <summary>
        /// Will remove all log entires from the log.
        /// </summary>
        /// <remarks>
        /// This will permanently delete all log entires; this action <i>cannot</i> be undone.
        /// <para>This only clears the log, it will <i>not</i> delete it.</para>
        /// </remarks>
        /// <returns>The number of log entires removed.</returns>
        public override int Clear()
        {
            var count = LogCount;
            Create();
            return count;
        }
        /// <summary>
        /// Gets the number of log entires contained in the log.
        /// </summary>
        public override int LogCount
        {
            get
            {
                if (Exists)
                {
                    lock (SyncObject)
                    {
                        var count = 0;
                        using (var sr = new System.IO.StreamReader(LogFilename))
                        {
                            while (sr.ReadLine() != null) { ++count; }
                            sr.Close();
                        }
                        return count;
                    }
                }
                return 0;
            }
        }
        /// <summary>
        /// Get the current statistics for the log.
        /// </summary>
        public override ILogStatistics Statistics
        {
            get
            {
                var stats = Converters.ConvertersHelper.Clone(_LogStats);
                stats.LogCount = LogCount;
                return stats;
            }
        }
        /// <summary>
        /// Reads each entry from the log.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{ILogEntry}"/> used to iterate over each <see cref="ILogEntry"/> in the log.</returns>
        public override IEnumerable<ILogEntry> Read()
        {
            return Read(x => true);
        }
        /// <summary>
        /// Returns each log entry in the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate"></param>
        /// <returns>Every log entry from the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public override IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate)
        {
            if (Exists)
            {
                lock (SyncObject)
                {
                    using (var sr = new System.IO.StreamReader(LogFilename))
                    {
                        var line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            var entry = LoggingHelper.ParseLogLine(line);
                            if (logEntryFilterPredicate(entry)) { yield return entry; }
                        }
                        sr.Close();
                    }
                }
            }
        }
        /// <summary>
        /// Writes an <see cref="ILogEntry"/> to the log.
        /// </summary>
        /// <param name="logEntry">The entry to write.</param>
        public override void Write(ILogEntry logEntry)
        {
            if (logEntry == null) { throw new ArgumentNullException(nameof(logEntry)); }
            Write(new ILogEntry[] { logEntry });
        }
        /// <summary>
        /// Writes a group of <see cref="ILogEntry"/>s to the log.
        /// </summary>
        /// <param name="logEntries">The list of <see cref="ILogEntry"/>s to write.</param>
        public override void Write(IEnumerable<ILogEntry> logEntries)
        {
            if (logEntries == null) { throw new ArgumentNullException(nameof(logEntries)); }
            if (logEntries.FirstOrDefault() != default(ILogEntry))
            {
                lock (SyncObject)
                {
                    using (var sw = new System.IO.StreamWriter(LogFilename, true))
                    {
                        foreach (var entry in logEntries)
                        {
                            if (entry != null)
                            {
                                sw.WriteLine(LoggingHelper.FormatLogLine(entry, _WriteLogEntriesUsingLocalTime));
                                ++_LogStats.LogWrites;
                            }
                        }
                    }
                    if ((AutoTruncateEnabled) && ((new System.IO.FileInfo(LogFilename)).Length > _MaxLogSizeBytes))
                    {
                        TruncateLogs();
                    }
                }
            }
        }
        /// <summary>
        /// Writes all of the logs in the <paramref name="logs"/> to this log.
        /// </summary>
        /// <param name="logs">The source of the logs to write.</param>
        public override void Write(Logs logs)
        {
            if (logs == null) { throw new ArgumentNullException(nameof(logs)); }
            Write((IEnumerable<ILogEntry>)logs);
        }
        /// <summary>
        /// Writes the specified information to the log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        /// <param name="category">The category number for this log entry.</param>
        public override void Write(LogEntryType entryType, string sourceId, string message, int eventId, ushort category)
        {
            Write(new LogEntry(entryType, sourceId, message, eventId, category, DateTime.UtcNow));
        }
        /// <summary>
        /// Writes the specified information to the log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        public override void Write(LogEntryType entryType, string sourceId, string message, int eventId)
        {
            Write(new LogEntry(entryType, sourceId, message, eventId));
        }
        /// <summary>
        /// Writes the specified information to the log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        public override void Write(LogEntryType entryType, string sourceId, string message)
        {
            Write(new LogEntry(entryType, sourceId, message));
        }
        #endregion
    }
}

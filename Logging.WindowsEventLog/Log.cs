using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging.WindowsEventLog
{
    /// <summary>
    /// Provides a <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa385780(v=vs.85).aspx">Window's Event Log</a> implementation of the <see cref="ILog"/> interface. 
    /// This allows the creation and management of logs in the <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa385780(v=vs.85).aspx">Window's Event Log</a>.
    /// </summary>
    /// <remarks>Research <b>Windows Event Viewer</b> for more information.</remarks>
    public class Log
        : LogBase
    {
        #region Ctor
        private Log() : base() { }
        /// <summary>
        /// Initializes a new instance of the EventLog class. 
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="debugReplacementEntryType">The type of <see cref="EventLogEntryType"/> to use instead of <see cref="dodSON.Core.Logging.LogEntryType.Debug"/>. Windows Event Log does not support a debug entry type.</param>
        /// <param name="machineName">The name of the computer hosting the log.</param>
        /// <param name="sourceName">The source name, to be registered and used to write to the log.</param>
        public Log(string logName,
                   System.Diagnostics.EventLogEntryType debugReplacementEntryType,
                   string machineName,
                   string sourceName)
            : this()
        {
            if (string.IsNullOrWhiteSpace(logName))
            {
                throw new ArgumentNullException(nameof(logName));
            }
            if (string.IsNullOrWhiteSpace(machineName))
            {
                throw new ArgumentNullException(nameof(machineName));
            }
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                throw new ArgumentNullException(nameof(sourceName));
            }
            LogName = logName;
            DebugReplacementEntryType = debugReplacementEntryType;
            MachineName = machineName;
            SourceName = sourceName;
        }
        /// <summary>
        /// Initializes a new instance of the EventLog class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="debugReplacementEntryType">The type of <see cref="EventLogEntryType"/> to use instead of <see cref="dodSON.Core.Logging.LogEntryType.Debug"/>. Windows Event Log does not support a debug entry type.</param>
        /// <param name="machineName">The name of the computer hosting the log.</param>
        public Log(string logName,
                   System.Diagnostics.EventLogEntryType debugReplacementEntryType,
                   string machineName) : this(logName, debugReplacementEntryType, machineName, logName) { }
        /// <summary>
        /// Initializes a new instance of the EventLog class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="debugReplacementEntryType">The type of <see cref="EventLogEntryType"/> to use instead of <see cref="dodSON.Core.Logging.LogEntryType.Debug"/>. Windows Event Log does not support a debug entry type.</param>
        public Log(string logName,
                   System.Diagnostics.EventLogEntryType debugReplacementEntryType) : this(logName, debugReplacementEntryType, ".") { }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public Log(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "Log")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Log\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // LogName 
            LogName = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "LogName", typeof(string)).Value;
            // DebugReplacementEntryType
            DebugReplacementEntryType = (System.Diagnostics.EventLogEntryType)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "DebugReplacementEntryType", typeof(System.Diagnostics.EventLogEntryType)).Value;
            // MachineName
            MachineName = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "MachineName", typeof(string)).Value;
            // SourceName
            SourceName = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "SourceName", typeof(string)).Value;
        }
        #endregion
        #region Private Fields
        private bool _IsOpen = false;
        private System.Diagnostics.EventLog _EventLog = null;
        private ILogStatistics _LogStats = new LogStatistics();
        private readonly object _SyncObject = new object();
        #endregion
        #region ILog Methods
        /// <summary>
        /// Returns the unique id for this <see cref="ILog"/>.
        /// </summary>
        public override string Id => $"{MachineName}, {LogName}";
        /// <summary>
        /// Gets a value indicating whether the log exists. <b>True</b> indicates it was found, <b>false</b> indicates it was <i>not</i> found.
        /// </summary>
        public override bool Exists => System.Diagnostics.EventLog.SourceExists(SourceName);
        /// <summary>
        /// Returns whether the log has been opened. <b>True</b> if the log is open, otherwise, <b>false</b>.
        /// </summary>
        public override bool IsOpen => _IsOpen;
        /// <summary>
        /// Opens, or creates, a log in the Window Event Log.
        /// </summary>
        /// <remarks>
        /// It is important to call the <see cref="Close"/> method; it will dispose of the underlying <see cref="System.Diagnostics.EventLog"/>.
        /// </remarks>
        public override void Open()
        {
            Create();
            if (_EventLog == null)
            {
                _EventLog = new System.Diagnostics.EventLog(LogName, MachineName, SourceName);
            }
            _IsOpen = true;
        }
        /// <summary>
        /// Closes the log. 
        /// </summary>
        /// <remarks>
        /// This will dispose of the underlying <see cref="System.Diagnostics.EventLog"/>. After calling this method, <see cref="Open"/> must be called before attempting to read or write to the log.
        /// </remarks>
        public override void Close()
        {
            if (_EventLog != null)
            {
                _EventLog.Dispose();
                _EventLog = null;
            }
            _IsOpen = false;
        }
        /// <summary>
        /// Will create an empty log.
        /// </summary>
        public override void Create()
        {
            if (!Exists)
            {
                System.Diagnostics.EventLog.CreateEventSource(SourceName, LogName);
                WaitUntilSourceExists(SourceName);
            }
        }
        /// <summary>
        /// Will delete the log.
        /// </summary>
        /// <remarks>This will permanently delete all messages; this action <i>cannot</i> be undone.</remarks>
        public override void Delete()
        {
            if (Exists)
            {
                System.Diagnostics.EventLog.Delete(LogName, MachineName);
            }
        }
        /// <summary>
        /// Will remove all messages from the log.
        /// </summary>
        /// <remarks>
        /// This will permanently delete all messages; this action <i>cannot</i> be undone.
        /// <para>This only clears the log, it will <i>not</i> delete it.</para>
        /// </remarks>
        /// <returns>The number of log entries removed.</returns>
        public override int Clear()
        {
            if (Exists)
            {
                var count = InternalEventLog.Entries.Count;
                InternalEventLog.Clear();
                return count;
            }
            return 0;
        }
        /// <summary>
        /// Gets the number of messages contained in the log.
        /// </summary>
        public override int LogCount
        {
            get
            {
                if (Exists)
                {
                    return InternalEventLog.Entries.Count;
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
        public override IEnumerable<ILogEntry> Read() => Read((e) => { return true; });
        /// <summary>
        /// Returns each log entry in the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate"></param>
        /// <returns>Every log entry from the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public override IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate)
        {
            if (Exists)
            {
                foreach (System.Diagnostics.EventLogEntry eventLogEntry in InternalEventLog.Entries)
                {
                    if (SeperateSourceIdAndMessage(eventLogEntry.Message, out string sourceID, out string message))
                    {
                        var logEntry = new LogEntry(ConvertToLogEntryType(eventLogEntry.EntryType),
                                                    sourceID,
                                                    message,
                                                    (int)eventLogEntry.InstanceId,
                                                    (ushort)eventLogEntry.CategoryNumber,
                                                    eventLogEntry.TimeGenerated.ToUniversalTime());
                        if (logEntryFilterPredicate(logEntry))
                        {
                            yield return logEntry;
                        }
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
            if (logEntry == null)
            {
                throw new ArgumentNullException(nameof(logEntry));
            }
            Create();
            InternalEventLog.WriteEntry(CombineSourceIdAndMessage(logEntry),
                                        ConvertFromLogEntryType(logEntry.EntryType),
                                        logEntry.EventId,
                                        (short)logEntry.Category);
            ++_LogStats.LogWrites;
        }
        /// <summary>
        /// Writes a group of <see cref="ILogEntry"/>s to the log.
        /// </summary>
        /// <param name="logEntries">The list of <see cref="ILogEntry"/>s to write.</param>
        public override void Write(IEnumerable<ILogEntry> logEntries)
        {
            if (logEntries == null)
            {
                throw new ArgumentNullException(nameof(logEntries));
            }
            foreach (var logEntry in logEntries)
            {
                Write(logEntry);
            }
        }
        /// <summary>
        /// Writes all of the logs in the <paramref name="logs"/> to this log.
        /// </summary>
        /// <param name="logs">The source of the logs to write.</param>
        public override void Write(Logs logs)
        {
            if (logs == null)
            {
                throw new ArgumentNullException(nameof(logs));
            }
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
        public override void Write(LogEntryType entryType, string sourceId, string message, int eventId, ushort category) => Write(new LogEntry(entryType, sourceId, message, eventId, category, DateTime.UtcNow));
        /// <summary>
        /// Writes the specified information to the log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        public override void Write(LogEntryType entryType, string sourceId, string message, int eventId) => Write(new LogEntry(entryType, sourceId, message, eventId));
        /// <summary>
        /// Writes the specified information to the log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        public override void Write(LogEntryType entryType, string sourceId, string message) => Write(new LogEntry(entryType, sourceId, message));
        #endregion
        #region Public Methods

        // TODO: enhance and add public methods to do things that the Windows Event Log will let you do.

        /// <summary>
        /// The name of the log.
        /// </summary>
        public string LogName { get; } = "";
        /// <summary>
        /// The type of <see cref="EventLogEntryType"/> to use instead of <see cref="dodSON.Core.Logging.LogEntryType.Debug"/>. Windows Event Log does not support a debug entry type.
        /// </summary>
        public EventLogEntryType DebugReplacementEntryType
        {
            get;
        }
        /// <summary>
        /// The name of the computer hosting the log.
        /// </summary>
        public string MachineName { get; } = "";
        /// <summary>
        /// The source name, to be registered and used to write to the log.
        /// </summary>
        public string SourceName { get; } = "";
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
                // _LogName
                result.Items.Add("LogName", LogName, LogName.GetType());
                // _DebugReplacementEntryType
                result.Items.Add("DebugReplacementEntryType", DebugReplacementEntryType, DebugReplacementEntryType.GetType());
                // _MachineName
                result.Items.Add("MachineName", MachineName, MachineName.GetType());
                // _SourceName
                result.Items.Add("SourceName", SourceName, SourceName.GetType());
                // 
                return result;
            }
        }
        #endregion
        #region Private Methods
        private System.Diagnostics.EventLog InternalEventLog
        {
            get
            {
                if (_EventLog == null)
                {
                    _EventLog = new System.Diagnostics.EventLog(LogName, MachineName, SourceName);
                }
                return _EventLog;
            }
        }
        private void WaitUntilSourceExists(string sourceName)
        {
            // wait up to 5 seconds for the sourceName log to become existent
            var counter = 50;
            while (true)
            {
                dodSON.Core.Threading.ThreadingHelper.Sleep(100);
                if (System.Diagnostics.EventLog.SourceExists(sourceName))
                {
                    break;
                }
                if (--counter <= 0)
                {
                    break;
                }
            }
        }
        private System.Diagnostics.EventLogEntryType ConvertFromLogEntryType(LogEntryType entryType)
        {
            switch (entryType)
            {
                case LogEntryType.Debug:
                    return DebugReplacementEntryType;
                case LogEntryType.Information:
                    return System.Diagnostics.EventLogEntryType.Information;
                case LogEntryType.Warning:
                    return System.Diagnostics.EventLogEntryType.Warning;
                case LogEntryType.Error:
                    return System.Diagnostics.EventLogEntryType.Error;
                case LogEntryType.AuditFailure:
                    return System.Diagnostics.EventLogEntryType.FailureAudit;
                case LogEntryType.AuditSuccess:
                    return System.Diagnostics.EventLogEntryType.SuccessAudit;
                default:
                    return System.Diagnostics.EventLogEntryType.Information;
            }
        }
        private LogEntryType ConvertToLogEntryType(EventLogEntryType entryType)
        {
            switch (entryType)
            {
                case EventLogEntryType.Error:
                    return LogEntryType.Error;
                case EventLogEntryType.Warning:
                    return LogEntryType.Warning;
                case EventLogEntryType.Information:
                    return LogEntryType.Information;
                case EventLogEntryType.SuccessAudit:
                    return LogEntryType.AuditSuccess;
                case EventLogEntryType.FailureAudit:
                    return LogEntryType.AuditFailure;
                default:
                    return LogEntryType.Information;
            }
        }
        private string CombineSourceIdAndMessage(ILogEntry logEntry) => $"[{logEntry.SourceId}]{Environment.NewLine}{logEntry.Message}";
        private bool SeperateSourceIdAndMessage(string value, out string sourceId, out string message)
        {
            value = value.Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.StartsWith("[") && value.Contains("]"))
                {
                    var startIndex = value.IndexOf("[") + 1;
                    var stopIndex = value.IndexOf("]");
                    sourceId = value.Substring(startIndex, stopIndex - startIndex);
                    message = value.Substring(stopIndex + 3);
                    return true;
                }
            }
            sourceId = "";
            message = "";
            return false;
        }
        #endregion
    }
}

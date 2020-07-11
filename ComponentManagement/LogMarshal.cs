using System;
using System.Collections.Generic;
using System.Linq;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// A configurable, memory-cached, archive-enabled logger derived from <see cref="MarshalByRefObject"/>.
    /// </summary>
    /// <seealso cref="MarshalByRefObject"/>
    /// <seealso cref="LogController"/>
    /// <seealso cref="Logging"/>
    /// <remarks>
    /// Being derived from <see cref="MarshalByRefObject"/>, allows this <see cref="Logging.ILogReaderWriter"/> to be passed from one <see cref="System.AppDomain"/> to another, yet still reference the singular instance.
    /// <br/>
    /// The logger is capable of using any <see cref="Logging.LogBase"/>-derived logger to read and write logs.
    /// <br/>
    /// The logger maintains an in-memory cache, writing logs in bursts when size or time-limits dictate.
    /// <br/>
    /// The logger will automatically archive logs when size limits have been reached. 
    /// When archiving, the logger will move a defined number of logs from the <see cref="Logging.LogBase"/>-derived logger to a <see cref="Logging.FileEventLog.Log"/>-generated text file stored in a zip file.
    /// </remarks>
    [Serializable()]
    public class LogMarshal
            : MarshalByRefObject,
              Logging.ILogReaderWriter
    {
        #region Ctor
        private LogMarshal() : base() { }
        /// <summary>
        /// Initializes a new instance of the LogMarshal using the specified <see cref="Logging.ILog"/> to perform the actual logging.
        /// </summary>
        /// <param name="log">The hosted <see cref="Logging.ILog"/>.</param>
        /// <param name="logSettings">The log settings.</param>
        public LogMarshal(Logging.ILog log,
                          LogControllerSettings logSettings)
            : this()
        {
            _ActualLog = log ?? throw new ArgumentNullException(nameof(log));
            _LogSettings = logSettings ?? throw new ArgumentNullException(nameof(logSettings));
        }
        #endregion
        #region Private Fields
        private Logging.ILog _ActualLog;
        private LogControllerSettings _LogSettings;
        //
        private readonly Converters.ITypeSerializer<Logging.ILogEntry> _Converter = new Converters.TypeSerializer<Logging.ILogEntry>();
        private readonly List<Logging.ILogEntry> _Cache = new List<Logging.ILogEntry>();
        #endregion
        #region Public Methods
        /// <summary>
        /// The <see cref="Type.AssemblyQualifiedName"/> of the provided <see cref="Logging.LogBase"/>-derived logger.
        /// </summary>
        public string InnerLogFullAssemblyName => _ActualLog.GetType().AssemblyQualifiedName;
        /// <summary>
        /// Returns an object that can be used for synchronizing access across multiple threads.
        /// </summary>
        public object CacheSyncObject { get; } = new object();
        /// <summary>
        /// Opens an existing log, or creates a new one if log not found.
        /// </summary>
        public void Open()
        {
            lock (CacheSyncObject)
            {
                if (!_ActualLog.IsOpen)
                {
                    _ActualLog.Open();
                }
            }
        }
        /// <summary>
        /// Closes the log.
        /// </summary>
        public void Close()
        {
            if (_ActualLog.IsOpen)
            {
                lock (CacheSyncObject)
                {
                    Flush();
                    _ActualLog.Close();
                }
            }
        }
        /// <summary>
        /// Will remove all log entries from the log.
        /// </summary>
        /// <remarks>
        /// This will permanently delete all log entries from the log; this action <i>cannot</i> be undone.
        /// <para>This only clears the log, it will <i>not</i> delete it.</para>
        /// </remarks>
        public void Clear()
        {
            if (_ActualLog.IsOpen)
            {
                lock (CacheSyncObject)
                {
                    _Cache.Clear();
                    _ActualLog.Clear();
                }
            }
        }
        /// <summary>
        /// The number of log entires received.
        /// </summary>
        public int IncomingCount { get; private set; } = 0;
        /// <summary>
        /// Gets the number of log entries in the cache. These are log entries, being held in memory, that have not been written to the actual log, yet.
        /// </summary>
        public int CachedCount => _Cache.Count;
        /// <summary>
        /// Gets the number of log entries contained in the log plus the number of cached log entries. (Logged Entries + Cached Entries)
        /// </summary>
        public int TotalCount
        {
            get
            {
                lock (CacheSyncObject)
                {
                    return LogCount + CachedCount;
                }
            }
        }
        /// <summary>
        /// This will write all log entries in the cache to the hosted event log, clearing all cached log entries.
        /// </summary>
        /// <returns>The number of log entries written to the log.</returns>
        public int Flush()
        {
            lock (CacheSyncObject)
            {
                if (_Cache.Count > 0)
                {
                    LastFlushCount = _Cache.Count;
                    if (LastFlushCount > LargestFlushCount)
                    {
                        LargestFlushCount = LastFlushCount;
                    }
                    _ActualLog.Write(_Cache);
                    _Cache.Clear();
                    ++FlushGeneration;
                    LastFlushDate = DateTime.Now;
                    return LastFlushCount;
                }
            }
            return 0;
        }
        /// <summary>
        /// The largest number of log entries written to the log during a cache flush.
        /// </summary>
        public int LargestFlushCount { get; private set; } = 0;
        /// <summary>
        /// The date when the cache was last flushed.
        /// </summary>
        public DateTime LastFlushDate { get; private set; } = DateTime.MinValue;
        /// <summary>
        /// The number of cache flushes have been executed.
        /// </summary>
        public int FlushGeneration { get; private set; } = 0;
        /// <summary>
        /// The number of log entries written to the log during the last cache flush.
        /// </summary>
        public int LastFlushCount { get; private set; } = 0;
        #endregion
        #region Logging.ILogWriter Methods
        /// <summary>
        /// Returns the unique id for this <see cref="Logging.ILog"/>.
        /// </summary>
        public string Id => _ActualLog.Id;
        /// <summary>
        /// Returns an object that can be used for synchronizing access to the underlying <see cref="Logging.LogBase"/>-derived logger across multiple threads.
        /// </summary>
        public object SyncObject => _ActualLog.SyncObject;
        /// <summary>
        /// Gets a value indicating whether the log exists. <b>True</b> indicates it was found, <b>false</b> indicates it was <i>not</i> found.
        /// </summary>
        public bool Exists => _ActualLog.Exists;
        /// <summary>
        /// Returns whether the log has been opened. <b>True</b> if the log is open, otherwise, <b>false</b>.
        /// </summary>
        public bool IsOpen => _ActualLog.IsOpen;
        /// <summary>
        /// Gets the number of log entries contained in the log.
        /// </summary>
        public int LogCount => _ActualLog.LogCount;
        /// <summary>
        /// Get the current statistics for the log.
        /// </summary>
        public Logging.ILogStatistics Statistics
        {
            get
            {
                var stats = _ActualLog.Statistics;
                stats.AutoFlushMaximumLogs = _LogSettings.AutoFlushMaximumLogs;
                stats.AutoFlushTimeLimit = _LogSettings.AutoFlushTimeLimit;
                return stats;
            }
        }
        /// <summary>
        /// Reads each log entry from the log.
        /// </summary>
        /// <returns>Returns all log entries in the log.</returns>
        public IEnumerable<Logging.ILogEntry> Read() => Read((e) => { return true; });
        /// <summary>
        /// Returns each log entry in the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate"></param>
        /// <returns>Every log entry from the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public IEnumerable<Logging.ILogEntry> Read(Func<Logging.ILogEntry, bool> logEntryFilterPredicate)
        {
            lock (CacheSyncObject)
            {
                Flush();
                return _ActualLog.Read(logEntryFilterPredicate);
            }
        }
        /// <summary>
        /// Writes a log entry to the log.
        /// </summary>
        /// <param name="logEntry">The log entry to write to this log.</param>
        public void Write(Logging.ILogEntry logEntry)
        {
            if (logEntry == null)
            {
                throw new ArgumentNullException(nameof(logEntry));
            }
            Write(new Logging.ILogEntry[] { logEntry });
        }
        /// <summary>
        /// Writes a group of log entries to the log.
        /// </summary>
        /// <param name="logEntries">The log entries to write to this log.</param>
        public void Write(IEnumerable<Logging.ILogEntry> logEntries)
        {
            if (logEntries == null)
            {
                throw new ArgumentNullException(nameof(logEntries));
            }
            lock (CacheSyncObject)
            {
                _Cache.AddRange(logEntries);
                IncomingCount += logEntries.Count();
            }
        }
        /// <summary>
        /// Writes all of the logs in the <paramref name="logs"/> to this log.
        /// </summary>
        /// <param name="logs">The source of the logs to write.</param>
        public void Write(Logging.Logs logs)
        {
            if (logs == null)
            {
                throw new ArgumentNullException(nameof(logs));
            }
            Write((IEnumerable<Logging.ILogEntry>)logs);
        }
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        /// <param name="category">The category number for this log entry.</param>
        public void Write(Logging.LogEntryType entryType, string sourceId, string message, int eventId, ushort category) => Write(new Logging.ILogEntry[] { new Logging.LogEntry(entryType, sourceId, message, eventId, category) });
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        public void Write(Logging.LogEntryType entryType, string sourceId, string message, int eventId) => Write(new Logging.ILogEntry[] { new Logging.LogEntry(entryType, sourceId, message, eventId) });
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        public void Write(Logging.LogEntryType entryType, string sourceId, string message) => Write(new Logging.ILogEntry[] { new Logging.LogEntry(entryType, sourceId, message) });
        #endregion
        #region Override Methods
        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// Returns <b>null</b> to keep this instance alive forever.
        /// <para/>I believe that returning null here is alright because the <see cref="ComponentManager"/> controls the lifetime of the <see cref="LogMarshal"/>; and as long as the <see cref="ComponentManager"/> is alive, so too should the <see cref="LogMarshal"/>.
        /// </returns>
        public override object InitializeLifetimeService() => null;
        #endregion
    }
}

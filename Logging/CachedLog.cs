using System;
using System.Collections.Generic;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Provides in-memory <see cref="ILogEntry"/> caching to any <see cref="LogBase"/> type.
    /// </summary>
    public class CachedLog
          : ICachedLog
    {
        #region Private Constants
        private readonly long _MinimumAutoFlushLogCount_ = 8;
        #endregion
        #region Ctor
        private CachedLog()
        {
        }
        /// <summary>
        /// Initializes a new instance of the CachedEventLog using the specified <see cref="ILog"/> as the log entries source.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> used to read and write log entries.</param>
        /// <param name="autoFlushLogs">Determines if the cache should be automatically flushed.</param>
        /// <param name="flushMaximumLogs">The maximum number of log entires to cache before writing them to the <paramref name="log"/>.</param>
        /// <param name="flushTimeLimit">The maximum amount of time to wait before writing the cached log entries to the <paramref name="log"/>.</param>
        public CachedLog(ILog log,
                         bool autoFlushLogs,
                         int flushMaximumLogs,
                         TimeSpan flushTimeLimit)
            : this()
        {
            AutoFlushEnabled = autoFlushLogs;
            if (flushMaximumLogs < _MinimumAutoFlushLogCount_)
            {
                throw new ArgumentOutOfRangeException(nameof(flushMaximumLogs), $"Parameter {nameof(flushMaximumLogs)} must be greater than {_MinimumAutoFlushLogCount_}.");
            }
            if (flushTimeLimit <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(flushTimeLimit), $"Parameter {nameof(flushTimeLimit)} must be greater than {TimeSpan.Zero}.");
            }
            InnerLog = log ?? throw new ArgumentNullException(nameof(log));
            _FlushMaximumLogs = flushMaximumLogs;
            _FlushTimeLimit = flushTimeLimit;
            if (!log.IsOpen)
            {
                log.Open();
            }
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public CachedLog(Configuration.IConfigurationGroup configuration)
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
            // Log 
            if (!configuration.ContainsKey("Log"))
            {
                throw new ArgumentException($"Configuration missing subgroup. Configuration must have subgroup: \"Log\".", nameof(configuration));
            }
            InnerLog = (ILog)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(null, configuration["Log"]);
            InnerLog.Open();
            // AutoFlushLogs
            AutoFlushEnabled = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "AutoFlushLogs", typeof(bool)).Value;
            // FlushMaximumLogs
            FlushMaximumLogs = (int)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "FlushMaximumLogs", typeof(int)).Value;
            // FlushTimeLimit
            FlushTimeLimit = (TimeSpan)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "FlushTimeLimit", typeof(TimeSpan)).Value;
        }
        #endregion
        #region Private Fields
        private int _FlushMaximumLogs = 16;
        private TimeSpan _FlushTimeLimit = TimeSpan.FromMinutes(5);
        //
        private readonly DateTime _StartedDate = DateTime.Now;
        private readonly List<ILogEntry> _MessageCache = new List<ILogEntry>();
        private readonly ILogStatistics _LogStats = new LogStatistics();
        private DateTime _LastDateCacheFlushed = DateTime.MinValue;
        #endregion
        #region ICachedLog Methods
        /// <summary>
        /// Returns the unique id for this <see cref="ILog"/>.
        /// </summary>
        public string Id => InnerLog.Id;
        /// <summary>
        /// Returns an object that can be used for synchronizing thread access to the underlying logging system.
        /// </summary>
        public object SyncObject
        {
            get
            {
                return InnerLog.SyncObject;
            }
        }
        /// <summary>
        /// The actual <see cref="ILog"/> used to read and write log entries. (ohmmm...my inner log.)
        /// </summary>
        public ILog InnerLog { get; } = null;
        /// <summary>
        /// Gets a value indicating whether the log exists. <b>True</b> indicates it was found, <b>false</b> indicates it was <i>not</i> found.
        /// </summary>
        public bool Exists
        {
            get
            {
                return InnerLog.Exists;
            }
        }
        /// <summary>
        /// Returns whether the log has been opened. <b>True</b> if the log is open, otherwise, <b>false</b>.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return InnerLog.IsOpen;
            }
        }
        /// <summary>
        /// Opens, or creates, the log.
        /// </summary>
        public void Open()
        {
            InnerLog.Open();
        }
        /// <summary>
        /// Closes the log.
        /// </summary>
        public void Close()
        {
            lock (InnerLog.SyncObject)
            {
                FlushLogs();
                InnerLog.Close();
            }
        }
        /// <summary>
        /// Will create an empty log.
        /// </summary>
        public void Create()
        {
            lock (InnerLog.SyncObject)
            {
                InnerLog.Create();
                _MessageCache.Clear();
            }
        }
        /// <summary>
        /// Will delete the log.
        /// </summary>
        /// <remarks>This will permanently delete all log entries; this action <i>cannot</i> be undone.</remarks>
        public void Delete()
        {
            lock (InnerLog.SyncObject)
            {
                InnerLog.Delete();
                _MessageCache.Clear();
            }
        }
        /// <summary>
        /// Will remove all log entries from the log.
        /// </summary>
        /// <remarks>
        /// This will permanently delete all log entries; this action <i>cannot</i> be undone.
        /// <para>This only clears the log, it will <i>not</i> delete it.</para>
        /// </remarks>
        /// <returns>The number of log entires removed.</returns>
        public int Clear()
        {
            lock (InnerLog.SyncObject)
            {
                var count = InnerLog.LogCount + _MessageCache.Count;
                InnerLog.Clear();
                _MessageCache.Clear();
                return count;
            }
        }
        /// <summary>
        /// Gets the number of log entries contained in the log.
        /// </summary>
        public int LogCount
        {
            get
            {
                return InnerLog.LogCount;
            }
        }
        /// <summary>
        /// Statistical information about the logging system.
        /// </summary>
        public ILogStatistics Statistics
        {
            get
            {
                lock (InnerLog.SyncObject)
                {
                    // get underlying Log Statistics
                    var stats = InnerLog.Statistics;
                    // populate Cache Log Statistics
                    stats.AutoFlushEnabled = AutoFlushEnabled;
                    stats.FlushGeneration = FlushGeneration;
                    stats.LastFlushDate = LastFlushDate;
                    stats.LastFlushCount = LastFlushCount;
                    stats.AutoFlushTimeLimit = _FlushTimeLimit;
                    stats.AutoFlushMaximumLogs = _FlushMaximumLogs;
                    stats.LargestFlushCount = LargestFlushCount;
                    stats.IncomingLogCount = IncomingLogCount;
                    stats.CachedLogCount = CachedLogCount;
                    stats.TotalLogCount = TotalLogCount;

                    if (_LastDateCacheFlushed == DateTime.MinValue)
                    {
                        stats.AutoFlushTimeRemaining = _FlushTimeLimit - (DateTime.Now - _StartedDate);
                    }
                    else
                    {
                        stats.AutoFlushTimeRemaining = _FlushTimeLimit - (DateTime.Now - _LastDateCacheFlushed);
                    }
                    return stats;
                }
            }
        }
        /// <summary>
        /// Gets the number of log entries in the cache. These are log entries, being held in memory, that have not been written to the actual log, yet.
        /// </summary>
        public int CachedLogCount
        {
            get
            {
                lock (InnerLog.SyncObject)
                {
                    return _MessageCache.Count;
                }
            }
        }
        /// <summary>
        /// Gets the number of log entries contained in the log plus the number of cached log entries. (Logged Entries + Cached Entries)
        /// </summary>
        public int TotalLogCount
        {
            get
            {
                lock (InnerLog.SyncObject)
                {
                    return InnerLog.LogCount + _MessageCache.Count;
                }
            }
        }
        /// <summary>
        /// Returns whether the cache has meet the requirements to flush.
        /// </summary>
        public bool IsFlushable
        {
            get
            {
                lock (InnerLog.SyncObject)
                {
                    // empty cache
                    if (_MessageCache.Count == 0)
                    {
                        return false;
                    }
                    // maxed out cache
                    if (_MessageCache.Count > _FlushMaximumLogs)
                    {
                        return true;
                    }
                    // timed out cache
                    if (_LastDateCacheFlushed == DateTime.MinValue)
                    {
                        return ((DateTime.Now - _StartedDate) > _FlushTimeLimit);
                    }
                    else
                    {
                        return ((DateTime.Now - _LastDateCacheFlushed) > _FlushTimeLimit);
                    }
                }
            }
        }
        /// <summary>
        /// This will write all log entries in the cache to the hosted event log, clearing all cached log entries.
        /// </summary>
        public void FlushLogs()
        {
            lock (InnerLog.SyncObject)
            {
                if (_MessageCache.Count > 0)
                {
                    if (_MessageCache.Count > LargestFlushCount)
                    {
                        LargestFlushCount = _MessageCache.Count;
                    }
                    InnerLog.Write(_MessageCache);
                    LastFlushCount = _MessageCache.Count;
                    _MessageCache.Clear();
                    _LastDateCacheFlushed = DateTime.Now;
                    ++FlushGeneration;
                }
            }
        }
        /// <summary>
        /// The date when the cache was last flushed.
        /// </summary>        
        public DateTime LastFlushDate
        {
            get
            {
                if (_LastDateCacheFlushed == DateTime.MinValue)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return _LastDateCacheFlushed;
                }
            }
        }
        /// <summary>
        /// The interval since the cache was last flushed.
        /// </summary>
        public TimeSpan LastFlushed
        {
            get
            {
                if (_LastDateCacheFlushed == DateTime.MinValue)
                {
                    return TimeSpan.Zero;
                }
                else
                {
                    return (DateTime.Now - _LastDateCacheFlushed);
                }
            }
        }
        /// <summary>
        /// The number of cache flushes have been executed.
        /// </summary>
        public int FlushGeneration { get; private set; } = 0;
        /// <summary>
        /// The number of log entries written to the log during the last cache flush.
        /// </summary>
        public int LastFlushCount { get; private set; } = 0;
        /// <summary>
        /// The largest number of log entries written to the log during a cache flush.
        /// </summary>
        public int LargestFlushCount { get; private set; } = 0;
        /// <summary>
        /// The number of log entires received.
        /// </summary>
        public int IncomingLogCount { get; private set; } = 0;
        /// <summary>
        /// The maximum number of log entries in the log before the auto-flush is engaged.
        /// </summary>
        public int FlushMaximumLogs
        {
            get
            {
                return _FlushMaximumLogs;
            }
            set
            {
                if (value < _MinimumAutoFlushLogCount_)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"Parameter {nameof(value)} must be greater than {_MinimumAutoFlushLogCount_}.");
                }
                _FlushMaximumLogs = value;
            }
        }
        /// <summary>
        /// The maximum amount of time allowed to pass before the auto-flush is engaged. 
        /// </summary>
        public TimeSpan FlushTimeLimit
        {
            get
            {
                return _FlushTimeLimit;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"Parameter {nameof(value)} must be greater than {TimeSpan.Zero}.");
                }
                _FlushTimeLimit = value;
            }
        }
        /// <summary>
        /// Gets, or sets, a value indicating whether auto-cache flushing is on or off.
        /// <b>True</b> will cause the cache to be flushed, if the cache is flushable, before writing a <see cref="ILogEntry"/> to the cache; otherwise, <b>false</b> will not automatically flush the cache.
        /// </summary>
        public bool AutoFlushEnabled { get; set; } = true;
        /// <summary>
        /// Reads each log entry from the log.
        /// </summary>
        /// <remarks>
        /// This will execute <see cref="FlushLogs"/> before opening the log.</remarks>
        /// <returns>Returns all log entries in the log.</returns>
        public IEnumerable<ILogEntry> Read()
        {
            return Read((e) => { return true; });
        }
        /// <summary>
        /// Returns each log entry in the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <remarks>This will execute <see cref="FlushLogs"/> before opening the log.</remarks>
        /// <param name="logEntryFilterPredicate">A function which takes an <see cref="ILogEntry"/> and returns <b>true</b> to include the log entry, or <b>false></b> to skip the log entry.</param>
        /// <returns>Every log entry from the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate)
        {
            lock (InnerLog.SyncObject)
            {
                FlushLogs();
                return InnerLog.Read(logEntryFilterPredicate);
            }
        }
        /// <summary>
        /// Writes an <see cref="ILogEntry"/> to the log.
        /// </summary>
        /// <param name="logEntry">The entry to write.</param>
        public void Write(ILogEntry logEntry)
        {
            if (logEntry == null)
            {
                throw new ArgumentNullException(nameof(logEntry));
            }
            Write(new ILogEntry[] { logEntry });
        }
        /// <summary>
        /// Writes a group of <see cref="ILogEntry"/>s to the log.
        /// </summary>
        /// <param name="logEntries">The list of <see cref="ILogEntry"/>s to write.</param>
        public void Write(IEnumerable<ILogEntry> logEntries)
        {
            if (logEntries == null)
            {
                throw new ArgumentNullException(nameof(logEntries));
            }
            lock (InnerLog.SyncObject)
            {
                foreach (var logEntry in logEntries)
                {
                    _MessageCache.Add(logEntry);
                    ++IncomingLogCount;
                    if (AutoFlushEnabled && IsFlushable)
                    {
                        FlushLogs();
                    }
                }
            }
        }
        /// <summary>
        /// Writes all of the logs in the <paramref name="logs"/> to this log.
        /// </summary>
        /// <param name="logs">The source of the logs to write.</param>
        public void Write(Logs logs)
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
        public void Write(LogEntryType entryType, string sourceId, string message, int eventId, ushort category)
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
        public void Write(LogEntryType entryType, string sourceId, string message, int eventId)
        {
            Write(new LogEntry(entryType, sourceId, message, eventId, 0, DateTime.UtcNow));
        }
        /// <summary>
        /// Writes the specified information to the log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        public void Write(LogEntryType entryType, string sourceId, string message)
        {
            Write(new LogEntry(entryType, sourceId, message, 0, 0, DateTime.UtcNow));
        }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("Log");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // _Log
                result.Add(InnerLog.Configuration);
                // _AutoFlushLogs
                result.Items.Add("AutoFlushLogs", AutoFlushEnabled, AutoFlushEnabled.GetType());
                // _FlushMaximumLogs
                result.Items.Add("FlushMaximumLogs", _FlushMaximumLogs, _FlushMaximumLogs.GetType());
                // _FlushTimeLimit
                result.Items.Add("FlushTimeLimit", _FlushTimeLimit, _FlushTimeLimit.GetType());
                // 
                return result;
            }
        }
        #endregion
    }
}

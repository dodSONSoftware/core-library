using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Provides access to and control of a configurable, single-entry, memory-cached, archive-enabled logging mechanism.
    /// </summary>
    /// <seealso cref="LogMarshal"/>
    /// <seealso cref="MarshalByRefObject"/>
    public class LogController
        : ILogController
    {
        #region Private Constants
        private static readonly TimeSpan _LogControllerThreadWorkerFrequency_ = TimeSpan.FromMilliseconds(100);
        #endregion
        #region Ctor
        private LogController()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="LogController"/>.
        /// </summary>
        /// <param name="log">The <see cref="Logging.ILog"/>-based logger used to write the actual logs.</param>
        /// <param name="logArchiveFileStore">A <see cref="FileStorage.IFileStore"/> when log archives will be created.</param>
        /// <param name="archiveFilenameFactory">A <see cref="IArchiveFilenameFactory"/> used to create archive log file names.</param>
        /// <param name="logControllerSettings">Settings to control the actions of the <see cref="ILogController"/>.</param>
        public LogController(Logging.ILog log,
                             FileStorage.IFileStore logArchiveFileStore,
                             IArchiveFilenameFactory archiveFilenameFactory,
                             LogControllerSettings logControllerSettings)
            : this()
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            // **** turn OFF auto-truncation if it is the known interface:{Logging.ITruncatable}
            if (log is Logging.ITruncatable)
            {
                (log as Logging.ITruncatable).AutoTruncateEnabled = false;
            }
            //
            _Log = log;
            Settings = logControllerSettings ?? throw new ArgumentNullException(nameof(logControllerSettings));
            _LogArchiveFileStore = logArchiveFileStore ?? throw new ArgumentNullException(nameof(logArchiveFileStore));
            _ArchiveFilenameFactory = archiveFilenameFactory ?? throw new ArgumentNullException(nameof(archiveFilenameFactory));
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public LogController(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "LogController")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"LogController\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // LogControllerSettings
            var logControllerSettingsConfig = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "LogControllerSettings", true);
            Settings = (LogControllerSettings)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(logControllerSettingsConfig);
            // Log
            var logConfig = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "Log", true);
            // **** add/update "WriteLogEntriesUsingLocalTime" to reflect the value in Settings
            logConfig.Items.Add("WriteLogEntriesUsingLocalTime", Settings.WritePrimaryLogEntriesUsingLocalTime, Settings.WritePrimaryLogEntriesUsingLocalTime.GetType());
            _Log = (Logging.ILog)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(logConfig);
            // **** turn OFF auto-truncation if it is the known interface:{Logging.ITruncatable}
            if (_Log is Logging.ITruncatable)
            {
                (_Log as Logging.ITruncatable).AutoTruncateEnabled = false;
            }
            // LogArchiveFileStore
            var fileStoreConfig = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "FileStore", true);
            if ((!fileStoreConfig.Items.ContainsKey("ExtractionRootDirectory")) ||
                (fileStoreConfig.Items["ExtractionRootDirectory"].Value.ToString() == ""))
            {
                fileStoreConfig.Items.Add("ExtractionRootDirectory", System.IO.Path.GetTempPath(), typeof(string));
            }
            _LogArchiveFileStore = (FileStorage.IFileStore)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(fileStoreConfig);
            // ArchiveFilenameFactory
            var archiveFilenameFactoryType = (Type)Type.GetType((string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ArchiveFilenameFactory", typeof(Type)).Value);
            _ArchiveFilenameFactory = (IArchiveFilenameFactory)Common.InstantiationHelper.InvokeDefaultCtor(archiveFilenameFactoryType);
        }
        #endregion
        #region Private Fields
        private Logging.ILog _Log;
        private FileStorage.IFileStore _LogArchiveFileStore;
        // 
        private IArchiveFilenameFactory _ArchiveFilenameFactory;
        private LogControllerThreadWorker _InternalThreadWorker;
        private Logging.ILogStatistics _FinalStatistics;
        private Logging.ILogStatistics _ArchiveStatistics = new Logging.LogStatistics();
        #endregion
        #region ILogController Methods
        /// <summary>
        /// Indicates the operating state of the <see cref="ILogController"/>.
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="ILogController"/> started.
        /// </summary>
        public DateTimeOffset StartTime { get; private set; } = DateTimeOffset.Now;
        /// <summary>
        /// The duration that the <see cref="ILogController"/> has run. 
        /// </summary>
        public TimeSpan RunTime => (DateTimeOffset.Now - StartTime);
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        public string LogSourceId => Settings.LogSourceId;
        /// <summary>
        /// Settings to control the actions of the <see cref="ILogController"/>.
        /// </summary>
        public LogControllerSettings Settings
        {
            get;
        }
        /// <summary>
        /// Statistical information about the logging system.
        /// </summary>
        public Logging.ILogStatistics Statistics
        {
            get
            {
                if (IsRunning)
                {
                    InternalGetStatistics();
                }
                return _FinalStatistics;
            }
        }
        /// <summary>
        /// A logger derived from <see cref="MarshalByRefObject"/>, that is configurable, memory-cached and archive-enabled.
        /// Being derived from <see cref="MarshalByRefObject"/>, allows the <see cref="LogMarshal"/> to be passed from one <see cref="AppDomain"/> to another, yet still reference the singular instance.
        /// </summary>
        public LogMarshal LogMarshal
        {
            get; private set;
        }
        /// <summary>
        /// Initializes and engages the logging sub-systems.
        /// </summary>
        public void Start()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("The LogController has already been started.");
            }
            // create/open LogMarshal
            LogMarshal = new LogMarshal(_Log, Settings);
            LogMarshal.Open();
            _FinalStatistics = null;
            // create/start cache flush and archive monitor thread
            _InternalThreadWorker = new LogControllerThreadWorker(LogMarshal, Settings, _ArchiveStatistics, _LogArchiveFileStore, _ArchiveFilenameFactory);
            _InternalThreadWorker.Start();
            //
            IsRunning = true;
            StartTime = DateTimeOffset.Now;
        }
        /// <summary>
        /// Terminates the logging sub-systems.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("The LogController has not been started.");
            }
            IsRunning = false;
            //
            // stop cache monitor thread
            if (_InternalThreadWorker.IsRunning)
            {
                // wait for thread to finish; do not cancel
                while (_InternalThreadWorker.IsRunning)
                {
                    Threading.ThreadingHelper.Sleep(5);
                }
            }
            _InternalThreadWorker.Stop();
            _InternalThreadWorker = null;
            // stop the log marshal
            LogMarshal.Close();
            // get final statistics
            InternalGetStatistics();
            LogMarshal = null;
        }
        /// <summary>
        /// Will remove all log entries.
        /// </summary>
        /// <param name="includeArchivedLogs"><b>True</b> will clear all archived logs; otherwise, <b>false</b> will leave the archived logs alone.</param>
        public void Clear(bool includeArchivedLogs)
        {
            LogMarshal.Clear();
            _LogArchiveFileStore.Clear();
            _LogArchiveFileStore.Save(false);
        }
        /// <summary>
        /// Reads each log entry from the log.
        /// </summary>
        /// <returns>Returns all log entries in the log.</returns>
        public IEnumerable<Logging.ILogEntry> Read()=> Read((s) => { return true; });
        /// <summary>
        /// Returns each log entry in the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate"></param>
        /// <returns>Every log entry from the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public IEnumerable<Logging.ILogEntry> Read(Func<Logging.ILogEntry, bool> logEntryFilterPredicate) => Read((f) => { return true; }, logEntryFilterPredicate);
        /// <summary>
        /// Returns each log entry in the log and the archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate">A function which takes an <see cref="Logging.ILogEntry"/> and returns <b>true</b> to include the log entry, or false to skip the log entry.</param>
        /// <param name="archiveFileFilterPredicate">A function which takes the <see cref="FileStorage.IFileStoreItem"/> about to be processed and returns <b>true</b> to include the log file, or false to skip the log file.</param>
        /// <returns>Every log entry from the log and the archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public IEnumerable<Logging.ILogEntry> Read(Func<FileStorage.IFileStoreItem, bool> archiveFileFilterPredicate,
                                                   Func<Logging.ILogEntry, bool> logEntryFilterPredicate)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("The LogController has not been started.");
            }
            if (archiveFileFilterPredicate == null)
            {
                throw new ArgumentNullException(nameof(archiveFileFilterPredicate));
            }
            if (logEntryFilterPredicate == null)
            {
                throw new ArgumentNullException(nameof(logEntryFilterPredicate));
            }
            // read all archived logs (from oldest to latest)
            var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            FileStorage.FileStorageHelper.CreateDirectory(tempPath);
            try
            {
                foreach (var fileStoreItem in from x in _LogArchiveFileStore.GetAll()
                                              orderby x.RootFileLastModifiedTimeUtc ascending
                                              select x)
                {
                    if (archiveFileFilterPredicate(fileStoreItem))
                    {
                        var extractedName = fileStoreItem.Extract(tempPath, true);
                        if (System.IO.File.Exists(extractedName))
                        {
                            var log = new Logging.FileEventLog.Log(extractedName, Settings.WritePrimaryLogEntriesUsingLocalTime);
                            log.Open();
                            try
                            {
                                foreach (var entry in from x in log.Read(logEntryFilterPredicate)
                                                      orderby x.Timestamp ascending
                                                      select x)
                                {
                                    yield return entry;
                                }
                            }
                            finally
                            {
                                log.Close();
                                FileStorage.FileStorageHelper.DeleteFile(extractedName);
                            }
                        }
                    }
                }

            }
            finally
            {
                FileStorage.FileStorageHelper.DeleteDirectory(tempPath);
            }
            // read current log
            foreach (var entry in from x in _Log.Read(logEntryFilterPredicate)
                                  orderby x.Timestamp ascending
                                  select x)
            {
                yield return entry;
            }
        }
        /// <summary>
        /// Writes an <see cref="Logging.ILogEntry"/> to the log.
        /// </summary>
        /// <param name="logEntry">The entry to write.</param>
        public void Write(Logging.ILogEntry logEntry)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("The LogController has not been started.");
            }
            LogMarshal.Write(logEntry);
        }
        /// <summary>
        /// Adds all the <see cref="Logging.ILogEntry"/>s from <paramref name="entries"/> to the <see cref="Logging.Logs"/>.
        /// </summary>
        /// <param name="entries">The <see cref="Logging.Logs"/> to add the <see cref="Logging.ILogEntry"/>s from.</param>
        public void Write(Logging.Logs entries) => Write((IEnumerable<Logging.ILogEntry>)entries);
        /// <summary>
        /// Adds all the <see cref="Logging.ILogEntry"/>s from a collection of <see cref="Logging.ILogEntry"/>s to the <see cref="Logging.Logs"/>.
        /// </summary>
        /// <param name="entries">The collection of <see cref="Logging.ILogEntry"/>s to add.</param>
        public void Write(IEnumerable<Logging.ILogEntry> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }
            foreach (var item in entries)
            {
                Write(item);
            }
        }
        /// <summary>
        /// Add a <see cref="Logging.ILogEntry"/> to the <see cref="Logging.Logs"/> with the given information.
        /// </summary>
        /// <param name="entryType">The type of log entry.</param>
        /// <param name="sourceId">The source of the log entry.</param>
        /// <param name="message">The log entry's message.</param>
        public void Write(Logging.LogEntryType entryType, string sourceId, string message) => Write(new Logging.LogEntry(entryType, sourceId, message, 0, 0, DateTimeOffset.Now));
        /// <summary>
        /// Add a <see cref="Logging.ILogEntry"/> to the <see cref="Logging.Logs"/> with the given information.
        /// </summary>
        /// <param name="entryType">The type of log entry.</param>
        /// <param name="sourceId">The source of the log entry.</param>
        /// <param name="message">The log entry's message.</param>
        /// <param name="eventId">The event id of the log entry.</param>
        public void Write(Logging.LogEntryType entryType, string sourceId, string message, int eventId) => Write(new Logging.LogEntry(entryType, sourceId, message, eventId, 0, DateTimeOffset.Now));
        /// <summary>
        /// Add a <see cref="Logging.ILogEntry"/> to the <see cref="Logging.Logs"/> with the given information.
        /// </summary>
        /// <param name="entryType">The type of log entry.</param>
        /// <param name="sourceId">The source of the log entry.</param>
        /// <param name="message">The log entry's message.</param>
        /// <param name="eventId">The event id of the log entry.</param>
        /// <param name="category">The category for this log entry.</param>
        public void Write(Logging.LogEntryType entryType, string sourceId, string message, int eventId, ushort category) => Write(new Logging.LogEntry(entryType, sourceId, message, eventId, category, DateTimeOffset.Now));
        /// <summary>
        /// Add a <see cref="Logging.ILogEntry"/> to the <see cref="Logging.Logs"/> with the given information.
        /// </summary>
        /// <param name="entryType">The type of log entry.</param>
        /// <param name="sourceId">The source of the log entry.</param>
        /// <param name="message">The log entry's message.</param>
        /// <param name="eventId">The event id of the log entry.</param>
        /// <param name="category">The category for this log entry.</param>
        /// <param name="timeStamp">The <see cref="DateTimeOffset"/> the log entry occurred.</param>
        public void Write(Logging.LogEntryType entryType, string sourceId, string message, int eventId, ushort category, DateTimeOffset timeStamp) => Write(new Logging.LogEntry(entryType, sourceId, message, eventId, category, timeStamp));
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("LogController");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // _Log
                var logConfig = _Log.Configuration;
                // remove extraneous "WriteLogEntriesUsingLocalTime" entry
                if (logConfig.Items.ContainsKey("WriteLogEntriesUsingLocalTime"))
                {
                    logConfig.Items.Remove("WriteLogEntriesUsingLocalTime");
                }
                result.Add(logConfig);
                // Settings
                result.Add(Settings.Configuration);
                // _LogArchiveFileStore
                result.Add(_LogArchiveFileStore.Configuration);
                // remove (local) temp directory from configuration (make it blank)
                if ((result.ContainsKey("FileStore")) &&
                    (result["FileStore"].Items.ContainsKey("ExtractionRootDirectory")) &&
                    (result["FileStore"].Items["ExtractionRootDirectory"].Value.ToString().Equals(System.IO.Path.GetTempPath(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    result["FileStore"].Items.Add("ExtractionRootDirectory", "", typeof(string));
                }
                // _ArchiveFilenameFactory
                result.Items.Add("ArchiveFilenameFactory", _ArchiveFilenameFactory.GetType(), typeof(Type));
                //
                return result;
            }
        }
        #endregion
        #region Private Methods
        private void InternalGetStatistics()
        {
            _FinalStatistics = _Log.Statistics;
            // populate Cache Log Statistics
            _FinalStatistics.AutoFlushTimeRemaining = (_InternalThreadWorker != null) ? _InternalThreadWorker.RemainingTimeToAutoFlush : TimeSpan.Zero;
            _FinalStatistics.AutoFlushEnabled = true;
            _FinalStatistics.AutoFlushTimeLimit = Settings.AutoFlushTimeLimit;
            _FinalStatistics.AutoFlushMaximumLogs = Settings.AutoFlushMaximumLogs;
            _FinalStatistics.TotalLogCount = LogMarshal.TotalCount;
            _FinalStatistics.IncomingLogCount = LogMarshal.IncomingCount;
            _FinalStatistics.CachedLogCount = LogMarshal.CachedCount;
            _FinalStatistics.LargestFlushCount = LogMarshal.LargestFlushCount;
            _FinalStatistics.LastFlushDate = LogMarshal.LastFlushDate;
            _FinalStatistics.FlushGeneration = LogMarshal.FlushGeneration;
            _FinalStatistics.LastFlushCount = LogMarshal.LastFlushCount;
            // populate Archive Statistics
            _FinalStatistics.AutoArchiveEnabled = Settings.AutoArchiveEnabled;
            _FinalStatistics.ArchivedLogCount = _ArchiveStatistics.ArchivedLogCount;
            _FinalStatistics.ArchiveGeneration = _ArchiveStatistics.ArchiveGeneration;
            _FinalStatistics.AutoArchiveMaximumLogs = Settings.AutoArchiveMaximumLogs;
            _FinalStatistics.LargestArchiveCount = _ArchiveStatistics.LargestArchiveCount;
            _FinalStatistics.LastArchiveCount = _ArchiveStatistics.LastArchiveCount;
            _FinalStatistics.LastArchiveDate = _ArchiveStatistics.LastArchiveDate;
        }
        #endregion
        #region Private Class: LogControllerThreadWorker
        // CURRENTLY HANDLING
        //      Cached Log Monitor      (Auto-Flush)
        //      Log Archive Monitor     (Auto-Archive)
        //      
        private class LogControllerThreadWorker
            : Threading.ThreadBase
        {
            #region Ctor
            public LogControllerThreadWorker(LogMarshal logMarshal,
                                             LogControllerSettings logSettings,
                                             Logging.ILogStatistics logStats,
                                             FileStorage.IFileStore archiveFileStore,
                                             IArchiveFilenameFactory archiveFilenameFactory)
            {
                _LogMarshal = logMarshal;
                _LogSettings = logSettings;
                _LogStats = logStats;
                _ArchiveFileStore = archiveFileStore;
                _ArchiveFilenameFactory = archiveFilenameFactory;
            }
            #endregion
            #region Private Fields
            private LogMarshal _LogMarshal;
            private LogControllerSettings _LogSettings;
            private Logging.ILogStatistics _LogStats;
            private FileStorage.IFileStore _ArchiveFileStore;
            private IArchiveFilenameFactory _ArchiveFilenameFactory;
            //
            private DateTime _StartingTime = DateTime.Now;
            private TimeSpan _ElapsedTime => (DateTime.Now - _StartingTime);
            #endregion
            #region Public Methods
            public TimeSpan RemainingTimeToAutoFlush => (_LogSettings.AutoFlushTimeLimit - _ElapsedTime);
            #endregion
            #region Threading.ThreadBase Methods
            protected override TimeSpan ExecutionInterval => _LogControllerThreadWorkerFrequency_;
            protected override bool CanExecute => true;
            protected override void OnExecute(Threading.ThreadCancelToken cancelToken)
            {
                if ((_ElapsedTime >= _LogSettings.AutoFlushTimeLimit) ||
                    (_LogMarshal.CachedCount > _LogSettings.AutoFlushMaximumLogs))
                {
                    lock (_LogMarshal.CacheSyncObject)
                    {
                        Flush();
                        Archive();
                        _StartingTime = DateTime.Now;
                    }
                }
            }
            protected override void OnStart()
            {
            }
            protected override void OnStop()
            {
            }
            #endregion
            #region Private Methods
            private void Flush() => _LogMarshal.Flush();
            private void Archive()
            {
                if (_LogMarshal.LogCount > _LogSettings.AutoArchiveMaximumLogs)
                {
                    _LogStats.LastArchiveCount = Logging.LoggingHelper.ArchiveLogs(_LogMarshal,
                                                                                   _ArchiveFileStore,
                                                                                   _ArchiveFilenameFactory.GenerateFilename,
                                                                                   _LogSettings.WriteArchivedLogEntriesUsingLocalTime,
                                                                                   _LogSettings.TruncateLogArchive,
                                                                                   _LogSettings.TruncateLogArchiveMaximumFiles,
                                                                                   _LogSettings.TruncateLogArchivePercentageToRemove);
                    if (_LogStats.LastArchiveCount > _LogStats.LargestArchiveCount)
                    {
                        _LogStats.LargestArchiveCount = _LogStats.LastArchiveCount;
                    }
                    _LogMarshal.Clear();
                    _LogStats.LastArchiveDate = DateTime.Now;
                    ++_LogStats.ArchiveGeneration;
                }
            }
            #endregion
        }
    }
    #endregion
}

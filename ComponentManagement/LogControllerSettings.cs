using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Provides setting required by the <see cref="ILogController"/> to perform it's function.
    /// </summary>
    /// <seealso cref="LogController"/>
    /// <seealso cref="Logging"/>
    public class LogControllerSettings
    {
        #region Public Constants
        /// <summary>
        /// The minimum value allowed when setting the <see cref="AutoFlushMaximumLogs"/> value. Current Value=10
        /// </summary>
        public static readonly int MinimumAutoFlushLogsCount_ = 10;
        /// <summary>
        /// The minimum amount of time allowed when setting the <see cref="AutoFlushTimeLimit"/> value. Current Value=10 seconds
        /// </summary>
        public static readonly TimeSpan MinimumAutoFlushTimeLimit_ = TimeSpan.FromSeconds(10);
        /// <summary>
        /// The minimum value allowed when setting the <see cref="AutoArchiveMaximumLogs"/> value. Current Value=500
        /// </summary>
        public static readonly int MinimumAutoArchiveLogsCount_ = 100;
        #endregion
        #region Ctor
        private LogControllerSettings() { }
        /// <summary>
        /// Instantiates a new instance of <see cref="LogControllerSettings"/>.
        /// </summary>
        /// <param name="logSourceId">The value used as the source id when creating log entries.</param>
        /// <param name="autoCacheFlushLogMax">The maximum number of log entries in the log before the auto-flush is engaged.</param>
        /// <param name="autoCacheFlushTimeLimit">The maximum amount of time allowed to pass before the auto-flush is engaged.</param>
        /// <param name="writePrimaryLogEntriesUsingLocalTime">A value indicating whether logs written into the primary log will be written relative to UTC or the local computer's timezone.</param>
        /// <param name="autoArchiveEnabled">A value indicating whether auto-archive system is on or off.</param>
        /// <param name="autoArchiveLogMax">The maximum number of log entries in the log before the auto-archive is engaged.</param>
        /// <param name="writeArchivedLogEntriesUsingLocalTime">A value indicating whether logs written into the archive will be written relative to UTC or the local computer's timezone.</param>
        /// <param name="truncateLogArchive">Sets whether the log archive will be automatically truncated.</param>
        /// <param name="truncateLogArchiveMaximumFiles">The total number of files allowed in the log archive before auto truncation is performed.</param>
        /// <param name="truncateLogArchivePercentageToRemove">The percentage of log archives to remove once <paramref name="truncateLogArchiveMaximumFiles"/> has been exceeded. Value must be between 0.0 and 1.0.</param>
        public LogControllerSettings(string logSourceId,
                                     int autoCacheFlushLogMax,
                                     TimeSpan autoCacheFlushTimeLimit,
                                     bool writePrimaryLogEntriesUsingLocalTime,
                                     bool autoArchiveEnabled,
                                     int autoArchiveLogMax,
                                     bool writeArchivedLogEntriesUsingLocalTime,
                                     bool truncateLogArchive,
                                     int truncateLogArchiveMaximumFiles,
                                     double truncateLogArchivePercentageToRemove)
            : this()
        {
            if (string.IsNullOrWhiteSpace(logSourceId)) { throw new ArgumentNullException(nameof(logSourceId)); }
            if (autoCacheFlushLogMax < MinimumAutoFlushLogsCount_) { throw new ArgumentOutOfRangeException(nameof(autoCacheFlushLogMax), $"Parameter {nameof(autoCacheFlushLogMax)} must be greater than {MinimumAutoFlushLogsCount_}."); }
            if (autoCacheFlushTimeLimit < MinimumAutoFlushTimeLimit_) { throw new ArgumentNullException(nameof(autoCacheFlushTimeLimit), $"Parameter {nameof(autoCacheFlushTimeLimit)} must be greater than {MinimumAutoFlushTimeLimit_}."); }
            if (autoArchiveLogMax < MinimumAutoArchiveLogsCount_) { throw new ArgumentOutOfRangeException(nameof(autoArchiveLogMax), $"Parameter {nameof(autoArchiveLogMax)} must be greater than {MinimumAutoArchiveLogsCount_}."); }
            LogSourceId = logSourceId;
            AutoFlushMaximumLogs = autoCacheFlushLogMax;
            AutoFlushTimeLimit = autoCacheFlushTimeLimit;
            AutoArchiveEnabled = autoArchiveEnabled;
            AutoArchiveMaximumLogs = autoArchiveLogMax;
            WriteArchivedLogEntriesUsingLocalTime = writeArchivedLogEntriesUsingLocalTime;
            WritePrimaryLogEntriesUsingLocalTime = writePrimaryLogEntriesUsingLocalTime;
            TruncateLogArchive = truncateLogArchive;
            TruncateLogArchiveMaximumFiles = truncateLogArchiveMaximumFiles;
            if ((truncateLogArchivePercentageToRemove < 0) || (truncateLogArchivePercentageToRemove > 1)) { throw new ArgumentOutOfRangeException(nameof(truncateLogArchivePercentageToRemove), $"Parameter truncateLogArchivePercentageToRemove must be between 0.0 and 1.0. Value={truncateLogArchivePercentageToRemove}"); }
            TruncateLogArchivePercentageToRemove = truncateLogArchivePercentageToRemove;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public LogControllerSettings(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "LogControllerSettings") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"LogControllerSettings\". Configuration Key={configuration.Key}", nameof(configuration)); }
            // SystemSourceId
             var temp1 = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "LogSourceId", typeof(string)).Value;
            if (string.IsNullOrWhiteSpace(temp1)) { throw new Exception($"Configuration invalid information. Configuration item: \"LogSourceId\" cannot be empty."); }
            LogSourceId = temp1;
            // AutoFlushMaximumLogs
            var temp2 = (int)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "AutoFlushMaximumLogs", typeof(int)).Value;
            if (temp2 < MinimumAutoFlushLogsCount_) { throw new Exception($"AutoFlushMaximumLogs must be greater than {MinimumAutoFlushLogsCount_}."); }
            AutoFlushMaximumLogs = temp2;
            // AutoFlushTimeLimit
            var temp3 = (TimeSpan)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "AutoFlushTimeLimit", typeof(TimeSpan)).Value;
            if (temp3 < MinimumAutoFlushTimeLimit_) { throw new Exception($"AutoFlushTimeLimit must be greater than {MinimumAutoFlushTimeLimit_}."); }
            AutoFlushTimeLimit = temp3;
            // AutoArchiveEnabled
           AutoArchiveEnabled = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "AutoArchiveEnabled", typeof(bool)).Value;
            // AutoArchiveMaximumLogs
            var temp4 = (int)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "AutoArchiveMaximumLogs", typeof(int)).Value;  
            if (temp4 < MinimumAutoArchiveLogsCount_) { throw new Exception($"AutoFlushMaximumLogs must be greater than {MinimumAutoArchiveLogsCount_}."); }
            AutoArchiveMaximumLogs = temp4;
            // WriteArchivedLogEntriesUsingLocalTime
            WriteArchivedLogEntriesUsingLocalTime = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "WriteArchivedLogEntriesUsingLocalTime", typeof(bool)).Value;
            // WritePrimaryLogEntriesUsingLocalTime
            WritePrimaryLogEntriesUsingLocalTime = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "WritePrimaryLogEntriesUsingLocalTime", typeof(bool)).Value;
            // TruncateLogArchive
            TruncateLogArchive = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "TruncateLogArchive", typeof(bool)).Value;
            // TruncateLogArchiveMaximumFiles
            TruncateLogArchiveMaximumFiles = (int)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "TruncateLogArchiveMaximumFiles", typeof(int)).Value;
            // TruncateLogArchivePercentageToRemove
            TruncateLogArchivePercentageToRemove = (double)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "TruncateLogArchivePercentageToRemove", typeof(double)).Value;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        public string LogSourceId { get; }
        /// <summary>
        /// The maximum number of log entries in the log before the auto-flush is engaged.
        /// </summary>
        public int AutoFlushMaximumLogs { get; }
        /// <summary>
        /// The maximum amount of time allowed to pass before the auto-flush is engaged. 
        /// </summary>
        public TimeSpan AutoFlushTimeLimit { get; }
        /// <summary>
        /// Gets, or sets, a value indicating whether auto-archive system is on or off.
        /// </summary>
        public bool AutoArchiveEnabled { get; set; }
        /// <summary>
        /// The maximum number of log entries in the log before the auto-archive is engaged.
        /// </summary>
        public int AutoArchiveMaximumLogs { get; }
        /// <summary>
        /// Gets a value indicating whether logs written into the archive will be written relative to UTC or the local computer's timezone.
        /// </summary>
        public bool WriteArchivedLogEntriesUsingLocalTime { get; }
        /// <summary>
        /// Gets a value indicating whether logs written into the primary log will be written relative to UTC or the local computer's timezone.
        /// </summary>
        public bool WritePrimaryLogEntriesUsingLocalTime { get; }
        /// <summary>
        /// Gets whether the log archive will be automatically truncated.
        /// </summary>
        public bool TruncateLogArchive { get; }
        /// <summary>
        /// The total number of files allowed in the log archive before auto truncation is performed.
        /// </summary>
        public int TruncateLogArchiveMaximumFiles { get; }
        /// <summary>
        /// The percentage of log archives to remove once <see cref="TruncateLogArchiveMaximumFiles"/> has been exceeded.
        /// </summary>
        public double TruncateLogArchivePercentageToRemove { get; }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("LogControllerSettings");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // SystemSourceId
                result.Items.Add("LogSourceId", LogSourceId, LogSourceId.GetType());
                // AutoFlushMaximumLogs
                result.Items.Add("AutoFlushMaximumLogs", AutoFlushMaximumLogs, AutoFlushMaximumLogs.GetType());
                // AutoFlushTimeLimit
                result.Items.Add("AutoFlushTimeLimit", AutoFlushTimeLimit, AutoFlushTimeLimit.GetType());
                // AutoArchiveEnabled
                result.Items.Add("AutoArchiveEnabled", AutoArchiveEnabled, AutoArchiveEnabled.GetType());
                // AutoArchiveMaximumLogs
                result.Items.Add("AutoArchiveMaximumLogs", AutoArchiveMaximumLogs, AutoArchiveMaximumLogs.GetType());
                // WriteArchivedLogEntriesUsingLocalTime
                result.Items.Add("WriteArchivedLogEntriesUsingLocalTime", WriteArchivedLogEntriesUsingLocalTime, WriteArchivedLogEntriesUsingLocalTime.GetType());
                // WritePrimaryLogEntriesUsingLocalTime
                result.Items.Add("WritePrimaryLogEntriesUsingLocalTime", WritePrimaryLogEntriesUsingLocalTime, WritePrimaryLogEntriesUsingLocalTime.GetType());
                // TruncateLogArchive
                result.Items.Add("TruncateLogArchive", TruncateLogArchive, TruncateLogArchive.GetType());
                // TruncateLogArchiveMaximumBytes
                result.Items.Add("TruncateLogArchiveMaximumFiles", TruncateLogArchiveMaximumFiles, TruncateLogArchiveMaximumFiles.GetType());
                //TruncateLogArchivePercentageToRemove
                result.Items.Add("TruncateLogArchivePercentageToRemove", TruncateLogArchivePercentageToRemove, TruncateLogArchivePercentageToRemove.GetType());
                return result;
            }
        }
        #endregion
    }
}

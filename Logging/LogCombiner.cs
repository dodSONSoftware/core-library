using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Combines multiple logs into a single log.
    /// </summary>
    public class LogCombiner
        : ILogCombiner
    {
        #region Ctor
        private LogCombiner()
        {
        }
        /// <summary>
        /// Instantiates a new LogCombiner with the given <see cref="ILog"/>.
        /// </summary>
        /// <param name="logs"></param>
        public LogCombiner(IEnumerable<ILog> logs)
            : this()
        {
            Logs = logs ?? new ILog[0];
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public LogCombiner(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "LogCombiner")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"LogCombiner\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // group: Logs
            if (configuration.ContainsKey("Logs"))
            {
                var logs = new List<ILog>();
                foreach (var item in configuration["Logs"])
                {
                    ((Configuration.IConfigurationGroupAdvanced)item).SetKey("Log");
                    logs.Add((ILog)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(item));
                }
                Logs = logs;
            }
        }
        #endregion
        #region ILogCombiner Methods
        /// <summary>
        /// Get all of the <see cref="ILog"/>s in this Log Combiner.
        /// </summary>
        public IEnumerable<ILog> Logs
        {
            get;
        }
        #endregion
        #region ILogReader Methods
        /// <summary>
        /// Returns the unique id for this <see cref="ILog"/>.
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// Returns an object that can be used for synchronizing access to the underlying logging system across multiple threads.
        /// </summary>
        public object SyncObject { get; } = new object();
        /// <summary>
        /// Gets a value indicating whether the log exists. <b>True</b> indicates it was found, <b>false</b> indicates it was <i>not</i> found.
        /// </summary>
        public bool Exists
        {
            get
            {
                foreach (var item in InternalLogs)
                {
                    if (!item.Exists)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Returns whether the log has been opened. <b>True</b> if the log is open, otherwise, <b>false</b>.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                foreach (var item in InternalLogs)
                {
                    if (!item.IsOpen)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Gets the number of log entries contained in the log.
        /// </summary>
        public int LogCount => InternalLogs.Select(x => x.LogCount).Aggregate(0, (total, count) => total += count);
        /// <summary>
        /// Reads each log entry from the log.
        /// </summary>
        /// <returns>Returns all log entries in the log.</returns>
        public IEnumerable<ILogEntry> Read() => Read(x => true);
        /// <summary>
        /// Returns each log entry in the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate"></param>
        /// <returns>Every log entry from the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate)
        {
            foreach (var log in InternalLogs)
            {
                foreach (var logEntry in log.Read(logEntryFilterPredicate))
                {
                    yield return logEntry;
                }
            }
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
                var result = new Configuration.ConfigurationGroup("LogCombiner");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // Axes
                // group: LogFilters
                var logs = result.Add("Logs");
                var count = 0;
                foreach (var item in InternalLogs)
                {
                    // clone the item, change its key, save clone to dependencies list
                    // you do not want to change the key in the original item
                    var clone = Core.Configuration.XmlConfigurationSerializer.Clone(item.Configuration);
                    ((Configuration.IConfigurationGroupAdvanced)clone).SetKey($"Log {++count}");
                    logs.Add(clone);
                }
                // 
                return result;
            }
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Will return all Logs without repetition.
        /// </summary>
        private IEnumerable<ILog> InternalLogs
        {
            get
            {
                // keep from processing a log more than once
                var processedLogs = new List<string>();
                // iterate through all log filters
                foreach (var log in Logs)
                {
                    if (!processedLogs.Contains(log.Id))
                    {
                        processedLogs.Add(log.Id);
                        // iterate through all log entries
                        yield return log;
                    }
                }
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Will write <see cref="ILogEntry"/>s to an <see cref="ILog"/> if the <see cref="ILogEntry"/> passes the <see cref="ILoggable.IsValid(ILogEntry)"/> test.
    /// </summary>
    public class LogFilter
        : Configuration.IConfigurable
    {
        #region Ctor
        private LogFilter()
        {

        }
        /// <summary>
        /// Instantiates a new instance using the defined <paramref name="logEntryValidator"/> and <paramref name="log"/>.
        /// </summary>
        /// <param name="logEntryValidator">Determines if an <see cref="ILogEntry"/> should be written to the <see cref="Log"/>.</param>
        /// <param name="log">Provides functionality to create, delete, clear, read and write a log with <see cref="ILogEntry"/> log entries.</param>
        public LogFilter(ILoggable logEntryValidator, ILog log)
            : this()
        {
            LogEntryValidator = logEntryValidator ?? throw new ArgumentNullException(nameof(logEntryValidator));
            Log = log ?? throw new ArgumentNullException(nameof(log));
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public LogFilter(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "LogFilter")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"LogFilter\". Configuration Key={configuration.Key}", nameof(configuration));
            }

            // ######## the following logic allows the use of either a basic type entry (type item) or an IConfigurable entry (type group)

            // LogEntryChecker 
            var dude = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "LogEntryValidator", false);
            if (dude != null)
            {
                // process dude (group)
                LogEntryValidator = (ILoggable)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(dude);
            }
            else
            {
                // get item
                var logEntryValidator = Type.GetType((string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "LogEntryValidator", typeof(Type)).Value);
                // process item (item)
                LogEntryValidator = (ILoggable)Common.InstantiationHelper.InvokeDefaultCtor(logEntryValidator);
            }

            // LogWriter
            var logWriterConfiguration = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "Log", true);
            Log = (ILog)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(logWriterConfiguration);
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Determines if an <see cref="ILogEntry"/> should be written to the <see cref="Log"/>.
        /// </summary>
        public ILoggable LogEntryValidator
        {
            get;
        }
        /// <summary>
        /// Provides functionality to write log entries to a log.
        /// </summary>
        public ILog Log
        {
            get;
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
                var result = new Configuration.ConfigurationGroup("LogFilter");
                //result.Items.Add("Type", this.GetType(), typeof(Type));
                result.Items.Add("Type", this.GetType());

                // ######## the following logic allows the use of either a basic type entry (type item) or an IConfigurable entry (type group)

                // LogEntryValidator
                if (LogEntryValidator is Configuration.IConfigurable)
                {
                    // add as Group
                    result.Add((LogEntryValidator as Configuration.IConfigurable).Configuration);
                }
                else
                {
                    // add as Item
                    // result.Items.Add("LogEntryValidator", LogEntryValidator.GetType(), typeof(Type));
                    result.Items.Add("LogEntryValidator", LogEntryValidator.GetType());
                }

                // LogWriter
                result.Add(Log.Configuration);
                // 
                return result;
            }
        }
        #endregion
    }
}

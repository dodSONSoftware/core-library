using System.Collections.Generic;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines functionality to read multiple logs as a single log.
    /// </summary>
    public interface ILogCombiner
        : ILogReader, 
          Configuration.IConfigurable
    {
        /// <summary>
        /// Get all of the <see cref="ILog"/>s in the Log Combiner.
        /// </summary>
        IEnumerable<ILog> Logs
        {
            get;
        }
    }
}
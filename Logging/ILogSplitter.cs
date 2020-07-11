using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines elements required to split <see cref="ILogEntry"/>s into different <see cref="ILog"/>s.
    /// </summary>
    public interface ILogSplitter
        : ILog
    {
        /// <summary>
        /// Returns all <see cref=" LogFilter"/>s in the <see cref="ILogSplitter"/>.
        /// </summary>
        IEnumerable<LogFilter> LogFilters
        {
            get;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines functionality to add <see cref="ILogEntry"/> splitting to the <see cref="ILog"/> messaging system.
    /// </summary>
    public interface ILoggable
    {
        /// <summary>
        /// Determines if the <paramref name="logEntry"/> is valid for writing to an <see cref="ILog"/>.
        /// </summary>
        /// <param name="logEntry">The candidate <see cref="ILogEntry"/> to test if it should be written to the <see cref="ILog"/> or not.</param>
        /// <returns><b>True</b> indicates that the <see cref="ILogEntry"/> should be written to the <see cref="ILog"/>; otherwise, <b>false</b> indicates that the <see cref="ILogEntry"/> should not be written to the <see cref="ILog"/>.</returns>
        bool IsValid(ILogEntry logEntry);
    }
}

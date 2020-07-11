using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// A factory used to create a filename when a log controller needs to create a file when archiving a log.
    /// </summary>
    /// <seealso cref="LogController"/>
    /// <seealso cref="Logging"/>
    public interface IArchiveFilenameFactory
    {
        /// <summary>
        /// Creates a filename.
        /// </summary>
        /// <remarks>
        /// The returned filename can include just the filename or a relative path. For example:
        /// <code>
        /// filename.txt
        /// SystemLogs\filename.txt
        /// </code>
        /// </remarks>
        string GenerateFilename { get; }
    }
}

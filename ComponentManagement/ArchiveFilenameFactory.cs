using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// A factory used to create a filename when a log controller needs to create a file when archiving a log.
    /// <br/>
    /// The filename format follows this basic pattern using the current, local, date time: yyyyMMddHHmmss.log
    /// <br/>
    /// Examples: 20181025140842.log
    /// </summary>
    public class ArchiveFilenameFactory
        : IArchiveFilenameFactory
    {
        /// <summary>
        /// Generates a filename in the following format: yyyyMMddHHmmss.log; using the current, local, date time.
        /// </summary>
        public string GenerateFilename => $"{DateTimeOffset.Now.ToString("yyyyMMddHHmmss")}.log";
    }
}

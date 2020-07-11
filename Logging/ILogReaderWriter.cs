using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines functionality to read and write log entries from a log.
    /// </summary>
    public interface ILogReaderWriter
          : ILogReader, ILogWriter
    { }
}

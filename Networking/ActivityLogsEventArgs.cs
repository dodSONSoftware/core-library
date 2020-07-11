using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Provides logs relating to network activity; with a header identifying the specific activity a group of <see cref="Logging.ILog"/>s represent.
    /// </summary>
    [Serializable]
    public class ActivityLogsEventArgs
          : EventArgs
    {
        #region Ctor
        private ActivityLogsEventArgs() : base() { }
        /// <summary>
        /// Creates an instance of the <see cref="ActivityLogsEventArgs"/> using the <paramref name="logs"/>.
        /// </summary>
        /// <param name="logs">The logs involved with the event.</param>
        /// <param name="header">The type of activity represented by this log.</param>
        public ActivityLogsEventArgs(ActivityLogsEventType header,
                                     Logging.Logs logs)
            : this()
        {
            Header = header;
            Logs = logs ?? throw new ArgumentNullException(nameof(logs));
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The header describing the <see cref="Logs"/>.
        /// </summary>
        public ActivityLogsEventType Header { get; }
        /// <summary>
        /// The logs involved with the event.
        /// </summary>
        public Logging.Logs Logs { get; }
        #endregion
    }
}

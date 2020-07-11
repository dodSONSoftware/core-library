using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains a portions of the logs requested by the <see cref="RequestResponseCommands.GetLogs"/> command.
    /// </summary>
    [Serializable]
    public class GetLogsResponse
    {
        #region Ctor
        private GetLogsResponse()
        {
        }
        /// <summary>
        /// Instantiates a new instance of <see cref="GetLogsResponse"/> with some logs.
        /// </summary>
        /// <param name="logs">Some of the <see cref="Logging.Logs"/> requested.</param>
        public GetLogsResponse(Logging.Logs logs)
        {
            Logs = logs ?? throw new ArgumentNullException(nameof(logs));
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// A collection of <see cref="Logging.ILogEntry"/>s.
        /// </summary>
        public Logging.Logs Logs
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Logs.Count={Logs.Count}";
        #endregion
    }
}

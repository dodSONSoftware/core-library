using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Indicates that the <see cref="GetLogs"/> command has complete.
    /// </summary>
    [Serializable]
    public class GetLogsComplete
    {
        #region Ctor
        private GetLogsComplete()
        {
        }
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="wasCanceled">Indicates whether the <see cref="GetLogs"/> command was canceled.</param>
        /// <param name="totalSent">The total number of <see cref="Logging.ILogEntry"/>s sent.</param>
        /// <param name="lastDate">The last date for a <see cref="Logging.ILogEntry"/> sent by the <see cref="GetLogs"/> command.</param>
        public GetLogsComplete(bool wasCanceled, 
                               int totalSent,
                               DateTimeOffset lastDate)
            : this()
        {
            WasCanceled = wasCanceled;
            TotalSent = totalSent;
            LastDate = lastDate;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Indicates whether the <see cref="GetLogs"/> command was canceled.
        /// </summary>
        public bool WasCanceled
        {
            get; 
        }
        /// <summary>
        /// The total number of <see cref="Logging.ILogEntry"/>s sent.
        /// </summary>
        public int TotalSent
        {
            get;
        }
        /// <summary>
        /// The last date for a <see cref="Logging.ILogEntry"/> sent by the <see cref="GetLogs"/> command.
        /// </summary>
        public DateTimeOffset LastDate
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"WasCanceled={WasCanceled}, TotalSent={TotalSent}, LastDate={LastDate.ToUniversalTime()}";
        #endregion
    }
}

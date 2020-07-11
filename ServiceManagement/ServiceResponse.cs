using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Represents a response to a <see cref="ServiceRequest"/>.
    /// </summary>
    [Serializable()]
    public class ServiceResponse
        : RequestResponseBase
    {
        #region Ctor
        /// <summary>
        /// The Constructor.
        /// </summary>
        /// <param name="sessionId">The value used to identify a login session.</param>
        /// <param name="groupKey">The id of the group which this request, or response, belongs.</param>
        /// <param name="command">The command represented by this request, or response.</param>
        /// <param name="serviceManagerId">A value which uniquely identifies a <see cref="IServiceManager"/>.</param>
        /// <param name="data">The data required, or provided, by this request, or response.</param>
        public ServiceResponse(string sessionId, string groupKey, RequestResponseCommands command, string serviceManagerId, object data) : base(groupKey, command, sessionId, serviceManagerId, data) { }
        /// <summary>
        /// The Constructor.
        /// </summary>
        /// <param name="groupKey">The id of the group which this request, or response, belongs.</param>
        /// <param name="command">The command represented by this request, or response.</param>
        /// <param name="data">The data required, or provided, by this request, or response.</param>
        public ServiceResponse(string groupKey, RequestResponseCommands command, object data) : this("", groupKey, command, "", data) { }
        #endregion
    }
}

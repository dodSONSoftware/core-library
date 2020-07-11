using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Represents a request for the <see cref="IServiceManager"/>.
    /// </summary>
    [Serializable()]
    public class ServiceRequest
        : RequestResponseBase
    {
        #region Ctor
        /// <summary>
        /// A Constructor.
        /// </summary>
        /// <param name="sessionId">The value used to identify a login session.</param>
        /// <param name="groupKey">The id of the group which this request, or response, belongs.</param>
        /// <param name="command">The command represented by this request, or response.</param>
        /// <param name="serviceManagerId">A value which uniquely identifies a <see cref="IServiceManager"/>.</param>
        /// <param name="data">The data required, or provided, by this request, or response.</param>
        public ServiceRequest(string sessionId, string groupKey, RequestResponseCommands command, string serviceManagerId, object data) : base(groupKey, command, sessionId, serviceManagerId, data) { }
        ///// <summary>
        ///// A Constructor.
        ///// </summary>
        ///// <param name="sessionId">The value used to identify a login session.</param>
        ///// <param name="command">The command represented by this request, or response.</param>
        ///// <param name="data">The data required, or provided, by this request, or response.</param>
        //public ServiceRequest(string sessionId, RequestResponseCommands command, object data) : this(sessionId, Guid.NewGuid().ToString("N"), command, "", data) { }
        /// <summary>
        /// A Constructor.
        /// </summary>
        /// <param name="sessionId">The value used to identify a login session.</param>
        /// <param name="command">The command represented by this request, or response.</param>
        public ServiceRequest(string sessionId, RequestResponseCommands command) : this(sessionId, Guid.NewGuid().ToString("N"), command, "", null) { }
        /// <summary>
        /// A Constructor.
        /// </summary>
        /// <param name="command">The command represented by this request, or response.</param>
        /// <param name="data">The data required, or provided, by this request, or response.</param>
        public ServiceRequest(RequestResponseCommands command, object data) : this("", Guid.NewGuid().ToString("N"), command, "", data) { }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Defines the base class for all requests and responses.
    /// </summary>
    [Serializable()]
    public abstract class RequestResponseBase
    {
        #region Ctor
        /// <summary>
        /// A protected constructor.
        /// </summary>
        private RequestResponseBase()
        {
        }
        /// <summary>
        /// A protected constructor.
        /// </summary>
        /// <param name="groupKey">The id of the group which this request, or response, belongs.</param>
        /// <param name="command">The command represented by this request, or response.</param>
        /// <param name="sessionId">The value used to identify a login session.</param>
        /// <param name="serviceManagerId">A value which uniquely identifies a <see cref="IServiceManager"/>.</param>
        /// <param name="data">The data required, or provided, by this request, or response.</param>
        protected RequestResponseBase(string groupKey,
                                      RequestResponseCommands command,
                                      string sessionId,
                                      string serviceManagerId,
                                      object data)
            : this()
        {
            if (string.IsNullOrWhiteSpace(groupKey))
            {
                throw new ArgumentNullException(nameof(groupKey));
            }
            GroupKey = groupKey;
            Command = command;
            SessionId = sessionId;
            ServiceManagerId = serviceManagerId;
            Data = data;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The id for a series, or group, of requests and responses.
        /// </summary>
        public string GroupKey
        {
            get;
        }
        /// <summary>
        /// The command represented by this request, or response.
        /// </summary>
        public RequestResponseCommands Command
        {
            get;
        }
        /// <summary>
        /// The value used to identify a login session.
        /// </summary>
        public string SessionId
        {
            get;
        }
        /// <summary>
        /// A value which uniquely identifies a <see cref="IServiceManager"/>.
        /// </summary>
        public string ServiceManagerId
        {   
            get;
        }
        /// <summary>
        /// The data required, or provided, by this request, or response.
        /// </summary>
        public object Data
        {
            get; set;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this request, or response.
        /// </summary>
        /// <returns>The string representation of this request, or response.</returns>
        public override string ToString()
        {
            var typeStr = "";
            if (Data != null)
            {
                typeStr = $", DataType={Data.GetType().FullName}";
            }
            return $"Command={Command}, ServiceManagerId={ServiceManagerId}, SessionId={SessionId}, GroupKey={GroupKey}, HasData={Data != null}{typeStr}";
        }
        #endregion
    }
}

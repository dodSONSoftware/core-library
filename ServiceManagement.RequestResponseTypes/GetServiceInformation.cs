using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Request information about a specific service.
    /// </summary>
    [Serializable]
    public class GetServiceInformation
    {
        #region Ctor
        private GetServiceInformation()
        {
        }
        /// <summary>
        /// Request information about a specific service.
        /// </summary>
        /// <param name="serviceId">The id of the service request information for.</param>
        public GetServiceInformation(string serviceId)
            : this()
        {
            if (string.IsNullOrWhiteSpace(serviceId))
            {
                throw new ArgumentNullException(nameof(serviceId));
            }
            ServiceId = serviceId;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The id of the service requesting information for.
        /// </summary>
        public string ServiceId
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"ServiceId={ServiceId}";
        #endregion
    }
}

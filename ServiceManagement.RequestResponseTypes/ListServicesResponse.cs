using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about a service.
    /// </summary>
    [Serializable]
    public class ListServicesResponse
    {
        #region Ctor
        private ListServicesResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ListServicesResponse"/> with the given list of <see cref="ServiceInformation"/>.
        /// </summary>
        /// <param name="serviceManagerId">The id of the <see cref="IServiceManager"/> providing the services information.</param>
        /// <param name="list"></param>
        public ListServicesResponse(string serviceManagerId,
                                    IEnumerable<ServiceInformation> list)
            : this()
        {
            if (string.IsNullOrWhiteSpace(serviceManagerId))
            {
                throw new ArgumentNullException(nameof(serviceManagerId));
            }
            ServiceManagerId = serviceManagerId;
            //
            Services = list ?? throw new ArgumentNullException(nameof(list));
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The id of the <see cref="IServiceManager"/> providing the services information.
        /// </summary>
        public string ServiceManagerId
        {
            get;
        }
        /// <summary>
        /// List of all of the <see cref="ServiceInformation"/>.
        /// </summary>
        public IEnumerable<ServiceInformation> Services
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"ServiceManagerId={ServiceManagerId}, Services={Services.Count()}";
        #endregion
    }
}

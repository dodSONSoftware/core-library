using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains detailed information about each <see cref="IService"/> in the <see cref="IServiceManager"/>.
    /// </summary>
    [Serializable]
    public class ServiceManagerServices
    {
        #region Ctor
        private ServiceManagerServices()
        {
        }
        /// <summary>
        /// Creates a new <see cref="ServiceManagerServices"/> object.
        /// </summary>
        /// <param name="serviceManager">The Service Manager to get details from.</param>
        public ServiceManagerServices(IServiceManager serviceManager)
            : this()
        {
            if (serviceManager == null)
            {
                throw new ArgumentNullException(nameof(serviceManager));
            }
            // extract information
            Extensions = new List<ServiceDetails>(serviceManager.ComponentManager.ComponentController.ExtensionComponents.Where(x => x.Component is IService)
                                                                                                                         .Select(x => new ServiceDetails(x.Component as IService)));
            Plugins = new List<ServiceDetails>(from x in serviceManager.ComponentManager.ComponentController.PluginComponents
                                               where (x.Component is IService)
                                               select new ServiceDetails(x.Component as IService));
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// All <see cref="ServiceExtensionBase"/>d services.
        /// </summary>
        public IEnumerable<ServiceDetails> Extensions
        {
            get;
        }
        /// <summary>
        /// All <see cref="ServicePluginBase"/>d services.
        /// </summary>
        public IEnumerable<ServiceDetails> Plugins
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Extensions={Extensions.Count()}, Plugins={Plugins.Count()}";
        #endregion
    }
}

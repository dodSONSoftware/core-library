using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Defines the base class for all services that will be running in the current <see cref="System.AppDomain"/>.
    /// </summary>
    [Serializable()]
    public abstract class ServiceExtensionBase
            : ComponentManagement.ComponentExtensionBase,
              IService
    {
        #region Ctor
        /// <summary>
        /// Instantiates an new instance of the <see cref="ServiceExtensionBase"/> class.
        /// </summary>
        public ServiceExtensionBase() : base() { }
        #endregion
    }
}

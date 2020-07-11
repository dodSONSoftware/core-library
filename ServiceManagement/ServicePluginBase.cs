using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Defines the base class for all services that will be running in separate <see cref="System.AppDomain"/>s.
    /// </summary>
    [Serializable()]
    public abstract class ServicePluginBase
            : ComponentManagement.ComponentPluginBase,
              IService
    {
        #region Ctor
        /// <summary>
        /// Instantiates an new instance of the <see cref="ServicePluginBase"/> class.
        /// </summary>
        public ServicePluginBase() : base() { }
        #endregion
    }
}

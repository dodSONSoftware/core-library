using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Indicates the state of a <see cref="RequestResponseTypes.ServiceInformation"/> object and what type of component it is.
    /// </summary>
    public enum ComponentState
    {
        /// <summary>
        /// Indicates that the <see cref="RequestResponseTypes.ServiceInformation"/> object is not running.
        /// </summary>
        ComponentFactory = 0,
        /// <summary>
        /// Indicates that the <see cref="RequestResponseTypes.ServiceInformation"/> object is running and is an <see cref="ComponentManagement.IComponent"/>.
        /// </summary>
        Component,
        /// <summary>
        /// Indicates that the <see cref="RequestResponseTypes.ServiceInformation"/> object is running and is an <see cref="ServiceManagement.IService"/>.
        /// </summary>
        Service
    }
}

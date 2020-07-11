using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    ///  Defines common controls for <see cref="ServicePluginBase"/>d and <see cref="ServiceExtensionBase"/>d base types.
    /// </summary>
    /// <seealso cref="ServiceExtensionBase"/>
    /// <seealso cref="ServicePluginBase"/>
    /// <seealso cref="ComponentManagement.ComponentExtensionBase"/>
    /// <seealso cref="ComponentManagement.ComponentPluginBase"/>
    public interface IService
        : ComponentManagement.IComponent
    {
    }
}

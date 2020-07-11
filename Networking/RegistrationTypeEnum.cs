using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Indicates the type of registration requested.
    /// </summary>
    public enum RegistrationTypeEnum
    {
        /// <summary>
        /// Indicates a client is attempting to register.
        /// </summary>
        Register = 0,
        /// <summary>
        /// Indicates a client is attempting to unregister.
        /// </summary>
        Unregister
    }
}

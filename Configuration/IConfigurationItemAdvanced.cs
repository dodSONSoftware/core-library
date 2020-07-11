using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Defines advanced functionality required by types in the dodSON.Core.Configuration namespace, but not generally used by the typical consumer.
    /// </summary>
    public interface IConfigurationItemAdvanced
    {
        /// <summary>
        /// Sets the parent <see cref="IConfigurationGroup"/> for this <see cref="IConfigurationGroup"/>, or set to null, if this <see cref="IConfigurationGroup"/> has no parent.
        /// </summary>
        IConfigurationGroup Parent { set; }
        /// <summary>
        /// Sets the key for this item.
        /// </summary>
        string Key { set; }
    }
}

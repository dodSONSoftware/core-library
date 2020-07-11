using System;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Defines advanced functionality required by types in the dodSON.Core.Configuration
    /// namespace, but not generally used by the typical consumer.
    /// </summary>
    public interface IConfigurationGroupAdvanced
    {
        /// <summary>
        /// Sets the key for this group.
        /// </summary>
        /// <param name="key">The new key for this group.</param>
        void SetKey(string key);
        /// <summary>
        /// Sets the parent <see cref="IConfigurationGroup"/> for this <see cref="IConfigurationGroup"/>, or set to null, if this <see cref="IConfigurationGroup"/> has no parent.
        /// </summary>
        IConfigurationGroup Parent { set; }
    }
}

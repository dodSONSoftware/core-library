using System;
using System.ComponentModel;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Defines types and methods required to be an item in the configuration system.
    /// </summary>
    public interface IConfigurationItem
        : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Get the parent <see cref="IConfigurationGroup"/> for this <see cref="IConfigurationItem"/>; will be null if this <see cref="IConfigurationItem"/> has no parent.
        /// </summary>
        IConfigurationGroup Parent { get; }
        /// <summary>
        /// The hierarchy of groups this group belongs to.
        /// </summary>
        string GroupAddress { get; }
        /// <summary>
        /// A key which uniquely identifies this configuration item.
        /// </summary>
        string Key { get; }
        /// <summary>
        /// The <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/> which represents the type of data this configuration item contains.
        /// </summary>
        /// <seealso cref="Value"/>
        /// <see cref="TryGetValue{T}(out T)"/>
        string ValueType { get; set; }
        /// <summary>
        /// Gets and sets object associated with this configuration item.
        /// </summary>
        object Value { get; set; }
        /// <summary>
        /// Gets the value associated with this configuration item.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to try and cast this <see cref="Value"/> to.</typeparam>
        /// <param name="value">The <b>out</b> variable populated with the cast object, or null.</param>
        /// <returns><b>True</b> if the <see cref="Value"/> was successfully cast to type <typeparamref name="T"/>; otherwise, <b>false</b>.</returns>
        bool TryGetValue<T>(out T value);
        /// <summary>
        /// Change state tracking. 
        /// </summary>
        Common.IStateChangeTracking StateChange { get; }
    }
}

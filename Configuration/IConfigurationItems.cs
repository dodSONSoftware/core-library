using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Represents a collection of <see cref="IConfigurationItem"/>s.
    /// </summary>
    public interface IConfigurationItems
        : ICollection<IConfigurationItem>
    {
        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>
        /// Get the parent <see cref="IConfigurationGroup"/> for this <see cref="IConfigurationItems"/>; will be null if this <see cref="IConfigurationItems"/> has no parent.
        /// </summary>
        IConfigurationGroup Parent { get; }
        /// <summary>
        /// Gets or sets the <see cref="IConfigurationItem"/> associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to get or set.</param>
        /// <returns>The <see cref="IConfigurationItem"/> associated with the specified key.</returns>
        IConfigurationItem this[string key] { get; set; }
        /// <summary>
        /// Gets the <see cref="IConfigurationItem"/>'s <see cref="IConfigurationItem.Value"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the <see cref="IConfigurationItem.Value"/>.</typeparam>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to get the <see cref="IConfigurationItem.Value"/> from.</param>
        /// <returns>The <see cref="IConfigurationItem"/>'s <see cref="IConfigurationItem.Value"/> associated with the specified key.</returns>
        T GetValue<T>(string key);
        /// <summary>
        /// Returns whether an <see cref="IConfigurationItem"/>  with the specified <paramref name="key"/> exists.
        /// </summary>
        /// <param name="key">The key to search each <see cref="IConfigurationItem"/> for.</param>
        /// <returns><b>True</b> if an <see cref="IConfigurationItem"/> with the specified <paramref name="key"/> is found; otherwise, <b>false</b>.</returns>
        bool ContainsKey(string key);
        /// <summary>
        /// Creates an <see cref="IConfigurationItem"/> using the specified key and value and adds it to this collection.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to add.</param>
        /// <param name="value">The value of the <see cref="IConfigurationItem"/> to add.</param>
        /// <param name="valueType">The <see cref="Type"/> the item should be.</param>
        void Add(string key, object value, Type valueType);
        /// <summary>
        /// Creates an <see cref="IConfigurationItem"/> using the specified key and value and adds it to this collection.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to add.</param>
        /// <param name="value">The value of the <see cref="IConfigurationItem"/> to add.</param>
        void Add(string key, object value);
        /// <summary>
        /// Removes the <see cref="IConfigurationItem"/> with the specified key.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to remove.</param>
        /// <returns><b>True</b> if the <see cref="IConfigurationItem"/> was removed; otherwise <b>false</b>. This method returns <b>false</b> if the key was not found.</returns>
        bool Remove(string key);
        /// <summary>
        /// Change state tracking.
        /// </summary>
        Common.IStateChangeView StateChange { get; }
        /// <summary>
        /// Clears the Dirty flags.
        /// </summary>
        void ClearDirty();
        /// <summary>
        /// Clears the New flags.
        /// </summary>
        void ClearNew();
        /// <summary>
        /// Sets the Dirty flag.
        /// </summary>
        void MarkDirty();
        /// <summary>
        /// Sets the New flag.
        /// </summary>
        void MarkNew();
    }
}

using System.Collections.Generic;
using System.Collections.Specialized;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// <para>Each keyed <see cref="IConfigurationGroup"/> contains a collection of <see cref="IConfigurationItem"/>s and is itself a collection of <see cref="IConfigurationGroup"/>s.</para>
    /// </summary>
    public interface IConfigurationGroup
        : ICollection<IConfigurationGroup>
    {
        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>
        /// Returns the parent <see cref="IConfigurationGroup"/> for this <see cref="IConfigurationGroup"/>, or null, if this <see cref="IConfigurationGroup"/> has no parent.
        /// </summary>
        IConfigurationGroup Parent { get; }
        /// <summary>
        /// The hierarchy of groups this group belongs to.
        /// </summary>
        string GroupAddress { get; }
        /// <summary>
        /// A key which uniquely identifies this configuration group.
        /// </summary>
        string Key { get; }
        /// <summary>
        /// Gets or sets the <see cref="IConfigurationGroup"/> associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationGroup"/> to get or set.</param>
        /// <returns>The <see cref="IConfigurationGroup"/> associated with the specified key.</returns>
        IConfigurationGroup this[string key] { get; set; }
        /// <summary>
        /// Returns whether an <see cref="IConfigurationGroup"/>  with the specified <paramref name="key"/> exists.
        /// </summary>
        /// <param name="key">The key to search each <see cref="IConfigurationGroup"/> for.</param>
        /// <returns><b>True</b> if an <see cref="IConfigurationGroup"/> with the specified <paramref name="key"/> is found; otherwise, <b>false</b>.</returns>
        bool ContainsKey(string key);
        /// <summary>
        /// Creates an <see cref="IConfigurationGroup"/> using the specified key and adds it to this collection.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationGroup"/> to add.</param>
        /// <returns>The new <see cref="IConfigurationGroup"/>.</returns>
        IConfigurationGroup Add(string key);
        /// <summary>
        /// Removes the <see cref="IConfigurationGroup"/> with the specified key.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationGroup"/> to remove.</param>
        /// <returns><b>True</b> if the <see cref="IConfigurationGroup"/> was removed; otherwise <b>false</b>. This method returns <b>false</b> if the key was not found.</returns>
        bool Remove(string key);
        /// <summary>
        /// Returns the collection of <see cref="IConfigurationItem"/>s associated with the group.
        /// </summary>
        IConfigurationItems Items { get; }
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
        /// <summary>
        /// Gets a comma-separated set of key, value pairs representing the name and types of each item in this group.
        /// </summary>
        string ItemsSignature { get; }
    }
}

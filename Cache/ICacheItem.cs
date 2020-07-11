using System;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Represents an item in the cache and provides methods and properties to retrieve the payload and to test its validity.
    /// </summary>
    /// <typeparam name="T">The type of the payload carried by the cache item.</typeparam>
    public interface ICacheItem<T>
        : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the ID for this cache item.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Gets a value indicating whether this cache item is valid or not. <b>True</b> indicates it is valid; otherwise <b>false</b>.
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// Gets, or sets, the validator for this cache item.
        /// </summary>
        ICacheValidater<T> Validator { get; set; }
        /// <summary>
        /// Gets, or sets, the payload for this cache item.
        /// </summary>
        T Payload { get; set; }
        /// <summary>
        /// Gets the <see cref="DateTime"/> that this cache item was created.
        /// </summary>
        DateTime CreationDate { get; }
        /// <summary>
        /// Gets the <see cref="DateTime"/> that this cache item was last checked for validity.
        /// </summary>
        DateTime LastValidityCheck { get; }
        /// <summary>
        /// Gets the <see cref="DateTime"/> that this cache item was last accessed.
        /// </summary>
        DateTime LastPayloadAccessDate { get; }
    }
}

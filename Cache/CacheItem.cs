using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Represents an item in the cache and provides methods and properties to retrieve the payload and to test its validity.
    /// </summary>
    /// <typeparam name="T">The type of the payload carried by the cache item.</typeparam>
    [Serializable]
    public class CacheItem<T>
        : dodSON.Core.Cache.ICacheItem<T>
    {
        #region System.ComponentModel.INotifyPropertyChanged Methods
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Will raise the property changed events with the provided property name.
        /// </summary>
        /// <param name="propertyName">The name of the property which has changed.</param>
        protected void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the CacheItem class.
        /// </summary>
        protected CacheItem() { }
        /// <summary>
        /// Initializes a new instance of the CacheItem class.
        /// </summary>
        /// <param name="payload">The value representing the payload to be carried by this CacheItem.</param>
        /// <param name="validator">The validator used to determine if this CacheItem is valid, or not.</param>
        public CacheItem(T payload,
                         dodSON.Core.Cache.ICacheValidater<T> validator)
            : this()
        {
            _Payload = payload;
            _Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #endregion
        #region Private Fields
        private T _Payload = default(T);
        private dodSON.Core.Cache.ICacheValidater<T> _Validator = null;
        #endregion
        #region Public Methods
        /// <summary>
        /// Gets the ID for this cache item.
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Gets a value indicating whether this cache item is valid or not. <b>True</b> indicates it is valid; otherwise <b>false</b>.
        /// </summary>
        public bool IsValid
        {
            get
            {
                // update metrics
                LastValidityCheck = DateTime.Now;
                // return validity
                return _Validator.IsValid(this);
            }
        }
        /// <summary>
        /// Gets, or sets, the validater for this cache item.
        /// </summary>
        public ICacheValidater<T> Validator
        {
            get { return _Validator; }
            set
            {
                _Validator = value ?? throw new Exception("The validator cannot be null.");
                RaisePropertyChangedEvent(nameof(Validator));
            }
        }
        /// <summary>
        /// Gets, or sets, the payload for this cache item.
        /// </summary>
        public T Payload
        {
            get
            {
                // update metrics
                LastPayloadAccessDate = DateTime.Now;
                // return payload
                return _Payload;
            }
            set
            {
                // update metrics
                LastPayloadAccessDate = DateTime.Now;
                // update payload
                _Payload = value;
                RaisePropertyChangedEvent(nameof(Payload));
            }
        }
        #endregion
        #region Metric/Diagnostic Information
        /// <summary>
        /// Gets the <see cref="DateTime"/> that this cache item was created.
        /// </summary>
        public DateTime CreationDate { get; } = DateTime.Now;
        /// <summary>
        /// Gets the <see cref="DateTime"/> that this cache item was last checked for validity.
        /// </summary>
        public DateTime LastValidityCheck { get; private set; } = DateTime.MinValue;
        /// <summary>
        /// Gets the <see cref="DateTime"/> that this cache item was last accessed.
        /// </summary>
        public DateTime LastPayloadAccessDate { get; private set; } = DateTime.MinValue;
        #endregion
    }
}

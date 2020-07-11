using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Represents a collection of <see cref="IConfigurationItem"/>s.
    /// </summary>
    [Serializable]
    public class ConfigurationItems
        : IConfigurationItems,
          IConfigurationItemsAdvanced
    {
        #region Public Events
        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>
        /// Will raise the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> to pass to the <see cref="CollectionChanged"/> event.</param>
        protected void RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);
        #endregion
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the ConfigurationItems class.
        /// </summary>
        public ConfigurationItems()
        {
        }
        /// <summary>
        /// Initializes a new instance of the ConfigurationItems class with the given parent.
        /// </summary>
        /// <param name="parent">The parent <see cref="IConfigurationGroup"/> for this <see cref="ConfigurationItems"/>; set to null if this <see cref="ConfigurationItems"/> has no parents.</param>
        public ConfigurationItems(IConfigurationGroup parent) : this()
        {
            Parent = parent;
        }
        #endregion
        #region Private Fields
        private Dictionary<string, IConfigurationItem> _Items = new Dictionary<string, IConfigurationItem>();
        private Common.IStateChangeTracking _StateChangeTracker = new Common.StateChangeTracking(false, true, false);
        #endregion
        #region IConfigurationItems Methods
        /// <summary>
        /// Gets the parent <see cref="IConfigurationGroup"/> for this <see cref="ConfigurationItems"/>; set to null if this <see cref="ConfigurationItems"/> has no parent.
        /// </summary>
        public IConfigurationGroup Parent
        {
            get; private set;
        }
        /// <summary>
        /// Gets a value indicating whether this collection is read-only.
        /// </summary>
        public bool IsReadOnly => false;
        /// <summary>
        /// Determines whether the collection contains a specific <see cref="IConfigurationItem"/>.
        /// </summary>
        /// <param name="item">The <see cref="IConfigurationItem"/> to locate.</param>
        /// <returns><b>True</b> if the <see cref="IConfigurationItem"/> is found; otherwise, <b>false</b>.</returns>
        public bool Contains(IConfigurationItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return _Items.ContainsKey(item.Key);
        }
        /// <summary>
        /// Gets the number of <see cref="IConfigurationItem"/>s contained in this collection.
        /// </summary>
        public int Count => _Items.Count;
        /// <summary>
        /// Gets or sets the <see cref="IConfigurationItem"/> associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to get or set.</param>
        /// <returns>The <see cref="IConfigurationItem"/> associated with the specified key.</returns>
        public IConfigurationItem this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }
                if (!_Items.ContainsKey(key))
                {
                    throw new ArgumentOutOfRangeException($"Configuration Item not found; (key={key}).", nameof(key));
                }
                return _Items[key];
            }
            set
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }
                if (_Items.ContainsKey(key))
                {
                    Remove(key);
                }
                Add(value);
            }
        }
        /// <summary>
        /// Gets the <see cref="IConfigurationItem"/>'s <see cref="IConfigurationItem.Value"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the <see cref="IConfigurationItem.Value"/>.</typeparam>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to get the <see cref="IConfigurationItem.Value"/> from.</param>
        /// <returns>The <see cref="IConfigurationItem"/>'s <see cref="IConfigurationItem.Value"/> associated with the specified key.</returns>
        public T GetValue<T>(string key) => (T)this[key].Value;
        /// <summary>
        /// Returns whether an <see cref="IConfigurationItem"/>  with the specified <paramref name="key"/> exists.
        /// </summary>
        /// <param name="key">The key to search each <see cref="IConfigurationItem"/> for.</param>
        /// <returns><b>True</b> if an <see cref="IConfigurationItem"/> with the specified <paramref name="key"/> is found; otherwise, <b>false</b>.</returns>
        public bool ContainsKey(string key) => _Items.ContainsKey(key);
        /// <summary>
        /// Adds the <see cref="IConfigurationItem"/> to the collection.
        /// </summary>
        /// <param name="item">The <see cref="IConfigurationItem"/> to add.</param>
        public void Add(IConfigurationItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            IConfigurationItem oldItem = null;
            if (Contains(item))
            {
                oldItem = this[item.Key];
                Remove(oldItem);
                (oldItem as IConfigurationItemAdvanced).Parent = null;
            }
            _Items.Add(item.Key, item);
            (item as IConfigurationItemAdvanced).Parent = Parent;
            MarkDirty();
            if (oldItem == null)
            {
                RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
            else
            {
                RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem));
            }
        }
        /// <summary>
        /// Creates an <see cref="IConfigurationItem"/> using the specified key and value and adds it to this collection.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to add.</param>
        /// <param name="value">The value of the <see cref="IConfigurationItem"/> to add.</param>
        public void Add(string key, object value)
        {
            Add(key, value, value.GetType());
        }
        /// <summary>
        /// Creates an <see cref="IConfigurationItem"/> using the specified key and value and adds it to this collection.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to add.</param>
        /// <param name="value">The value of the <see cref="IConfigurationItem"/> to add.</param>
        /// <param name="valueType">The <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/> the item should be.</param>
        public void Add(string key, object value, string valueType) => Add(new ConfigurationItem(key, value, valueType));
        /// <summary>
        /// Creates an <see cref="IConfigurationItem"/> using the specified key and value and adds it to this collection.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to add.</param>
        /// <param name="value">The value of the <see cref="IConfigurationItem"/> to add.</param>
        /// <param name="valueType">The <see cref="Type"/> the item should be.</param>
        public void Add(string key, object value, Type valueType)=> Add(new ConfigurationItem(key, value, valueType));
        /// <summary>
        /// Removes the <see cref="IConfigurationItem"/> with the specified key.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationItem"/> to remove.</param>
        /// <returns><b>True</b> if the <see cref="IConfigurationItem"/> was removed; otherwise <b>false</b>. This method returns <b>false</b> if the key was not found.</returns>
        public bool Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (_Items.ContainsKey(key))
            {
                var dude = _Items[key];
                _Items.Remove(key);
                (dude as IConfigurationItemAdvanced).Parent = null;
                MarkDirty();
                RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, dude));
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Removes the specified <see cref="IConfigurationItem"/> from the collection.
        /// </summary>
        /// <param name="item">The <see cref="IConfigurationItem"/> to remove.</param>
        /// <returns><b>True</b> if the <see cref="IConfigurationItem"/> was removed; otherwise <b>false</b>. This method returns <b>false</b> if the key was not found.</returns>
        public bool Remove(IConfigurationItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return Remove(item.Key);
        }
        /// <summary>
        /// Removes all <see cref="IConfigurationItem"/>s from this collection.
        /// </summary>
        public void Clear()
        {
            foreach (var item in _Items)
            {
                (item.Value as IConfigurationItemAdvanced).Parent = null;
            }
            _Items.Clear();
            MarkDirty();
            RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        /// <summary>
        /// Copies the <see cref="IConfigurationItem"/>s to an <see cref="Array"/>, starting at the specified <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the <see cref="IConfigurationItem"/>s. The <see cref="Array"/> must have zero-based indexing and be large enough to contain all the <see cref="IConfigurationItem"/>s in this collection.</param>
        /// <param name="arrayIndex">The zero-based index in <see cref="Array"/> at which copying begins.</param>
        public void CopyTo(IConfigurationItem[] array, int arrayIndex)
        {
            foreach (var item in _Items.Values)
            {
                array[arrayIndex++] = item;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through this collection.</returns>
        public IEnumerator<IConfigurationItem> GetEnumerator()
        {
            foreach (var item in from x in _Items.Values
                                 orderby x.Key ascending
                                 select x)
            {
                yield return item;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through this collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()=> GetEnumerator();
        /// <summary>
        /// Change state tracking.
        /// </summary>
        public Common.IStateChangeView StateChange
        {
            get
            {
                var tempState = new Common.StateChangeTracking(_StateChangeTracker);
                foreach (var item in _Items)
                {
                    if (item.Value.StateChange.IsNew)
                    {
                        tempState.MarkNew();
                    }
                    if (item.Value.StateChange.IsDirty)
                    {
                        tempState.MarkDirty();
                    }
                }
                return tempState;
            }
        }
        /// <summary>
        /// Clears the Dirty flags.
        /// </summary>
        public void ClearDirty()=>    _StateChangeTracker.ClearDirty();
        /// <summary>
        /// Clears the New flags.
        /// </summary>
        public void ClearNew()
        {
            _StateChangeTracker.ClearNew();
        }
        /// <summary>
        /// Sets the Dirty flags.
        /// </summary>
        public void MarkDirty()=>_StateChangeTracker.MarkDirty();
        /// <summary>
        /// Sets the New flags.
        /// </summary>
        public void MarkNew()=>_StateChangeTracker.MarkNew();
        #endregion
        #region IConfigurationItemsAdvanced Methods
        /// <summary>
        /// Sets the parent <see cref="IConfigurationGroup"/> for this <see cref="IConfigurationItems"/>, or set to null, if this <see cref="IConfigurationItems"/> has no parent.
        /// </summary>        
        IConfigurationGroup IConfigurationItemsAdvanced.Parent
        {
            set
            {
                Parent = value;
            }
        }
        #endregion
    }
}

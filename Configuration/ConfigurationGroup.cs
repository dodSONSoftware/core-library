using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// <para>Represents a group of configuration items and sub-groups.</para>
    /// <para>Each keyed <see cref="IConfigurationGroup"/> contains a collection of <see cref="IConfigurationItem"/>s and is itself a collection of <see cref="IConfigurationGroup"/>s.</para>
    /// </summary>
    /// <example>
    /// <para>The following example will create a <see cref="ConfigurationGroup"/>, add some <see cref="ConfigurationItem"/>s and <see cref="ConfigurationGroup"/>s, serializes it into INI and XML formats and displays the results.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// // create a configuration
    /// dodSON.Core.Configuration.IConfigurationGroup configuration = new dodSON.Core.Configuration.ConfigurationGroup();
    /// 
    /// // add some items
    /// configuration.Items.Add("Date Created", DateTime.Now);
    /// configuration.Items.Add("User Name", Environment.UserName);
    /// 
    /// // add a sub group
    /// configuration.Add("Integers");
    /// 
    /// // add items to the new sub group
    /// configuration["Integers"].Items.Add("byte", (byte)128);
    /// configuration["Integers"].Items.Add("int", (int)7);
    /// configuration["Integers"].Items.Add("long", (long)49);
    /// 
    /// // add a sub group
    /// configuration.Add("RationalNumbers");
    /// 
    /// // add items to the new sub group
    /// configuration["RationalNumbers"].Items.Add("float", (float)3.1415927);
    /// configuration["RationalNumbers"].Items.Add("double", (double)2.71828182845905);
    /// configuration["RationalNumbers"].Items.Add("decimal", (decimal)2.4142135);
    /// 
    /// // create INI serializer
    /// dodSON.Core.Configuration.IniConfigurationSerializer iniSerializer = new dodSON.Core.Configuration.IniConfigurationSerializer();
    ///
    /// // serialize into INI
    /// StringBuilder iniString = iniSerializer.SerializeGroup(configuration);
    ///
    /// // output INI
    /// Console.WriteLine("--------------------------------");
    /// Console.WriteLine(iniString);
    /// 
    /// // create XML serializer
    /// dodSON.Core.Configuration.XmlConfigurationSerializer xmlSerializer = new dodSON.Core.Configuration.XmlConfigurationSerializer();
    /// 
    /// // serialize into XML
    /// StringBuilder xmlString = xmlSerializer.SerializeGroup(configuration);
    ///
    /// // output XML
    /// Console.WriteLine("--------------------------------");
    /// Console.WriteLine(xmlString);
    ///
    /// // ----
    /// Console.WriteLine("--------------------------------");
    /// Console.WriteLine("press anykey&gt;");
    /// Console.ReadKey(true);
    ///
    /// // This code produces output similar to the following:
    ///
    /// // --------------------------------
    /// // Date Created\System.DateTime\2016-11-10 12:05:24 AM
    /// // User Name\System.String\User
    /// // [Integers]
    /// //         byte\System.Byte\128
    /// //         int\System.Int32\7
    /// //         long\System.Int64\49
    /// // [RationalNumbers]
    /// //         decimal\System.Decimal\2.4142135
    /// //         double\System.Double\2.71828182845905
    /// //         float\System.Single\3.141593
    /// // 
    /// // --------------------------------
    /// // &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// // &lt;group key=""&gt;
    /// //   &lt;items&gt;
    /// //     &lt;item key="Date Created" type="System.DateTime"&gt;2016-11-10 12:05:24 AM&lt;/item&gt;
    /// //     &lt;item key="User Name" type="System.String"&gt;User&lt;/item&gt;
    /// //   &lt;/items&gt;
    /// //   &lt;groups&gt;
    /// //     &lt;group key="Integers"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="byte" type="System.Byte"&gt;128&lt;/item&gt;
    /// //         &lt;item key="int" type="System.Int32"&gt;7&lt;/item&gt;
    /// //         &lt;item key="long" type="System.Int64"&gt;49&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //     &lt;group key="RationalNumbers"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="decimal" type="System.Decimal"&gt;2.4142135&lt;/item&gt;
    /// //         &lt;item key="double" type="System.Double"&gt;2.71828182845905&lt;/item&gt;
    /// //         &lt;item key="float" type="System.Single"&gt;3.141593&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //   &lt;/groups&gt;
    /// // &lt;/group&gt;
    /// // --------------------------------
    /// // press anykey&gt;
    /// </code>
    /// </example>
    [Serializable]
    public class ConfigurationGroup
         : IConfigurationGroup,
           IConfigurationGroupAdvanced
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
        /// Initializes a new instance of the ConfigurationGroup class with the <see cref="Key"/> set to <see cref="string.Empty"/>.
        /// </summary>
        private ConfigurationGroup()
        {
        }
        /// <summary>
        /// Initializes a new instance of the ConfigurationGroup class with the specified key.
        /// </summary>
        /// <param name="key">A key which uniquely identifies this configuration group.</param>
        public ConfigurationGroup(string key) : this(key, false) { }
        /// <summary>
        /// Initializes a new instance of the ConfigurationGroup class with the specified key.
        /// </summary>
        /// <param name="key">A key which uniquely identifies this configuration group.</param>
        /// <param name="ignoreSubGroups">True stops an exception from being thrown if the <paramref name="key"/> contains a Group Name Separator character; false and an exception will be thrown if the <paramref name="key"/> contains a Group Name Separator character.</param>
        public ConfigurationGroup(string key, bool ignoreSubGroups)
            : this()
        {
            if (key == null)
            {
                key = "";
            }
            if ((!ignoreSubGroups) && (key.Contains(ConfigurationHelper.GroupNameSeparator)))
            {
                throw new ArgumentOutOfRangeException(nameof(key), $"Parameter {nameof(key)} cannot contain the Group Name Separator character: value='{ConfigurationHelper.GroupNameSeparator}'");
            }
            Key = key;
            Items = new ConfigurationItems(this);
        }
        /// <summary>
        /// Initializes a new instance of the ConfigurationGroup class with the given <see cref="IConfigurationItems"/> and the <see cref="Key"/> set to the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="items">A collection of <see cref="IConfigurationItem"/>s.</param>
        /// <param name="key">A key which uniquely identifies this configuration group.</param>
        public ConfigurationGroup(IConfigurationItems items, string key)
        : this()
        {
            if (key == null)
            {
                key = "";
            }
            Key = key;
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
        #endregion
        #region Private Fields
        private readonly Dictionary<string, IConfigurationGroup> _SubGroups = new Dictionary<string, IConfigurationGroup>();
        private readonly Common.IStateChangeTracking _InternalStateChangeTracker = new Common.StateChangeTracking(false, true, false);
        #endregion
        #region IConfigurationGroup Methods
        /// <summary>
        /// Returns the parent <see cref="IConfigurationGroup"/> for this <see cref="IConfigurationGroup"/>, or null, if this <see cref="IConfigurationGroup"/> has no parent.
        /// </summary>
        public IConfigurationGroup Parent
        {
            get; private set;
        }
        /// <summary>
        /// The hierarchy of groups this group belongs to.
        /// </summary>
        public string GroupAddress
        {
            get
            {
                var result = Key;
                var dude = Parent;
                while (dude != null)
                {
                    if (!string.IsNullOrWhiteSpace(dude.Key))
                    {
                        result = dude.Key + ConfigurationHelper.GroupNameSeparator + result;
                    }
                    dude = dude.Parent;
                }
                return result;
            }
        }
        /// <summary>
        /// A key which uniquely identifies this configuration group.
        /// </summary>
        public string Key { get; private set; } = "";
        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Gets the number of sub-groups contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return _SubGroups.Count;
            }
        }
        /// <summary>
        /// Gets or sets the <see cref="IConfigurationGroup"/> associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationGroup"/> to get or set.</param>
        /// <returns>The <see cref="IConfigurationGroup"/> associated with the specified key.</returns>
        public IConfigurationGroup this[string key]
        {
            get
            {
                if (!_SubGroups.ContainsKey(key))
                {
                    throw new ArgumentOutOfRangeException($"Configuration Group not found in this group's sub-groups; (key={key}).", nameof(key));
                }
                return _SubGroups[key];
            }
            set
            {
                if (_SubGroups.ContainsKey(key))
                {
                    Remove(key);
                }
                Add(value);
            }
        }
        /// <summary>
        /// Returns whether an <see cref="ConfigurationGroup"/>  with the specified <paramref name="key"/> exists.
        /// </summary>
        /// <param name="key">The key to search each <see cref="ConfigurationGroup"/> for.</param>
        /// <returns><b>True</b> if an <see cref="ConfigurationGroup"/> with the specified <paramref name="key"/> is found; otherwise, <b>false</b>.</returns>
        public bool ContainsKey(string key)
        {
            return _SubGroups.ContainsKey(key);
        }
        /// <summary>
        /// Determines whether the collection contains a specific <see cref="IConfigurationGroup"/>.
        /// </summary>
        /// <param name="item">The <see cref="IConfigurationGroup"/> to locate.</param>
        /// <returns><b>True</b> if the <see cref="IConfigurationGroup"/> is found; otherwise, <b>false</b>.</returns>
        public bool Contains(IConfigurationGroup item)
        {
            return _SubGroups.ContainsKey(item.Key);
        }
        /// <summary>
        /// Adds the <see cref="IConfigurationGroup"/> to the collection.
        /// </summary>
        /// <param name="item">The <see cref="IConfigurationGroup"/> to add.</param>
        /// <returns>The added <see cref="IConfigurationGroup"/>.</returns>
        public IConfigurationGroup Add(IConfigurationGroup item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            IConfigurationGroup oldItem = null;
            if (Contains(item))
            {
                oldItem = _SubGroups[item.Key];
                Remove(oldItem);
                (oldItem as IConfigurationGroupAdvanced).Parent = null;
            }
            _SubGroups.Add(item.Key, item);
            (item as IConfigurationGroupAdvanced).Parent = this;
            MarkDirty();
            if (oldItem == null)
            {
                RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
            else
            {
                RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem));
            }
            return item;
        }
        /// <summary>
        /// Creates an <see cref="IConfigurationGroup"/> using the specified key and adds it to this collection.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationGroup"/> to add.</param>
        /// <returns>The new <see cref="IConfigurationGroup"/>.</returns>
        public IConfigurationGroup Add(string key) => Add(new ConfigurationGroup(key));
        /// <summary>
        /// Adds the <see cref="IConfigurationGroup"/> to the collection.
        /// </summary>
        /// <param name="item">The <see cref="IConfigurationGroup"/> to add.</param>
        void ICollection<IConfigurationGroup>.Add(IConfigurationGroup item) => Add(item);
        /// <summary>
        /// Removes the specified <see cref="IConfigurationGroup"/> from the collection.
        /// </summary>
        /// <param name="item">The <see cref="IConfigurationGroup"/> to remove.</param>
        /// <returns><b>True</b> if the <see cref="IConfigurationGroup"/> was removed; otherwise <b>false</b>. This method returns <b>false</b> if the key was not found.</returns>
        public bool Remove(IConfigurationGroup item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return Remove(item.Key);
        }
        /// <summary>
        /// Removes the <see cref="IConfigurationGroup"/> with the specified key.
        /// </summary>
        /// <param name="key">The key of the <see cref="IConfigurationGroup"/> to remove.</param>
        /// <returns><b>True</b> if the <see cref="IConfigurationGroup"/> was removed; otherwise <b>false</b>. This method returns <b>false</b> if the key was not found.</returns>
        public bool Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (_SubGroups.ContainsKey(key))
            {
                var dude = _SubGroups[key];
                _SubGroups.Remove(key);
                (dude as IConfigurationGroupAdvanced).Parent = null;
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
        /// Removes all <see cref="IConfigurationGroup"/>s from this collection.
        /// </summary>
        public void Clear()
        {
            foreach (var item in _SubGroups)
            {
                (item.Value as IConfigurationGroupAdvanced).Parent = null;
            }
            _SubGroups.Clear();
            MarkDirty();
            RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        /// <summary>
        /// Copies the <see cref="IConfigurationGroup"/>s to an <see cref="Array"/>, starting at the specified <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the <see cref="IConfigurationGroup"/>s. The <see cref="Array"/> must have zero-based indexing and be large enough to contain all the <see cref="IConfigurationGroup"/>s in this collection.</param>
        /// <param name="arrayIndex">The zero-based index in <see cref="Array"/> at which copying begins.</param>
        public void CopyTo(IConfigurationGroup[] array, int arrayIndex)
        {
            foreach (var item in _SubGroups.Values)
            {
                array[arrayIndex++] = item;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through this collection.</returns>
        public IEnumerator<IConfigurationGroup> GetEnumerator()
        {
            foreach (var item in from x in _SubGroups.Values
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
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Returns the collection of <see cref="IConfigurationItem"/>s associated with the <see cref="IConfigurationGroup"/>.
        /// </summary>
        public IConfigurationItems Items
        {
            get;
        }
        /// <summary>
        /// Change state tracking.
        /// </summary>
        public Common.IStateChangeView StateChange
        {
            get
            {
                var tempState = new Common.StateChangeTracking(_InternalStateChangeTracker);
                var itemSC = Items.StateChange;
                if (itemSC.IsNew)
                {
                    tempState.MarkNew();
                }
                if (itemSC.IsDirty)
                {
                    tempState.MarkDirty();
                }
                foreach (var subGroup in _SubGroups)
                {
                    var subGroupState = subGroup.Value.StateChange;
                    if (subGroupState.IsNew)
                    {
                        tempState.MarkNew();
                    }
                    if (subGroupState.IsDirty)
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
        public void ClearDirty()
        {
            _InternalStateChangeTracker.ClearDirty();
            Items.ClearDirty();
            foreach (var subGroup in _SubGroups)
            {
                subGroup.Value.ClearDirty();
            }
        }
        /// <summary>
        /// Clears the New flags.
        /// </summary>
        public void ClearNew()
        {
            _InternalStateChangeTracker.ClearNew();
            Items.ClearNew();
            foreach (var subGroup in _SubGroups)
            {
                subGroup.Value.ClearNew();
            }
        }
        /// <summary>
        /// Sets the Dirty flag.
        /// </summary>
        public void MarkDirty()
        {
            _InternalStateChangeTracker.MarkDirty();
            Items.MarkDirty();
            foreach (var subGroup in _SubGroups)
            {
                subGroup.Value.MarkDirty();
            }
        }
        /// <summary>
        /// Sets the New flag.
        /// </summary>
        public void MarkNew()
        {
            _InternalStateChangeTracker.MarkNew();
            Items.MarkNew();
            foreach (var subGroup in _SubGroups)
            {
                subGroup.Value.MarkNew();
            }
        }
        /// <summary>
        /// Gets a comma-separated set of key, value pairs representing the name and types of each item in this group.
        /// </summary>
        public string ItemsSignature
        {
            get
            {
                var result = new System.Text.StringBuilder();
                //
                foreach (var item in Items)
                {
                    result.Append($"{item.Key}={item.ValueType},");
                }
                if (result.Length > 0)
                {
                    --result.Length;
                }
                //
                return result.ToString();
            }
        }
        #endregion
        #region IConfigurationGroupAdvanced Methods
        void IConfigurationGroupAdvanced.SetKey(string key)
        {
            if (key == null)
            {
                key = "";
            }
            if (key.Contains('.'))
            {
                throw new ArgumentOutOfRangeException(nameof(key), $"Parameter {nameof(key)} cannot contain the period character: char='.' ascii hex=(0x2E)");
            }
            Key = key;
        }
        /// <summary>
        /// Sets the parent <see cref="IConfigurationGroup"/> for this <see cref="IConfigurationGroup"/>, or set to null, if this <see cref="IConfigurationGroup"/> has no parent.
        /// </summary>
        IConfigurationGroup IConfigurationGroupAdvanced.Parent
        {
            set
            {
                Parent = value;
            }
        }
        #endregion
    }
}

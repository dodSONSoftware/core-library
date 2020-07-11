using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Represents a single, self-typed, item within the configuration system.
    /// </summary>
    /// <example>
    /// <para>
    /// The following example will create a configuration, add some <i>standard</i> and <i>custom</i> items to it, serialize it into INI and output it.
    /// It will then deserialize that INI into a new group and display the deserialized items, including the CustomType. Finally, it will serialize the new group into XML and output it.
    /// </para> 
    /// <para>
    /// When a serializer, either the <see cref="IniConfigurationSerializer"/> or the <see cref="XmlConfigurationSerializer"/>, comes across a type it does not recognizes, it will serialize that type using standard .NET serialization and compression; by default it will use a <see cref="dodSON.Core.Compression.DeflateStreamCompressor"/>.
    /// </para>
    /// <para>First create the following class:</para>
    /// <code>
    /// [Serializable]
    /// private class CustomType
    /// {
    ///     public string ModuleName { get; set; }
    ///     public DateTime LastStartDate { get; set; }
    ///     public TimeSpan TimeSinceLastStartup { get { return DateTime.Now - LastStartDate; } }
    /// }
    /// </code>
    /// <para>Next, create a console application and add the following code:</para>
    /// <code>
    /// // create a configuration
    /// dodSON.Core.Configuration.IConfigurationGroup configuration = new dodSON.Core.Configuration.ConfigurationGroup();
    /// 
    /// // populate with some (standard) items
    /// configuration.Items.Add("CurrentDate", DateTime.Now);
    /// configuration.Items.Add("UserName", Environment.UserName);
    /// 
    /// // populate with custom items
    /// configuration.Items.Add("CustomType", new CustomType() { ModuleName = "Test", LastStartDate = DateTime.Now.AddSeconds(-12802560) });
    /// 
    /// // create INI converter
    /// dodSON.Core.Configuration.IniConfigurationSerializer iniConverter = new dodSON.Core.Configuration.IniConfigurationSerializer();
    /// 
    /// // serialize to INI
    /// StringBuilder iniString = iniConverter.SerializeGroup(configuration);
    /// 
    /// // output INI
    /// Console.WriteLine("--- INI -----------------------------");
    /// Console.WriteLine(iniString);
    /// 
    /// // deserialize from INI
    /// dodSON.Core.Configuration.IConfigurationGroup newGroup = iniConverter.DeserializeGroup(iniString);
    /// 
    /// // display info (returned from the serialized INI)
    /// Console.WriteLine();
    /// Console.WriteLine("--- NEW GROUP DATA ------------------");
    /// Console.WriteLine($"{newGroup.Items["CurrentDate"].Key}={newGroup.Items["CurrentDate"].Value}");
    /// Console.WriteLine($"{newGroup.Items["UserName"].Key}={newGroup.Items["UserName"].Value}");
    /// CustomType cType = (CustomType)newGroup.Items["CustomType"].Value;
    /// Console.WriteLine($"CustomType:ModuleName={cType.ModuleName}");
    /// Console.WriteLine($"CustomType:LastStartDate={cType.LastStartDate}");
    /// Console.WriteLine($"CustomType:TimeSinceLastStartup={cType.TimeSinceLastStartup}");
    /// 
    /// // create XML converter
    /// dodSON.Core.Configuration.XmlConfigurationSerializer xmlConverter = new dodSON.Core.Configuration.XmlConfigurationSerializer();
    /// 
    /// // serialize to XML
    /// StringBuilder xmlString = xmlConverter.SerializeGroup(newGroup);
    /// 
    /// // output XML
    /// Console.WriteLine();
    /// Console.WriteLine("--- XML -----------------------------");
    /// Console.WriteLine(xmlString);
    /// 
    /// // ----
    /// Console.WriteLine("--------------------------------");
    /// Console.WriteLine("press anykey&gt;");
    /// Console.ReadKey(true);
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // --- INI -----------------------------
    /// // CurrentDate\System.DateTime\2016-11-11 4:12:43 PM
    /// // CustomType\Playground_Configuration.Program+CustomType, Playground_Configuration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\Y2BkYGD4DwQgGgR4mICEf0BOYmV6UX5pXkq8c35eWmZ6aVFiSWZ+no5CWGpRMZBha6hnAII6Cs6lOSWlRam2eamlJUWJOToKAaVJOZnJ3qmVIfnZqXm2eaU5Oawg07VxGaoXUJSfXpSYq+1cWlySnxtSWZAKcoS0jW9+SmlOql9ibqpddny8U2JydmZeultmak6KnI1PYnFJcEliUYlLYgmGNCMDL8gENmYgwRKSWlwivzPE8+7Uyx3cDGgAAA==
    /// // UserName\System.String\User
    /// // 
    /// // --- NEW GROUP DATA ------------------
    /// // CurrentDate=2016-11-11 4:12:43 PM
    /// // UserName=User
    /// // CustomType:ModuleName=Test
    /// // CustomType:LastStartDate=2016-06-16 11:56:43 AM
    /// // CustomType:TimeSinceLastStartup=148.04:16:00.0155009
    /// // 
    /// // --- XML -----------------------------
    /// // &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// // &lt;group key=""&gt;
    /// //   &lt;items&gt;
    /// //     &lt;item key="CurrentDate" type="System.DateTime"&gt;2016-11-11 4:12:43 PM&lt;/item&gt;
    /// //     &lt;item key="CustomType" type="Playground_Configuration.Program+CustomType, Playground_Configuration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"&gt;Y2BkYGD4DwQgGgR4mICEf0BOYmV6UX5pXkq8c35eWmZ6aVFiSWZ+no5CWGpRMZBha6hnAII6Cs6lOSWlRam2eamlJUWJOToKAaVJOZnJ3qmVIfnZqXm2eaU5Oawg07VxGaoXUJSfXpSYq+1cWlySnxtSWZAKcoS0jW9+SmlOql9ibqpddny8U2JydmZeultmak6KnI1PYnFJcEliUYlLYgmGNCMDL8gENmYgwRKSWlwivzPE8+7Uyx3cDGgAAA==&lt;/item&gt;
    /// //     &lt;item key="UserName" type="System.String"&gt;User&lt;/item&gt;
    /// //   &lt;/items&gt;
    /// // &lt;/group&gt;
    /// // --------------------------------
    /// // press anykey&gt;
    /// </code>
    /// </example>

    [Serializable]
    public class ConfigurationItem
        : IConfigurationItem,
          IConfigurationItemAdvanced,
          System.Runtime.Serialization.ISerializable
    {
        #region Public Events
        /// <summary>
        /// Occurs when the <see cref="Value"/> has been changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Will raise the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> to pass to the <see cref="PropertyChanged"/> event.</param>
        protected void RaisePropertyChangedEvent(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
        #endregion
        #region Ctor
        private ConfigurationItem()
        {
            StateChange = new Common.StateChangeTracking();
            StateChange.PropertyChanged += (object ss, PropertyChangedEventArgs ee) =>
            {
                RaisePropertyChangedEvent(new PropertyChangedEventArgs(ee.PropertyName));
                RaisePropertyChangedEvent(new PropertyChangedEventArgs(nameof(StateChange)));
            };
        }
        /// <summary>
        /// Initializes a new instance of the ConfigurationItem class using the specified key and value.
        /// </summary>
        /// <param name="key">A key which uniquely identifies this configuration item.</param>
        /// <param name="value">The object to associate with this configuration item.</param>
        /// <param name="valueType">The <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/> which represents the type of data this configuration item contains.</param>
        public ConfigurationItem(string key,
                                 object value,
                                 string valueType)
            : this()
        {
            // #### KEY
            Key = key;
            // #### VALUE
            if (value is Type)
            {
                if (ConfigurationShared._AllKnownTypes.Contains((value as Type)))
                {
                    Value = (value as Type).FullName;
                }
                else
                {
                    Value = (value as Type).AssemblyQualifiedName;
                }
            }
            else
            {
                Value = value;
            }
            // #### TYPE
            ValueType = valueType;
        }
        /// <summary>
        /// Initializes a new instance of the ConfigurationItem class using the specified key and value.
        /// </summary>
        /// <param name="key">A key which uniquely identifies this configuration item.</param>
        /// <param name="value">The object to associate with this configuration item.</param>
        /// <param name="valueType">The <see cref="Type"/> which represents the type of data this configuration item contains.</param>
        public ConfigurationItem(string key,
                                 object value,
                                 Type valueType)
            : this()
        {
            // #### KEY
            Key = key;
            // #### VALUE
            if (value is Type)
            {
                if (ConfigurationShared._AllKnownTypes.Contains((value as Type)))
                {
                    Value = (value as Type).FullName;
                }
                else
                {
                    Value = (value as Type).AssemblyQualifiedName;
                }
            }
            else
            {
                Value = value;
            }
            // #### TYPE
            if (ConfigurationShared._AllKnownTypes.Contains(valueType))
            {
                ValueType = valueType.FullName;
            }
            else if (valueType.AssemblyQualifiedName.StartsWith("System.RuntimeType"))
            {
                ValueType = typeof(Type).FullName;
            }
            else if (valueType.AssemblyQualifiedName.StartsWith("System."))
            {
                ValueType = typeof(Type).FullName;
            }
            else
            {
                ValueType = valueType.AssemblyQualifiedName;
            }
        }
        #endregion
        #region Private Fields
        private string _Key = "";
        private object _Value;
        private string _ValueType = "";
        #endregion
        #region IConfigurationItem Methods
        /// <summary>
        /// Get or sets the parent <see cref="IConfigurationGroup"/> for this <see cref="ConfigurationItem"/>; set to null if this <see cref="ConfigurationItem"/> has no parent.
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
                    result = dude.Key + ConfigurationHelper.GroupNameSeparator + result;
                    dude = dude.Parent;
                }
                return result;
            }
        }
        /// <summary>
        /// A key which uniquely identifies this configuration item.
        /// </summary>
        public string Key
        {
            get => _Key;
            private set
            {
                if (value == null)
                {
                    value = "";
                }
                if (_Key != value)
                {
                    StateChange.MarkDirty();
                }
                else
                {
                    StateChange.ClearDirty();
                }
                _Key = value;
                RaisePropertyChangedEvent(new PropertyChangedEventArgs(nameof(Key)));
            }
        }
        /// <summary>
        /// The <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/> which represents the type of data this configuration item contains.
        /// </summary>
        /// <seealso cref="Value"/>
        /// <see cref="TryGetValue{T}(out T)"/>
        public string ValueType
        {
            get => _ValueType;
            set
            {
                if (value == null)
                {
                    value = "";
                }
                if (_ValueType != value)
                {
                    StateChange.MarkDirty();
                }
                else
                {
                    StateChange.ClearDirty();
                }
                _ValueType = value;
                RaisePropertyChangedEvent(new PropertyChangedEventArgs(nameof(ValueType)));
            }
        }
        /// <summary>
        /// Gets and sets object associated with this configuration item.
        /// </summary>
        public object Value
        {
            get => _Value;
            set
            {
                if (_Value != value)
                {
                    StateChange.MarkDirty();
                }
                else
                {
                    StateChange.ClearDirty();
                }
                _Value = value;
                RaisePropertyChangedEvent(new PropertyChangedEventArgs(nameof(Value)));
            }
        }
        /// <summary>
        /// Change state tracking.
        /// </summary>
        public Common.IStateChangeTracking StateChange
        {
            get;
        }
        /// <summary>
        /// Gets the object associated with this configuration item.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to try and cast this <see cref="Value"/> to.</typeparam>
        /// <param name="value">The <b>out</b> variable populated with the cast object, or null.</param>
        /// <returns><b>True</b> if the <see cref="Value"/> was successfully cast to type <typeparamref name="T"/>; otherwise, <b>false</b>.</returns>
        public bool TryGetValue<T>(out T value)
        {
            try
            {
                value = (T)Convert.ChangeType(Value, typeof(T));
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }
        #endregion
        #region IConfigurationGroupAdvanced Methods
        /// <summary>
        /// Sets the parent <see cref="IConfigurationGroup"/> for this <see cref="ConfigurationItem"/>, or set to null, if this <see cref="ConfigurationItem"/> has no parent.
        /// </summary>
        IConfigurationGroup IConfigurationItemAdvanced.Parent
        {
            set => Parent = value;
        }
        /// <summary>
        /// Sets the key for this item.
        /// </summary>
        string IConfigurationItemAdvanced.Key
        {
            set => Key = value;
        }
        #endregion
        #region System.Runtime.Serialization.ISerializable Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationItem"/> class.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provide an additional caller-defined context.</param>
        protected ConfigurationItem(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : this()
        {
            Key = info.GetString("Key");
            ValueType = info.GetString("ValueType");
            Value = info.GetValue("Obj", typeof(object));
            StateChange = (Common.IStateChangeTracking)info.GetValue("State", typeof(Common.IStateChangeTracking));
        }
        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and returns the data needed to serialize the state for this instance.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object that contains the information required to serialize the state instance.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> that contains the source and destination of the serialized stream associated with the state instance.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", Key, typeof(string));
            info.AddValue("ValueType", ValueType, typeof(string));
            info.AddValue("Obj", Value, typeof(object));
            info.AddValue("State", StateChange, typeof(Common.IStateChangeTracking));
        }
        #endregion
    }
}

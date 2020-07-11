using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Configuration information for a server.
    /// </summary>
    [Serializable]
    public class ServerConfiguration
        : IServerConfiguration
    {
        #region Ctor
        private ServerConfiguration()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ServerConfiguration"/> with <see cref="OverrideTypesFilter"/>s.
        /// </summary>
        /// <param name="id">The server's unique id.</param>
        /// <param name="overrideTypesFilter">A collection of <see cref="IPayloadTypeInfo"/> representing the types this server will send to clients regardless of the client's <see cref="IClientConfiguration.ReceivableTypesFilter"/>.</param>
        public ServerConfiguration(string id,
                                   IEnumerable<IPayloadTypeInfo> overrideTypesFilter)
            : this()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }
            Id = id;
            ReplaceOverrideTypesFilter(overrideTypesFilter);
        }
        /// <summary>
        /// Instantiates a new <see cref="ServerConfiguration"/>.
        /// </summary>
        /// <param name="id">The server's unique id.</param>
        public ServerConfiguration(string id) : this(id, null) { }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public ServerConfiguration(Configuration.IConfigurationGroup configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "ServerConfiguration")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"ServerConfiguration\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // Id
            Id = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Id", typeof(string)).Value;
            // list: OverrideTypesFilter
            var overrideFilterList = new List<IPayloadTypeInfo>();
            var overrideTypeFilterGroup = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "OverrideTypesFilter", true);
            foreach (var item in overrideTypeFilterGroup)
            {
                ((Configuration.IConfigurationGroupAdvanced)item).SetKey("PayloadTypeInfo");
                overrideFilterList.Add((IPayloadTypeInfo)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(PayloadTypeInfo), item));
            }
            if (overrideFilterList.Count > 0)
            {
                ReplaceOverrideTypesFilter(overrideFilterList);
            }
        }
        #endregion
        #region Private Fields
        private string _Id = "";
        #endregion
        #region ICommunicationServerConfiguration Methods
        /// <summary>
        /// The server's unique id.
        /// </summary>
        public string Id
        {
            get => _Id;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _Id = value.Replace(",", "");
            }
        }
        /// <summary>
        /// A collection of <see cref="IPayloadTypeInfo"/> representing the types this server will send to clients regardless of the client's <see cref="IClientConfiguration.ReceivableTypesFilter"/>.
        /// </summary>
        public IEnumerable<IPayloadTypeInfo> OverrideTypesFilter { get; private set; } = new List<IPayloadTypeInfo>();
        /// <summary>
        /// Will replace the <see cref="OverrideTypesFilter"/>.
        /// </summary>
        /// <param name="list">The new collection of <see cref="IPayloadTypeInfo"/> representing the types this server will send to clients regardless of the client's <see cref="IClientConfiguration.ReceivableTypesFilter"/>.</param>
        public void ReplaceOverrideTypesFilter(IEnumerable<IPayloadTypeInfo> list)
        {
            if (list != null)
            {
                OverrideTypesFilter = new List<IPayloadTypeInfo>(list);
            }
        }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("ServerConfiguration");
                // Id
                result.Items.Add("Id", Id, Id.GetType());
                // list: OverrideTypesFilter
                var overrideTypesFilter = result.Add("OverrideTypesFilter");
                var count = 0;
                foreach (var item in OverrideTypesFilter)
                {
                    // clone the item, change its key, save clone to dependencies list
                    // you do not want to change the key in the original item
                    var clone = Converters.ConvertersHelper.Clone(item.Configuration);
                    ((Configuration.IConfigurationGroupAdvanced)clone).SetKey($"PayloadTypeInfo {++count}");
                    overrideTypesFilter.Add(clone);
                }
                //
                return result;
            }
        }
        #endregion
    }
}

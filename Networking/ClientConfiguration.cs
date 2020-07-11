using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Represents configuration information for a client.
    /// </summary>
    [Serializable]
    public class ClientConfiguration
        : IClientConfiguration
    {
        #region Ctor
        private ClientConfiguration()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ClientConfiguration"/>.
        /// </summary>
        /// <param name="id">The client's unique id.</param>
        /// <param name="receiveSelfSentMessages">Determines if a client will receives messages that it send. When <b>true</b> messages sent by the client will be received on the message bus; otherwise, no messages sent by the client will be received by the client.</param>
        /// <param name="receivableTypesFilter">A collection of <see cref="IPayloadTypeInfo"/> representing the types this client will accept.</param>
        /// <param name="transmittableTypesFilter">A collection of <see cref="IPayloadTypeInfo"/> representing the types this client will send.</param>
        public ClientConfiguration(string id,
                                   bool receiveSelfSentMessages,
                                   IEnumerable<IPayloadTypeInfo> receivableTypesFilter,
                                   IEnumerable<IPayloadTypeInfo> transmittableTypesFilter)
            : this()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            Id = id;
            ReceiveSelfSentMessages = receiveSelfSentMessages;
            ReplaceReceivableTypesFilter(receivableTypesFilter);
            ReplaceTransmittableTypesFilter(transmittableTypesFilter);
        }
        /// <summary>
        /// Instantiates a new <see cref="ClientConfiguration"/> without a <see cref="ReceivableTypesFilter"/> or a <see cref="TransmittableTypesFilter"/>.
        /// </summary>
        /// <remarks>
        /// Using <b>null</b> for the <see cref="ReceivableTypesFilter"/> means no filter; that is, it will allow all messages through. Excepting, of course, self-sent messages, which is based on <see cref="ReceiveSelfSentMessages"/> as well.
        /// </remarks>
        /// <param name="id">The client's unique id.</param>
        /// <param name="receiveSelfSentMessages">Determines if a client will receives messages that it send. When <b>true</b> messages sent by the client will be received on the message bus; otherwise, no messages sent by the client will be received by the client.</param>
        public ClientConfiguration(string id, bool receiveSelfSentMessages) : this(id, receiveSelfSentMessages, null, null) { }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public ClientConfiguration(Configuration.IConfigurationGroup configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "ClientConfiguration")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"ClientConfiguration\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // Id
            Id = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Id", typeof(string)).Value;
            // ReceiveSelfSentMessages
            ReceiveSelfSentMessages = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ReceiveSelfSentMessages", typeof(bool)).Value;
            // list: ReceivableTypesFilter
            var receivableTypesFilterList = new List<IPayloadTypeInfo>();
            var receivableTypesFilterGroup = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "ReceivableTypesFilter", false);
            if (receivableTypesFilterGroup != null)
            {
                foreach (var item in receivableTypesFilterGroup)
                {
                    ((Configuration.IConfigurationGroupAdvanced)item).SetKey("PayloadTypeInfo");
                    receivableTypesFilterList.Add((IPayloadTypeInfo)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(PayloadTypeInfo), item));
                }
                if (receivableTypesFilterList.Count > 0)
                {
                    ReplaceReceivableTypesFilter(receivableTypesFilterList);
                }
            }
            // list: TransmittableTypesFilter
            var transmittableTypesFilterList = new List<IPayloadTypeInfo>();
            var transmittableTypesFilterGroup = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "TransmittableTypesFilter", false);
            if (transmittableTypesFilterGroup != null)
            {
                foreach (var item in transmittableTypesFilterGroup)
                {
                    ((Configuration.IConfigurationGroupAdvanced)item).SetKey("PayloadTypeInfo");
                    transmittableTypesFilterList.Add((IPayloadTypeInfo)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(PayloadTypeInfo), item));
                }
                if (transmittableTypesFilterList.Count > 0)
                {
                    ReplaceTransmittableTypesFilter(transmittableTypesFilterList);
                }
            }
        }
        #endregion
        #region Private Fields
        private string _Id = "";
        #endregion
        #region Public Methods
        /// <summary>
        /// The client's unique id.
        /// </summary>
        public string Id
        {
            get => _Id;
            private set => _Id = value.Replace(",", "");
        }
        /// <summary>
        /// Determines if a client will receives messages that it send. When <b>true</b> messages sent by the client will be received on the message bus; otherwise, no messages sent by the client will be received by the client.
        /// </summary>
        public bool ReceiveSelfSentMessages
        {
            get; private set;
        }
        /// <summary>
        /// A collection of <see cref="IPayloadTypeInfo"/> representing the types this client will accept.
        /// </summary>
        public IEnumerable<IPayloadTypeInfo> ReceivableTypesFilter { get; private set; } = new List<Networking.IPayloadTypeInfo>();
        /// <summary>
        /// Will replace the <see cref="ReceivableTypesFilter"/>.
        /// </summary>
        /// <param name="list">The new collection of <see cref="IPayloadTypeInfo"/> representing the types this client will accept.</param>
        public void ReplaceReceivableTypesFilter(IEnumerable<IPayloadTypeInfo> list)
        {
            if (list != null)
            {
                ReceivableTypesFilter = new List<IPayloadTypeInfo>(list);
            }
        }
        /// <summary>
        /// A collection of <see cref="IPayloadTypeInfo"/> representing the types this client will send.
        /// </summary>
        public IEnumerable<IPayloadTypeInfo> TransmittableTypesFilter { get; private set; } = new List<Networking.IPayloadTypeInfo>();
        /// <summary>
        /// Will replace the <see cref="TransmittableTypesFilter"/>.
        /// </summary>
        /// <param name="list">The new collection of <see cref="IPayloadTypeInfo"/> representing the types this client will send.</param>
        public void ReplaceTransmittableTypesFilter(IEnumerable<IPayloadTypeInfo> list)
        {
            if (list != null)
            {
                TransmittableTypesFilter = new List<IPayloadTypeInfo>(list);
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
                var result = new Configuration.ConfigurationGroup("ClientConfiguration");
                // Id
                result.Items.Add("Id", Id, Id.GetType());
                // ReceiveSelfSentMessages
                result.Items.Add("ReceiveSelfSentMessages", ReceiveSelfSentMessages, ReceiveSelfSentMessages.GetType());
                // list: ReceivableTypesFilter
                var receivableTypesFilterList = result.Add("ReceivableTypesFilter");
                var count = 0;
                foreach (var item in ReceivableTypesFilter)
                {
                    // clone the item, change its key, save clone to dependencies list
                    // you do not want to change the key in the original item
                    var clone = Converters.ConvertersHelper.Clone(item.Configuration);
                    ((Configuration.IConfigurationGroupAdvanced)clone).SetKey($"PayloadTypeInfo {++count}");
                    receivableTypesFilterList.Add(clone);
                }
                // list: TransmittableTypesFilter
                var transmittableTypesFilterList = result.Add("TransmittableTypesFilter");
                count = 0;
                foreach (var item in TransmittableTypesFilter)
                {
                    // clone the item, change its key, save clone to dependencies list
                    // you do not want to change the key in the original item
                    var clone = Converters.ConvertersHelper.Clone(item.Configuration);
                    ((Configuration.IConfigurationGroupAdvanced)clone).SetKey($"PayloadTypeInfo {++count}");
                    transmittableTypesFilterList.Add(clone);
                }
                //
                return result;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking.Http
{
    /// <summary>
    /// Provides a <a href="https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/duplex-services?view=netframework-4.7">.Net WCF Duplex Services</a> implementation of the <see cref="ServerBase"/>.
    /// </summary>
    [Serializable]
    public class Server
        : BasicServerBase
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="Server"/>.
        /// </summary>
        public Server() : base() { }
        /// <summary>
        /// Instantiates a new <see cref="Server"/>.
        /// </summary>
        /// <param name="channelAddress">The client's address.</param>
        /// <param name="configuration">The client's configuration.</param>
        /// <param name="transportController">Controls the transportation layer.</param>
        /// <param name="logSourceId">The value used for the <see cref="Logging.ILogEntry.SourceId"/> when generating <see cref="Logging.ILog"/>s.</param>
        public Server(IChannelAddress channelAddress,
                      IServerConfiguration configuration,
                      ITransportController transportController,
                      string logSourceId) : base(channelAddress, configuration, transportController, logSourceId)
        {
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public Server(Configuration.IConfigurationGroup configuration) : base(configuration) { }
        #endregion
        #region SimpleServer Methods
        /// <summary>
        /// The channel's address converted into a <see cref="Uri"/>.
        /// </summary>
        public override Uri AddressUri => (new UriBuilder(string.Format("http://{0}:{1}/{2}", base.Address.IPAddress, base.Address.Port, base.Address.Name))).Uri;
        /// <summary>
        /// The <see cref="System.ServiceModel.Channels.Binding"/> used for this network resource.
        /// </summary>
        protected override System.ServiceModel.Channels.Binding Binding => new System.ServiceModel.WSDualHttpBinding(System.ServiceModel.WSDualHttpSecurityMode.None);
        /// <summary>
        /// A new <see cref="IClient"/> required by the server to use as its internal client. (The client is not cached; every call will create a new client.)
        /// </summary>
        /// <param name="clientName">The name of the new client.</param>
        /// <param name="receiveSelfSentMessages">Determines if a client will receives messages that it send. When <b>true</b> messages sent by the client will be received on the message bus; otherwise, no messages sent by the client will be received by the client.</param>
        /// <returns>A new client. (Do not cache the client; every call should create a new client.)</returns>
        protected override IClient Client(string clientName,
                                          bool receiveSelfSentMessages) => new Client(base.Address,
                                                                                      new ClientConfiguration(clientName, receiveSelfSentMessages),
                                                                                      base.TransportController.RegistrationController);
        #endregion
    }
}

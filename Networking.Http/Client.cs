﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking.Http
{
    /// <summary>
    /// Provides a <a href="https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/duplex-services?view=netframework-4.7">.Net WCF Duplex Services</a> implementation of the <see cref="ClientBase"/>.
    /// </summary>
    [Serializable]
    public class Client
        : BasicClientBase
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="Client"/>.
        /// </summary>
        public Client() : base() { }
        /// <summary>
        /// Instantiates a new <see cref="Client"/>.
        /// </summary>
        /// <param name="channelAddress">This client's address.</param>
        /// <param name="configuration">This client's configuration.</param>
        /// <param name="registrationController">This client's registration controller.</param>
        public Client(IChannelAddress channelAddress,
                      IClientConfiguration configuration,
                      IRegistrationController registrationController) : base(channelAddress, configuration, registrationController) { }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public Client(Configuration.IConfigurationGroup configuration) : base(configuration) { }
        #endregion
        #region SimpleClient Methods
        /// <summary>
        /// The channel's address converted into a <see cref="Uri"/>.
        /// </summary>
        public override Uri AddressUri => (new UriBuilder(string.Format("http://{0}:{1}/{2}", base.Address.IPAddress, base.Address.Port, base.Address.Name))).Uri;
        /// <summary>
        /// The <see cref="System.ServiceModel.Channels.Binding"/> used for this network resource.
        /// </summary>
        protected override System.ServiceModel.Channels.Binding Binding => new System.ServiceModel.WSDualHttpBinding(System.ServiceModel.WSDualHttpSecurityMode.None);
        #endregion
    }
}

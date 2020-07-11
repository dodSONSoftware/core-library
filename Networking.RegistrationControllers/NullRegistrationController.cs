using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking.RegistrationControllers
{
    /// <summary>
    /// Provides properties and methods used to control registering and unregistering clients.
    /// </summary>
    [Serializable]
    public class NullRegistrationController
        : IRegistrationController
    {
        #region Ctor
        private NullRegistrationController()
        {
        }

        // CLIENT-SIDE CTOR
        // client uses: challegeEvidence

        /// <summary>
        /// Instantiates a new client-side <see cref="NullRegistrationController"/>.
        /// </summary>
        /// <param name="challegeEvidence">The challenge evidence required to access the communication system.</param>
        public NullRegistrationController(byte[] challegeEvidence)
            : this()
        {
            _ChallengeEvidence = challegeEvidence ?? new byte[0];
            ChallengeController = new ChallengeControllers.NullChallengeController();
        }

        // SERVER-SIDE CTOR
        // server uses: challengeController, transportConfiguration
        // client uses: (when-shared thru AddonManager) challegeEvidence

        /// <summary>
        /// Instantiates a new client-side <see cref="NullRegistrationController"/>.
        /// </summary>
        /// <param name="transportConfiguration">The transportation layer configuration.</param>
        /// <param name="challengeController">Controls access to the communication system.</param>
        public NullRegistrationController(ITransportConfiguration transportConfiguration,
                                          IChallengeController challengeController)
            : this()
        {
            TransportConfiguration = transportConfiguration ?? throw new ArgumentNullException(nameof(transportConfiguration));
            ChallengeController = challengeController ?? throw new ArgumentNullException(nameof(challengeController));
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public NullRegistrationController(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "RegistrationController")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"RegistrationController\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // _TransportConfiguration
            if (configuration.ContainsKey("TransportConfiguration"))
            {
                TransportConfiguration = (ITransportConfiguration)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(TransportConfiguration), configuration["TransportConfiguration"]);
            }
            // _ChallengeController 
            if (configuration.ContainsKey("ChallengeController"))
            {
                ChallengeController = (IChallengeController)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(null, configuration["ChallengeController"]);
            }
            else if (configuration.Items.ContainsKey("Evidence"))
            {
                if (!configuration.Items["Evidence"].ValueType.StartsWith(typeof(byte[]).FullName))
                {
                    throw new Exception($"Configuration item invalid format. Configuration item \"Evidence\" must be a <{typeof(byte[])}>.");
                }
                _ChallengeEvidence = (byte[])configuration.Items["Evidence"].Value;
            }
            else
            {
                throw new ArgumentException($"Configuration missing information. Configuration must have either an \"Evidence\" item or a \"ChallengeController\" subgroup.", nameof(configuration));
            }
        }
        #endregion
        #region Private Fields
        private byte[] _ChallengeEvidence = new byte[0];
        #endregion
        #region IRegistrationController Methods
        /// <summary>
        /// The evidence needed to register with the server.
        /// </summary>
        public byte[] ChallengeEvidence
        {
            get
            {
                if (_ChallengeEvidence != null)
                {
                    return _ChallengeEvidence;
                }
                else if (ChallengeController != null)
                {
                    return ChallengeController.Evidence;
                }
                return null;
            }
        }
        /// <summary>
        /// Used to control access to the communication system.
        /// </summary>
        public IChallengeController ChallengeController { get; } = null;
        /// <summary>
        /// Configuration information used by the transportation layer.
        /// </summary>
        public ITransportConfiguration TransportConfiguration { get; private set; } = null;
        // ****
        // **** CLIENT-ONLY CALLED METHODS
        // ****

        /// <summary>
        /// Will attempt to register a client with a server.
        /// </summary>
        /// <param name="serviceProxy">The server's service proxy. (Provides callback methods to the server.)</param>
        /// <param name="client">The client to register.</param>
        /// <param name="transportControllerAdvanced">Provides advanced access to the transport controller.</param>
        /// <returns><b>True</b> if registration succeeded; otherwise <b>false</b>.</returns>
        public bool Register(IService serviceProxy,
                             IClient client,
                             ITransportControllerAdvanced transportControllerAdvanced)
        {
            if ((serviceProxy != null) && (client != null) && (transportControllerAdvanced != null))
            {
                // RequestServerSetup
                var request = (new dodSON.Core.Converters.TypeSerializer<string>()).ToByteArray("RequestServerSetup");
                var response = serviceProxy.RegistrationChannel(RegistrationTypeEnum.Register, request);
                try
                {
                    var transportConfiguration = (new dodSON.Core.Converters.TypeSerializer<ITransportConfiguration>()).FromByteArray(response);
                    transportControllerAdvanced.SetTransportConfiguration(transportConfiguration);
                    // Register
                    request = (new dodSON.Core.Converters.TypeSerializer<IClientConfiguration>()).ToByteArray(client.ClientConfiguration);
                    response = serviceProxy.RegistrationChannel(RegistrationTypeEnum.Register, request);
                    return (new dodSON.Core.Converters.TypeSerializer<bool>()).FromByteArray(response);
                }
                catch { }
            }
            return false;
        }
        /// <summary>
        /// Will attempt to unregister a client from a server.
        /// </summary>
        /// <param name="serviceProxy">The server's service proxy. (Provides callback methods to the server.)</param>
        /// <param name="client">The client to unregister.</param>
        public void Unregister(IService serviceProxy, IClient client)
        {
            if ((serviceProxy != null) && (client != null))
            {
                var request = (new dodSON.Core.Converters.TypeSerializer<IClientConfiguration>()).ToByteArray(client.ClientConfiguration);
                serviceProxy.RegistrationChannel(RegistrationTypeEnum.Unregister, request);
            }
        }
        // ****
        // **** SERVER-ONLY CALLED METHODS
        // ****

        /// <summary>
        /// Attempts to register the client with the server.
        /// </summary>
        /// <param name="data">Information related to registration.</param>
        /// <param name="newRegisterableClientConfiguration">A <see cref="IClientConfiguration"/> for the newly registered client.</param>
        /// <returns>A response relative to the registration process.</returns>
        public byte[] TryAddDataFromRegistrationChannel(byte[] data,
                                                        out IClientConfiguration newRegisterableClientConfiguration)
        {
            if ((data == null) || (data.Length == 0))
            {
                throw new ArgumentNullException(nameof(data));
            }
            newRegisterableClientConfiguration = null;
            try
            {
                // Register
                newRegisterableClientConfiguration = (new dodSON.Core.Converters.TypeSerializer<IClientConfiguration>()).FromByteArray(data);
                return (new dodSON.Core.Converters.TypeSerializer<bool>()).ToByteArray(true);
            }
            catch
            {
                newRegisterableClientConfiguration = null;
            }
            // ----
            try
            {
                // RequestServerSetup

                // ################
                // TODO: Problem ID: dc95c9d1ab4a422cbca3834e1361aa0 : The Problem
                //                   The Problem. The TransportConfiguration referenced here is different depending on the ctor used to create it.
                //                            Using the Standard Ctors, this reference here is the same as the reference in RegistrationController.
                //                            However, using the IConfigurable interface code results in the two references being different objects. 
                //
                //                            ######## When building in code, be sure to use the same reference in all types ########. 
                //
                //                            Search for the Problem ID: dc95c9d1ab4a422cbca3834e1361aa0 for other elements of this problem.
                //                            Current hypothesis is that somehow the referenced registration controller are different in the two places when it should be one.
                //                            This problem manifests itself ONLY when the objects are created using the IConfiguration system. 
                //                            The answer is apparent, the IConfiguration system is creating two of these items.
                return (new dodSON.Core.Converters.TypeSerializer<ITransportConfiguration>()).ToByteArray(TransportConfiguration);
            }
            catch { }
            return (new dodSON.Core.Converters.TypeSerializer<bool>()).ToByteArray(false);
        }
        /// <summary>
        /// Replaces the current <see cref="ITransportController.TransportConfiguration"/> with the given <see cref="ITransportConfiguration"/>.
        /// </summary>
        /// <param name="transportConfiguration">The new <see cref="ITransportConfiguration"/>.</param>
        void IRegistrationControllerAdvanced.SetTransportConfiguration(ITransportConfiguration transportConfiguration) => TransportConfiguration = transportConfiguration;
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("RegistrationController");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                if (TransportConfiguration != null)
                {
                    result.Add(TransportConfiguration.Configuration);
                }
                if (ChallengeController != null)
                {
                    result.Add(ChallengeController.Configuration);
                }
                else
                {
                    if (_ChallengeEvidence != null)
                    {
                        result.Items.Add("Evidence", _ChallengeEvidence, _ChallengeEvidence.GetType());
                    }
                }
                return result;
            }
        }
        #endregion
    }
}

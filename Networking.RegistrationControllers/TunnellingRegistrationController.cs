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
    public class TunnellingRegistrationController
        : IRegistrationController
    {
        #region Constants
        private static readonly int RSAKeyLengthInBits = 2048;
        private static readonly int TransportPartsLengthInBytes = 80;
        #endregion
        #region Ctor
        private TunnellingRegistrationController()
        {
        }

        // CLIENT-SIDE CTOR
        // client uses: challegeEvidence

        ///// <summary>
        ///// Instantiates a new client-side <see cref="TunnellingRegistrationController"/>.
        ///// </summary>
        ///// <param name="challegeEvidence">The challenge evidence required to access the communication system.</param>
        //public TunnellingRegistrationController(byte[] challegeEvidence)
        //    : this()
        //{
        //    _ChallengeEvidence = challegeEvidence ?? new byte[0];
        //    ChallengeController = new ChallengeControllers.NullChallengeController();
        //}

        /// <summary>
        /// Instantiates a new client-side <see cref="TunnellingRegistrationController"/>.
        /// </summary>
        /// <param name="challengeController">Controls access to the communication system.</param>
        public TunnellingRegistrationController(IChallengeController challengeController)
            : this()
        {
            ChallengeController = challengeController ?? throw new ArgumentNullException(nameof(challengeController));
        }

        // SERVER-SIDE CTOR
        // server uses: challengeController, transportConfiguration
        // client uses: (when-shared through AddonManager) challegeEvidence

        /// <summary>
        /// Instantiates a new client-side <see cref="TunnellingRegistrationController"/>.
        /// </summary>
        /// <param name="transportConfiguration">The transportation layer configuration.</param>
        /// <param name="challengeController">Controls access to the communication system.</param>
        public TunnellingRegistrationController(ITransportConfiguration transportConfiguration,
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
        public TunnellingRegistrationController(Configuration.IConfigurationGroup configuration)
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
        private byte[] _ChallengeEvidence = null;
        private Dictionary<string, ServerRegistrationCandidate> _RegistrationCandidates = new Dictionary<string, ServerRegistrationCandidate>();
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
        public ITransportConfiguration TransportConfiguration
        {
            get; private set;
        }

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
        public bool Register(IService serviceProxy, IClient client, ITransportControllerAdvanced transportControllerAdvanced)
        {
            if ((serviceProxy != null) && (client != null) && (transportControllerAdvanced != null))
            {
                var transportConfigurationConvertor = new dodSON.Core.Converters.TypeSerializer<ITransportConfiguration>();
                var transportDataConverter = new dodSON.Core.Converters.TypeSerializer<TransportData>();
                var listConverter = new dodSON.Core.Converters.TypeSerializer<List<byte[]>>();
                var boolConverter = new dodSON.Core.Converters.TypeSerializer<bool>();
                var clientConfigurationConverter = new dodSON.Core.Converters.TypeSerializer<IClientConfiguration>();
                // ######## HELLO (create tunnel)
                // create public/private keys
                System.Security.Cryptography.RSACryptoServiceProvider serverCryptoProvider = null;
                var clientCryptoProvider = AsymmetricCryptoProvider;
                var clientPublicKey = clientCryptoProvider.ToXmlString(false);
                var clientPrivateKey = clientCryptoProvider.ToXmlString(true);
                var request = transportDataConverter.ToByteArray(
                                    new TransportData()
                                    {
                                        Alpha = ContextFor("Hello"),
                                        Beta = client.Id,
                                        Gamma = System.Text.Encoding.Unicode.GetBytes(clientPublicKey),
                                        Delta = null,
                                        Epsilon = null
                                    });
                var response = transportDataConverter.FromByteArray(serviceProxy.RegistrationChannel(RegistrationTypeEnum.Register, request));
                // ######## test for HELLO
                if ((response.Alpha == ContextFor("Hello")) && (response.Beta == client.Id))
                {
                    // ######## get server public key
                    var serverPublicKey = System.Text.Encoding.Unicode.GetString(response.Gamma);
                    serverCryptoProvider = AsymmetricCryptoProvider;
                    serverCryptoProvider.FromXmlString(serverPublicKey);
                    // ######## meet challenge
                    request = transportDataConverter.ToByteArray(
                                    new TransportData()
                                    {
                                        Alpha = ContextFor("Challenge"),
                                        Beta = client.Id,
                                        Gamma = null,
                                        Delta = null,
                                        Epsilon = PrepareForTransport(ChallengeEvidence, serverCryptoProvider, TransportPartsLengthInBytes)
                                    });
                    if (boolConverter.FromByteArray(serviceProxy.RegistrationChannel(RegistrationTypeEnum.Register, request)))
                    {
                        // ######## get transport setup
                        request = transportDataConverter.ToByteArray(
                                        new TransportData()
                                        {
                                            Alpha = ContextFor("RequestServerSetup"),
                                            Beta = client.Id,
                                            Gamma = null,
                                            Delta = null,
                                            Epsilon = PrepareForTransport(ChallengeEvidence, serverCryptoProvider, TransportPartsLengthInBytes)
                                        });
                        var byteResponse = serviceProxy.RegistrationChannel(RegistrationTypeEnum.Register, request);
                        if (byteResponse != null)
                        {
                            var list = listConverter.FromByteArray(byteResponse);
                            var restored = RestoreFromTransport(list, clientCryptoProvider);
                            var config = transportConfigurationConvertor.FromByteArray(restored);
                            if (config != null)
                            {
                                transportControllerAdvanced.SetTransportConfiguration(config);
                                // ######## send registration
                                request = transportDataConverter.ToByteArray(
                                        new TransportData()
                                        {
                                            Alpha = ContextFor("Register"),
                                            Beta = client.Id,
                                            Gamma = null,
                                            Delta = PrepareForTransport(clientConfigurationConverter.ToByteArray(client.ClientConfiguration),
                                                                        serverCryptoProvider,
                                                                        TransportPartsLengthInBytes),
                                            Epsilon = PrepareForTransport(ChallengeEvidence, serverCryptoProvider, TransportPartsLengthInBytes)
                                        });
                                return boolConverter.FromByteArray(serviceProxy.RegistrationChannel(RegistrationTypeEnum.Register, request));
                            }
                        }
                    }
                }
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
                var boolConverter = new dodSON.Core.Converters.TypeSerializer<bool>();
                var transportDataConverter = new dodSON.Core.Converters.TypeSerializer<TransportData>();
                var clientConfigurationConverter = new dodSON.Core.Converters.TypeSerializer<IClientConfiguration>();
                // ######## HELLO (create tunnel)
                // create public/private keys
                System.Security.Cryptography.RSACryptoServiceProvider serverCryptoProvider = null;
                var clientCryptoProvider = AsymmetricCryptoProvider;
                var clientPublicKey = clientCryptoProvider.ToXmlString(false);
                var clientPrivateKey = clientCryptoProvider.ToXmlString(true);
                var request = transportDataConverter.ToByteArray(
                                    new TransportData()
                                    {
                                        Alpha = ContextFor("Hello"),
                                        Beta = client.Id,
                                        Gamma = System.Text.Encoding.Unicode.GetBytes(clientPublicKey),
                                        Delta = null,
                                        Epsilon = null
                                    });
                var response = transportDataConverter.FromByteArray(serviceProxy.RegistrationChannel(RegistrationTypeEnum.Unregister, request));
                // ######## test for HELLO
                if ((response.Alpha == ContextFor("Hello")) && (response.Beta == client.Id))
                {
                    // ######## get server public key
                    var serverPublicKey = System.Text.Encoding.Unicode.GetString(response.Gamma);
                    serverCryptoProvider = AsymmetricCryptoProvider;
                    serverCryptoProvider.FromXmlString(serverPublicKey);
                    // ######## meet challenge
                    request = transportDataConverter.ToByteArray(
                                    new TransportData()
                                    {
                                        Alpha = ContextFor("Challenge"),
                                        Beta = client.Id,
                                        Gamma = null,
                                        Delta = null,
                                        Epsilon = PrepareForTransport(ChallengeEvidence, serverCryptoProvider, TransportPartsLengthInBytes)
                                    });
                    if (boolConverter.FromByteArray(serviceProxy.RegistrationChannel(RegistrationTypeEnum.Unregister, request)))
                    {
                        request = transportDataConverter.ToByteArray(
                                        new TransportData()
                                        {
                                            Alpha = ContextFor("Unregister"),
                                            Beta = client.Id,
                                            Gamma = null,
                                            Delta = PrepareForTransport(clientConfigurationConverter.ToByteArray(client.ClientConfiguration),
                                                                        serverCryptoProvider,
                                                                        TransportPartsLengthInBytes),
                                            Epsilon = PrepareForTransport(ChallengeEvidence, serverCryptoProvider, TransportPartsLengthInBytes)
                                        });
                        serviceProxy.RegistrationChannel(RegistrationTypeEnum.Unregister, request);
                    }
                }
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
        public byte[] TryAddDataFromRegistrationChannel(byte[] data, out IClientConfiguration newRegisterableClientConfiguration)
        {
            if ((data == null) || (data.Length == 0))
            {
                throw new ArgumentNullException("data");
            }
            newRegisterableClientConfiguration = null;
            var transportConfigurationConvertor = new dodSON.Core.Converters.TypeSerializer<ITransportConfiguration>();
            var transportDataConverter = new dodSON.Core.Converters.TypeSerializer<TransportData>();
            var listConverter = new dodSON.Core.Converters.TypeSerializer<List<byte[]>>();
            var boolConverter = new dodSON.Core.Converters.TypeSerializer<bool>();
            var clientConfigurationConverter = new dodSON.Core.Converters.TypeSerializer<IClientConfiguration>();
            //
            var request = transportDataConverter.FromByteArray(data);
            if (request.Alpha == ContextFor("Hello"))
            {
                // ######## create tunnel
                // create public/private keys
                var KeyGenerator = AsymmetricCryptoProvider;
                var serverPublicKey = KeyGenerator.ToXmlString(false);
                var serverPrivateKey = KeyGenerator.ToXmlString(true);
                // clear some bullshit here, I do not understand this, it refuses to register clients whom have, in theory, already unregistered...
                if (_RegistrationCandidates.ContainsKey(request.Beta))
                {
                    _RegistrationCandidates.Remove(request.Beta);
                }
                // add candidate
                _RegistrationCandidates.Add(request.Beta,
                                            new ServerRegistrationCandidate()
                                            {
                                                ClientId = request.Beta,
                                                ClientPublicKey = System.Text.Encoding.Unicode.GetString(request.Gamma),
                                                CreatedDate = DateTime.Now,
                                                ServerPrivateKey = serverPrivateKey,
                                                ServerPublicKey = serverPublicKey
                                            });
                // generate response
                var response = transportDataConverter.ToByteArray(
                                    new TransportData()
                                    {
                                        Alpha = ContextFor("Hello"),
                                        Beta = request.Beta,
                                        Gamma = System.Text.Encoding.Unicode.GetBytes(serverPublicKey),
                                        Delta = null,
                                        Epsilon = null
                                    });
                return response;
            }
            else if (request.Alpha == ContextFor("Challenge"))
            {
                if (_RegistrationCandidates.ContainsKey(request.Beta))
                {
                    var candidate = _RegistrationCandidates[request.Beta];
                    var cryptoProvider = AsymmetricCryptoProvider;
                    cryptoProvider.FromXmlString(candidate.ServerPrivateKey);
                    var challengeEvidence = RestoreFromTransport(request.Epsilon, cryptoProvider);
                    if (ChallengeController.Challenge(challengeEvidence))
                    {
                        return boolConverter.ToByteArray(true);
                    }
                    _RegistrationCandidates.Remove(request.Beta);
                    return boolConverter.ToByteArray(false);
                }
                return boolConverter.ToByteArray(false);
            }
            else if (request.Alpha == ContextFor("RequestServerSetup"))
            {
                if (_RegistrationCandidates.ContainsKey(request.Beta))
                {
                    var candidate = _RegistrationCandidates[request.Beta];
                    var cryptoProvider = AsymmetricCryptoProvider;
                    cryptoProvider.FromXmlString(candidate.ServerPrivateKey);
                    var challengeEvidence = RestoreFromTransport(request.Epsilon, cryptoProvider);
                    if (ChallengeController.Challenge(challengeEvidence))
                    {
                        var clientProvider = AsymmetricCryptoProvider;
                        clientProvider.FromXmlString(candidate.ClientPublicKey);

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
                        var response = listConverter.ToByteArray(PrepareForTransport(transportConfigurationConvertor.ToByteArray(TransportConfiguration),
                                                                                     clientProvider,
                                                                                     TransportPartsLengthInBytes));
                        return response;
                    }
                    _RegistrationCandidates.Remove(request.Beta);
                    return null;
                }
                return null;
            }
            else if (request.Alpha == ContextFor("Register"))
            {
                if (_RegistrationCandidates.ContainsKey(request.Beta))
                {
                    var candidate = _RegistrationCandidates[request.Beta];
                    var cryptoProvider = AsymmetricCryptoProvider;
                    cryptoProvider.FromXmlString(candidate.ServerPrivateKey);
                    var challengeEvidence = RestoreFromTransport(request.Epsilon, cryptoProvider);
                    if (ChallengeController.Challenge(challengeEvidence))
                    {
                        // register
                        newRegisterableClientConfiguration = clientConfigurationConverter.FromByteArray(RestoreFromTransport(request.Delta, cryptoProvider));
                        _RegistrationCandidates.Remove(request.Beta);
                        // generate response
                        return boolConverter.ToByteArray(true);
                    }
                    _RegistrationCandidates.Remove(request.Beta);
                    return boolConverter.ToByteArray(false);
                }
                return boolConverter.ToByteArray(false);
            }
            else if (request.Alpha == ContextFor("Unregister"))
            {
                if (_RegistrationCandidates.ContainsKey(request.Beta))
                {
                    var candidate = _RegistrationCandidates[request.Beta];
                    var cryptoProvider = AsymmetricCryptoProvider;
                    cryptoProvider.FromXmlString(candidate.ServerPrivateKey);
                    var challengeEvidence = RestoreFromTransport(request.Epsilon, cryptoProvider);
                    if (ChallengeController.Challenge(challengeEvidence))
                    {
                        // unregister
                        newRegisterableClientConfiguration = clientConfigurationConverter.FromByteArray(RestoreFromTransport(request.Delta, cryptoProvider));
                        _RegistrationCandidates.Remove(request.Beta);
                        // generate response
                        return boolConverter.ToByteArray(true);
                    }
                    _RegistrationCandidates.Remove(request.Beta);
                    return boolConverter.ToByteArray(false);
                }
                return boolConverter.ToByteArray(false);
            }
            return null;
        }
        /// <summary>
        /// Replaces the current <see cref="ITransportController.TransportConfiguration"/> with the given <see cref="ITransportConfiguration"/>.
        /// </summary>
        /// <param name="transportConfiguration">The new <see cref="ITransportConfiguration"/>.</param>
        void IRegistrationControllerAdvanced.SetTransportConfiguration(ITransportConfiguration transportConfiguration)
        {
            TransportConfiguration = transportConfiguration;
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
        #region Private Methods
        private string ContextFor(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }
            switch (key)
            {
                case "Hello":
                    return "AAE6C469-D436-42E0-BE49-F1DAC2CC5B2E";
                case "Challenge":
                    return "E4403132-10A9-42B9-BBD1-F21121BD2851";
                case "RequestServerSetup":
                    return "3361897D-AAA4-4BD0-ABF2-4CD496B02211";
                case "Register":
                    return "8ED8081F-5678-42EB-B7A2-F451BCE45FE8";
                case "Unregister":
                    return "D0AA3276-30C1-49E3-8B55-46B3DD2F5BB0";
                default:
                    throw new InvalidOperationException(string.Format("Unrecognized key: {0}", key));
            }
        }
        private List<byte[]> PrepareForTransport(byte[] data,
                                                 System.Security.Cryptography.RSACryptoServiceProvider encryptor,
                                                 int partLength)
        {
            var list = new List<byte[]>();
            foreach (var item in dodSON.Core.Common.ByteArrayHelper.SplitByteArray(data, partLength))
            {
                list.Add(encryptor.Encrypt(item, true));
            }
            return list;
        }
        private byte[] RestoreFromTransport(List<byte[]> parts,
                                            System.Security.Cryptography.RSACryptoServiceProvider encryptor)
        {
            var list = new List<byte[]>();
            foreach (var item in parts)
            {
                list.Add(encryptor.Decrypt(item, true));
            }
            return dodSON.Core.Common.ByteArrayHelper.RestoreByteArray(list);
        }
        private System.Security.Cryptography.RSACryptoServiceProvider AsymmetricCryptoProvider
        {
            get
            {
                return new System.Security.Cryptography.RSACryptoServiceProvider(RSAKeyLengthInBits);
            }
        }
        #endregion
        #region Private Class ServerRegistrationCandidatesData
        [Serializable]
        private class ServerRegistrationCandidate
        {
            public DateTime CreatedDate
            {
                get; set;
            }
            public string ClientId
            {
                get; set;
            }
            public string ClientPublicKey
            {
                get; set;
            }
            public string ServerPublicKey
            {
                get; set;
            }
            public string ServerPrivateKey
            {
                get; set;
            }
        }
        #endregion
    }
    #region Internal Class TransportData
    [Serializable]
    internal class TransportData
    {
        public string Alpha
        {
            get; set;
        }
        public string Beta
        {
            get; set;
        }
        public byte[] Gamma
        {
            get; set;
        }
        public List<byte[]> Delta
        {
            get; set;
        }
        public List<byte[]> Epsilon
        {
            get; set;
        }
    }
    #endregion
}

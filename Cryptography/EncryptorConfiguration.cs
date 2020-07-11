using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: Fix This. Problem Id= 3333E15BFE8746CEAA1D5664BFFD0E18
//      I'm using the salted password wrong in the dodSON.Core.Cryptography.CryptographyHelper code.
//      See dodSON.Core.Cryptography.CryptographyHelper where I'm only using the PASSWORD byte array as the PASSWORD; that's not right
// Must Rethink This!

namespace dodSON.Core.Cryptography
{
    /// <summary>
    /// Defines the elements needed to properly encrypt and decrypt data.
    /// </summary>
    [Serializable]
    public class EncryptorConfiguration
        : IEncryptorConfiguration
    {
        #region Ctor
        private EncryptorConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EncryptorConfiguration.
        /// </summary>
        /// <param name="saltedPassword">The salted password required to encrypt and decrypt data.</param>
        /// <param name="symmetricAlgorithmType">The <see cref="System.Security.Cryptography.SymmetricAlgorithm"/> required to encrypt and decrypt data.</param>
        public EncryptorConfiguration(ISaltedPassword saltedPassword,
                                      Type symmetricAlgorithmType)
            : this()
        {
            if (symmetricAlgorithmType == null)
            {
                throw new ArgumentNullException(nameof(symmetricAlgorithmType));
            }
            if (!typeof(System.Security.Cryptography.SymmetricAlgorithm).IsAssignableFrom(symmetricAlgorithmType))
            {
                throw new ArgumentException($"Parameter {nameof(symmetricAlgorithmType)} must be a type of System.Security.Cryptography.SymmetricAlgorithm.");
            }
            SaltedPassword = saltedPassword ?? throw new ArgumentNullException(nameof(saltedPassword));
            SymmetricAlgorithmType = symmetricAlgorithmType;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public EncryptorConfiguration(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "EncryptorConfiguration")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"EncryptorConfiguration\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // SymmetricAlgorithm
            try
            {
                SymmetricAlgorithmType = Type.GetType((string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "SymmetricAlgorithmType", typeof(Type)).Value);
            }
            catch { }
            // SaltedPassword
            if (!configuration.ContainsKey("SaltedPassword"))
            {
                throw new Exception($"Configuration missing information. Configuration must have item: \"SaltedPassword\".");
            }
            SaltedPassword = (Cryptography.ISaltedPassword)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(Cryptography.SaltedPassword), configuration["SaltedPassword"]);
        }

        #endregion
        #region IEncryptorConfiguration Methods
        /// <summary>
        /// The <see cref="ISaltedPassword"/> to use for encryption and decryption.
        /// </summary>
        public ISaltedPassword SaltedPassword { get; } = null;
        /// <summary>
        /// The <see cref="System.Security.Cryptography.SymmetricAlgorithm"/> to use for encryption and decryption.
        /// </summary>
        public Type SymmetricAlgorithmType { get; } = null;
        /// <summary>
        /// Instantiates an new instance of the <see cref="SymmetricAlgorithmType"/>.
        /// </summary>
        public System.Security.Cryptography.SymmetricAlgorithm SymmetricAlgorithm => Common.InstantiationHelper.InvokeDefaultCtor(SymmetricAlgorithmType) as System.Security.Cryptography.SymmetricAlgorithm;
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("EncryptorConfiguration");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                result.Items.Add("SymmetricAlgorithmType", SymmetricAlgorithmType, typeof(Type));
                result.Add(SaltedPassword.Configuration);
                return result;
            }
        }
        #endregion
    }
}

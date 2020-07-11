using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Cryptography
{
    /// <summary>
    /// Implements the <see cref="IEncryptor"/> interface, however, it does not mutate the source byte array. 
    /// Can be used when the <see cref="IEncryptor"/> type is required, but no encryption is needed or desired.
    /// </summary>
    [Serializable]
    public class NullEncryptor
        : IEncryptor
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the NullEncryptor.
        /// </summary>
        public NullEncryptor()
        {
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public NullEncryptor(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "Encryptor")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Encryptor\". Configuration Key={configuration.Key}", nameof(configuration));
            }
        }
        #endregion
        #region IEncyptor Methods
        /// <summary>
        /// Returns the <paramref name="source"/> byte array.
        /// </summary>
        /// <param name="source">The byte array to return.</param>
        /// <returns>The <paramref name="source"/> byte array.</returns>
        public byte[] Encrypt(byte[] source) => source;
        /// <summary>
        /// Returns the <paramref name="source"/> byte array.
        /// </summary>
        /// <param name="source">The byte array to return.</param>
        /// <returns>The <paramref name="source"/> byte array.</returns>
        public byte[] Decrypt(byte[] source) => source;
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("Encryptor");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                return result;
            }
        }
        #endregion
    }
}

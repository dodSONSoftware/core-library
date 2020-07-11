using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Cryptography
{
    /// <summary>
    /// Defines the elements needed to properly encrypt and decrypt data.
    /// </summary>
    public interface IEncryptorConfiguration
        : Configuration.IConfigurable
    {
        /// <summary>
        /// The <see cref="ISaltedPassword"/> to use for encryption and decryption.
        /// </summary>
        ISaltedPassword SaltedPassword { get; }
        /// <summary>
        /// The <see cref="System.Security.Cryptography.SymmetricAlgorithm"/> to use for encryption and decryption.
        /// </summary>
        Type SymmetricAlgorithmType { get; }
        /// <summary>
        /// Using reflection, instantiates an instance of the <see cref="SymmetricAlgorithmType"/>.
        /// </summary>
        System.Security.Cryptography.SymmetricAlgorithm SymmetricAlgorithm { get; }
    }
}

using System;

namespace dodSON.Core.Cryptography
{
    /// <summary>
    /// Provides mechanisms for viewing and validating salted password hashes.
    /// </summary>
    public interface ISaltedPassword
        : Configuration.IConfigurable, 
          System.Runtime.Serialization.ISerializable
    {
        /// <summary>
        /// The type of <see cref="System.Security.Cryptography.HashAlgorithm"/> used to create the computed <see cref="PasswordSaltHash"/>.
        /// </summary>
        Type HashAlgorithmType { get; }
        /// <summary>
        /// An instance of the <see cref="HashAlgorithmType"/>.
        /// </summary>
        System.Security.Cryptography.HashAlgorithm HashAlgorithm { get; }
        /// <summary>
        /// The salt used to create the computed <see cref="PasswordSaltHash"/>.
        /// </summary>
        byte[] Salt { get; }
        /// <summary>
        /// The hash created using the password and the salt.
        /// </summary>
        byte[] PasswordSaltHash { get; }
        /// <summary>
        /// Determines if the <paramref name="candidatePasswordSaltHash"/> is equal to the computed <see cref="PasswordSaltHash"/>.
        /// </summary>
        /// <param name="candidatePasswordSaltHash">A byte array to compare against the computed <see cref="PasswordSaltHash"/>.</param>
        /// <returns><b>True</b> if the <paramref name="candidatePasswordSaltHash"/> is equal to the computed <see cref="PasswordSaltHash"/>, otherwise <b>false</b>.</returns>
        bool IsValid(byte[] candidatePasswordSaltHash);
        /// <summary>
        /// Determines if the <paramref name="candidateSaltedPassword"/> is equal to this <see cref="ISaltedPassword"/>.
        /// </summary>
        /// <param name="candidateSaltedPassword">The <see cref="ISaltedPassword"/> to compare to this <see cref="ISaltedPassword"/>.</param>
        /// <returns><b>True</b> if the <paramref name="candidateSaltedPassword"/> is equal to this <see cref="ISaltedPassword"/>, otherwise <b>false</b>.</returns>
        bool IsValid(ISaltedPassword candidateSaltedPassword);
    }
}

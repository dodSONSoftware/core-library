using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Cryptography
{
    /// <summary>
    /// Defines a simple, and stackable, means to encrypt and decrypt bytes arrays.
    /// </summary>
    public interface IEncryptor
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Encrypts and returns the <paramref name="source"/> byte array.
        /// </summary>
        /// <param name="source">The byte array to encrypt.</param>
        /// <returns>The encrypted <paramref name="source"/> byte array.</returns>
        byte[] Encrypt(byte[] source);
        /// <summary>
        /// Decrypts and returns the <paramref name="source"/> byte array.
        /// </summary>
        /// <param name="source">The byte array to decrypt.</param>
        /// <returns>The decrypted <paramref name="source"/> byte array.</returns>
        byte[] Decrypt(byte[] source);
    }
}

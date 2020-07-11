using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Compression
{
    /// <summary>
    /// Implements the <see cref="ICompressor"/> interface, however, it does not mutate the source byte array. Can be used when the <see cref="ICompressor"/> type is required, but no compression is needed or desired.
    /// </summary>
    [Serializable]
    public class NullCompressor
        : ICompressor
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the NullCompressor.
        /// </summary>
        public NullCompressor()
        {
        }
        #endregion
        /// <summary>
        /// Simply returns the <paramref name="source"/> byte array.
        /// </summary>
        /// <param name="source">The byte array to return.</param>
        /// <returns>The <paramref name="source"/> byte array.</returns>
        public byte[] Compress(byte[] source) => source;
        /// <summary>
        /// Simply returns the <paramref name="source"/> byte array.
        /// </summary>
        /// <param name="source">The byte array to return.</param>
        /// <returns>The <paramref name="source"/> byte array.</returns>
        public byte[] Decompress(byte[] source) => source;
    }
}

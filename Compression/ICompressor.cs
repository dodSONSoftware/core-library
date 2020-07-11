using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Compression
{
    /// <summary>
    /// Defines a simple interface to compress and decompress bytes arrays.
    /// </summary>
    public interface ICompressor
    {
        /// <summary>
        /// Compresses and returns the <paramref name="source"/> byte array.
        /// </summary>
        /// <param name="source">The byte array to compress.</param>
        /// <returns>The compressed <paramref name="source"/> byte array.</returns>
        byte[] Compress(byte[] source);
        /// <summary>
        /// Decompresses and returns the <paramref name="source"/> byte array.
        /// </summary>
        /// <param name="source">The byte array to decompress.</param>
        /// <returns>The decompressed <paramref name="source"/> byte array.</returns>
        byte[] Decompress(byte[] source);
    }
}

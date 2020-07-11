using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Defines functionality for creating and managing compressed disconnected file stores.
    /// </summary>
    public interface ICompressedFileStore
        : IFileStore
    {
        /// <summary>
        /// Gets the name of the compression engine used to perform the compression.
        /// </summary>
        string CompressionEngineName { get; }
        /// <summary>
        /// Gets the version of the compression engine used to perform the compression.
        /// </summary>
        Version CompressionEngineVersion { get; }
        /// <summary>
        /// Gets a list of file extensions that will be stored, not compressed.
        /// </summary>
        /// <remarks>
        /// This has been provided as an IList&lt;string&gt; so it will be possible to add and remove file extensions.
        /// </remarks>
        IList<string> ExtensionsToStore { get; }
        /// <summary>
        /// Gets the <see cref="CompressionStorageStrategy"/> by file <paramref name="extension"/>.
        /// </summary>
        /// <param name="extension">The file extension to get the <see cref="CompressionStorageStrategy"/> for.</param>
        /// <returns>The <see cref="CompressionStorageStrategy"/> for a specific <paramref name="extension"/>.</returns>
        CompressionStorageStrategy GetCompressionStrategy(string extension);
        /// <summary>
        /// The total compressed size of all items in this file store.
        /// </summary>
        long CompressedSize { get; }
        /// <summary>
        /// The total uncompressed size of all items in this file store.
        /// </summary>
        long UncompressedSize { get; }
        /// <summary>
        /// The total compression rate of all items in this file store expressed as a ratio.
        /// </summary>
        double CompressionRatio { get; }
        /// <summary>
        /// The total compression percentage of the all of the items in this file store.
        /// </summary>
        double CompressionPercentage { get; }
    }
}

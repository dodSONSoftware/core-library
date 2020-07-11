using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Defines functionality to control a file in a <see cref="ICompressedFileStore"/>.
    /// </summary>
    public interface ICompressedFileStoreItem
        : IFileStoreItem
    {
        /// <summary>
        /// Gets how a file will be added to a compressed file store.
        /// </summary>
        CompressionStorageStrategy CompressionStrategy { get; }
        /// <summary>
        /// The compressed size, in bytes, of the <see cref="IFileStoreItem.RootFilename"/>.
        /// </summary>
        long CompressedFileSize { get; }
        /// <summary>
        /// The compression rate of the <see cref="IFileStoreItem.RootFilename"/> expressed as a ratio. 
        /// </summary>
        double CompressionRatio { get; }
    }
}

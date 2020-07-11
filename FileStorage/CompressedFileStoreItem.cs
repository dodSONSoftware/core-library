using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Defines functionality to control a file in a <see cref="ICompressedFileStore"/>.
    /// </summary>
    public class CompressedFileStoreItem
        : FileStoreItem,
          ICompressedFileStoreItem,
          ICompressedFileStoreItemAdvanced
    {
        #region Ctor
        /// <summary>
        /// A protected constructor.
        /// </summary>
        protected CompressedFileStoreItem()
            : base() { }
        /// <summary>
        /// Initializes a new instance of the CompressedFileStoreItem class.
        /// </summary>
        /// <param name="parent">The <see cref="IFileStore"/> this file belong to.</param>
        /// <param name="rootPath">
        /// The relative filename, inside the file store, identified with this file store item. 
        /// This is a relative file name; for example: filename.txt, folder/filename.txt and folder/subfolder/filename.txt are all valid and will be found at the locations defined.
        /// </param>
        /// <param name="originalFilename">The filename of the original file this file store item is based on.</param>
        /// <param name="originalFileLastModifiedTimeUtc">The last time, in universal coordinated time, the original file has been modified.</param>
        /// <param name="originalFileSize">The size, in bytes, of the file.</param>
        /// <param name="compressionStrategy">Specifies how this file store item will be added to a compressed file store.</param>
        public CompressedFileStoreItem(IFileStore parent,
                                       string rootPath,
                                       string originalFilename,
                                       DateTime originalFileLastModifiedTimeUtc,
                                       long originalFileSize,
                                       CompressionStorageStrategy compressionStrategy)
            : base(parent, rootPath, originalFilename, originalFileSize, originalFileLastModifiedTimeUtc)
        {
            CompressionStrategy = compressionStrategy;
        }
        #endregion
        #region ICompressedFileStoreItem Methods
        /// <summary>
        /// Gets how a file will be added to a compressed file store.
        /// </summary>
        public CompressionStorageStrategy CompressionStrategy { get; private set; } = CompressionStorageStrategy.Compress;
        /// <summary>
        /// The number of bytes this file was compressed to.
        /// </summary>
        public long CompressedFileSize { get; private set; } = 0;
        /// <summary>
        /// The compression rate of this file expressed as a ratio. 
        /// </summary>
        public double CompressionRatio
        {
            get
            {
                if (CompressedFileSize == FileSize) { return 0.0; }
                return (FileSize != 0) ? ((double)CompressedFileSize / (double)FileSize) : 0.0;
            }
        }
        #endregion
        #region ICompressedFileStoreItemAdvanced Methods
        void ICompressedFileStoreItemAdvanced.SetCompressionValues(long compressedSize)
        {
            CompressedFileSize = compressedSize;
        }
        void ICompressedFileStoreItemAdvanced.SetCompressionStrategy(CompressionStorageStrategy compressedStrategy) => CompressionStrategy = compressedStrategy;
        #endregion
    }
}

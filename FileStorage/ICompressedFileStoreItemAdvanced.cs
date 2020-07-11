using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Defines advanced functionality required by types in the dodSON.Core.FileStorage
    /// namespace, but not generally used by the typical consumer.
    /// </summary>
    public interface ICompressedFileStoreItemAdvanced
    {
        /// <summary>
        /// Sets the value of the <see cref="ICompressedFileStoreItem.CompressedFileSize"/> property.
        /// </summary>
        /// <param name="compressedSize">The number of bytes in the original file.</param>
        void SetCompressionValues(long compressedSize);
        /// <summary>
        /// Sets how this file will be added to a compressed file store.
        /// </summary>
        /// <param name="compressedStrategy">The strategy used to add this file to a compressed file store.</param>
        void SetCompressionStrategy(CompressionStorageStrategy compressedStrategy);
    }
}

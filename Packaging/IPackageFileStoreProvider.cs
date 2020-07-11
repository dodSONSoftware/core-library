using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Facilitates creating and connecting to package file store.
    /// </summary>
    public interface IPackageFileStoreProvider
    {
        /// <summary>
        /// Creates a new package file store.
        /// </summary>
        /// <param name="packageFilename">The new package filename.</param>
        /// <returns>An <see cref="FileStorage.ICompressedFileStore"/>.</returns>
        FileStorage.ICompressedFileStore Create(string packageFilename);
        /// <summary>
        /// Connects to an existing package file store.
        /// </summary>
        /// <param name="packageFilename">The filename of the package to connect to.</param>
        /// <returns>An <see cref="FileStorage.ICompressedFileStore"/> containing the package files.</returns>
        FileStorage.ICompressedFileStore Connect(string packageFilename);
    }
}

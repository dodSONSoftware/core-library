using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Packaging.MSdotNETZip
{
    /// <summary>
    /// Provides a <see cref="System.IO.Compression">Mircosoft .NET</see> implementation of the <see cref="IPackageFileStoreProvider"/>.
    /// </summary>
    public class PackageFileStoreProvider
        : IPackageFileStoreProvider
    {
        #region Ctor
        /// <summary>
        /// Instantiate a new <see cref="System.IO.Compression">Mircosoft .NET</see> implementation of <see cref="PackageFileStoreProvider"/>
        /// </summary>
        public PackageFileStoreProvider()
            : base() { }
        #endregion
        #region dodSON.Core.Packaging.IPackageFileStoreProvider Methods
        /// <summary>
        /// Creates a new package file store.
        /// </summary>
        /// <param name="packageFilename">The new package filename.</param>
        /// <returns>An <see cref="FileStorage.ICompressedFileStore"/>.</returns>
        public FileStorage.ICompressedFileStore Create(string packageFilename) => new FileStorage.MSdotNETZip.FileStore(packageFilename, System.IO.Path.GetTempPath(), true, null);
        /// <summary>
        /// Connects to an existing package file store.
        /// </summary>
        /// <param name="packageFilename">The file name of the package to connect to.</param>
        /// <returns>An <see cref="FileStorage.ICompressedFileStore"/> containing the package files.</returns>
        public FileStorage.ICompressedFileStore Connect(string packageFilename) => new FileStorage.MSdotNETZip.FileStore(packageFilename, System.IO.Path.GetTempPath(), true);
        #endregion
    }
}

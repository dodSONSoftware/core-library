using System.Collections.Generic;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Defines a single folder in a list of hierarchical folders.
    /// See the <see cref="dodSON.Core.FileStorage.FileStorageHelper.ConvertToHierarchicalList(IFileStore, string, string)"/> method.
    /// </summary>
    public interface IHierarchicalFolder
    {
        /// <summary>
        /// The parent folder this folder belongs to.
        /// </summary>
        IHierarchicalFolder Parent { get; set; }
        /// <summary>
        /// Gets and set whether this <see cref="HierarchicalFolder"/> represents a compressed folder.
        /// </summary>
        bool IsCompressed { get; set; }
        /// <summary>
        /// The name of the folder.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The full root path for the folder.
        /// </summary>
        string Rootpath { get; set; }
        /// <summary>
        /// The size, in bytes, of all files in this folder.
        /// </summary>
        long FilesTotalSize { get; }
        /// <summary>
        /// The compressed size, in bytes, of all files in this folder.
        /// </summary>
        long CompressedFilesTotalSize { get; }
        /// <summary>
        /// The compressed percentage of all the files in this folder.
        /// </summary>
        double CompressionFilesRatio { get; }
        /// <summary>
        /// The list of <see cref="HierarchicalFolder"/> contained in this <see cref="HierarchicalFolder"/>.
        /// </summary>
        System.Collections.ObjectModel.ObservableCollection<IHierarchicalFolder> Folders { get; set; }
        /// <summary>
        /// The list of <see cref="IFileStoreItem"/>s contained in this <see cref="HierarchicalFolder"/>.
        /// </summary>
        System.Collections.ObjectModel.ObservableCollection<IFileStoreItem> Files { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Defines functionality to control a file in a <see cref="IFileStore"/>.
    /// </summary>
    public interface IFileStoreItem
        : IFileStoreItemAdvanced
    {
        /// <summary>
        /// The <see cref="IFileStore"/> this file belong to.
        /// </summary>
        IFileStore Parent { get; }
        /// <summary>
        /// Displays change state.
        /// </summary>
        Common.IStateChangeView StateChangeViewer { get; }
        /// <summary>
        /// Get the Name of the <see cref="RootFilename"/>; that is, without the <see cref="Path"/>.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Get the path of the <see cref="RootFilename"/>.
        /// </summary>
        string Path { get; }
        /// <summary>
        /// The size, in bytes, of the <see cref="RootFilename"/>.
        /// </summary>
        long FileSize { get; }
        /// <summary>
        /// The name of the file in the store. This is a relative file name; for example: filename.txt, 
        /// folder/filename.txt and folder/subfolder/filename.txt are all valid and will be found at
        /// the locations defined.
        /// </summary>
        string RootFilename { get; }
        /// <summary>
        /// The filename of the original file this file store item is based on.
        /// </summary>
        string OriginalFilename { get; }
        /// <summary>
        /// The filename of the extracted file, if any, for this file store item.
        /// </summary>
        string ExtractedFilename { get; }
        /// <summary>
        /// Returns if the actual file is available in the backend storage system.
        /// </summary>
        bool IsRootFileAvailable { get; }
        /// <summary>
        /// Returns if the actual file in the backend storage system can be updated from the <see cref="OriginalFilename"/>.
        /// </summary>
        bool IsRootFileUpdatable { get; }
        /// <summary>
        /// Returns if the original file this file store item is based on exists.
        /// </summary>
        bool IsOriginalFileAvailable { get; }
        /// <summary>
        /// Returns if this file store item's actual backend file has been extracted.
        /// </summary>
        bool IsExtractedFileAvailable { get; }
        /// <summary>
        /// Returns if the file store item's extracted file is updatable from the actual backend file.
        /// </summary>
        bool IsExtractedFileUpdatable { get; }
        /// <summary>
        /// The last time, in universal coordinated time, the actual backend file has been modified.
        /// </summary>
        DateTime RootFileLastModifiedTimeUtc { get; }
        /// <summary>
        /// The last time, in universal coordinated time, the original file this file store item is based on has been modified.
        /// </summary>
        DateTime OriginalFileLastModifiedTimeUtc { get; }
        /// <summary>
        /// The last time, in universal coordinated time, the file store item's extracted file has been modified.
        /// </summary>
        DateTime ExtractedFileLastModifiedTimeUtc { get; }
        /// <summary>
        /// Will extract the actual backend file to the default root path. 
        /// </summary>
        /// <param name="explicitExtraction"></param>
        /// <returns>The name of the extracted file.</returns>
        string Extract(bool explicitExtraction);
        /// <summary>
        /// Will extract the actual backend file to the <paramref name="destRootPath"/>.
        /// </summary>
        /// <param name="destRootPath">The root directory to extract this file store item.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>The name of the extracted file.</returns>
        string Extract(string destRootPath, bool explicitExtraction);
    }
}

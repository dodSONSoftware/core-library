using System;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Contains information about the comparison of two files.
    /// </summary>
    public interface ICompareResult
    {
        /// <summary>
        /// Gets the type of item being compared.
        /// </summary>
        CompareType ItemType { get; }
        /// <summary>
        /// Gets the recommended action.
        /// </summary>
        CompareAction Action { get; }
        /// <summary>
        /// Gets the root path for the source file.
        /// </summary>
        string SourceRootPath { get; }
        /// <summary>
        /// Gets the path and filename for the source file relative to the <see cref="SourceRootPath"/>.
        /// </summary>
        string SourceRootFilename { get; }
        /// <summary>
        /// Gets the combined path and filename for <see cref="SourceRootPath"/> and <see cref="SourceRootFilename"/>.
        /// </summary>
        string SourceFullPath { get; }
        /// <summary>
        /// Gets the last modified time, in Universal Coordinated Time, for the source file.
        /// </summary>
        DateTime SourceLastModifiedTimeUtc { get; }
        /// <summary>
        /// The size, in bytes, of the file.
        /// </summary>
        long SourceFileSizeInBytes { get; }
        /// <summary>
        /// Gets the root path for the destination file.
        /// </summary>
        string DestinationRootPath { get; }
        /// <summary>
        /// Gets the path and filename for the destination item relative to the <see cref="DestinationRootPath"/>.
        /// </summary>
        string DestinationRootFilename { get; }
        /// <summary>
        /// Gets the combined path and filename for <see cref="DestinationRootPath"/> and <see cref="DestinationRootFilename"/>.
        /// </summary>
        string DestinationFullPath { get; }
        /// <summary>
        /// Gets the last modified time, in Universal Coordinated Time, for the destination file.
        /// </summary>
        DateTime DestinationLastModifiedTimeUtc { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Contains information about the comparison of two files.
    /// </summary>
    public class CompareResult
        : ICompareResult
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareResult"/> class.
        /// </summary>
        public CompareResult() { }
        #endregion
        #region Public Methods
        /// <summary>
        /// Gets the <see cref="CompareType"/> for the files in this comparison.
        /// </summary>
        public CompareType ItemType { get; internal protected set; } = CompareType.Unknown;
        /// <summary>
        /// Gets the <see cref="CompareAction"/> for the files in this comparison.
        /// </summary>
        public CompareAction Action { get; internal protected set; } = CompareAction.Unknown;
        /// <summary>
        /// Gets the root path for the source file.
        /// </summary>
        public string SourceRootPath { get; internal protected set; } = "";
        /// <summary>
        /// Gets the path and filename for the source file relative to the <see cref="SourceRootPath"/>.
        /// </summary>
        public string SourceRootFilename { get; internal protected set; } = "";
        /// <summary>
        /// Gets the combined path and filename for <see cref="SourceRootPath"/> and <see cref="SourceRootFilename"/>.
        /// </summary>
        public string SourceFullPath
        {
            get { return System.IO.Path.Combine(SourceRootPath, SourceRootFilename); }
        }
        /// <summary>
        /// Gets the last modified time, in Universal Coordinated Time, for the source file.
        /// </summary>
        public DateTime SourceLastModifiedTimeUtc { get; internal protected set; } = DateTime.MinValue;
        /// <summary>
        /// The size, in bytes, of the file.
        /// </summary>
        public long SourceFileSizeInBytes { get; internal protected set; }
        /// <summary>
        /// Gets the root path for the destination file.
        /// </summary>
        public string DestinationRootPath { get; internal protected set; } = "";
        /// <summary>
        /// Gets the path and filename for the destination item relative to the <see cref="DestinationRootPath"/>.
        /// </summary>
        public string DestinationRootFilename { get; internal protected set; } = "";
        /// <summary>
        /// Gets the combined path and filename for <see cref="DestinationRootPath"/> and <see cref="DestinationRootFilename"/>.
        /// </summary>
        public string DestinationFullPath
        {
            get { return System.IO.Path.Combine(DestinationRootPath, DestinationRootFilename); }
        }
        /// <summary>
        /// Gets the last modified time, in Universal Coordinated Time, for the destination file.
        /// </summary>
        public DateTime DestinationLastModifiedTimeUtc { get; internal protected set; } = DateTime.MinValue;
        #endregion
    }
}

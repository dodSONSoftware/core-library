using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Identifies the type of file storage item being compared.
    /// </summary>
    public enum CompareType
    {
        /// <summary>
        /// The Unknown!
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Represents a directory.
        /// </summary>
        Directory,
        /// <summary>
        /// Represents a file.
        /// </summary>
        File
    }
}

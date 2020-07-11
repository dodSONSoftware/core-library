using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Represents the state of the source file relative to the destination file based on the 
    /// existence of the source file, the destination file and their relative last modified timestamps.
    /// </summary>
    public enum CompareAction
    {
        /// <summary>
        /// Indicates an unknown action.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Indicates no action required.
        /// </summary>
        Ok,
        /// <summary>
        /// Indicates no destination item.
        /// </summary>
        New,
        /// <summary>
        /// Indicates the source item is older than the destination item.
        /// </summary>
        Old,
        /// <summary>
        /// Indicates the source item is newer than the destination item.
        /// </summary>
        Update,
        /// <summary>
        /// Indicates no source item.
        /// </summary>
        Remove
    }
}

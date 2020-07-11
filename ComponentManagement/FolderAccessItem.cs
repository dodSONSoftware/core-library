using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Represents a folder.
    /// </summary>
    [Serializable]
    public class FolderAccessItem
    {
        #region Ctor
        private FolderAccessItem() { }
        /// <summary>
        /// Represents a folder.
        /// </summary>
        /// <param name="path">The folder path.</param>
        /// <param name="specialPathKey">The key for this special folder. Can be null if the folder is not a special folder.</param>
        public FolderAccessItem(string path,
                                string specialPathKey)
            : this()
        {
            if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentNullException(nameof(path)); }
            Path = path;
            // 
            if (string.IsNullOrWhiteSpace(specialPathKey))
            {
                SpecialPathKey = null;
            }
            else
            {
                SpecialPathKey = specialPathKey;
            }
        }
        /// <summary>
        /// Represents a folder.
        /// </summary>
        /// <param name="path">The folder path.</param>
        public FolderAccessItem(string path)
            : this(path, null)
        {
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The folder path.
        /// </summary>
        public string Path
        {
            get;
        }
        /// <summary>
        /// The key for this special folder. Null if the folder is not a special folder.
        /// </summary>
        public string SpecialPathKey
        {
            get;
        }
        /// <summary>
        /// Indicates if this folder has a <see cref="SpecialPathKey"/>.
        /// </summary>
        public bool IsSpecialFolder => (SpecialPathKey != null);
        #endregion
        #region Overrides
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string representing the current object.</returns>
        public override string ToString()
        {
            if (IsSpecialFolder)
            {
                return $"IsSpecialFolder={IsSpecialFolder}, SpecialFolderKey={SpecialPathKey}, Path={Path}";
            }
            else
            {
                return $"IsSpecialFolder={IsSpecialFolder}, SpecialFolderKey=(null), Path={Path}";
            }
        }
        #endregion
    }
}

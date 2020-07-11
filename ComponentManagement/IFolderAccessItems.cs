using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Defines a collection of <see cref="FolderAccessItem"/>.
    /// </summary>
    public interface IFolderAccessItems
        : ICollection<FolderAccessItem>,
          IReadOnlyCollection<FolderAccessItem>,
          Configuration.IConfigurable
    {
        /// <summary>
        /// All of the folders.
        /// </summary>
        IEnumerable<FolderAccessItem> Folders
        {
            get;
        }
        /// <summary>
        /// All of the Special Folders.
        /// </summary>
        IEnumerable<FolderAccessItem> SpecialFolders
        {
            get;
        }
        /// <summary>
        /// All of the folders that are not marked as special folders.
        /// </summary>
        IEnumerable<FolderAccessItem> NonSpecialFolders
        {
            get;
        }
        /// <summary>
        /// Returns the path for the specified <paramref name="specialFolderKey"/>; otherwise, it will return <b>null</b> if the <paramref name="specialFolderKey"/> is not found.
        /// </summary>
        /// <param name="specialFolderKey">The key to search for.</param>
        /// <returns>The path for the specified <paramref name="specialFolderKey"/>; otherwise, it will return <b>null</b> if the <paramref name="specialFolderKey"/> is not found.</returns>
        string SpecialFolder(string specialFolderKey);
        /// <summary>
        /// Returns whether the <paramref name="path"/> is in this collection.
        /// </summary>
        /// <param name="path">The path to search for.</param>
        /// <returns><b>True</b> if the <paramref name="path"/> is in this collection; otherwise <b>false</b>.</returns>
        bool IsFolderValid(string path);
    }
}

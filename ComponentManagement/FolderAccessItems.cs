using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Represents a collection of <see cref="FolderAccessItem"/>.
    /// </summary>
    [Serializable]
    public class FolderAccessItems
        : IFolderAccessItems
    {
        #region Ctor
        /// <summary>
        /// Initiates a new instance.
        /// </summary>
        public FolderAccessItems()
        {
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public FolderAccessItems(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "FolderAccessItems")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"FolderAccessItems\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // special folders
            var specialFolders = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "SpecialFolders", false);
            if (specialFolders != null)
            {
                foreach (var item in specialFolders.Items)
                {
                    _Folders.Add(new FolderAccessItem(item.Value as string, item.Key));
                }
            }
            // non-special folders
            var nonSpecialFolders = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "Folders", false);
            if (nonSpecialFolders != null)
            {
                foreach (var item in nonSpecialFolders.Items)
                {
                    _Folders.Add(new FolderAccessItem(item.Value as string));
                }
            }
        }
        #endregion
        #region Private Fields
        private List<FolderAccessItem> _Folders = new List<FolderAccessItem>();
        #endregion
        #region IFolderAccessItems Methods
        /// <summary>
        /// returns all of the folders.
        /// </summary>
        public IEnumerable<FolderAccessItem> Folders
        {
            get
            {
                foreach (var item in _Folders)
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// Returns only the special folders.
        /// </summary>
        public IEnumerable<FolderAccessItem> SpecialFolders => from x in Folders where x.IsSpecialFolder select x;
        /// <summary>
        /// Returns only the non-special folders.
        /// </summary>
        public IEnumerable<FolderAccessItem> NonSpecialFolders => from x in Folders where !x.IsSpecialFolder select x;
        /// <summary>
        /// Returns the path for the specified <paramref name="specialFolderKey"/>; otherwise, it will return <b>null</b> if the <paramref name="specialFolderKey"/> is not found.
        /// </summary>
        /// <param name="specialFolderKey">The key to search for.</param>
        /// <returns>The path for the specified <paramref name="specialFolderKey"/>; otherwise, it will return <b>null</b> if the <paramref name="specialFolderKey"/> is not found.</returns>
        public string SpecialFolder(string specialFolderKey)
        {
            foreach (var item in _Folders)
            {
                if ((item.IsSpecialFolder) && (specialFolderKey.Equals(item.SpecialPathKey, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return item.Path;
                }
            }
            //
            return null;
        }
        /// <summary>
        /// Returns whether the <paramref name="path"/> is in this collection.
        /// </summary>
        /// <param name="path">The path to search for.</param>
        /// <returns><b>True</b> if the <paramref name="path"/> is in this collection; otherwise <b>false</b>.</returns>
        public bool IsFolderValid(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }
            //
            foreach (var folder in _Folders)
            {
                if (path.StartsWith(folder.Path, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            //
            return false;
        }
        #endregion
        #region ICollection and IReadOnlyCollection Methods
        /// <summary>
        /// Gets the number of elements contained in this collection.
        /// </summary>
        public int Count => _Folders.Count;
        /// <summary>
        /// Gets a value indicating whether this collection is read only.
        /// </summary>
        public bool IsReadOnly => false;
        /// <summary>
        /// Adds an item to this collection.
        /// </summary>
        /// <param name="item">The <see cref="FolderAccessItem"/> to add to this collection.</param>
        public void Add(FolderAccessItem item)
        {
            if (!_Folders.Contains(item))
            {
                _Folders.Add(item);
            }
        }
        /// <summary>
        /// Removes all items from this collection.
        /// </summary>
        public void Clear() => _Folders.Clear();
        /// <summary>
        /// Determines whether the <paramref name="item"/> is in this collection.
        /// </summary>
        /// <param name="item">The object to locate in this collection.</param>
        /// <returns><b>True</b> if item is found in this collection; otherwise, <b>false</b>.</returns>
        public bool Contains(FolderAccessItem item) => _Folders.Contains(item);
        /// <summary>
        /// Copies the elements of this collection to an <see cref="System.Array"/>, starting at a particular index.
        /// </summary>
        /// <param name="array">The array to receive the items.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(FolderAccessItem[] array, int arrayIndex) => _Folders.CopyTo(array, arrayIndex);
        /// <summary>
        /// Removes the first occurrence of a specific <see cref="FolderAccessItem"/> from this collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><b>True</b> if item is removed from this collection; otherwise, <b>false</b>.</returns>
        public bool Remove(FolderAccessItem item) => _Folders.Remove(item);
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<FolderAccessItem> GetEnumerator() => _Folders.GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _Folders.GetEnumerator();
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("FolderAccessItems");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // special folders
                var specialFolders = result.Add("SpecialFolders");
                foreach (var folder in SpecialFolders)
                {
                    specialFolders.Items.Add(folder.SpecialPathKey, folder.Path, typeof(string));
                }
                // non -special folders
                var count = 0;
                var folders = result.Add("Folders");
                foreach (var folder in NonSpecialFolders)
                {
                    folders.Items.Add($"Folder {++count}", folder.Path, typeof(string));
                }
                // ####
                return result;
            }
        }
        #endregion
    }
}

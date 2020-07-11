using System.Collections.Generic;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Defines advanced functionality required by types in the dodSON.Core.FileStorage
    /// namespace, but not generally used by the typical consumer.
    /// </summary>
    public interface IFileStoreAdvanced
    {
        /// <summary>
        /// This will remove all files from the file store, including those files marked for deletion.
        /// </summary>
        void ResetLists();
        /// <summary>
        /// This will remove all files marked for deletion from the file store.
        /// </summary>
        void ClearDeleted();
        /// <summary>
        /// Will return a dictionary, keyed by the root filename, of all files and their cooresponding original filename.
        /// </summary>
        /// <returns>A dictionary, keyed by the root filename, of all files and their cooresponding original filename.</returns>
        IDictionary<string, string> GetOriginalFilenames();
        /// <summary>
        /// For each root filename in the <paramref name="dictionary"/>, this function will 
        /// set its original filename to the cooresponding value.
        /// </summary>
        /// <param name="dictionary">A dictionary, keyed by root filename and containing a value for the new original filename.</param>
        void SetOriginalFilenames(IDictionary<string, string> dictionary);
        /// <summary>
        /// Clears the original filenames of all files.
        /// </summary>
        void ClearOriginalFilenames();
    }
}

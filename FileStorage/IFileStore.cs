using System;
using System.Collections.Generic;

namespace dodSON.Core.FileStorage
{
    // TODO: create an IReadOnlyFileStore

    
    // TODO: consider adding the concept of extracting to 'non-file-system-based' locations, i.e. streams, bytes arrays, strings, ...
    //       this will include new Extract...() methods, the abstraction of some Count...() methods

    /// <summary>
    /// Defines functionality to control a disconnected file store.
    /// </summary>
    public interface IFileStore
        : IFileStoreAdvanced,
        Configuration.IConfigurable,
        System.Collections.Specialized.INotifyCollectionChanged
    {
        /// <summary>
        /// Displays change state.
        /// </summary>
        Common.IStateChangeView StateChangeViewer { get; }
        /// <summary>
        /// Gets or sets whether the original filenames should be persisted, or not.
        /// </summary>
        bool SaveOriginalFilenames { get; set; }
        /// <summary>
        /// Get or sets the top directory where files will be extracted by default.
        /// </summary>
        string ExtractionRootDirectory { get; set; }
        /// <summary>
        /// Gets the number of files in the store. (This includes all files, those actually in the backend store and those which have been added to the store but not yet saved to the backend.)
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets the number of files that have been marked for deletion.
        /// </summary>
        int CountDeleted { get; }
        /// <summary>
        /// Gets the number of files which have original files available.
        /// </summary>
        int CountOriginal { get; }
        /// <summary>
        /// Gets the number of files which have extracted files available.
        /// </summary>
        int CountExtracted { get; }
        /// <summary>
        /// Gets the number of files where the extracted file can be updated by the file in the store.
        /// </summary>
        int CountExtractedUpdatable { get; }
        /// <summary>
        /// Get the number of files in the backend store.
        /// </summary>
        int CountAvailableRootFiles { get; }
        /// <summary>
        /// The size, in bytes, of all files in the store.
        /// </summary>
        long SizeInBytes { get; }
        /// <summary>
        /// Determines whether the <paramref name="rootFilename"/> is in the store.
        /// </summary>
        /// <param name="rootFilename">The file to locate in the store.</param>
        /// <returns><b>True</b> if the file is found in the store; otherwise, <b>false</b>.</returns>
        bool Contains(string rootFilename);
        /// <summary>
        /// Determines whether the <paramref name="rootFilename"/> is in the store and has been marked for deletion.
        /// </summary>
        /// <param name="rootFilename">The file to locate in the store.</param>
        /// <returns><b>True</b> if the file is found in the store; otherwise, <b>false</b>.</returns>
        bool ContainsDeleted(string rootFilename);
        /// <summary>
        /// Creates the proper <see cref="IFileStoreItem"/>, populates it with the given information and adds it to the store.
        /// </summary>
        /// <param name="rootFilename">The name of the file.</param>
        /// <param name="originalFilename">The full filename to the original file.</param>
        /// <param name="rootFileLastModifiedTimeUtc">Last time the file was modified, in UTC time.</param>
        /// <param name="sizeInBytes">The size, in bytes, of the file.</param>
        /// <returns>The added <see cref="IFileStoreItem"/>.</returns>
        IFileStoreItem Add(string rootFilename, string originalFilename, DateTime rootFileLastModifiedTimeUtc, long sizeInBytes);
        /// <summary>
        /// Locates the <see cref="IFileStoreItem"/> by <paramref name="rootFilename"/> and marks it for deletion.
        /// </summary>
        /// <param name="rootFilename">The <see cref="IFileStoreItem"/> to delete.</param>
        /// <returns>The deleted <see cref="IFileStoreItem"/>.</returns>
        /// <remarks>In order to commit any changes to the store, like adding and deleting files, the store must be <see cref="Save"/>ed.</remarks>
        IFileStoreItem Delete(string rootFilename);
        /// <summary>
        /// Locates the <see cref="IFileStoreItem"/> by <paramref name="rootFilename"/> and un-marks it for deletion.
        /// </summary>
        /// <param name="rootFilename">The <see cref="IFileStoreItem"/> to undelete.</param>
        /// <returns>The undeleted <see cref="IFileStoreItem"/>.</returns>
        /// <remarks>In order to commit any changes to the store, like adding and deleting files, the store must be <see cref="Save"/>ed.</remarks>
        IFileStoreItem Undelete(string rootFilename);
        /// <summary>
        /// Deletes all files.
        /// </summary>
        /// <remarks>
        /// Depending on the state of the files in the store, this function could delete some <see cref="IFileStoreItem"/>
        /// and mark others for deletion. Therefore, it is incumbent on the user to commit any changes to the store by 
        /// executing the <see cref="Save"/> method.
        /// </remarks>
        void Clear();
        /// <summary>
        /// Gets the <see cref="IFileStoreItem"/> by <paramref name="rootFilename"/>.
        /// </summary>
        /// <param name="rootFilename">The file to locate.</param>
        /// <returns>The <see cref="IFileStoreItem"/>, or null.</returns>
        IFileStoreItem Get(string rootFilename);
        /// <summary>
        /// Gets the <see cref="IFileStoreItem"/> by <paramref name="rootFilename"/> from the set of files which have been marked for deletion.
        /// </summary>
        /// <param name="rootFilename">The <see cref="IFileStoreItem"/> to locate.</param>
        /// <returns>The <see cref="IFileStoreItem"/>, or null.</returns>
        IFileStoreItem GetDeleted(string rootFilename);
        /// <summary>
        /// Gets all files in the store.
        /// </summary>
        /// <returns>All files in the store.</returns>
        IEnumerable<IFileStoreItem> GetAll();
        /// <summary>
        /// Gets all of the files in the store marked for deletion.
        /// </summary>
        /// <returns>All files in the store marked for deletion.</returns>
        IEnumerable<IFileStoreItem> GetAllDeleted();
        /// <summary>
        /// Searches the store for any matches, as defined by the specified predicate, and returns all matching items.
        /// </summary>
        /// <param name="predicate">A delegate which defines the conditions of the items to search for.</param>
        /// <returns>An IEnumerable containing only the items which the predicate delegate returned true.</returns>
        IEnumerable<IFileStoreItem> Find(Predicate<IFileStoreItem> predicate);
        /// <summary>
        /// Performs the provided action on each item contained within the store.
        /// </summary>
        /// <param name="action">The delegate to perform on each item.</param>
        void ForEach(Action<IFileStoreItem> action);
        /// <summary>
        /// Cause the store to reload.
        /// </summary>
        void Refresh();
        /// <summary>
        /// Persist all of the changes in the store.
        /// </summary>
        /// <param name="autoUpdateNonDirtyRootFiles">Determines if an updatable file should be updated.</param>
        void Save(bool autoUpdateNonDirtyRootFiles);
        /// <summary>
        /// Determines if the store can restore it state.
        /// </summary>
        bool SupportRollback { get; }
        /// <summary>
        /// Restore the original state of the store.
        /// </summary>
        void Rollback();
        /// <summary>
        /// Extracts all files to the default root directory.
        /// </summary>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>A list of all the extracted files.</returns>
        IEnumerable<string> ExtractAll(bool explicitExtraction);
        /// <summary>
        /// Extracts all files to the <paramref name="destRootPath"/>.
        /// </summary>
        /// <param name="destRootPath">The directory to extract the files to.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>A list of all the extracted files.</returns>
        IEnumerable<string> ExtractAll(string destRootPath, bool explicitExtraction);
        /// <summary>
        /// Extracts the <paramref name="item"/> to the default root directory.
        /// </summary>
        /// <param name="item">The file to extract.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>The full name of the extracted file.</returns>
        string Extract(IFileStoreItem item, bool explicitExtraction);
        /// <summary>
        /// Extracts the <paramref name="item"/> to the <paramref name="destRootPath"/>.
        /// </summary>
        /// <param name="destRootPath">The directory to extract the files to.</param>
        /// <param name="item">The file to extract.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>The full name of the extracted file.</returns>
        string Extract(string destRootPath, IFileStoreItem item, bool explicitExtraction);
        /// <summary>
        /// Extracts the list of files to the default root directory.
        /// </summary>
        /// <param name="items">The files to extract.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>A list of the extracted files.</returns>
        IEnumerable<string> Extract(IEnumerable<IFileStoreItem> items, bool explicitExtraction);
        /// <summary>
        /// Extracts the list of files to the <paramref name="destRootPath"/>.
        /// </summary>
        /// <param name="destRootPath">The directory to extract the files to.</param>
        /// <param name="items">The files to extract.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>A list of the extracted files.</returns>
        IEnumerable<string> Extract(string destRootPath, IEnumerable<IFileStoreItem> items, bool explicitExtraction);
        /// <summary>
        /// Will extract the <paramref name="items"/> from the back-end directories to the <paramref name="destRootPath"/>.
        /// </summary>
        /// <param name="destRootPath">The directory to extract the files to.</param>
        /// <param name="items">The files to extract.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>A list of the extracted files.</returns>
        IEnumerable<string> ExtractToPath(string destRootPath, IEnumerable<IFileStoreItem> items, bool explicitExtraction);
        /// <summary>
        /// Creates a <see cref="IFileStoreItem"/> specific for this store.
        /// </summary>
        /// <param name="rootFilename">The name of the file.</param>
        /// <param name="originalFilename">The full filename to the original file.</param>
        /// <param name="rootFileLastModifiedTimeUtc">Last time the file was modified, in UTC time.</param>
        /// <param name="sizeInBytes">The size, in bytes, of the file.</param>
        /// <returns>A <see cref="IFileStoreItem"/> specific for this store, populated with the given file.</returns>
        IFileStoreItem CreateNewFileStoreItem(string rootFilename, string originalFilename, DateTime rootFileLastModifiedTimeUtc, long sizeInBytes);
        /// <summary>
        /// Determines if the specified file is available in the store.
        /// </summary>
        /// <param name="rootFilename">The file to locate.</param>
        /// <returns><b>True</b> means the file is available; otherwise, <b>false</b> means the file is not available.</returns>
        bool IsRootFileAvailable(string rootFilename);
    }
}

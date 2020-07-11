
namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Defines advanced functionality required by types in the dodSON.Core.FileStorage
    /// namespace, but not generally used by the typical consumer.
    /// </summary>
    public interface IFileStoreItemAdvanced
    {
        /// <summary>
        /// Sets the <see cref="IFileStore"/> this file store item belongs to.
        /// </summary>
        /// <param name="fileStore">The file store this file store item belongs to.</param>
        void SetParent(IFileStore fileStore);
        /// <summary>
        /// Allows changes to be made to the change state of this file.
        /// </summary>
        Common.IStateChangeTracking StateChangeTracker { get; }
        /// <summary>
        /// Sets the original filename for this file store item to <paramref name="filename"/>.
        /// </summary>
        /// <param name="filename">The filename of the file this file store item is based on.</param>
        void SetOriginalFilename(string filename);
        /// <summary>
        /// Sets the last modified time, in universal coordinated time, for this file store item.
        /// </summary>
        /// <param name="utc">The last modified time, in universal coordinated time, this file store item was modified.</param>
        void SetRootFileLastModifiedTimeUtc(System.DateTime utc);
    }
}

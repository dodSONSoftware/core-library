using System;
using System.Collections.Generic;
using System.Linq;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Defines functionality to control a file in a <see cref="IFileStore"/>.
    /// </summary>
    public class FileStoreItem
        : IFileStoreItem
    {
        #region Ctor
        /// <summary>
        /// A protected constructor.
        /// </summary>
        protected FileStoreItem()
        {
            _StateChangeTracker.MarkNew();
        }
        /// <summary>
        /// Initializes a new instance of the FileStoreItem class.
        /// </summary>
        /// <param name="parent">The <see cref="IFileStore"/> this file belong to.</param>
        /// <param name="rootFilename">The name of the file in the store. This is a relative file name; for example: filename.txt, folder/filename.txt and folder/subfolder/filename.txt are all valid and will be found at the locations defined.</param>
        /// <param name="originalFilename">The filename of the original file this file store item is based on.</param>
        /// <param name="sizeInBytes">The size, in bytes.</param>
        /// <param name="lastModifiedTimeUtc">The last time, in universal coordinated time, the actual back-end file has been modified.</param>
        public FileStoreItem(IFileStore parent,
                             string rootFilename,
                             string originalFilename,
                             long sizeInBytes,
                             DateTime lastModifiedTimeUtc)
            : this()
        {
            if (string.IsNullOrWhiteSpace(rootFilename)) { throw new ArgumentNullException("rootFilename"); }
            // persist data
            Parent = parent ?? throw new ArgumentNullException("parent");
            RootFilename = rootFilename;
            RootFileLastModifiedTimeUtc = NormalizeDateTime(lastModifiedTimeUtc);
            OriginalFilename = originalFilename;
            FileSize = sizeInBytes;
        }
        #endregion
        #region Private Fields
        private readonly Common.IStateChangeTracking _StateChangeTracker = new dodSON.Core.Common.StateChangeTracking();
        #endregion
        #region IFileStoreItem Methods
        /// <summary>
        /// The <see cref="IFileStore"/> this file belong to.
        /// </summary>
        public IFileStore Parent { get; private set; } = null;
        /// <summary>
        /// Displays change state.
        /// </summary>
        public Common.IStateChangeView StateChangeViewer => _StateChangeTracker;
        /// <summary>
        /// Get the Name of the <see cref="RootFilename"/>; that is, without the <see cref="Path"/>.
        /// </summary>
        public string Name { get => GetFileName; }
        /// <summary>
        /// Get the path of the <see cref="RootFilename"/>.
        /// </summary>
        public string Path { get => GetRootPath; }
        /// <summary>
        /// The size, in bytes, of the <see cref="RootFilename"/>.
        /// </summary>
        public long FileSize { get; }
        /// <summary>
        /// The name of the file in the store. This is a relative file name; for example: filename.txt, 
        /// folder/filename.txt and folder/subfolder/filename.txt are all valid and will be found at
        /// the locations defined.
        /// </summary>
        public string RootFilename { get; } = "";
        /// <summary>
        /// The filename of the original file this file store item is based on.
        /// </summary>
        public string OriginalFilename { get; private set; } = "";
        /// <summary>
        /// The filename of the extracted file, if any, for this file store item.
        /// </summary>
        public string ExtractedFilename
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Parent.ExtractionRootDirectory))
                {
                    return "";
                }
                return System.IO.Path.Combine(Parent.ExtractionRootDirectory, RootFilename);
            }
        }
        /// <summary>
        /// Returns if the actual file is available in the backend storage system.
        /// </summary>
        public bool IsRootFileAvailable
        {
            get
            {
                // this works because of the assumption that the "store" will never contain a NEW & DELETED file.
                return (StateChangeViewer.IsDeleted || !StateChangeViewer.IsNew);
            }
        }
        /// <summary>
        /// Returns if the original file this file store item is based on exists.
        /// </summary>
        public bool IsOriginalFileAvailable => (System.IO.File.Exists(OriginalFilename));
        /// <summary>
        /// Returns if this file store item's actual backend file has been extracted.
        /// </summary>
        public bool IsExtractedFileAvailable => System.IO.File.Exists(ExtractedFilename);
        /// <summary>
        /// The last time, in universal coordinated time, the actual backend file has been modified.
        /// </summary>
        public DateTime RootFileLastModifiedTimeUtc { get; private set; } = DateTime.MinValue;
        /// <summary>
        /// The last time, in universal coordinated time, the original file this file store item is based on has been modified.
        /// </summary>
        public DateTime OriginalFileLastModifiedTimeUtc
        {
            get
            {
                if (IsOriginalFileAvailable)
                {
                    var fileInfo = new System.IO.FileInfo(OriginalFilename);
                    return NormalizeDateTime(fileInfo.LastWriteTimeUtc);
                }
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// The last time, in universal coordinated time, the file store item's extracted file has been modified.
        /// </summary>
        public DateTime ExtractedFileLastModifiedTimeUtc
        {
            get
            {
                if (IsExtractedFileAvailable)
                {
                    var fileInfo = new System.IO.FileInfo(ExtractedFilename);
                    return NormalizeDateTime(fileInfo.LastWriteTimeUtc);
                }
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// Returns if the actual file in the back end storage system can be updated from the <see cref="OriginalFilename"/>.
        /// </summary>
        public bool IsRootFileUpdatable
        {
            get
            {
                if (IsOriginalFileAvailable)
                {
                    return (FileTimeDifferential(RootFileLastModifiedTimeUtc, OriginalFileLastModifiedTimeUtc) < TimeSpan.Zero);
                }
                return false;
            }
        }
        /// <summary>
        /// Returns if the file store item's extracted file is updatable from the actual back end file.
        /// </summary>
        public bool IsExtractedFileUpdatable
        {
            get
            {
                if (IsExtractedFileAvailable)
                {
                    return (FileTimeDifferential(ExtractedFileLastModifiedTimeUtc, RootFileLastModifiedTimeUtc) < TimeSpan.Zero);
                }
                return false;
            }
        }
        /// <summary>
        /// Will extract the actual backend file to the default root path. 
        /// </summary>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>The name of the extracted file.</returns>
        public string Extract(bool explicitExtraction)
        {
            if (Parent == null)
            {
                throw new Exception("Cannot extract file. ParentFileStore is null.");
            }
            if (string.IsNullOrWhiteSpace(Parent.ExtractionRootDirectory))
            {
                throw new Exception("The Extracted Root Directory has not been set.");
            }
            if (!System.IO.Directory.Exists(Parent.ExtractionRootDirectory))
            {
                throw new System.IO.DirectoryNotFoundException(string.Format("Directory={0}", Parent.ExtractionRootDirectory));
            }
            return Extract(Parent.ExtractionRootDirectory, explicitExtraction);
        }
        /// <summary>
        /// Will extract the actual backend file to the <paramref name="destRootPath"/>.
        /// </summary>
        /// <param name="destRootPath"></param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>The name of the extracted file.</returns>
        public string Extract(string destRootPath, bool explicitExtraction)
        {
            if (Parent == null)
            {
                throw new Exception("Cannot extract file. ParentFileStore is null.");
            }
            if (string.IsNullOrWhiteSpace(destRootPath))
            {
                throw new ArgumentNullException("destRootPath", "Parameter destRootPath cannot be null, empty or whitespace.");
            }
            if (!System.IO.Directory.Exists(destRootPath))
            {
                throw new System.IO.DirectoryNotFoundException(string.Format("Directory={0}", destRootPath));
            }
            return Parent.Extract(destRootPath, new List<IFileStoreItem>() { this as IFileStoreItem }, explicitExtraction).ElementAtOrDefault(0);
        }
        #endregion
        #region Explicit IFileStoreItemAdvanced Methods
        void IFileStoreItemAdvanced.SetParent(IFileStore parent)
        {
            //if (parent == null) { throw new ArgumentNullException("parent"); }
            Parent = parent;
        }
        Common.IStateChangeTracking IFileStoreItemAdvanced.StateChangeTracker => _StateChangeTracker;
        void IFileStoreItemAdvanced.SetOriginalFilename(string filename)
        {
            if (!OriginalFilename.Equals(filename))
            {
                OriginalFilename = filename;
            }
        }
        void IFileStoreItemAdvanced.SetRootFileLastModifiedTimeUtc(DateTime utc) => RootFileLastModifiedTimeUtc = NormalizeDateTime(utc);
        #endregion
        #region Private Methods
        private DateTime NormalizeDateTime(DateTime dTime) => new DateTime(dTime.Year, dTime.Month, dTime.Day, dTime.Hour, dTime.Minute, dTime.Second, dTime.Kind);
        private TimeSpan FileTimeDifferential(DateTime file1_DateTimeUtc, DateTime file2_DateTimeUtc) => (file1_DateTimeUtc - file2_DateTimeUtc);
        private string GetFileName
        {
            get
            {
                if (RootFilename.Contains("\\")) { return RootFilename.Substring(RootFilename.LastIndexOf("\\") + 1); }
                else { return RootFilename; }
            }
        }
        private string GetRootPath
        {
            get
            {
                if (RootFilename.Contains("\\")) { return RootFilename.Substring(0, RootFilename.LastIndexOf("\\")); }
                else { return ""; }
            }
        }
        #endregion
    }
}

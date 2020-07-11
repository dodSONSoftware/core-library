using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Represents a single folder in a list of hierarchical folders.
    /// See the <see cref="dodSON.Core.FileStorage.FileStorageHelper.ConvertToHierarchicalList(IFileStore, string, string)"/> method.
    /// </summary>
    public class HierarchicalFolder
        : dodSON.Core.Common.NotifyPropertyChangedBase,
          IHierarchicalFolder
    {
        #region Ctor
        /// <summary>
        /// Creates a default <see cref="HierarchicalFolder"/>.
        /// </summary>
        protected HierarchicalFolder() : this(null, "", "", false) { }
        /// <summary>
        /// Instantiates a new <see cref="HierarchicalFolder"/> with the given <paramref name="name"/> and <paramref name="rootPath"/>.
        /// </summary>
        /// <param name="parent">The parent folder this folder belongs to.</param>
        /// <param name="name">The name of the folder.</param>
        /// <param name="rootPath">The full root path for the folder.</param>
        /// <param name="isCompressed">Sets if this <see cref="HierarchicalFolder"/> represents a compressed folder.</param>
        public HierarchicalFolder(IHierarchicalFolder parent,
                                  string name,
                                  string rootPath,
                                  bool isCompressed)
            : base()
        {
            Parent = parent;
            Name = name;
            Rootpath = rootPath;
            IsCompressed = isCompressed;
        }
        #endregion
        #region Public Properties
        private IHierarchicalFolder _Parent = null;
        /// <summary>
        /// The parent folder this folder belongs to.
        /// </summary>
        public IHierarchicalFolder Parent
        {
            get => _Parent;
            set => SetProperty(ref _Parent, value);
        }
        private bool _IsCompressed = false;
        /// <summary>
        /// Gets and set whether this <see cref="HierarchicalFolder"/> represents a compressed folder.
        /// </summary>
        public bool IsCompressed
        {
            get => _IsCompressed;
            set => SetProperty(ref _IsCompressed, value);
        }

        private string _Name = "";
        /// <summary>
        /// The name of the folder.
        /// </summary>
        public string Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }

        private string _Rootpath = "";
        /// <summary>
        /// The full root path for the folder.
        /// </summary>
        public string Rootpath
        {
            get => _Rootpath;
            set => SetProperty(ref _Rootpath, value);
        }

        /// <summary>
        /// The size, in bytes, of all files in this folder.
        /// </summary>
        public long FilesTotalSize
        {
            get
            {
                long total = 0;
                foreach (var item in Files) { total += item.FileSize; }
                return total;
            }
        }

        /// <summary>
        /// The compressed size, in bytes, of all files in this folder.
        /// </summary>
        public long CompressedFilesTotalSize
        {
            get
            {
                long total = 0;
                if (IsCompressed) { foreach (var item in Files) { total += (item as ICompressedFileStoreItem).CompressedFileSize; } }
                return total;
            }
        }

        /// <summary>
        /// The compressed percentage of all the files in this folder.
        /// </summary>
        public double CompressionFilesRatio
        {
            get
            {
                double total = 0;
                if (IsCompressed) { total = (double)CompressedFilesTotalSize / FilesTotalSize; }
                return total;
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<IHierarchicalFolder> _Folders = new System.Collections.ObjectModel.ObservableCollection<IHierarchicalFolder>();

        /// <summary>
        /// The list of <see cref="HierarchicalFolder"/> contained in this <see cref="HierarchicalFolder"/>.
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<IHierarchicalFolder> Folders
        {
            get => _Folders;
            set => SetProperty(ref _Folders, value);
        }

        private System.Collections.ObjectModel.ObservableCollection<IFileStoreItem> _Files = new System.Collections.ObjectModel.ObservableCollection<IFileStoreItem>();
        /// <summary>
        /// The list of <see cref="IFileStoreItem"/>s contained in this <see cref="HierarchicalFolder"/>.
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<IFileStoreItem> Files
        {
            get => _Files;
            set => SetProperty(ref _Files, value);
        }
        #endregion
    }
}

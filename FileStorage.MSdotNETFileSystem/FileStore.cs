using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.FileStorage.MSdotNETFileSystem
{
    /// <summary>
    /// Provides a directory-based implementation of the <see cref="dodSON.Core.FileStorage.FileStoreBase"/>.
    /// </summary>
    public class FileStore
        : dodSON.Core.FileStorage.FileStoreBase
    {
        //#region Constants
        //private readonly string _OriginalFilenames_Filename_ = "AAA1966CB552437D45F7AE3408E247A87RED49.filestore.dat";
        //#endregion
        #region Ctor
        private FileStore() : base() { }
        /// <summary>
        /// Initializes an new instance of the MSdotNETFileSystem FileStore with the specified arguments.
        /// </summary>
        /// <param name="sourceDirectory">The name of the directory used for the back-end storage's root directory.</param>
        /// <param name="extractionDirectory">The directory to use as the default root path for extracting files.</param>
        /// <param name="saveOriginalFilenames">Determines whether the original filenames should be persisted.</param>
        public FileStore(string sourceDirectory,
                         string extractionDirectory,
                         bool saveOriginalFilenames)
            : base(extractionDirectory, saveOriginalFilenames)
        {
            if (string.IsNullOrWhiteSpace(sourceDirectory))
            {
                throw new ArgumentNullException(nameof(sourceDirectory));
            }
            if (!System.IO.Directory.Exists(sourceDirectory))
            {
                System.IO.Directory.CreateDirectory(sourceDirectory);
            }
            RootPath = sourceDirectory;
            InitializeStore();
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public FileStore(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "FileStore")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"FileStore\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // SourceDirectory
            var temp1 = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "SourceDirectory", typeof(string)).Value;
            if (string.IsNullOrWhiteSpace(temp1))
            {
                throw new Exception($"Configuration invalid information. Configuration item: \"SourceDirectory\" cannot be empty.");
            }
            if (!System.IO.Directory.Exists(temp1))
            {
                throw new System.IO.DirectoryNotFoundException(temp1);
            }
            RootPath = temp1;
            // ExtractionRootDirectory
            var temp2 = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ExtractionRootDirectory", typeof(string)).Value;
            if (string.IsNullOrWhiteSpace(temp2))
            {
                throw new Exception($"Configuration invalid information. Configuration item: \"ExtractionRootDirectory\" cannot be empty.");
            }
            if (!System.IO.Directory.Exists(temp2))
            {
                throw new System.IO.DirectoryNotFoundException(temp2);
            }
            ExtractionRootDirectory = temp2;
            // SaveOriginalFilenames
            SaveOriginalFilenames = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "SaveOriginalFilenames", typeof(bool)).Value;
            //            
            InitializeStore();
        }
        #endregion
        #region Private Fields
        private string _Extract_DestRootPath = "";
        #endregion
        #region Public Methods
        /// <summary>
        /// The name of the directory used for the back-end storage's root directory.
        /// </summary>
        public string RootPath { get; } = "";
        /// <summary>
        /// Creates a file store item, specifically for the MSdotNETFileSystem, based on the given meta-data.
        /// </summary>
        /// <param name="rootFilename">The name of the <see cref="IFileStoreItem"/> relative to the file store.</param>
        /// <param name="rootFileLastModifiedTimeUtc">A <see cref="DateTime"/> representing the last modified time of the <see cref="IFileStoreItem"/> to be created, in Universal Coordinated Time.</param>
        /// <param name="originalFilename">The full pathname of the file to add.</param>
        /// <param name="sizeInBytes">The size, in bytes.</param>
        /// <returns>An <see cref="IFileStoreItem"/> created using the original filename and given meta-data.</returns>
        public override dodSON.Core.FileStorage.IFileStoreItem CreateNewFileStoreItem(string rootFilename,
                                                                                      string originalFilename,
                                                                                      DateTime rootFileLastModifiedTimeUtc,
                                                                                      long sizeInBytes) => new dodSON.Core.FileStorage.FileStoreItem(this, rootFilename, originalFilename, sizeInBytes, rootFileLastModifiedTimeUtc);
        /// <summary>
        /// Returns <b>false</b>, which indicates that this file store <b>does not</b> supports <see cref="Rollback"/> functionality.
        /// Any call to <see cref="Rollback"/> will cause a <see cref="NotImplementedException"/> to be thrown.
        /// <seealso cref="Rollback"/>
        /// </summary>
        public override bool SupportRollback => false;
        /// <summary>
        /// Rollback <b>is not</b> supported for this type. 
        /// This function <b>will</b> throw an exception when called.
        /// <seealso cref="SupportRollback"/>
        /// </summary>
        /// <exception cref="NotImplementedException"/>
        public override void Rollback()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Protected Abstract Methods
        /// <summary>
        /// Reloads the file store.
        /// </summary>
        protected override void Refresh_Refresh() => InitializeStore();
        /// <summary>
        /// Will initialize the back-end directories to prepare to add, update and remove files.
        /// </summary>
        /// <param name="state">A null reference, <b>Nothing</b> in VB, because no state needs to be passed around. This reference will be passed to all Save_ methods.</param>
        protected override void Save_Startup(out object state) => state = null;
        /// <summary>
        /// Will add the <paramref name="item"/> to the back-end directories.
        /// </summary>
        /// <param name="item">The store item to add.</param>
        /// <param name="state">A null reference, <b>Nothing</b> in VB.</param>
        protected override void Save_AddFileToSource(IFileStoreItem item, object state) => AddFile(item);
        /// <summary>
        /// Will update an existing file in the back-end directories with given <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The store item to update from.</param>
        /// <param name="state">A null reference, <b>Nothing</b> in VB.</param>
        protected override void Save_UpdateFileInSource(IFileStoreItem item, object state) => AddFile(item);
        /// <summary>
        /// Will remove an existing file, referenced by <paramref name="item"/>, from the back-end directories. 
        /// </summary>
        /// <param name="item">The store item to remove.</param>
        /// <param name="state">A null reference, <b>Nothing</b> in VB.</param>
        protected override void Save_RemoveFileFromSource(IFileStoreItem item, object state) => RemoveFile(item);
        /// <summary>
        /// Will persist the list of original filenames to the back-end directories.
        /// </summary>
        /// <param name="state">A null reference, <b>Nothing</b> in VB.</param>
        protected override void Save_SaveOriginalFilenames(object state)
        {
            var filename = System.IO.Path.Combine(RootPath, _OriginalFilenames_Filename);
            using (var sw = new System.IO.StreamWriter(filename, false))
            {
                sw.Write(OriginalFilenamesToString());
                sw.Flush();
            }
        }
        /// <summary>
        /// Will finalize the adding, updating and removing of files from the back-end directories.
        /// </summary>
        /// <param name="state">A null reference, <b>Nothing</b> in VB.</param>
        protected override void Save_Shutdown(ref object state)
        {
        }
        /// <summary>
        /// Will initialize the back-end directories to prepare to extract files.
        /// </summary>
        /// <param name="destRootPath">The root directory where extraction will be performed.</param>
        /// <param name="state">A null reference, <b>Nothing</b> in VB, because no state needs to be passed around. This reference will be passed to all Extract_ methods.</param>
        protected override void Extract_StartUp(string destRootPath, out object state)
        {
            _Extract_DestRootPath = destRootPath;
            state = null;
        }
        /// <summary>
        /// Will extract the <paramref name="item"/> from the back-end directories to the destination root path given in the method <see cref="Extract_StartUp(string, out object)"/>.
        /// </summary>
        /// <param name="item">The store item to extract.</param>
        /// <param name="state">A null reference, <b>Nothing</b> in VB.</param>
        /// <returns>The filename of the extracted file.</returns>
        protected override string Extract_ExtractFile(IFileStoreItem item, object state)
        {
            if (!item.StateChangeViewer.IsNew)
            {
                CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.Combine(_Extract_DestRootPath, item.RootFilename)));
                var sourceFilename = System.IO.Path.Combine(RootPath, CleanFilenameString(item.RootFilename));
                var destFilename = System.IO.Path.Combine(_Extract_DestRootPath, CleanFilenameString(item.RootFilename));
                System.IO.File.Copy(sourceFilename, destFilename, true);
                return (destFilename);
            }
            return "";
        }
        /// <summary>
        /// Will extract the <paramref name="item"/> from the back-end directories to the <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="destinationPath">The directory to extract files to.</param>
        /// <param name="item">The store item to extract.</param>
        /// <param name="state">A null reference, <b>Nothing</b> in VB.</param>
        /// <returns>The filename of the extracted file.</returns>
        protected override string Extract_ExtractFileToPath(string destinationPath, IFileStoreItem item, object state)
        {
            CreateDirectory(destinationPath);
            var sourceFilename = System.IO.Path.Combine(RootPath, CleanFilenameString(item.RootFilename));
            var destFilename = System.IO.Path.Combine(destinationPath, CleanFilenameString(item.Name));
            System.IO.File.Copy(sourceFilename, destFilename, true);
            return destFilename;
        }
        /// <summary>
        /// Will finalize the extraction of files from the back-end directories. 
        /// </summary>
        /// <param name="state">A null reference, <b>Nothing</b> in VB.</param>
        protected override void Extract_Shutdown(ref object state)
        {
        }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public override Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("FileStore");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                result.Items.Add("SourceDirectory", RootPath, RootPath.GetType());
                result.Items.Add("ExtractionRootDirectory", ExtractionRootDirectory, ExtractionRootDirectory.GetType());
                result.Items.Add("SaveOriginalFilenames", SaveOriginalFilenames, SaveOriginalFilenames.GetType());
                return result;
            }
        }
        #endregion
        #region Private Methods
        private void InitializeStore()
        {
            // clean self
            ((dodSON.Core.FileStorage.IFileStoreAdvanced)this).ResetLists();
            // iterate all files, in all subdirectories, in _SourceDirectory
            foreach (var originalFilename in System.IO.Directory.GetFiles(RootPath, "*.*", System.IO.SearchOption.AllDirectories))
            {
                // do not process _OriginalFilenames_Filename_
                if (!System.IO.Path.GetFileName(originalFilename).Equals(_OriginalFilenames_Filename, StringComparison.InvariantCultureIgnoreCase))
                {
                    // process file
                    var rootName = CleanFilenameString(originalFilename.Substring(RootPath.Length));
                    // get file information
                    var sizeInBytes = (new System.IO.FileInfo(originalFilename)).Length;
                    // create new store item/add to self
                    var newItem = Add(rootName, originalFilename, new System.IO.FileInfo(originalFilename).LastWriteTimeUtc, sizeInBytes);
                    // clear all state flags
                    ((dodSON.Core.FileStorage.IFileStoreItemAdvanced)newItem).StateChangeTracker.ClearAll();
                }
            }
            // load original filenames file. (this is easier, may, or may not, be more efficient.)
            if (System.IO.File.Exists(System.IO.Path.Combine(RootPath, _OriginalFilenames_Filename)))
            {
                try
                {
                    using (var sr = new System.IO.StreamReader(System.IO.Path.Combine(RootPath, _OriginalFilenames_Filename)))
                    {
                        ((dodSON.Core.FileStorage.IFileStoreAdvanced)this).SetOriginalFilenames(StringToOriginalFilenames(sr.ReadToEnd()));
                    }
                }
                catch
                {
                    // ignore any problems; this may fail for a variety of non-related issues. 
                }
            }
        }
        private string CleanFilenameString(string filename)
        {
            filename = filename.Replace('/', '\\');
            while (filename.StartsWith("\\"))
            {
                filename = filename.Substring(1);
            }
            return filename;
        }
        // serialize type into string
        private string OriginalFilenamesToString() => (new dodSON.Core.Converters.TypeSerializer<IDictionary<string, string>>()).ToString(((dodSON.Core.FileStorage.IFileStoreAdvanced)this).GetOriginalFilenames());
        // deserialize string into type
        private IDictionary<string, string> StringToOriginalFilenames(string source) => (new dodSON.Core.Converters.TypeSerializer<IDictionary<string, string>>()).FromString(source);
        private void CreateDirectory(string directory)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        }
        private void AddFile(dodSON.Core.FileStorage.IFileStoreItem item)
        {
            var destFilename = System.IO.Path.Combine(RootPath, CleanFilenameString(item.RootFilename));
            if (!string.Equals(item.OriginalFilename, destFilename, StringComparison.InvariantCultureIgnoreCase))
            {
                CreateDirectory(System.IO.Path.GetDirectoryName(destFilename));
                System.IO.File.Copy(item.OriginalFilename, destFilename, true);
            }
        }
        private void RemoveFile(dodSON.Core.FileStorage.IFileStoreItem item)
        {
            var filename = System.IO.Path.Combine(RootPath, CleanFilenameString(item.RootFilename));
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
        }
        #endregion
    }
}

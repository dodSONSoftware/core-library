using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;



// TODO: this type contains an [Ionic.Zip.ZipFile] which is Disposable.
//          see [dodSON.Core.FileStorage.Save(...) and dodSON.Core.FileStorageExtract(...)] for try...finally statements that guarantee the Save_ShutDown(...) and Extract_Shutdown(...) are called.
//          Save_ShutDown(...) and Extract_Shutdown(...) guarantee that the Ionic.Zip.ZipFile object is disposed.



namespace dodSON.Core.FileStorage.MSdotNETZip
{
    /// <summary>
    /// Provides a <see cref="System.IO.Compression">Mircosoft .NET</see> implementation of the <see cref="dodSON.Core.FileStorage.CompressedFileStoreBase"/>.
    /// </summary>
    public class FileStore
        : dodSON.Core.FileStorage.CompressedFileStoreBase
    {
        #region Constants
        //// for backwards/sideways compatibility, do not change this value.
        //private static readonly string _OriginalFilenames_Filename_ = "AAA1966B51F095A794E276C7163FAB537RED49.filestore.dat";
        //
        private static readonly string _EngineName_ = ".NET (System.IO.Compression)";
        private static readonly Version _EngineVersion_ = Environment.Version;
        #endregion
        #region Ctor
        private FileStore() : base() { }

        /// <summary>
        /// Initializes an new instance of the dotNETZip FileStore with the specified arguments.
        /// </summary>
        /// <param name="backendStorageZipFilename">The name of the .ZIP file used for back-end storage.</param>
        /// <param name="extractionRootDirectory">The directory to use as the default root path for extracting files.</param>
        /// <param name="saveOriginalFilenames">Determines whether the original filenames should be persisted.</param>
        public FileStore(string backendStorageZipFilename,
                         string extractionRootDirectory,
                         bool saveOriginalFilenames)
            : base(extractionRootDirectory, saveOriginalFilenames, _EngineName_, _EngineVersion_)
        {
            // ######## CAUTION ########
            // Be sure to duplicate the CHECK CODE and the INITIALIZESTORE() method call in both ctors, because were calling different base ctors.
            // #########################
            if (string.IsNullOrWhiteSpace(backendStorageZipFilename))
            {
                throw new ArgumentNullException("backendStorageZipFilename");
            }
            BackendStorageZipFilename = backendStorageZipFilename;
            InitializeStore();
        }
        /// <summary>
        /// Initializes an new instance of the dotNETZip FileStore with the specified arguments.
        /// </summary>
        /// <param name="backendStorageZipFilename">The name of the .ZIP file used for back-end storage.</param>
        /// <param name="extractionRootDirectory">The directory to use as the default root path for extracting files.</param>
        /// <param name="saveOriginalFilenames">Determines whether the original filenames should be persisted.</param>
        /// <param name="extensionsToStore"></param>
        public FileStore(string backendStorageZipFilename,
                         string extractionRootDirectory,
                         bool saveOriginalFilenames,
                         IEnumerable<string> extensionsToStore)
            : base(extractionRootDirectory, saveOriginalFilenames, _EngineName_, _EngineVersion_, extensionsToStore)
        {
            // ######## CAUTION ########
            // Be sure to duplicate the CHECK CODE and the INITIALIZESTORE() method call in both ctors, because were calling different base ctors.
            // #########################
            if (string.IsNullOrWhiteSpace(backendStorageZipFilename))
            {
                throw new ArgumentNullException("backendStorageZipFilename");
            }
            BackendStorageZipFilename = backendStorageZipFilename;
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
            // BackendStorageZipFilename
            var temp1 = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "BackendStorageZipFilename", typeof(string)).Value; //    
            if (string.IsNullOrWhiteSpace(temp1))
            {
                throw new Exception($"Configuration invalid information. Configuration item: \"BackendStorageZipFilename\" cannot be empty.");
            }
            BackendStorageZipFilename = temp1;
            // ExtractionRootDirectory
            var temp2 = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ExtractionRootDirectory", typeof(string)).Value; //    
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
            // Engine Name/Version
            CompressionEngineName = _EngineName_;
            CompressionEngineVersion = _EngineVersion_;
            //

            // TODO: add CompressedFileStoreBase stuff to this list.
            //         IList<string> ExtensionsToStore

            InitializeStore();
        }
        #endregion
        #region Private Fields
        private bool _BackendChanged = false;
        //
        private string _Extract_DestRootPath = "";
        #endregion
        #region Public Methods
        /// <summary>
        /// The name of the .zip file used for back-end storage.
        /// </summary>
        public string BackendStorageZipFilename { get; } = "";
        #endregion
        #region dodSON.Core.FileStorage.CompressedFileStoreBase Overrides
        // NEW ITEM
        /// <summary>
        /// Creates a file store item, specifically for the dotNETZip FileStore, based on the given meta-data.
        /// </summary>
        /// <param name="rootFilename">The name of the <see cref="IFileStoreItem"/> relative to the file store.</param>
        /// <param name="rootFileLastModifiedTimeUtc">A <see cref="DateTime"/> representing the last modified time of the <see cref="IFileStoreItem"/> to be created, in Universal Coordinated Time.</param>
        /// <param name="originalFilename">The full pathname of the file to add.</param>
        /// <param name="sizeInBytes">The size, in bytes, of the file.</param>
        /// <returns>An <see cref="IFileStoreItem"/> created using the original filename and given meta-data.</returns>
        public override IFileStoreItem CreateNewFileStoreItem(string rootFilename, string originalFilename, DateTime rootFileLastModifiedTimeUtc, long sizeInBytes)
        {
            return new dodSON.Core.FileStorage.CompressedFileStoreItem(this,
                                                                       rootFilename,
                                                                       originalFilename,
                                                                       rootFileLastModifiedTimeUtc,
                                                                       sizeInBytes,
                                                                       GetCompressionStrategy(System.IO.Path.GetExtension(rootFilename)));
        }
        // ROLLBACK
        /// <summary>
        /// Returns <b>true</b>, which indicates that this file store supports <see cref="Rollback"/> functionality.
        /// </summary>
        public override bool SupportRollback => true;
        /// <summary>
        /// This function will restore the file store to it's original state; abandoning all changes.
        /// </summary>
        public override void Rollback() => InitializeStore();
        /// <summary>
        /// Reloads the file store.
        /// </summary>
        protected override void Refresh_Refresh() => InitializeStore();
        // EXTRACT
        /// <summary>
        /// Will initialize the .zip file to prepare to extract files.
        /// </summary>
        /// <param name="destRootPath">The root directory where extraction will be performed.</param>
        /// <param name="state">A System.IO.Compression.ZipArchive reference initialized to the specified <see cref="BackendStorageZipFilename"/>. This reference will be passed to all Extract_ methods.</param>
        protected override void Extract_StartUp(string destRootPath, out object state)
        {
            _Extract_DestRootPath = destRootPath;
            state = CreateZipper(System.IO.Compression.ZipArchiveMode.Read);
        }
        /// <summary>
        /// Will extract the <paramref name="item"/> from the .zip file to the destination root path given in the method <see cref="Extract_StartUp(string, out object)"/>.
        /// </summary>
        /// <param name="item">The store item to extract.</param>
        /// <param name="state">A reference to an initialized System.IO.Compression.ZipArchive object. This reference, created in the <see cref="Extract_StartUp(string, out object)"/> will be passed to all Extract_ methods.</param>
        /// <returns>The filename of the extracted file.</returns>
        protected override string Extract_ExtractFile(dodSON.Core.FileStorage.IFileStoreItem item, object state)
        {
            // find zip entry
            var found = ((System.IO.Compression.ZipArchive)state).Entries.FirstOrDefault((zf) => { return CleanFilenameString(zf.FullName).Equals(item.RootFilename, StringComparison.InvariantCultureIgnoreCase); });
            // extract file
            if (found != null)
            {
                return ZipperExtractFile(_Extract_DestRootPath, found);
            }
            return "";
        }
        /// <summary>
        /// When overridden, should extract the <paramref name="item"/> from the underlying storage system to the <paramref name="destRootPath"/>.
        /// </summary>
        /// <param name="destRootPath">The directory to export the files to.</param>
        /// <param name="item">The store item to extract.</param>
        /// <param name="state">An object, defined by the user, that will be passed to all Extract_ methods.</param>
        /// <returns>The filename of the extract file.</returns>
        protected override string Extract_ExtractFileToPath(string destRootPath, IFileStoreItem item, object state)
        {
            // find zip entry
            var found = ((System.IO.Compression.ZipArchive)state).Entries.FirstOrDefault((zf) => { return CleanFilenameString(zf.FullName).Equals(item.RootFilename, StringComparison.InvariantCultureIgnoreCase); });
            // extract file
            if (found != null)
            {
                return ZipperExtractFileToPath(destRootPath, found);
            }
            return "";
        }
        /// <summary>
        /// Will finalize the extraction of files from the .zip file. 
        /// </summary>
        /// <param name="state">A reference to an initialized System.IO.Compression.ZipArchive object. This reference, created in the <see cref="Extract_StartUp(string, out object)"/> will be destroyed and set to null in this method.</param>
        protected override void Extract_Shutdown(ref object state) => DisposeOfZipper(ref state);
        // SAVE
        /// <summary>
        /// Will initialize the .zip file to prepare to add, update and remove files.
        /// </summary>
        /// <param name="state">A System.IO.Compression.ZipArchive reference initialized to the specified <see cref="BackendStorageZipFilename"/>. This reference will be passed to all Save_ methods.</param>
        protected override void Save_Startup(out object state)
        {
            _BackendChanged = false;
            state = CreateZipper(System.IO.Compression.ZipArchiveMode.Update);
        }
        /// <summary>
        /// Will add the <paramref name="item"/> to the .zip file.
        /// </summary>
        /// <param name="item">The store item to add.</param>
        /// <param name="state">A reference to an initialized System.IO.Compression.ZipArchive object. This reference, created in the <see cref="Save_Startup(out object)"/> will be passed to all Save_ methods.</param>
        protected override void Save_AddFileToSource(IFileStoreItem item, object state)
        {
            _BackendChanged = true;
            ZipperAddFile(((System.IO.Compression.ZipArchive)state), (dodSON.Core.FileStorage.ICompressedFileStoreItem)item);
        }
        /// <summary>
        /// Will update an existing file in the .zip file with the <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The store item to update from.</param>
        /// <param name="state">A reference to an initialized System.IO.Compression.ZipArchive object. This reference, created in the <see cref="Save_Startup(out object)"/> will be passed to all Save_ methods.</param>
        protected override void Save_UpdateFileInSource(IFileStoreItem item, object state)
        {
            _BackendChanged = true;
            ZipperUpdateFile(((System.IO.Compression.ZipArchive)state), (dodSON.Core.FileStorage.ICompressedFileStoreItem)item);
        }
        /// <summary>
        /// Will remove an existing file, referenced by the <paramref name="item"/>, from the .zip file. 
        /// </summary>
        /// <param name="item">The store item to remove.</param>
        /// <param name="state">A reference to an initialized System.IO.Compression.ZipArchive object. This reference, created in the <see cref="Save_Startup(out object)"/> will be passed to all Save_ methods.</param>
        protected override void Save_RemoveFileFromSource(IFileStoreItem item, object state)
        {
            _BackendChanged = true;
            ZipperRemoveFile(((System.IO.Compression.ZipArchive)state), item);
        }
        /// <summary>
        /// Will persist the list of original filenames to the .zip file.
        /// </summary>
        /// <param name="state">A reference to an initialized System.IO.Compression.ZipArchive object. This reference, created in the <see cref="Save_Startup(out object)"/> will be passed to all Save_ methods.</param>
        protected override void Save_SaveOriginalFilenames(object state)
        {
            _BackendChanged = true;
            var zipper = (System.IO.Compression.ZipArchive)state;
            var entry = (zipper.Entries.FirstOrDefault((e) => { return (e.FullName.Equals(_OriginalFilenames_Filename, StringComparison.InvariantCultureIgnoreCase)); }));
            // delete current from zip
            if (entry != null)
            {
                entry.Delete();
            }
            // save new to temp file
            var tempFilename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            System.IO.File.WriteAllText(tempFilename, OriginalFilenamesToString());
            // upload temp file into zip file
            zipper.CreateEntryFromFile(tempFilename, _OriginalFilenames_Filename, System.IO.Compression.CompressionLevel.Optimal);
            // delete temp file
            System.IO.File.Delete(tempFilename);
        }
        /// <summary>
        /// Will finalize the adding, updating and removing of files from the .zip file.
        /// </summary>
        /// <param name="state">A reference to an initialized System.IO.Compression.ZipArchive object. This reference, created in the <see cref="Save_Startup(out object)"/> will be destroyed and set to null in this method.</param>
        protected override void Save_Shutdown(ref object state)
        {
            try
            {
                // save state (zipper)
                if (state != null)
                {
                    if (this.Count == 0)
                    {
                        // #### no files left; delete back end storage
                        // dispose of zipper --> closes and commits the zip file
                        DisposeOfZipper(ref state);
                        FileStorageHelper.DeleteFile(BackendStorageZipFilename);
                    }
                    else
                    {
                        if (_BackendChanged)
                        {
                            // #### back end has changed; save zip and update compression information
                            // dispose of zipper --> closes and commits the zip file
                            DisposeOfZipper(ref state);
                            // create a NEW zipper
                            // use a stand-in variable for the lambda expression
                            System.IO.Compression.ZipArchive zipState = CreateZipper(System.IO.Compression.ZipArchiveMode.Read);
                            // set the 'ref' object to the NEW zipper
                            state = zipState;
                            // **** update all compression information
                            ForEach(item =>
                            {
                                // reinitialize all compressed/uncompressed values
                                var found = zipState.Entries.FirstOrDefault((zf) => { return CleanFilenameString(zf.FullName).Equals(item.RootFilename, StringComparison.InvariantCultureIgnoreCase); });
                                if (found != null)
                                {
                                    ((dodSON.Core.FileStorage.ICompressedFileStoreItemAdvanced)item).SetCompressionValues(found.CompressedLength);
                                    ((dodSON.Core.FileStorage.ICompressedFileStoreItemAdvanced)item).SetCompressionStrategy(NormalizeCompressionStrategy(found));
                                }
                                else
                                {
                                    ((dodSON.Core.FileStorage.ICompressedFileStoreItemAdvanced)item).SetCompressionValues(0);
                                    ((dodSON.Core.FileStorage.ICompressedFileStoreItemAdvanced)item).SetCompressionStrategy(CompressionStorageStrategy.Store);
                                }
                            });
                            // **** clear the lambda stand-in variable
                            zipState = null;
                            _BackendChanged = false;
                        }
                    }
                }
            }
            finally
            {
                // #### fail-safe
                DisposeOfZipper(ref state);
            }
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
                result.Items.Add("BackendStorageZipFilename", BackendStorageZipFilename, BackendStorageZipFilename.GetType());
                result.Items.Add("ExtractionRootDirectory", ExtractionRootDirectory, ExtractionRootDirectory.GetType());
                result.Items.Add("SaveOriginalFilenames", SaveOriginalFilenames, SaveOriginalFilenames.GetType());

                // TODO: add CompressedFileStoreBase stuff to this list.
                //         IList<string> ExtensionsToStore

                return result;
            }
        }
        #endregion
        #region Private Methods
        private void InitializeStore()
        {
            // clean self
            ((dodSON.Core.FileStorage.IFileStoreAdvanced)this).ResetLists();
            // only initialize if the back end store exists.
            if (System.IO.File.Exists(BackendStorageZipFilename))
            {
                // iterate all files, in all subdirectories, in the zip file
                using (var zipper = CreateZipper(System.IO.Compression.ZipArchiveMode.Read))
                {
                    // process all files
                    foreach (var zipItem in zipper.Entries)
                    {
                        var filename = CleanFilenameString(zipItem.FullName);
                        // do not process non-files
                        if (!filename.EndsWith(@"\"))
                        {
                            // do not process the (ORIGINAL FILENAMES) file
                            if (!filename.Equals(_OriginalFilenames_Filename, StringComparison.InvariantCultureIgnoreCase))
                            {
                                // create new store item/add to self
                                var newItem = this.Add(filename, "", zipItem.LastWriteTime.UtcDateTime, zipItem.Length);
                                // clear all state flags
                                ((dodSON.Core.FileStorage.IFileStoreItemAdvanced)newItem).StateChangeTracker.ClearAll();
                                // set compressed size and uncompressed size
                                ((dodSON.Core.FileStorage.ICompressedFileStoreItemAdvanced)newItem).SetCompressionValues(zipItem.CompressedLength);
                            }
                            else
                            {
                                // load original filenames file.
                                // extract to temp file
                                var tempFilename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
                                try
                                {
                                    zipItem.ExtractToFile(tempFilename, true);
                                    // read temp file
                                    using (var sr = new System.IO.StreamReader(tempFilename))
                                    {
                                        // read file and convert
                                        var originalFilenames = StringToOriginalFilenames(sr.ReadToEnd());
                                        // set all original filenames
                                        ((dodSON.Core.FileStorage.IFileStoreAdvanced)this).SetOriginalFilenames(originalFilenames);
                                    }
                                }
                                catch { }
                                finally
                                {
                                    // delete temp file
                                    FileStorageHelper.DeleteFile(tempFilename);
                                }
                            }
                        }
                    }
                }
            }
        }
        //
        private string CleanFilenameString(string filename)
        {
            filename = filename.Replace('/', '\\');
            while (filename.StartsWith("\\"))
            {
                filename = filename.Substring(1);
            }
            return filename;
        }
        //
        // deserialize string into type
        private IDictionary<string, string> StringToOriginalFilenames(string source) => (new dodSON.Core.Converters.TypeSerializer<IDictionary<string, string>>()).FromString(source);
        // serialize type into string
        private string OriginalFilenamesToString() => (new dodSON.Core.Converters.TypeSerializer<IDictionary<string, string>>()).ToString(((dodSON.Core.FileStorage.IFileStoreAdvanced)this).GetOriginalFilenames());
        private Core.FileStorage.CompressionStorageStrategy NormalizeCompressionStrategy(System.IO.Compression.ZipArchiveEntry entry) => GetCompressionStrategy(System.IO.Path.GetExtension(entry.FullName));
        //
        private System.IO.Compression.ZipArchive CreateZipper(System.IO.Compression.ZipArchiveMode mode)
        {
            try
            {
                // create a blank zip file, if no zip file exists
                if (!System.IO.File.Exists(BackendStorageZipFilename))
                {
                    System.IO.Compression.ZipFile.Open(BackendStorageZipFilename, System.IO.Compression.ZipArchiveMode.Update).Dispose();
                }
                return System.IO.Compression.ZipFile.Open(BackendStorageZipFilename, mode);
            }
            catch
            {
                throw;
            }
        }
        private void DisposeOfZipper(ref object zipper)
        {
            if (zipper != null)
            {
                ((IDisposable)zipper).Dispose();
                zipper = null;
            }
        }
        //
        private string ZipperExtractFile(string destRootPath, System.IO.Compression.ZipArchiveEntry item)
        {
            var destFilename = CleanFilenameString(System.IO.Path.Combine(destRootPath, item.FullName));
            FileStorageHelper.DeleteFile(destFilename);
            FileStorageHelper.CreateDirectory(System.IO.Path.GetDirectoryName(destFilename));
            //
            var count = 5;
            do
            {
                try
                {
                    item.ExtractToFile(destFilename, true);
                    if (System.IO.File.Exists(destFilename))
                    {
                        var fileInfo = new System.IO.FileInfo(destFilename);
                        fileInfo.LastWriteTime = DateTime.Now;
                    }
                    break;
                }
                catch
                {
                    if (--count <= 0)
                    {
                        throw;
                    }
                    Threading.ThreadingHelper.Sleep(200);
                }
            } while (true);
            //
            return destFilename;
        }
        private string ZipperExtractFileToPath(string destRootPath, System.IO.Compression.ZipArchiveEntry item)
        {
            var destFilename = CleanFilenameString(System.IO.Path.Combine(destRootPath, item.Name));
            FileStorageHelper.DeleteFile(destFilename);
            FileStorageHelper.CreateDirectory(System.IO.Path.GetDirectoryName(destFilename));
            //
            var count = 5;
            do
            {
                try
                {
                    item.ExtractToFile(destFilename, true);
                    if (System.IO.File.Exists(destFilename))
                    {
                        var fileInfo = new System.IO.FileInfo(destFilename);
                        fileInfo.LastWriteTime = DateTime.Now;
                    }
                    break;
                }
                catch
                {
                    if (--count <= 0)
                    {
                        throw;
                    }
                    Threading.ThreadingHelper.Sleep(200);
                }
            } while (true);
            //
            return destFilename;
        }
        private void ZipperAddFile(System.IO.Compression.ZipArchive zipper, dodSON.Core.FileStorage.ICompressedFileStoreItem item)
        {
            // create new zip item
            zipper.CreateEntryFromFile(item.OriginalFilename, item.RootFilename, (item.CompressionStrategy == CompressionStorageStrategy.Compress) ?
                                                                                    CompressionLevel.Optimal :
                                                                                    CompressionLevel.NoCompression);

        }
        private void ZipperUpdateFile(System.IO.Compression.ZipArchive zipper, dodSON.Core.FileStorage.ICompressedFileStoreItem item)
        {
            // find existing zip item
            var entry = zipper.Entries.FirstOrDefault((e) => { return e.FullName.Equals(item.RootFilename, StringComparison.InvariantCultureIgnoreCase); });
            if (entry != null)
            {
                // delete entry
                entry.Delete();
                // add (new) entry
                ZipperAddFile(zipper, item);
            }
        }
        private void ZipperRemoveFile(System.IO.Compression.ZipArchive zipper, dodSON.Core.FileStorage.IFileStoreItem item)
        {
            // find existing zip item
            var entry = zipper.Entries.FirstOrDefault((e) => { return e.FullName.Equals(item.RootFilename, StringComparison.InvariantCultureIgnoreCase); });
            if (entry != null)
            {
                entry.Delete();
            }
        }
        #endregion
    }
}

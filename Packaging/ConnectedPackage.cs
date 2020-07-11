using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Represents a single, connected package.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The package, which should be some form a single file, file container, like a zip file, has been extracted from the package file store, to a temporary location, and automatically connected to.
    /// In order to save and return this temporarily extracted zip file back to the package store it was extracted from, the <see cref="Close"/> method must be called.
    /// <br/><br/>
    /// This type implements the <see cref="IDisposable"/> interface which ensures that the <see cref="Close"/> method is called. 
    /// Best practice would be to use a <b>using(...)</b> statement; otherwise, you must call <see cref="Close"/> explicitly.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>The following code example will create a <see cref="PackageProvider"/> use it to create a new <see cref="Package"/>, which it gets in the form of a <see cref="ConnectedPackage"/>, and adds files to the package file store.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // **** Create Package Provider
    ///     // the package files source directory
    ///     var packagesSourceDirectory = @"C:\(WORKING)\Dev\Packaging\Packs";    // #### CHANGE THIS TO A LOCAL PATH ####
    ///
    ///     // create a file store to the packages source directory
    ///     var packagesFileStore = new dodSON.Core.FileStorage.MSdotNETFileSystem.FileStore(packagesSourceDirectory, System.IO.Path.GetTempPath(), false);
    ///
    ///     // define the filenames of the configuration files in the packages
    ///     var configurationFilename = "package.config.xml";
    ///
    ///     // define the serializer used to read and write configuration files
    ///     var serializer = new dodSON.Core.Configuration.XmlConfigurationSerializer(new dodSON.Core.Compression.DeflateStreamCompressor());
    ///
    ///     // create a package file store provider
    ///     //     This type provides the package store used to create and connect to the packages inside the package store
    ///     var packageFileStoreProvider = new dodSON.Core.Packaging.MSdotNETZip.PackageFileStoreProvider();
    ///
    ///     // create a package provider
    ///     Console.WriteLine($"Creating Package Provider.");
    ///     var packageProvider = new dodSON.Core.Packaging.PackageProvider(packagesFileStore, configurationFilename, serializer, packageFileStoreProvider);
    ///
    ///     // **** Create Package
    ///     // create package configuration
    ///     var packageName = "TestPackage";
    ///     var version = new Version(0, 1, 0, 0);
    ///     var isEnabled = true;
    ///     var isDependencyPackage = false;
    ///     var config = new dodSON.Core.Packaging.PackageConfiguration(packageName, version, isEnabled, isDependencyPackage);
    ///
    ///     // create package and add files
    ///     var exampleFilesSourceDirectory = @"C:\(WORKING)\Dev\Packaging\SourcePacks\ExamplePackageFiles";    // #### CHANGE THIS TO A LOCAL PATH, WITH EXAMPLE FILES ####
    ///     var overwrite = true;
    ///     var packageRootFilename = $"{packageName}.package.zip";
    ///     // create and open package
    ///     Console.WriteLine();
    ///     Console.WriteLine($"Creating {packageName}");
    ///     using (var connectedPackage = packageProvider.Create(packageRootFilename, config, overwrite))
    ///     {
    ///         Console.WriteLine($"Adding Files:");
    ///         // add files to package file store
    ///         foreach (var fileName in System.IO.Directory.GetFiles(exampleFilesSourceDirectory, "*", System.IO.SearchOption.TopDirectoryOnly))
    ///         {
    ///             var rootFilename = System.IO.Path.GetFileName(fileName);
    ///             Console.WriteLine($"    {rootFilename}");
    ///             var fileInfo = new System.IO.FileInfo(fileName);
    ///             connectedPackage.FileStore.Add(rootFilename, fileInfo.LastWriteTimeUtc, fileName);
    ///         }
    ///     }
    ///     // leaving the USING statement closes the connected package, saves the file store and cleans up any residual files
    ///
    ///     // **** Connect to 'Package A', display files inside
    ///     Console.WriteLine();
    ///     Console.WriteLine($"Connecting to {packageName}");
    ///     using (var connectedPackage = packageProvider.Connect(packageRootFilename))
    ///     {
    ///         Console.WriteLine($"Reading Files:");
    ///         foreach (var fileStoreItem in connectedPackage.FileStore.GetAll())
    ///         {
    ///             Console.WriteLine($"    {fileStoreItem.RootFilename}");
    ///         }
    ///         // **** display configuration as INI and XML
    ///         Console.WriteLine();
    ///         Console.WriteLine($"Reading Configuration:");
    ///         // create INI serialization
    ///         var iniSerializer = new dodSON.Core.Configuration.IniConfigurationSerializer(new dodSON.Core.Compression.DeflateStreamCompressor());
    ///         var displayStr1 = iniSerializer.SerializeGroup(connectedPackage.PackageConfiguration.Configuration).ToString();
    ///         Console.WriteLine("----------------------------------------");
    ///         Console.WriteLine("INI:");
    ///         Console.WriteLine($"{displayStr1}");
    ///         // create XML serialization
    ///         var displayStr2 = serializer.SerializeGroup(connectedPackage.PackageConfiguration.Configuration).ToString();
    ///         Console.WriteLine("----------------------------------------");
    ///         Console.WriteLine("XML:");
    ///         Console.WriteLine($"{displayStr2}");
    ///     }
    ///     // leaving the USING statement closes the connected package, saves the file store and cleans up any residual files
    ///
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    ///
    ///
    /// // This code produces output similar to the following:
    ///
    /// // Creating Package Provider.
    /// // 
    /// // Creating TestPackage
    /// // Adding Files:
    /// //     Alpha-1.txt
    /// //     Alpha-2.txt
    /// //     Alpha-3.txt
    /// //     Beta-1.txt
    /// //     Beta-2.txt
    /// //     Beta-3.txt
    /// //     Gamma-1.txt
    /// //     Gamma-2.txt
    /// //     Gamma-3.txt
    /// // 
    /// // Connecting to TestPackage
    /// // Reading Files:
    /// //     package.config.xml
    /// //     Alpha-1.txt
    /// //     Alpha-2.txt
    /// //     Alpha-3.txt
    /// //     Beta-1.txt
    /// //     Beta-2.txt
    /// //     Beta-3.txt
    /// //     Gamma-1.txt
    /// //     Gamma-2.txt
    /// //     Gamma-3.txt
    /// // 
    /// // Reading Configuration:
    /// // ----------------------------------------
    /// // INI:
    /// // [PackageConfiguration]
    /// //         IsDependencyPackage/System.Boolean/False
    /// //         IsEnabled/System.Boolean/True
    /// //         Name/System.String/TestPackage
    /// //         Version/System.Version/0.1.0.0
    /// // ----------------------------------------
    /// // XML:
    /// // &lt;?xml version = "1.0" encoding="utf-8"?&gt;
    /// // &lt;group key = "PackageConfiguration"&gt;
    /// //   &lt;items&gt;
    /// //     &lt;item key="IsDependencyPackage" type="System.Boolean"&gt;False&lt;/item&gt;
    /// //     &lt;item key = "IsEnabled" type="System.Boolean"&gt;True&lt;/item&gt;
    /// //     &lt;item key = "Name" type="System.String"&gt;TestPackage&lt;/item&gt;
    /// //     &lt;item key = "Version" type="System.Version"&gt;0.1.0.0&lt;/item&gt;
    /// //   &lt;/items&gt;
    /// //   &lt;groups&gt;
    /// //     &lt;group key = "DependencyPackages"/&gt;
    /// //   &lt;/groups 
    /// // &lt;/group&gt;
    /// // 
    /// // press anykey...
    /// </code>
    /// </example>
    [Serializable()]
    public class ConnectedPackage
        : IConnectedPackage
    {
        #region Ctor
        private ConnectedPackage() { }
        /// <summary>
        /// Instantiates a new connected package.
        /// </summary>
        /// <param name="packagesStore">The file store containing the package.</param>
        /// <param name="packageRootFilename">The package file name in the file store.</param>
        /// <param name="originalFilename">The file name for the package file store.</param>
        /// <param name="packFileStore">A file store connected to the package file store.</param>
        /// <param name="packConfiguration">The package configuration read from this package.</param>
        internal ConnectedPackage(FileStorage.IFileStore packagesStore,
                                  string packageRootFilename,
                                  string originalFilename,
                                  FileStorage.IFileStore packFileStore,
                                  IPackageConfiguration packConfiguration)
            : this()
        {
            _PackagesStore = packagesStore ?? throw new ArgumentNullException(nameof(packagesStore));
            if (string.IsNullOrWhiteSpace(packageRootFilename)) { throw new ArgumentNullException(nameof(packageRootFilename)); }
            _PackageRootFilename = packageRootFilename;
            if (string.IsNullOrWhiteSpace(originalFilename)) { throw new ArgumentNullException(nameof(originalFilename)); }
            _OriginalFilename = originalFilename;
            _FileStore = packFileStore ?? throw new ArgumentNullException(nameof(packFileStore));
            PackageConfiguration = packConfiguration ?? throw new ArgumentNullException(nameof(packConfiguration));
        }
        #endregion
        #region Private Fields
        private readonly FileStorage.IFileStore _PackagesStore;
        private readonly string _PackageRootFilename;
        private readonly string _OriginalFilename;
        private readonly FileStorage.IFileStore _FileStore;
        #endregion
        #region Public Methods
        /// <summary>
        /// The <see cref="FileStorage.IFileStore"/> containing all the files for this package.
        /// </summary>
        /// <remarks>
        /// <para>The package, which should be some form of zip file, has been extracted from the package file store and automatically connected to; this property is that <see cref="FileStorage.ICompressedFileStore"/>. Which means it must be disconnected and deleted when finished.
        /// Therefore, it is imperative that the <see cref="Close"/> method is called so it can save the package file store and disconnect from it; allowing the <see cref="PackageProvider"/> to delete the extracted zip file, and any other residual files.</para>
        /// <para>This type implements <see cref="IDisposable"/>, best practice would be to use a <b>using(...)</b> statement. Otherwise, you must call <see cref="Close"/>.</para>
        /// </remarks>
        public FileStorage.IFileStore FileStore
        {
            get
            {
                if (!IsOpen) { throw new Exception("Package is not open."); }
                return _FileStore;
            }
        }
        /// <summary>
        /// The package configuration representing this package.
        /// </summary>
        public IPackageConfiguration PackageConfiguration { get; }
        /// <summary>
        /// <para>Indicates whether this package is opened or closed.</para>
        /// <para><b>True</b> indicates the package is open, meaning the file store is connected to a zip file extracted from the package store; otherwise <b>false</b>, the package is closed and the file store has been saved and the extracted zip file deleted.</para>
        /// </summary>
        /// <remarks>    
        /// <para>The package, which should be some form of zip file, has been extracted from the package file store and automatically connected to. Which means it must be disconnected and deleted when finished.
        /// Therefore, it is imperative that the <see cref="Close"/> method is called so it can save the package file store and disconnect from it; allowing the <see cref="PackageProvider"/> to delete the extracted zip file, and any other residual files.</para>
        /// <para>This type implements <see cref="IDisposable"/>, best practice would be to use a <b>using(...)</b> statement. Otherwise, you must call <see cref="Close"/>.</para>
        /// </remarks>
        public bool IsOpen { get; private set; } = true;
        /// <summary>
        /// Will save the package file store, clean up residual files and close the package.
        /// </summary>
        /// <remarks>
        /// <para>The package, which should be some form of zip file, has been extracted from the package file store and automatically connected to. Which means it must be disconnected and deleted when finished.
        /// Therefore, it is imperative that the <see cref="Close"/> method is called so it can save the package file store and disconnect from it; allowing the <see cref="PackageProvider"/> to delete the extracted zip file, and any other residual files.</para>
        /// <para>This type implements <see cref="IDisposable"/>, best practice would be to use a <b>using(...)</b> statement. Otherwise, you must call <see cref="Close"/>.</para>
        /// </remarks>
        public void Close()
        {
            if (IsOpen)
            {
                try
                {
                    // save package zip file store before it gets added to the packages file store
                    _FileStore.Save(false);
                    // add package zip file to packages store
                    _PackagesStore.Add(_PackageRootFilename, _OriginalFilename, DateTime.Now, _FileStore.SizeInBytes);
                    // save packages store
                    _PackagesStore.Save(false);
                }
                finally
                {
                    // delete the original FileStore file
                    try { if (System.IO.File.Exists(_OriginalFilename)) { System.IO.File.Delete(_OriginalFilename); } }
                    catch { }
                    // set flag to closed
                    IsOpen = false;
                }
            }
        }
        #endregion
        #region IDisposable Methods
        /// <summary>
        /// Provides a mechanism for releasing resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Ensures that the package file store is closed and residual files are removed.
        /// </summary>
        /// <remarks>
        /// <para>The package, which should be some form of zip file, has been extracted from the package file store and automatically connected to. Which means it must be disconnected and deleted when finished.
        /// Therefore, it is imperative that the <see cref="Close"/> method is called so it can save the package file store and disconnect from it; allowing the <see cref="PackageProvider"/> to delete the extracted zip file, and any other residual files.</para>
        /// <para>This type implements <see cref="IDisposable"/>, best practice would be to use a <b>using(...)</b> statement. Otherwise, you must call <see cref="Close"/>.</para>
        /// </remarks>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) { Close(); }
        }
        #endregion
    }
}

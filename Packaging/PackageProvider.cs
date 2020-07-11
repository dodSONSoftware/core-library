using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Provides methods to create and connect to packages, iterate packages, find packages and read and write package configuration files.
    /// </summary>
    /// <example>
    /// <para><b>[Step #1: Create Packages A and B]</b></para>
    /// <br/>
    /// <note type="note">
    /// <para>
    /// This example is the first in a series of examples designed to work together to demonstrate creating, installing and patching packages.
    /// </para>
    /// <br/>
    /// <para>
    /// The scenario follows that package A and package B, along with their dependency packages, have been installed and are working fine. 
    /// Now, along comes package C with a new version of package Y, version 2.0. 
    /// After installing it, using <see cref="Installation.InstallType.HighestVersionOnly"/>, package B fails. 
    /// Looking at package B's <see cref="Installation.IInstaller.DependencyChain(Installation.IInstalledPackage)"/>, you see that it now references package Y 2.0; the wrong package.
    /// You can patch package B to specifically use package Y 1.0, if package Y 1.0 is available, by setting the <see cref="Packaging.IDependencyPackage.SpecificVersion"/> of the Y dependency in package B to version 1.0.
    /// This will tell the packaging and installation systems to try to use the <see cref="Packaging.IDependencyPackage.SpecificVersion"/> is possible. (see <see cref="Installation.InstallType"/> and <see cref="Installation.IInstaller.DependencyChain(Installation.IInstalledPackage)"/> for more information.)
    /// This demonstrates the differences between <see cref="Installation.InstallType.HighestVersionOnly"/> and <see cref="Installation.InstallType.SideBySide"/>.
    /// </para>
    /// <br/>
    /// <para>
    /// Run these examples in the following order:
    /// <list type="bullet">
    /// <item>[Step #1: Create Packages A and B] in <see cref="Packaging.PackageProvider"/></item>
    /// <item>[Step #2: Install Packages A and B] in <see cref="Installation.Installer"/></item>
    /// <item>[Step #3: Create Package C] in <see cref="Packaging.PackageProvider"/></item>
    /// <item>[Step #4: Install Package C] in <see cref="Installation.Installer"/></item>
    /// </list>
    /// </para>
    /// </note>
    /// <br/>
    /// <note type="note">
    /// Before running this example please remove all files from the test package directory; defined by the variable: <b>sourceDirectory</b>.
    /// </note>
    /// <br/>
    /// <para>The following code example will create a <see cref="PackageProvider"/> and use it to create a set of <see cref="Package"/>s and a shared set of dependency <see cref="Package"/>s.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // the path to create the package files       #### CHANGE THIS TO A LOCAL PATH ####
    ///     var sourceDirectory = @"C:\(WORKING)\Dev\Packaging\Packs";
    ///     if (System.IO.Directory.Exists(sourceDirectory)) { System.IO.Directory.Delete(sourceDirectory, true); dodSON.Core.Threading.ThreadingHelper.Sleep(50); }
    ///     System.IO.Directory.CreateDirectory(sourceDirectory);
    ///     
    ///     // create a file store to the packages source directory
    ///     var packagesFileStore = new dodSON.Core.FileStorage.MSdotNETFileSystem.FileStore(sourceDirectory, System.IO.Path.GetTempPath(), false);
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
    ///     // create non-dependency package A
    ///     Console.WriteLine($"Creating Package-A, v1.0.0.0");
    ///     CreatePackage(packageProvider,
    ///                   "Package-A",
    ///                   new Version(1, 0, 0, 0),
    ///                   true,
    ///                   false,
    ///                   new dodSON.Core.Packaging.IDependencyPackage[]
    ///                   {
    ///               new dodSON.Core.Packaging.DependencyPackage("Package-X", new Version(1, 0, 0, 0)),
    ///               new dodSON.Core.Packaging.DependencyPackage("Package-Y", new Version(1, 0, 0, 0))
    ///                   });
    ///     // create non-dependency package B
    ///     Console.WriteLine($"Creating Package-B, v1.0.0.0");
    ///     CreatePackage(packageProvider,
    ///                   "Package-B",
    ///                   new Version(1, 0, 0, 0),
    ///                   true,
    ///                   false,
    ///                   new dodSON.Core.Packaging.IDependencyPackage[]
    ///                   {
    ///               new dodSON.Core.Packaging.DependencyPackage("Package-Y", new Version(1, 0, 0, 0)),
    ///               new dodSON.Core.Packaging.DependencyPackage("Package-Z", new Version(1, 0, 0, 0))
    ///                   });
    ///     // create dependency package X
    ///     Console.WriteLine($"Creating Package-X, v1.0.0.0");
    ///     CreatePackage(packageProvider,
    ///                   "Package-X",
    ///                   new Version(1, 0, 0, 0),
    ///                   true,
    ///                   true,
    ///                   null);
    ///     // create dependency package Y
    ///     Console.WriteLine($"Creating Package-Y, v1.0.0.0");
    ///     CreatePackage(packageProvider,
    ///                   "Package-Y",
    ///                   new Version(1, 0, 0, 0),
    ///                   true,
    ///                   true,
    ///                   null);
    ///     // create dependency package Z
    ///     Console.WriteLine($"Creating Package-Z, v1.0.0.0");
    ///     CreatePackage(packageProvider,
    ///                   "Package-Z",
    ///                   new Version(1, 0, 0, 0),
    ///                   true,
    ///                   true,
    ///                   null);
    ///     
    ///     // iterate through all packages in the package provider
    ///     Console.WriteLine();
    ///     Console.WriteLine("All Packages:");
    ///     foreach (var item in packageProvider.Packages)
    ///     {
    ///         Console.WriteLine($"Found {item.FullyQualifiedPackageName}");
    ///     }
    ///     
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// private static void CreatePackage(dodSON.Core.Packaging.IPackageProvider packProvider,
    ///                                   string packageName,
    ///                                   Version version,
    ///                                   bool isEnabled,
    ///                                   bool isDependencyPackage,
    ///                                   IList&lt;dodSON.Core.Packaging.IDependencyPackage&gt; dependencyPackages)
    /// {
    ///     // create package configuration
    ///     var config = new dodSON.Core.Packaging.PackageConfiguration(packageName, version, isEnabled, isDependencyPackage, dependencyPackages);
    ///     // create package and close it.
    ///     packProvider.Create($"{packageName}-{version}.zip", config, true).Close();
    /// }
    /// 
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // Creating Package Provider.
    /// // Creating Package-A, v1.0.0.0
    /// // Creating Package-B, v1.0.0.0
    /// // Creating Package-X, v1.0.0.0
    /// // Creating Package-Y, v1.0.0.0
    /// // Creating Package-Z, v1.0.0.0
    /// // 
    /// // All Packages:
    /// // Found Package-A, v1.0.0.0, Package-A-1.0.0.0.zip
    /// // Found Package-B, v1.0.0.0, Package-B-1.0.0.0.zip
    /// // Found Package-X, v1.0.0.0, Package-X-1.0.0.0.zip
    /// // Found Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip
    /// // Found Package-Z, v1.0.0.0, Package-Z-1.0.0.0.zip
    /// // 
    /// // press anykey...
    /// </code>    
    /// <br/>
    /// <font size="4"><b>Example</b></font>
    /// <para><b>[Step #3: Create Package C]</b></para>
    /// <br/>
    /// <note type="note">
    /// <para>Before running this example please see <b>[Step #1: Create Packages A and B]</b> in <see cref="dodSON.Core.Packaging.PackageProvider"/>, it explains how to run these examples correctly.</para>
    /// <para>Also, be sure to use the same package and install directories.</para>
    /// </note>
    /// <br/>
    /// <para>This example will create a <see cref="PackageProvider"/>, generate a new <see cref="Packaging.Package"/>, (Package C, v1.0.0.0), and it's new dependency <see cref="Packaging.Package"/>, (Package Y, v2.0.0.0); that (Package C, v1.0.0.0) requires as a <see cref="IDependencyPackage.MinimumVersion"/>.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // **** Create Package Provider
    ///     // the path to create the package files
    ///     var sourceDirectory = @"C:\(WORKING)\Dev\Packaging\Packs";    // #### CHANGE THIS TO A LOCAL PATH ####
    /// 
    ///     // create a file store to the packages source directory
    ///     var packagesFileStore = new dodSON.Core.FileStorage.MSdotNETFileSystem.FileStore(sourceDirectory, System.IO.Path.GetTempPath(), false);
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
    ///     Console.WriteLine($"Creating Package Provider");
    ///     var packageProvider = new dodSON.Core.Packaging.PackageProvider(packagesFileStore, configurationFilename, serializer, packageFileStoreProvider);
    /// 
    ///     // *** Create Package C
    ///     Console.WriteLine($"Creating Package-C, v1.0.0.0");
    ///     CreatePackage(packageProvider,
    ///                   "Package-C",
    ///                   new Version(1, 0, 0, 0),
    ///                   true,
    ///                   false,
    ///                   new dodSON.Core.Packaging.IDependencyPackage[] { new dodSON.Core.Packaging.DependencyPackage("Package-Y", new Version(2, 0, 0, 0)),
    ///                                                                    new dodSON.Core.Packaging.DependencyPackage("Package-Z", new Version(1, 0, 0, 0))});
    /// 
    ///     // *** Create Package Y Version 2
    ///     Console.WriteLine($"Creating Package-Y, v2.0.0.0");
    ///     CreatePackage(packageProvider,
    ///                   "Package-Y",
    ///                   new Version(2, 0, 0, 0),
    ///                   true,
    ///                   true,
    ///                   null);
    /// 
    ///     //
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// private static void CreatePackage(dodSON.Core.Packaging.IPackageProvider packProvider,
    ///                                   string packageName,
    ///                                   Version version,
    ///                                   bool isEnabled,
    ///                                   bool isDependencyPackage,
    ///                                   IList&lt;dodSON.Core.Packaging.IDependencyPackage&gt; dependencyPackages)
    /// {
    ///     // create package configuration
    ///     var config = new dodSON.Core.Packaging.PackageConfiguration(packageName, version, isEnabled, isDependencyPackage, dependencyPackages);
    ///     // create package and close it.
    ///     packProvider.Create($"{packageName}-{version}.zip", config, true).Close();
    /// }
    /// 
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // Creating Package Provider
    /// // Creating Package-C, v1.0.0.0
    /// // Creating Package-Y, v2.0.0.0
    /// // 
    /// // press anykey...
    /// </code>
    /// </example>
    public class PackageProvider
            : IPackageProvider
    {
        #region Ctor
        private PackageProvider()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="PackageProvider"/> .
        /// </summary>
        /// <param name="packagesFileStore">The file store containing the package files.</param>
        /// <param name="configurationFilename">The filename of the configuration file in the packages.</param>
        /// <param name="configurationSerializer">The configuration serializer to use to serialize and deserialize configuration files.</param>
        /// <param name="packageFileStoreProvider">A file store provider which facilitates creating and connecting to packages.</param>
        public PackageProvider(FileStorage.IFileStore packagesFileStore,
                               string configurationFilename,
                               Configuration.IConfigurationSerializer<StringBuilder> configurationSerializer,
                               IPackageFileStoreProvider packageFileStoreProvider)
            : this()
        {
            _AllPackagesFileStore = packagesFileStore ?? throw new ArgumentNullException(nameof(packagesFileStore));
            if (string.IsNullOrWhiteSpace(configurationFilename))
            {
                throw new ArgumentNullException(nameof(configurationFilename));
            }
            _ConfigurationFilename = configurationFilename;
            _ConfigurationSerializer = configurationSerializer ?? throw new ArgumentNullException(nameof(configurationSerializer));
            PackageFileStoreProvider = packageFileStoreProvider ?? throw new ArgumentNullException(nameof(packageFileStoreProvider));
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public PackageProvider(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "PackageProvider")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"PackageProvider\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // _AllPackagesFileStore
            var fileStoreConfig = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "FileStore", true);
            // add support for the ExtractionRootDirectory to be an empty string. Empty string equates to using the current user's temporary folder.
            if ((!fileStoreConfig.Items.ContainsKey("ExtractionRootDirectory")) ||
                (fileStoreConfig.Items["ExtractionRootDirectory"].Value.ToString() == ""))
            {
                fileStoreConfig.Items.Add("ExtractionRootDirectory", System.IO.Path.GetTempPath(), typeof(string));
            }
            _AllPackagesFileStore = (FileStorage.IFileStore)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(fileStoreConfig);
            // configurationFilename
            _ConfigurationFilename = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationItem<string>(configuration, "ConfigurationFilename", true);
            // serializer
            _ConfigurationSerializer = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationItem<Core.Configuration.IConfigurationSerializer<StringBuilder>>(configuration, "ConfigurationFileSerializer", true);
            // packageFileStoreProvider
            PackageFileStoreProvider = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationItem<IPackageFileStoreProvider>(configuration, "PackageFileStoreProvider", true);
        }
        #endregion
        #region Private Fields
        private FileStorage.IFileStore _AllPackagesFileStore;
        private string _ConfigurationFilename;
        private Core.Configuration.IConfigurationSerializer<StringBuilder> _ConfigurationSerializer;
        #endregion
        #region dodSON.Core.Packaging.IPackageProvider Methods
        /// <summary>
        /// Provides the proper <see cref="FileStorage.ICompressedFileStore"/> to open packages.
        /// </summary>
        public IPackageFileStoreProvider PackageFileStoreProvider
        {
            get;
        }
        /// <summary>
        /// Gets a collection of all packages.
        /// </summary>
        public IEnumerable<IPackage> Packages
        {
            get
            {
                foreach (var packageItem in _AllPackagesFileStore.GetAll())
                {
                    var extractedPackageFilename = "";
                    try
                    {
                        // create package file store at temp location
                        extractedPackageFilename = packageItem.Extract(System.IO.Path.GetTempPath(), true);
                        // connect IFileStore
                        dodSON.Core.FileStorage.IFileStore packageStore = null;
                        try
                        {
                            packageStore = PackageFileStoreProvider.Connect(extractedPackageFilename);
                        }
                        catch
                        {
                            // ignore any files that cannot be opened as .zip files
                        }
                        if (packageStore != null)
                        {
                            // extract PackageConfiguration
                            IPackageConfiguration config = RetrieveConfiguration(packageStore, _ConfigurationFilename);
                            if (config != null)
                            {
                                // yield results
                                yield return new Package(packageItem.RootFilename, _ConfigurationFilename, config);
                            }
                        }
                    }
                    finally
                    {
                        // delete package at temp location
                        FileStorage.FileStorageHelper.DeleteFile(extractedPackageFilename);
                    }
                }
            }
        }
        /// <summary>
        /// Gets a collection of only the enabled, highest versions of any one package by name.
        /// </summary>
        public IEnumerable<IPackage> PackagesEnabledHighestVersionsOnly
        {
            get
            {
                var distinctPacks = new Dictionary<string, IPackage>();
                // go through all packages
                foreach (var packInfo in Packages)
                {
                    if (packInfo.PackageConfiguration.IsEnabled)
                    {
                        var key = packInfo.PackageConfiguration.Name;
                        if (!distinctPacks.ContainsKey(key))
                        {
                            // not found; add it
                            distinctPacks.Add(key, packInfo);
                        }
                        else
                        {
                            // have it already; check versions
                            if (packInfo.PackageConfiguration.Version > distinctPacks[key].PackageConfiguration.Version)
                            {
                                // higher version; replace it
                                distinctPacks[key] = packInfo;
                            }
                        }
                    }
                }
                return from xyz in distinctPacks
                       select xyz.Value;
            }
        }
        /// <summary>
        ///  Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>false</b>.
        /// </summary>
        public IEnumerable<IPackage> NonDependencyPackages => from x in Packages
                                                              where !x.PackageConfiguration.IsDependencyPackage
                                                              select x;
        /// <summary>
        ///  Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b>.
        /// </summary>
        public IEnumerable<IPackage> DependencyPackages => from x in Packages
                                                           where x.PackageConfiguration.IsDependencyPackage
                                                           select x;
        /// <summary>
        /// Will search for a package.
        /// </summary>
        /// <param name="name">The package name to search for.</param>
        /// <param name="version">The version to search for.</param>
        /// <returns>An <see cref="IPackage"/> representing the desired package; otherwise a <b>null</b>.</returns>
        public IPackage FindPackage(string name, Version version) => (from x in Packages
                                                                      where ((x.PackageConfiguration.Name == name) &&
                                                                             (x.PackageConfiguration.Version == version))
                                                                      select x).FirstOrDefault();
        /// <summary>
        /// Will search for a set of packages by name.
        /// </summary>
        /// <param name="name">The packages name to search for.</param>
        /// <returns>A collection of packages with the same name.</returns>
        public IEnumerable<IPackage> FindPackages(string name) => from x in Packages
                                                                  where x.PackageConfiguration.Name == name
                                                                  select x;
        /// <summary>
        /// Will create a new package.
        /// </summary>
        /// <param name="packageRootFilename">The package filename.</param>
        /// <param name="configurationFilename">The filename to use when writing the <paramref name="packageConfiguration"/> to the package.</param>
        /// <param name="packageConfiguration">The configuration for this package.</param>
        /// <param name="overwrite"><b>True</b> to write-over the package file if it already exists, otherwise, <b>false</b>.</param>
        /// <returns>An <see cref="IConnectedPackage"/>, which represents the desired package.</returns>
        /// <remarks>
        /// See <see cref="ConnectedPackage"/> concerning proper handling of the <see cref="ConnectedPackage"/>.
        /// </remarks>
        public IConnectedPackage Create(string packageRootFilename,
                                        string configurationFilename,
                                        IPackageConfiguration packageConfiguration,
                                        bool overwrite)
        {
            var tempConfigFilename = "";
            try
            {
                // test for existing package
                if (_AllPackagesFileStore.Contains(packageRootFilename) && !overwrite)
                {
                    throw new Exception($"File already exists. Filename={packageRootFilename}");
                }
                // create package file store at temp location
                var backendStorageZipFilename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "packStore_" + Guid.NewGuid().ToString());
                var packageFileStore = PackageFileStoreProvider.Create(backendStorageZipFilename);
                // save configuration to package file store
                tempConfigFilename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "configuration_" + Guid.NewGuid().ToString());
                System.IO.File.WriteAllText(tempConfigFilename, _ConfigurationSerializer.Serialize(packageConfiguration.Configuration).ToString());
                packageFileStore.Add(configurationFilename, tempConfigFilename, DateTime.Now, new System.IO.FileInfo(tempConfigFilename).Length);
                // must save now to store the configuration file before it is deleted
                packageFileStore.Save(false);
                // adds package file store to file store
                _AllPackagesFileStore.Add(packageRootFilename, backendStorageZipFilename, DateTime.Now, new System.IO.FileInfo(tempConfigFilename).Length);
                // create IPackage
                return new ConnectedPackage(_AllPackagesFileStore, packageRootFilename, backendStorageZipFilename, packageFileStore, packageConfiguration);
            }
            finally
            {
                // delete temp configuration file
                FileStorage.FileStorageHelper.DeleteFile(tempConfigFilename);
            }
        }
        /// <summary>
        /// Will create a new package using a default package configuration name.
        /// </summary>
        /// <param name="packageRootFilename">The package filename.</param>
        /// <param name="packageConfiguration">The configuration for this package.</param>
        /// <param name="overwrite"><b>True</b> to write-over the package file if it already exists, otherwise, <b>false</b>.</param>
        /// <returns>An <see cref="IConnectedPackage"/>, which represents the desired package.</returns>
        /// <remarks>
        /// See <see cref="ConnectedPackage"/> concerning proper handling of the <see cref="ConnectedPackage"/>.
        /// </remarks>
        public IConnectedPackage Create(string packageRootFilename,
                                        IPackageConfiguration packageConfiguration,
                                        bool overwrite)
        {
            return Create(packageRootFilename, _ConfigurationFilename, packageConfiguration, overwrite);
        }
        /// <summary>
        /// Will create a new package using the supplied <paramref name="package"/>.
        /// </summary>
        /// <param name="package">The package to use to create the package.</param>
        /// <param name="overwrite"><b>True</b> to write-over the package file if it already exists, otherwise, <b>false</b>.</param>
        /// <returns>An <see cref="IConnectedPackage"/>, which represents the desired package.</returns>
        /// <remarks>
        /// See <see cref="ConnectedPackage"/> concerning proper handling of the <see cref="ConnectedPackage"/>.
        /// </remarks>
        public IConnectedPackage Create(IPackage package, bool overwrite) => Create(package.RootFilename, package.ConfigurationFilename, package.PackageConfiguration, overwrite);
        /// <summary>
        /// Will connect to an existing package.
        /// </summary>
        /// <param name="packageRootFilename">The package filename.</param>
        /// <returns>An <see cref="IConnectedPackage"/>, which represents the desired package.</returns>
        /// <remarks>
        /// See <see cref="ConnectedPackage"/> concerning proper handling of the <see cref="ConnectedPackage"/>.
        /// </remarks>
        public IConnectedPackage Connect(string packageRootFilename) => Connect(packageRootFilename, _ConfigurationFilename);
        /// <summary>
        /// Will connect to an existing package.
        /// </summary>
        /// <param name="packageRootFilename">The package filename.</param>
        /// <param name="configurationFilename">The filename to use when reading the <paramref name="configurationFilename"/> from the package.</param>
        /// <returns>An <see cref="IConnectedPackage"/>, which represents the desired package.</returns>
        /// <remarks>
        /// See <see cref="ConnectedPackage"/> concerning proper handling of the <see cref="ConnectedPackage"/>.
        /// </remarks>
        public IConnectedPackage Connect(string packageRootFilename,
                                         string configurationFilename)
        {
            // find package
            if (!_AllPackagesFileStore.Contains(packageRootFilename))
            {
                throw new ArgumentException($"Package not found. Package Filename={packageRootFilename}");
            }
            // extract to temp location
            var extractedPackageFilename = _AllPackagesFileStore.Get(packageRootFilename).Extract(System.IO.Path.GetTempPath(), true);
            if (!System.IO.File.Exists(extractedPackageFilename))
            {
                throw new Exception($"PackageProvider; Unable to extract Package. Filename={packageRootFilename}");
            }
            // connect IFileStore
            var packageStore = PackageFileStoreProvider.Connect(extractedPackageFilename);
            // extract PackageConfiguration
            IPackageConfiguration config = RetrieveConfiguration(packageStore, configurationFilename);
            if (config != null)
            {
                // create IPackage
                return new ConnectedPackage(_AllPackagesFileStore, packageRootFilename, extractedPackageFilename, packageStore, config);
            }
            //
            return null;
        }
        /// <summary>
        /// Will read a configuration file from a package.
        /// </summary>
        /// <param name="package">The package to read the configuration file from.</param>
        /// <returns>The contents of the package configuration file deserialized into an <see cref="IPackageConfiguration"/> type.</returns>
        public IPackageConfiguration ReadConfigurationFile(IPackage package)
        {
            // get package information
            var packInfo = FindPackage(package.PackageConfiguration.Name, package.PackageConfiguration.Version);
            if (packInfo != null)
            {
                // get package
                using (var connectedPackage = Connect(packInfo.RootFilename))
                {
                    return RetrieveConfiguration(connectedPackage.FileStore, _ConfigurationFilename);
                }
            }
            else
            {
                throw new Exception($"Package {package.FullyQualifiedPackageName} not found.");
            }
        }
        /// <summary>
        /// Will write a configuration file to a package.
        /// </summary>
        /// <param name="package">The package to write the configuration file to.</param>
        /// <param name="configuration">The package configuration to write.</param>
        public void WriteConfigurationFile(IPackage package,
                                           IPackageConfiguration configuration)
        {
            // get package information
            var packInfo = FindPackage(package.PackageConfiguration.Name, package.PackageConfiguration.Version);
            if (packInfo != null)
            {
                var tempFilePath = System.IO.Path.GetTempFileName();
                try
                {
                    // get package
                    using (var connectedPackage = Connect(packInfo.RootFilename))
                    {
                        // get original filename
                        var originalFilename = "";
                        var originalConfigFileItem = connectedPackage.FileStore.Find((x) => { return x.RootFilename == _ConfigurationFilename; }).FirstOrDefault();
                        if (originalConfigFileItem != null)
                        {
                            originalFilename = originalConfigFileItem.OriginalFilename;
                        }
                        // save new configuration to tempFilePath
                        var newConfig = _ConfigurationSerializer.Serialize(configuration.Configuration);
                        System.IO.File.WriteAllText(tempFilePath, newConfig.ToString());
                        // import new configuration to package
                        var fileInfo = new System.IO.FileInfo(tempFilePath);
                        var addItem = connectedPackage.FileStore.Add(_ConfigurationFilename, tempFilePath, fileInfo.LastWriteTimeUtc, new System.IO.FileInfo(tempFilePath).Length);
                        // save file store
                        connectedPackage.FileStore.Save(false);
                        // restore original filename
                        var configFileItem = connectedPackage.FileStore.Find((x) => { return x.RootFilename == _ConfigurationFilename; }).FirstOrDefault();
                        if ((configFileItem != null) && (!string.IsNullOrWhiteSpace(originalFilename)))
                        {
                            configFileItem.SetOriginalFilename(originalFilename);
                        }
                        if (!string.IsNullOrWhiteSpace(originalFilename))
                        {
                            ((dodSON.Core.FileStorage.IFileStoreItemAdvanced)addItem).SetOriginalFilename(originalFilename);
                        }
                    }
                }
                finally
                {
                    FileStorage.FileStorageHelper.DeleteFile(tempFilePath);
                }
            }
            else
            {
                throw new Exception($"Package {package.FullyQualifiedPackageName} not found.");
            }
        }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("PackageProvider");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // _AllPackagesFileStore
                // remove (local) temp directory from configuration (make it blank)
                var dude = _AllPackagesFileStore.Configuration;
                if ((dude.Items.ContainsKey("ExtractionRootDirectory")) &&
                    (dude.Items["ExtractionRootDirectory"].Value.ToString().Equals(System.IO.Path.GetTempPath(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    dude.Items.Add("ExtractionRootDirectory", "", typeof(string));
                }
                result.Add(dude);
                // configurationFilename
                result.Items.Add("ConfigurationFilename", _ConfigurationFilename, _ConfigurationFilename.GetType());
                // configurationSerializer
                result.Items.Add("ConfigurationFileSerializer", _ConfigurationSerializer.GetType(), typeof(Type));
                // packageFileStoreProvider
                result.Items.Add("PackageFileStoreProvider", PackageFileStoreProvider.GetType(), typeof(Type));
                // 
                return result;
            }
        }
        #endregion
        #region Private Methods
        private IPackageConfiguration RetrieveConfiguration(FileStorage.IFileStore packageStore, string configurationFilename) => ExtractAndReadConfiguration(packageStore, _ConfigurationSerializer, configurationFilename);
        private IPackageConfiguration ExtractAndReadConfiguration(FileStorage.IFileStore fileStore,
                                                                  Configuration.IConfigurationSerializer<StringBuilder> serializer,
                                                                  string filename)
        {
            if (!fileStore.Contains(filename))
            {
                return null;
            }
            var extractedFilename = fileStore.Get(filename).Extract(System.IO.Path.GetTempPath(), true);
            if (!System.IO.File.Exists(extractedFilename))
            {
                return null;
            }
            try
            {
                var config = serializer.Deserialize(new StringBuilder(System.IO.File.ReadAllText(extractedFilename)));
                return (IPackageConfiguration)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(PackageConfiguration), config);
            }
            catch
            {
                // swallow any exception; return null
                return null;
            }
            finally
            {
                FileStorage.FileStorageHelper.DeleteFile(extractedFilename);
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Installation
{
    /// <summary>
    /// Provides methods to install and uninstall packages, iterate installed packages, find installed packages and read and write installed package configuration files.
    /// </summary>
    /// <example>
    /// <para>The following code example will create a new <see cref="Packaging.Package"/>, install that <see cref="Packaging.Package"/> and output the installation logs.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // **** create package configuration
    ///     Console.WriteLine("Creating Packaging Configuration");
    ///     var packageName = "MyPackage";
    ///     var packageVersion = new Version(1, 0, 0, 0);
    ///     var isEnabled = true;
    ///     var isDependencyPackage = false;
    ///     var configuration = new dodSON.Core.Packaging.PackageConfiguration(packageName, packageVersion, isEnabled, isDependencyPackage);
    ///     
    ///     // **** create package
    ///     Console.WriteLine("Creating Package");
    ///     var packageRootFilename = $"{packageName}.package.zip";
    ///     var configurationFilename = "package.configuration.ini";
    ///     var package = new dodSON.Core.Packaging.Package(packageRootFilename, configurationFilename, configuration);
    ///     
    ///     // **** save package
    ///     Console.WriteLine("Save Package");
    ///     // create packages file store
    ///     var packagesFileStoreSourceDirectory = @"C:\(WORKING)\Dev\Packaging\Packs";    // #### CHANGE THIS TO A LOCAL PATH ####
    ///     var extractionDirectory = System.IO.Path.GetTempPath();
    ///     var saveOriginalFilenames = false;
    ///     var packagesFileStore = new dodSON.Core.FileStorage.MSdotNETFileSystem.FileStore(packagesFileStoreSourceDirectory, extractionDirectory, saveOriginalFilenames);
    ///     
    ///     // create package provider
    ///     var packageSerializer = new dodSON.Core.Configuration.IniConfigurationSerializer(new dodSON.Core.Compression.DeflateStreamCompressor());
    ///     var packageFileStoreProvider = new dodSON.Core.Packaging.MSdotNETZip.PackageFileStoreProvider();
    ///     var packageProvider = new dodSON.Core.Packaging.PackageProvider(packagesFileStore, configurationFilename, packageSerializer, packageFileStoreProvider);
    ///     
    ///     // create empty package
    ///     var overwrite = true;
    ///     packageProvider.Create(package, overwrite).Close();    // #### Create(...) returns a dodSON.Core.Packaging.IConnectPackage, which must be closed to clean up residual resources. ####
    ///                                                            // #### See dodSON.Core.Packaging.IConnectPackage and dodSON.Core.Packaging.ConnectPackage for more information. #### 
    ///     
    ///     // **** install package
    ///     // create installer
    ///     Console.WriteLine("Create Installer");
    ///     var installationRootPath = @"C:\(WORKING)\Dev\Packaging\InstallPath";    // #### CHANGE THIS TO A LOCAL PATH ####
    ///     var installer = new dodSON.Core.Installation.Installer(installationRootPath, configurationFilename, packageSerializer);
    ///     
    ///     // install package
    ///     Console.WriteLine("Install Package");
    ///     var installType = dodSON.Core.Installation.InstallType.HighestVersionOnly;
    ///     var cleanInstall = false;
    ///     var enablePackageUpdates = false;
    ///     var removeOrphanedPackages = false;
    ///     dodSON.Core.Logging.ILogs installLog = installer.Install(package, packageProvider, installType, cleanInstall, enablePackageUpdates, removeOrphanedPackages);
    ///     
    ///     // output installation log
    ///     Console.WriteLine();
    ///     Console.WriteLine("Installation Log:");
    ///     Console.WriteLine("--------------------------------");
    ///     foreach (var logEntry in installLog)
    ///     {
    ///         Console.WriteLine($"[{logEntry.SourceId}] {logEntry.Message}");
    ///     }
    ///     
    ///     //
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    ///
    ///
    /// // This code produces output similar to the following:
    ///
    /// // Creating Packaging Configuration
    /// // Creating Package
    /// // Save Package
    /// // Create Installer
    /// // Install Package
    /// // 
    /// // Installation Log:
    /// // --------------------------------
    /// // [Installer] Installing package and dependency chain for [MyPackage, v1.0.0.0, MyPackage.package.zip]
    /// // [Installer] Install Path=C:\(WORKING)\Dev\Packaging\InstallPath\MyPackage_v1.0.0.0
    /// // [Installer] Installation Type=HighestVersionOnly
    /// // [Installer] Clean Install=False
    /// // [Installer] Package Update=False
    /// // [Installer] Remove Orphaned Packages=False
    /// // [Installer] Total installed packages=0
    /// // [Installer] Enabled packages=0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Adding package[MyPackage, v1.0.0.0, MyPackage.package.zip]
    /// // [MyPackage_v1.0.0.0] Creating directory=C:\(WORKING)\Dev\Packaging\InstallPath\MyPackage_v1.0.0.0
    /// // [MyPackage_v1.0.0.0] Adding File=C:\(WORKING)\Dev\Packaging\InstallPath\MyPackage_v1.0.0.0\package.configuration.ini
    /// // [MyPackage_v1.0.0.0] Files: Added=1
    /// // [Installer] Total packages processed:Added=1
    /// // [Installer] Total installed packages=1
    /// // [Installer] Enabled packages=1
    /// // [Installer] #1=MyPackage, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\MyPackage_v1.0.0.0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Completed installation of package and dependency chain for [MyPackage, v1.0.0.0, MyPackage.package.zip]
    /// // [Installer] Elapsed Time=00:00:00.0440014
    /// // --------------------------------
    /// // press anykey...
    /// </code>
    /// <br/>
    /// <font size="4"><b>Example</b></font>
    /// <br/>
    /// <b>[Step #2: Install Packages A and B]</b>
    /// <br/><br/>
    /// <note type="note">
    /// <para>Before running this example please see <b>[Step #1: Create Packages A and B]</b> in <see cref="dodSON.Core.Packaging.PackageProvider"/>, it explains how to run these examples correctly.</para>
    /// <para>Also, be sure to use the same package and install directories.</para>
    /// </note>
    /// <note type="note">
    /// Before running this example please remove all files from the test install directory; defined by the variable: <b>installationRootPath</b>.
    /// </note>
    /// <br/>
    /// This example will install <see cref="Packaging.Package"/>s A and B along with a shared set of dependency <see cref="Packaging.Package"/>s and display the installation logs.
    /// <br/><br/>
    /// Create a console application and add the following code:
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // **** create package provider
    ///     Console.WriteLine("Creating Package Provider");
    ///     // create packages file store
    ///     var packagesFileStoreSourceDirectory = @"C:\(WORKING)\Dev\Packaging\Packs";    // #### CHANGE THIS TO A LOCAL PATH ####
    ///     var extractionDirectory = System.IO.Path.GetTempPath();
    ///     var saveOriginalFilenames = false;
    ///     var packagesFileStore = new dodSON.Core.FileStorage.MSdotNETFileSystem.FileStore(packagesFileStoreSourceDirectory, extractionDirectory, saveOriginalFilenames);
    ///
    ///     // create package provider
    ///     var configurationFilename = "package.config.xml";
    ///     var packageSerializer = new dodSON.Core.Configuration.XmlConfigurationSerializer(new dodSON.Core.Compression.DeflateStreamCompressor());
    ///     var packageFileStoreProvider = new dodSON.Core.Packaging.MSdotNETZip.PackageFileStoreProvider();
    ///     var packageProvider = new dodSON.Core.Packaging.PackageProvider(packagesFileStore, configurationFilename, packageSerializer, packageFileStoreProvider);
    ///
    ///     // **** create installer
    ///     Console.WriteLine("Creating Package Installer");
    ///     var installationRootPath = @"C:\(WORKING)\Dev\Packaging\InstallPath";    // #### CHANGE THIS TO A LOCAL PATH ####
    ///     var installer = new dodSON.Core.Installation.Installer(installationRootPath, configurationFilename, packageSerializer);
    ///
    ///     // **** install all non-dependency packages in the package provider
    ///     Console.WriteLine("Installing all non-dependency packages");
    ///     var installType = dodSON.Core.Installation.InstallType.HighestVersionOnly;
    ///     var cleanInstall = false;
    ///     var enablePackageUpdates = false;
    ///     var removeOrphanedPackages = false;
    ///     foreach (var package in packageProvider.NonDependencyPackages.ToList())
    ///     {
    ///         Console.WriteLine("--------------------------------");
    ///         Console.WriteLine();
    ///         Console.WriteLine($"Installing Package [{package.FullyQualifiedPackageName}]");
    ///         var installLog = installer.Install(package, packageProvider, installType, cleanInstall, enablePackageUpdates, removeOrphanedPackages);
    ///
    ///         // output installation log
    ///         Console.WriteLine();
    ///         Console.WriteLine("Installation Log:");
    ///         Console.WriteLine("--------------------------------");
    ///         foreach (var logEntry in installLog)
    ///         {
    ///             Console.WriteLine($"[{logEntry.SourceId}] {logEntry.Message}");
    ///         }
    ///     }
    ///     //
    ///     Console.WriteLine();
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    ///
    ///
    /// // This code produces output similar to the following:
    ///
    /// // Creating Package Provider
    /// // Creating Package Installer
    /// // Installing all non-dependency packages
    /// // --------------------------------
    /// // 
    /// // Installing Package [Package-A, v1.0.0.0, Package-A-1.0.0.0.zip]
    /// // 
    /// // Installation Log:
    /// // --------------------------------
    /// // [Installer] Installing package and dependency chain for [Package-A, v1.0.0.0, Package-A-1.0.0.0.zip]
    /// // [Installer] Install Path=C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Installer] Installation Type=HighestVersionOnly
    /// // [Installer] Clean Install=False
    /// // [Installer] Package Update=False
    /// // [Installer] Remove Orphaned Packages=False
    /// // [Installer] Total installed packages=0
    /// // [Installer] Enabled packages=0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Adding package [Package-A, v1.0.0.0, Package-A-1.0.0.0.zip]
    /// // [Package-A_v1.0.0.0] Creating directory=C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Package-A_v1.0.0.0] Adding File=C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0\package.config.xml
    /// // [Package-A_v1.0.0.0] Files: Added=1
    /// // [Installer] Adding package [Package-X, v1.0.0.0, Package-X-1.0.0.0.zip]
    /// // [Package-X_v1.0.0.0] Creating directory=C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0
    /// // [Package-X_v1.0.0.0] Adding File=C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0\package.config.xml
    /// // [Package-X_v1.0.0.0] Files: Added=1
    /// // [Installer] Adding package [Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip]
    /// // [Package-Y_v1.0.0.0] Creating directory=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Package-Y_v1.0.0.0] Adding File=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0\package.config.xml
    /// // [Package-Y_v1.0.0.0] Files: Added=1
    /// // [Installer] Total packages processed:Added=3
    /// // [Installer] Total installed packages=3
    /// // [Installer] Enabled packages=3
    /// // [Installer] #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Installer] #2=Package-X, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0
    /// // [Installer] #3=Package-Y, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Completed installation of package and dependency chain for [Package-A, v1.0.0.0, Package-A-1.0.0.0.zip]
    /// // [Installer] Elapsed Time=00:00:00.2236451
    /// // --------------------------------
    /// // 
    /// // Installing Package [Package-B, v1.0.0.0, Package-B-1.0.0.0.zip]
    /// // 
    /// // Installation Log:
    /// // --------------------------------
    /// // [Installer] Installing package and dependency chain for [Package-B, v1.0.0.0, Package-B-1.0.0.0.zip]
    /// // [Installer] Install Path=C:\(WORKING)\Dev\Packaging\InstallPath\Package-B_v1.0.0.0
    /// // [Installer] Installation Type=HighestVersionOnly
    /// // [Installer] Clean Install=False
    /// // [Installer] Package Update=False
    /// // [Installer] Remove Orphaned Packages=False
    /// // [Installer] Total installed packages=3
    /// // [Installer] Enabled packages=3
    /// // [Installer] #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Installer] #2=Package-X, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0
    /// // [Installer] #3=Package-Y, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Adding package [Package-B, v1.0.0.0, Package-B-1.0.0.0.zip]
    /// // [Package-B_v1.0.0.0] Creating directory=C:\(WORKING)\Dev\Packaging\InstallPath\Package-B_v1.0.0.0
    /// // [Package-B_v1.0.0.0] Adding File=C:\(WORKING)\Dev\Packaging\InstallPath\Package-B_v1.0.0.0\package.config.xml
    /// // [Package-B_v1.0.0.0] Files: Added=1
    /// // [Installer] Skipping package [Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip]. Package already installed.
    /// // [Installer] Adding package [Package-Z, v1.0.0.0, Package-Z-1.0.0.0.zip]
    /// // [Package-Z_v1.0.0.0] Creating directory=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // [Package-Z_v1.0.0.0] Adding File=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0\package.config.xml
    /// // [Package-Z_v1.0.0.0] Files: Added=1
    /// // [Installer] Total packages processed: Skipped=1, Added=2
    /// // [Installer] Total installed packages=5
    /// // [Installer] Enabled packages=5
    /// // [Installer] #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Installer] #2=Package-B, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-B_v1.0.0.0
    /// // [Installer] #3=Package-X, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0
    /// // [Installer] #4=Package-Y, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Installer] #5=Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Completed installation of package and dependency chain for [Package-B, v1.0.0.0, Package-B-1.0.0.0.zip]
    /// // [Installer] Elapsed Time=00:00:00.2187511
    /// // 
    /// // --------------------------------
    /// // press anykey...
    /// </code>
    /// <br/>
    /// <font size="4"><b>Example</b></font>
    /// <br/>
    /// <b>[Step #4: Install Package C]</b>
    /// <br/><br/>
    /// <note type="note">
    /// <para>Before running this example please see <b>[Step #1: Create Packages A and B]</b> in <see cref="dodSON.Core.Packaging.PackageProvider"/>, it explains how to run these examples correctly.</para>
    /// <para>Also, be sure to use the same package and install directories.</para>
    /// </note>
    /// <br/>
    /// <para>
    /// This example will demonstrate how to patch a package to depend on a specific package version.
    /// </para>
    /// <para>
    /// The scenario follows that package A and package B, along with their dependency packages, have been installed and are working fine. 
    /// Now, along comes package C with a new version of package Y, version 2.0. 
    /// After installing it, using <see cref="Installation.InstallType.HighestVersionOnly"/>, package B fails. 
    /// Looking at package B's <see cref="Installation.IInstaller.DependencyChain(IInstalledPackage)"/>, you see that it now references package Y 2.0; the wrong package.
    /// You can patch package B to specifically use package Y 1.0, if package Y 1.0 is available, by setting the <see cref="Packaging.IDependencyPackage.SpecificVersion"/> of the Y dependency in package B to version 1.0.
    /// This will tell the packaging and installation systems to try to use the <see cref="Packaging.IDependencyPackage.SpecificVersion"/> is possible. (see <see cref="Installation.InstallType"/> and <see cref="DependencyChain(IInstalledPackage)"/> for more information.)
    /// This demonstrates the differences between <see cref="InstallType.HighestVersionOnly"/> and <see cref="InstallType.SideBySide"/>.
    /// </para>
    /// First, is will display package B's dependency chain. Then is will install package C with it's dependencies; using the <see cref="InstallType.HighestVersionOnly"/> installation method.
    /// Then it will display package B's dependency chain again; showing that it now depends on (Package Y, 2.0.0.0).
    /// Let's assume this broke package B because it can not work with (Package Y, v2.0.0.0).
    /// It will patch package B to specifically depend on (Package Y, 1.0.0.0) and display package B's dependency chain; showing that it still depends on (Package Y, 2.0.0.0), because (Package-Y, v1.0.0.0) was orphaned and removed.
    /// Next, install (Package Y, v1.0.0.0), using the <see cref="InstallType.HighestVersionOnly"/> installation method, to demonstrate how it will not allow a lower version to be installed.
    /// Then install (Package Y, v1.0.0.0), using the <see cref="InstallType.SideBySide"/> installation method, to allow the installation of both versions of Package Y.
    /// Finally, display package B's dependency chain; showing that it now depends on (Package Y, 1.0.0.0), like it should.
    /// <br/><br/>
    /// Create a console application and add the following code:
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // **** create package provider
    ///     Console.WriteLine("Creating Package Provider");
    ///     // create packages file store
    ///     var packagesFileStoreSourceDirectory = @"C:\(WORKING)\Dev\Packaging\Packs";    // #### CHANGE THIS TO A LOCAL PATH ####
    ///     var extractionDirectory = System.IO.Path.GetTempPath();
    ///     var saveOriginalFilenames = false;
    ///     var packagesFileStore = new dodSON.Core.FileStorage.MSdotNETFileSystem.FileStore(packagesFileStoreSourceDirectory, extractionDirectory, saveOriginalFilenames);
    ///
    ///     // create package provider
    ///     var configurationFilename = "package.config.xml";
    ///     var packageSerializer = new dodSON.Core.Configuration.XmlConfigurationSerializer(new dodSON.Core.Compression.DeflateStreamCompressor());
    ///     var packageFileStoreProvider = new dodSON.Core.Packaging.MSdotNETZip.PackageFileStoreProvider();
    ///     var packageProvider = new dodSON.Core.Packaging.PackageProvider(packagesFileStore, configurationFilename, packageSerializer, packageFileStoreProvider);
    ///
    ///     // **** create installer
    ///     Console.WriteLine("Creating Package Installer");
    ///     var installationRootPath = @"C:\(WORKING)\Dev\Packaging\InstallPath";    // #### CHANGE THIS TO A LOCAL PATH ####
    ///     var installer = new dodSON.Core.Installation.Installer(installationRootPath, configurationFilename, packageSerializer);
    ///
    ///     // **** find (Package B)
    ///     var packageB = installer.FindInstalledPackage("Package-B", new Version(1, 0, 0, 0));
    ///
    ///     // **** find (Package C)
    ///     var packageC = packageProvider.FindPackage("Package-C", new Version(1, 0, 0, 0));
    ///
    ///     // **** find (Package Y)
    ///     var packageY = packageProvider.FindPackage("Package-Y", new Version(1, 0, 0, 0));
    ///
    ///     if ((packageB != null) &amp;&amp; (packageC != null) &amp;&amp; (packageY != null))
    ///     {
    ///         // **** display Package B's dependency chain
    ///         Console.WriteLine();
    ///         Console.WriteLine("Package-B's Dependency Chain:");
    ///         foreach (var item in installer.DependencyChain(packageB)) { Console.WriteLine("    " + item.Package.FullyQualifiedPackageName); }
    ///
    ///         // **** install (Package C) from the package provider
    ///         Console.WriteLine();
    ///         Console.WriteLine("Installing (Package-C, v1.0.0.0)");
    ///         var installType = dodSON.Core.Installation.InstallType.HighestVersionOnly;
    ///         var cleanInstall = false;
    ///         var enablePackageUpdates = false;
    ///         var removeOrphanedPackages = true;
    ///         var installLog_C = installer.Install(packageC, packageProvider, installType, cleanInstall, enablePackageUpdates, removeOrphanedPackages);
    ///
    ///         // **** output installation log
    ///         Console.WriteLine();
    ///         Console.WriteLine("Installation Log, Package-C:");
    ///         Console.WriteLine("--------------------------------");
    ///         foreach (var logEntry in installLog_C) { Console.WriteLine($"[{logEntry.SourceId}] {logEntry.Message}"); }
    ///
    ///         // **** display Package B's dependency chain
    ///         Console.WriteLine();
    ///         Console.WriteLine("Package-B's Dependency Chain:");
    ///         foreach (var item in installer.DependencyChain(packageB)) { Console.WriteLine("    " + item.Package.FullyQualifiedPackageName); }
    ///
    ///         // **** patch Package B
    ///         Console.WriteLine();
    ///         Console.WriteLine("Patching Package-B to specifically depend on (Package-Y, v1.0.0.0)");
    ///         // add a specific version to Package B's dependency for Package Y Version 1.0.0.0
    ///         var yDependency = packageB.Package.PackageConfiguration.DependencyPackages.Where(x => { return x.Name == "Package-Y"; }).FirstOrDefault();
    ///         if (yDependency != null)
    ///         {
    ///             // add specific version for Package-Y.
    ///             yDependency.SpecificVersion = new Version(1, 0, 0, 0);
    ///             // save the changed configuration to the package
    ///             packageProvider.WriteConfigurationFile(packageB.Package, packageB.Package.PackageConfiguration);
    ///
    ///             // #### However, changing the package is not enough; we must also change the installed package.
    ///             // #### There are two choices: 1) reinstall Package-B
    ///             // ####                        2) write a new configuration to the installed package
    ///
    ///             // write a new configuration to the installed package
    ///             installer.WriteConfigurationFile(packageB, packageB.Package.PackageConfiguration);
    ///         }
    ///         else
    ///         {
    ///             throw new Exception("Cannot find Package-B's dependency definition for (Package-Y, v1.0.0.0)");
    ///         }
    ///
    ///         // **** display Package B's dependency chain
    ///         Console.WriteLine();
    ///         Console.WriteLine("Package-B's Dependency Chain:");
    ///         foreach (var item in installer.DependencyChain(packageB)) { Console.WriteLine("    " + item.Package.FullyQualifiedPackageName); }
    ///
    ///         // **** Install (Package Y, Version 1.0.0.0), HighestVersionOnly; highlight skipping installing because too low a version
    ///         Console.WriteLine();
    ///         Console.WriteLine("Installing (Package-Y, Version 1.0.0.0), HighestVersionOnly");
    ///         var installLog_Y1 = installer.Install(packageY,
    ///                                               packageProvider,
    ///                                               dodSON.Core.Installation.InstallType.HighestVersionOnly,
    ///                                               cleanInstall,
    ///                                               enablePackageUpdates,
    ///                                               removeOrphanedPackages);
    ///
    ///         // **** output installation log
    ///         Console.WriteLine();
    ///         Console.WriteLine("Installation Log, Package-Y:");
    ///         Console.WriteLine("--------------------------------");
    ///         foreach (var logEntry in installLog_Y1) { Console.WriteLine($"[{logEntry.SourceId}] {logEntry.Message}"); }
    ///
    ///         // **** Install (Package Y, Version 1.0.0.0), SideBySide; highlight installed package
    ///         Console.WriteLine();
    ///         Console.WriteLine("Installing (Package-Y, Version 1.0.0.0), SideBySide");
    ///         var installLog_Y2 = installer.Install(packageY,
    ///                                               packageProvider,
    ///                                               dodSON.Core.Installation.InstallType.SideBySide,
    ///                                               cleanInstall,
    ///                                               enablePackageUpdates,
    ///                                               removeOrphanedPackages);
    ///
    ///         // **** output installation log
    ///         Console.WriteLine();
    ///         Console.WriteLine("Installation Log, Package-Y:");
    ///         Console.WriteLine("--------------------------------");
    ///         foreach (var logEntry in installLog_Y2) { Console.WriteLine($"[{logEntry.SourceId}] {logEntry.Message}"); }
    ///
    ///         // **** display Package B's dependency chain
    ///         Console.WriteLine();
    ///         Console.WriteLine("Package-B's Dependency Chain:");
    ///         foreach (var item in installer.DependencyChain(packageB)) { Console.WriteLine("    " + item.Package.FullyQualifiedPackageName); }
    ///     }
    ///     else
    ///     {
    ///         Console.WriteLine();
    ///         if (packageB == null) { Console.WriteLine($"ERROR: Could not file (Package-B, v1.0.0.0)"); }
    ///         if (packageC == null) { Console.WriteLine($"ERROR: Could not file (Package-C, v1.0.0.0)"); }
    ///         if (packageY == null) { Console.WriteLine($"ERROR: Could not file (Package-Y, v1.0.0.0)"); }
    ///     }
    ///
    ///     //
    ///     Console.WriteLine();
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    ///
    ///
    /// // This code produces output similar to the following:
    ///
    /// // Creating Package Provider
    /// // Creating Package Installer
    /// // 
    /// // Package-B's Dependency Chain:
    /// //     Package-Y, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// //     Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // 
    /// // Installing (Package-C, v1.0.0.0)
    /// // 
    /// // Installation Log, Package-C:
    /// // --------------------------------
    /// // [Installer] Installing package and dependency chain for [Package-C, v1.0.0.0, Package-C-1.0.0.0.zip]
    /// // [Installer] Install Path=C:\(WORKING)\Dev\Packaging\InstallPath\Package-C_v1.0.0.0
    /// // [Installer] Installation Type=HighestVersionOnly
    /// // [Installer] Clean Install=False
    /// // [Installer] Package Update=False
    /// // [Installer] Remove Orphaned Packages=True
    /// // [Installer] Total installed packages=5
    /// // [Installer] Enabled packages=5
    /// // [Installer] #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Installer] #2=Package-B, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-B_v1.0.0.0
    /// // [Installer] #3=Package-X, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0
    /// // [Installer] #4=Package-Y, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Installer] #5=Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Adding package [Package-C, v1.0.0.0, Package-C-1.0.0.0.zip]
    /// // [Package-C_v1.0.0.0] Creating directory=C:\(WORKING)\Dev\Packaging\InstallPath\Package-C_v1.0.0.0
    /// // [Package-C_v1.0.0.0] Adding File=C:\(WORKING)\Dev\Packaging\InstallPath\Package-C_v1.0.0.0\package.config.xml
    /// // [Package-C_v1.0.0.0] Files: Added=1
    /// // [Installer] Adding package [Package-Y, v2.0.0.0, Package-Y-2.0.0.0.zip]
    /// // [Package-Y_v2.0.0.0] Creating directory=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v2.0.0.0
    /// // [Package-Y_v2.0.0.0] Adding File=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v2.0.0.0\package.config.xml
    /// // [Package-Y_v2.0.0.0] Files: Added=1
    /// // [Installer] Skipping package [Package-Z, v1.0.0.0, Package-Z-1.0.0.0.zip]. Package already installed.
    /// // [Installer] Removing orphaned packages.
    /// // [Installer] Install Path=C:\(WORKING)\Dev\Packaging\InstallPath
    /// // [Installer] Remove orphaned packages, pass #1
    /// // [Installer] Uninstalling package and dependency chain for [Package-Y, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0]
    /// // [Installer] Install Path=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Installer] Removing package [Package-Y, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0]
    /// // [Package-Y_v1.0.0.0] Deleting directory=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Installer] Completed uninstall of package and dependency chain for [Package-Y, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0]
    /// // [Installer] Total packages processed: Removed=1
    /// // [Installer] Completed removing orphaned packages.
    /// // [Installer] Elapsed Time=00:00:00.2980011
    /// // [Installer] Total packages processed: Skipped=1, Added=2, Removed=1
    /// // [Installer] Total installed packages=6
    /// // [Installer] Enabled packages=6
    /// // [Installer] #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Installer] #2=Package-B, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-B_v1.0.0.0
    /// // [Installer] #3=Package-C, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-C_v1.0.0.0
    /// // [Installer] #4=Package-X, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0
    /// // [Installer] #5=Package-Y, v2.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v2.0.0.0
    /// // [Installer] #6=Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Completed installation of package and dependency chain for [Package-C, v1.0.0.0, Package-C-1.0.0.0.zip]
    /// // [Installer] Elapsed Time=00:00:00.7129993
    /// // 
    /// // Package-B's Dependency Chain:
    /// //     Package-Y, v2.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v2.0.0.0
    /// //     Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // 
    /// // Patching Package-B to specifically depend on (Package-Y, v1.0.0.0)
    /// // 
    /// // Package-B's Dependency Chain:
    /// //     Package-Y, v2.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v2.0.0.0
    /// //     Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // 
    /// // Installing (Package-Y, Version 1.0.0.0), HighestVersionOnly
    /// // 
    /// // Installation Log, Package-Y:
    /// // --------------------------------
    /// // [Installer] Installing package and dependency chain for [Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip]
    /// // [Installer] Install Path=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Installer] Installation Type=HighestVersionOnly
    /// // [Installer] Clean Install=False
    /// // [Installer] Package Update=False
    /// // [Installer] Remove Orphaned Packages=True
    /// // [Installer] Total installed packages=6
    /// // [Installer] Enabled packages=6
    /// // [Installer] #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Installer] #2=Package-B, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-B_v1.0.0.0
    /// // [Installer] #3=Package-C, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-C_v1.0.0.0
    /// // [Installer] #4=Package-X, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0
    /// // [Installer] #5=Package-Y, v2.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v2.0.0.0
    /// // [Installer] #6=Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Error:HighestVersionOnly rules will not allow installation of this package. Package [Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip] version too low. Installed Package=[Package-Y, v2.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v2.0.0.0]
    /// // [Installer] Total packages processed:Errors=1
    /// // [Installer] Total installed packages=6
    /// // [Installer] Enabled packages=6
    /// // [Installer] #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Installer] #2=Package-B, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-B_v1.0.0.0
    /// // [Installer] #3=Package-C, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-C_v1.0.0.0
    /// // [Installer] #4=Package-X, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0
    /// // [Installer] #5=Package-Y, v2.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v2.0.0.0
    /// // [Installer] #6=Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Completed installation of package and dependency chain for [Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip]
    /// // [Installer] Elapsed Time=00:00:00.1989999
    /// // 
    /// // Installing (Package-Y, Version 1.0.0.0), SideBySide
    /// // 
    /// // Installation Log, Package-Y:
    /// // --------------------------------
    /// // [Installer] Installing package and dependency chain for [Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip]
    /// // [Installer] Install Path=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Installer] Installation Type=SideBySide
    /// // [Installer] Clean Install=False
    /// // [Installer] Package Update=False
    /// // [Installer] Remove Orphaned Packages=True
    /// // [Installer] Total installed packages=6
    /// // [Installer] Enabled packages=6
    /// // [Installer] #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Installer] #2=Package-B, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-B_v1.0.0.0
    /// // [Installer] #3=Package-C, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-C_v1.0.0.0
    /// // [Installer] #4=Package-X, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0
    /// // [Installer] #5=Package-Y, v2.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v2.0.0.0
    /// // [Installer] #6=Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Adding package [Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip]
    /// // [Package-Y_v1.0.0.0] Creating directory=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Package-Y_v1.0.0.0] Adding File=C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0\package.config.xml
    /// // [Package-Y_v1.0.0.0] Files: Added=1
    /// // [Installer] Total packages processed:Added=1
    /// // [Installer] Total installed packages=7
    /// // [Installer] Enabled packages=7
    /// // [Installer] #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-A_v1.0.0.0
    /// // [Installer] #2=Package-B, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-B_v1.0.0.0
    /// // [Installer] #3=Package-C, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-C_v1.0.0.0
    /// // [Installer] #4=Package-X, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-X_v1.0.0.0
    /// // [Installer] #5=Package-Y, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// // [Installer] #6=Package-Y, v2.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v2.0.0.0
    /// // [Installer] #7=Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // [Installer] Disabled packages=0
    /// // [Installer] Orphaned packages=0
    /// // [Installer] Completed installation of package and dependency chain for [Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip]
    /// // [Installer] Elapsed Time=00:00:00.3169986
    /// // 
    /// // Package-B's Dependency Chain:
    /// //     Package-Y, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Y_v1.0.0.0
    /// //     Package-Z, v1.0.0.0, C:\(WORKING)\Dev\Packaging\InstallPath\Package-Z_v1.0.0.0
    /// // 
    /// // --------------------------------
    /// // press anykey...
    /// </code>
    /// </example>
    public class Installer
    : IInstaller
    {
        #region Ctor
        private Installer()
        {
        }
        /// <summary>
        /// Instantiates a new installer.
        /// </summary>
        /// <param name="installationRootPath">The path where the installed packages will be installed. Each package will be installed into it's own directory in this directory.</param>
        /// <param name="configurationFilename">The name of the configuration files in the packages.</param>
        /// <param name="serializer">The serializer to use to serializer and deserialize package configurations.</param>
        /// <param name="logSourceId">The value used as the source id when creating log entries.</param>
        public Installer(string installationRootPath,
                         string configurationFilename,
                         Configuration.IConfigurationSerializer<StringBuilder> serializer,
                         string logSourceId)
            : this()
        {

            if (string.IsNullOrWhiteSpace(installationRootPath))
            {
                throw new ArgumentNullException(nameof(installationRootPath));
            }
            if (!System.IO.Directory.Exists(installationRootPath))
            {
                System.IO.Directory.CreateDirectory(installationRootPath);
            }
            InstallationPath = installationRootPath;
            if (string.IsNullOrWhiteSpace(configurationFilename))
            {
                throw new ArgumentNullException(nameof(configurationFilename));
            }
            _ConfigurationFilename = configurationFilename;
            _Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            if (string.IsNullOrWhiteSpace(logSourceId))
            {
                throw new ArgumentNullException(nameof(logSourceId));
            }
            LogSourceId = logSourceId;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public Installer(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "Installer")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Installer\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // _InstallationRootPath
            InstallationPath = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "InstallationRootPath", typeof(string)).Value;
            // _ConfigurationFilename
            _ConfigurationFilename = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ConfigurationFilename", typeof(string)).Value;
            // _Serializerserializer
            var serializerType = Type.GetType((string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ConfigurationFileSerializer", typeof(Type)).Value);
            _Serializer = (Core.Configuration.IConfigurationSerializer<StringBuilder>)Core.Common.InstantiationHelper.InvokeDefaultCtor(serializerType);
            // LogSourceId
            LogSourceId = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "LogSourceId", typeof(string)).Value;
        }

        #endregion
        #region Private Fields
        private string _ConfigurationFilename;
        private Configuration.IConfigurationSerializer<StringBuilder> _Serializer;
        #endregion
        #region Installer Methods
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        public string LogSourceId { get; } = "Installer";
        /// <summary>
        /// The path the Installer will install packages.
        /// </summary>
        public string InstallationPath
        {
            get;
        }
        /// <summary>
        /// Get or set whether superfluous information is output to the log during installation.
        /// </summary>
        public bool WriteVerboseInstallationLogs { get; set; } = false;
        /// <summary>
        /// Returns <see cref="Logging.Logs"/> with information listing the installed packages.
        /// </summary>
        /// <param name="logSourceId">The log source id string to use when generating logs.</param>
        /// <returns>Logs listing all of the installed packages.</returns>
        public Logging.Logs LogInstalledPackages(string logSourceId)
        {
            var logs = new Logging.Logs();
            WriteInstalledPackagesToInstallLog(logs, LogSourceId);
            return logs;
        }
        /// <summary>
        /// Gets a collection of all installed packages.
        /// </summary>
        public IEnumerable<IInstalledPackage> InstalledPackages
        {
            get
            {
                foreach (var packageRootPath in System.IO.Directory.GetDirectories(InstallationPath, "*", System.IO.SearchOption.TopDirectoryOnly))
                {
                    var configFilename = System.IO.Path.Combine(packageRootPath, _ConfigurationFilename);
                    if (System.IO.File.Exists(configFilename))
                    {
                        // get PackageConfiguration
                        var config = ReadConfigurationFile(configFilename, _Serializer);
                        if (config != null)
                        {
                            yield return new InstalledPackage(packageRootPath,
                                                              new Packaging.Package(packageRootPath, _ConfigurationFilename, config));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets a collection of only the highest versions of the installed packages.
        /// </summary>
        public IEnumerable<IInstalledPackage> InstalledHighestVersionsOnlyPackages
        {
            get
            {
                var distinctPacks = new Dictionary<string, IInstalledPackage>();
                foreach (var packageRootPath in System.IO.Directory.GetDirectories(InstallationPath, "*", System.IO.SearchOption.TopDirectoryOnly))
                {
                    var configFilename = System.IO.Path.Combine(packageRootPath, _ConfigurationFilename);
                    if (System.IO.File.Exists(configFilename))
                    {
                        // get PackageConfiguration
                        var config = ReadConfigurationFile(configFilename, _Serializer);
                        if (config != null)
                        {
                            if (!distinctPacks.ContainsKey(config.Name))
                            {
                                distinctPacks.Add(config.Name, new InstalledPackage(packageRootPath,
                                                                                    new Packaging.Package(packageRootPath, _ConfigurationFilename, config)));
                            }
                            else
                            {
                                if (config.Version > distinctPacks[config.Name].Package.PackageConfiguration.Version)
                                {
                                    distinctPacks[config.Name] = new InstalledPackage(packageRootPath,
                                                                                      new Packaging.Package(packageRootPath, _ConfigurationFilename, config));
                                }
                            }
                        }
                    }
                }
                return from xyz in distinctPacks
                       select xyz.Value;
            }
        }
        /// <summary>
        /// Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>false</b>.
        /// </summary>
        public IEnumerable<IInstalledPackage> InstalledNonDependencyPackages
        {
            get
            {
                return from item in InstalledPackages
                       where !item.Package.PackageConfiguration.IsDependencyPackage
                       select item;
            }
        }
        /// <summary>
        /// Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b>.
        /// </summary>
        public IEnumerable<IInstalledPackage> InstalledDependencyPackages
        {
            get
            {
                return from item in InstalledPackages
                       where item.Package.PackageConfiguration.IsDependencyPackage
                       select item;
            }
        }
        /// <summary>
        /// Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b> and the package is not referenced by any other package.
        /// </summary>
        public IEnumerable<IInstalledPackage> InstalledOrphanedDependencyPackages
        {
            get
            {
                return from x in InstalledDependencyPackages
                       where ((x.Package.PackageConfiguration.IsEnabled) &&
                              (InstalledPackagesReferencingPackage(x)).Count() == 0)
                       select x;
            }
        }
        /// <summary>
        /// Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsEnabled"/> flag is <b>false</b>.
        /// </summary>
        public IEnumerable<IInstalledPackage> InstalledDisabledPackages
        {
            get
            {
                return from x in InstalledPackages
                       where (!x.Package.PackageConfiguration.IsEnabled)
                       select x;
            }
        }
        /// <summary>
        /// Gets a collection containing packages that reference the given <paramref name="referencedPackage"/>.
        /// </summary>
        /// <param name="referencedPackage">The package to search for in other package's dependency packages list.</param>
        /// <returns>A collection containing packages that reference the given <paramref name="referencedPackage"/>.</returns>
        public IEnumerable<IInstalledPackage> InstalledPackagesReferencingPackage(IInstalledPackage referencedPackage)
        {
            foreach (var pack in InstalledPackages)
            {
                foreach (var depPack in pack.Package.PackageConfiguration.DependencyPackages)
                {
                    if (depPack.Name == referencedPackage.Package.PackageConfiguration.Name)
                    {
                        var found = FindProperDependencyPackageInInstalledPackages(depPack, false);
                        if (found != null)
                        {
                            if (found.Package.PackageConfiguration.Id == referencedPackage.Package.PackageConfiguration.Id)
                            {
                                yield return pack;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets a collection containing packages that are missing dependency packages.
        /// </summary>
        public IEnumerable<IInstalledPackage> InstalledPackagesWithMissingDependencies
        {
            get
            {
                var list = new Dictionary<string, IInstalledPackage>();
                foreach (var package in InstalledPackages)
                {
                    var dependencyList = new Dictionary<string, IInstalledPackage>();
                    DependencyChain2(package, dependencyList, false);
                    foreach (var checkItem in from x in dependencyList
                                              select x.Value)
                    {
                        if (checkItem.InstallPath == "ERROR")
                        {
                            var key = checkItem.Package.ConfigurationFilename;
                            if (!list.ContainsKey(key))
                            {
                                var parts = checkItem.Package.ConfigurationFilename.Split(',');
                                if (parts.Length >= 2)
                                {
                                    var dude = FindInstalledPackage(parts[0], new Version(parts[1].Substring(2)));
                                    if (dude != null)
                                    {
                                        list.Add(key, dude);
                                    }
                                }
                            }
                        }
                    }
                }
                return from x in list.Values
                       select x;
            }
        }
        /// <summary>
        /// Gets a collection containing packages that are missing dependency packages, plus the referencing <see cref="Packaging.IDependencyPackage"/>.
        /// </summary>
        public IEnumerable<Tuple<IInstalledPackage, Packaging.IDependencyPackage>> InstalledPackagesWithMissingDependenciesEx
        {
            get
            {
                var list = new Dictionary<string, Tuple<IInstalledPackage, Packaging.IDependencyPackage>>();
                foreach (var searchPack in InstalledPackagesWithMissingDependencies)
                {
                    var dependencyList = new Dictionary<string, IInstalledPackage>();
                    DependencyChain2(searchPack, dependencyList, false);
                    foreach (var checkItem in from x in dependencyList
                                              select x.Value)
                    {
                        if (checkItem.InstallPath == "ERROR")
                        {
                            var key = checkItem.Package.ToString();
                            if (!list.ContainsKey(key))
                            {
                                Packaging.IDependencyPackage info = null;
                                var parts = checkItem.Package.RootFilename.Split(',');
                                if (parts.Length == 2)
                                {
                                    info = new Packaging.DependencyPackage(parts[0], new Version(parts[1].Substring(2)));
                                }
                                else if (parts.Length >= 2)
                                {
                                    info = new Packaging.DependencyPackage(parts[0], new Version(parts[1].Substring(2)), new Version(parts[2].Substring(2)));
                                }
                                list.Add(key, Tuple.Create(checkItem, info));
                            }
                        }
                    }
                }
                return from x in list.Values
                       select x;
            }
        }

        // NOTE: this method honors the IsEnabled flag
        /// <summary>
        /// Gets a collection containing enabled packages that are required for the <paramref name="searchPackage"/>, based on it's package dependencies and the available enabled packages. 
        /// If a dependency definition contains a <see cref="Packaging.IDependencyPackage.SpecificVersion"/> and that version is installed and enabled, then that specific version will be referenced.
        /// Otherwise, the highest, enabled version will be referenced.
        /// </summary>
        /// <param name="searchPackage">The package to generate the dependency package collection for.</param>
        /// <returns>A collection containing packages that are required for the <paramref name="searchPackage"/>.</returns>
        public IEnumerable<IInstalledPackage> DependencyChain(IInstalledPackage searchPackage)
        {
            var tempList = new Dictionary<string, IInstalledPackage>();
            DependencyChain2(searchPackage, tempList, false);
            return tempList.Values;
        }
        /// <summary>
        /// Searches for the installed package that matches the given <paramref name="package"/>'s <see cref="Packaging.IPackageConfiguration.Name"/> and <see cref="Packaging.IPackageConfiguration.Version"/>.
        /// </summary>
        /// <param name="package">The package to use for the search.</param>
        /// <returns>The installed package that matches the given <paramref name="package"/>.</returns>
        public IInstalledPackage FindInstalledPackage(Packaging.IPackage package) => FindInstalledPackage(package.PackageConfiguration.Name, package.PackageConfiguration.Version);
        /// <summary>
        /// Searches for the installed package that matches the given <paramref name="name"/> and <paramref name="version"/>.
        /// </summary>
        /// <param name="name">The package name to search for.</param>
        /// <param name="version">The package version to search for.</param>
        /// <returns>The installed package that matches the given <paramref name="name"/> and <paramref name="version"/>.</returns>
        public IInstalledPackage FindInstalledPackage(string name, Version version)
        {
            return (from x in InstalledPackages
                    where ((x.Package.PackageConfiguration.Name == name) &&
                           (x.Package.PackageConfiguration.Version == version))
                    select x).FirstOrDefault();
        }
        /// <summary>
        /// Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.Name"/> equals <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the package to search for.</param>
        /// <returns>A collection containing packages where the <see cref="Packaging.IPackageConfiguration.Name"/> equals <paramref name="name"/>.</returns>
        public IEnumerable<IInstalledPackage> FindInstalledPackages(string name)
        {
            return from x in InstalledPackages
                   where (x.Package.PackageConfiguration.Name == name)
                   select x;
        }

        // NOTE: this method honors the IsEnabled flag
        /// <summary>
        /// Will install the <paramref name="package"/> using packages from the <paramref name="packageProvider"/> using the given parameters.
        /// </summary>
        /// <param name="package">The package to install.</param>
        /// <param name="packageProvider">The source of the package and any dependency packages.</param>
        /// <param name="installationSettings">Determines the installation actions.</param>
        /// <returns>A log detailing the installation actions.</returns>
        public Logging.Logs Install(Packaging.IPackage package,
                                    Packaging.IPackageProvider packageProvider,
                                    InstallationSettings installationSettings)
        {
            var stTotalWatch = System.Diagnostics.Stopwatch.StartNew();
            // create install log
            var installLog = new Logging.Logs();
            // output initial log entries
            var thisType = this.GetType();
            var coreAssembly = System.Reflection.Assembly.GetAssembly(thisType);
            var parts = coreAssembly.FullName.Split(',');
            if (WriteVerboseInstallationLogs)
            {
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Starting Installation, ({thisType.FullName}, {parts[0]}, v{parts[1].Split('=')[1]})");
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Install Root Path={InstallationPath}");
                WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            }
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Installing package and dependency chain for {package.FullyQualifiedPackageName}");
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Install Path={GetInstallPath(package)}");
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Installation Type={installationSettings.InstallType}");
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Update Packages Before Installing={installationSettings.UpdatePackagesBeforeInstalling}");
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Clean Install={installationSettings.CleanInstall}");
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Package Update={installationSettings.EnablePackageUpdates}");
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Remove Orphaned Packages={installationSettings.RemoveOrphanedPackages}");
            // get all enabled, installed packs
            var allInstalledPacks = InstalledPackages.Where((x) => { return x.Package.PackageConfiguration.IsEnabled; }).ToList();
            // create initial instruction set, this is because of the recursive nature of the GetInstallInstructions method.
            var instructionSet = new Dictionary<string, Tuple<string, Packaging.IPackage, InstallationSettings>>();
            // get instruction set
            int totalSkipped = 0;
            int totalAdd = 0;
            int totalUpdate = 0;
            int totalRemoved = 0;
            var totalError = 0;
            GetInstallInstructions(instructionSet, packageProvider, package, allInstalledPacks, installationSettings, ref totalSkipped, ref totalAdd, ref totalRemoved, ref totalUpdate);
            // output all currently installed packages
            if (WriteVerboseInstallationLogs)
            {
                WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            }
            // execute instruction set
            ProcessInstallInstructions(instructionSet, packageProvider, allInstalledPacks, installLog);
            // count errors (from log)
            foreach (var logEntry in installLog)
            {
                if (logEntry.Message.StartsWith("Error"))
                {
                    ++totalError;
                }
            }
            // remove orphaned packages
            if (installationSettings.RemoveOrphanedPackages && InstalledOrphanedDependencyPackages.Count() > 0)
            {
                foreach (var item in RemoveOrphanedPackagesActual(InstalledOrphanedDependencyPackages, true, out totalRemoved))
                {
                    installLog.Add(item);
                }
            }
            else if (installationSettings.InstallType == InstallType.HighestVersionOnly)
            {
                // do not remove the package just installed just because it is orphaned; it could have been orphaned to begin with
                var list = (from x in InstalledOrphanedDependencyPackages
                            where (x.Package.PackageConfiguration.Name == package.PackageConfiguration.Name &&
                                   x.Package.PackageConfiguration.Version != package.PackageConfiguration.Version)
                            select x).ToList();
                if (list.Count > 0)
                {
                    installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Removing {list.Count} orphaned packages for package [{package.PackageConfiguration.Name}]");
                    var counter = 0;
                    foreach (var item in list)
                    {
                        installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"#{++counter}={item.Package.FullyQualifiedPackageName}");
                        RemovePackage(installLog, item.Package);
                    }
                    totalRemoved += counter;
                }
            }
            // output total packages uninstalled
            var displayStr = new StringBuilder("Total packages processed:");
            if (totalSkipped > 0)
            {
                displayStr.Append($" Skipped={totalSkipped}, ");
            }
            if (totalAdd > 0)
            {
                displayStr.Append($"Added={totalAdd}, ");
            }
            if (totalUpdate > 0)
            {
                displayStr.Append($"Updated={totalUpdate}, ");
            }
            if (totalRemoved > 0)
            {
                displayStr.Append($"Removed={totalRemoved}, ");
            }
            if (totalError > 0)
            {
                displayStr.Append($"Errors={totalError}, ");
            }
            displayStr.Length -= 2;
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, displayStr.ToString());
            // output all currently installed packages
            if (WriteVerboseInstallationLogs)
            {
                WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            }
            // output completion message
            stTotalWatch.Stop();
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Completed installation of package and dependency chain for {package.FullyQualifiedPackageName}, Elapsed Time={stTotalWatch.Elapsed}");
            // 
            return installLog;
        }

        // NOTE: this method honors the IsEnabled flag
        /// <summary>
        /// Will install the <paramref name="package"/> using packages from the <paramref name="packageProvider"/> using the given parameters and returns a (success/failure) flag. <b>True</b> indicates successful installation; otherwise, <b>false</b> indicates an error occurred. See the <paramref name="installLog"/> for details.
        /// </summary>
        /// <param name="package">The package to install.</param>
        /// <param name="packageProvider">The source of the package and any dependency packages.</param>
        /// <param name="installationSettings">Determines the installation actions.</param>
        /// <param name="installLog">A log detailing the installation actions.</param>
        /// <returns><b>True</b> indicates the installation proceeded without error; other <b>false</b> indicates an error occurred. See the <paramref name="installLog"/> for details.</returns>
        public bool TryInstall(Packaging.IPackage package,
                               Packaging.IPackageProvider packageProvider,
                               InstallationSettings installationSettings,
                               out Logging.Logs installLog)
        {
            installLog = Install(package, packageProvider, installationSettings);
            foreach (var item in installLog)
            {
                if (item.Message.StartsWith("Error", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Will uninstall the <paramref name="installedPackage"/>.
        /// </summary>
        /// <param name="installedPackage">The installed package to uninstall.</param>
        /// <param name="removeOrphanedPackages">If <b>true</b> the installer will remove orphaned packages after uninstall is complete.</param>
        /// <returns>A log detailing the uninstall actions.</returns>
        public Logging.Logs Uninstall(IInstalledPackage installedPackage, bool removeOrphanedPackages) => UninstallActual(installedPackage, removeOrphanedPackages, !WriteVerboseInstallationLogs);
        /// <summary>
        /// Will remove all packages.
        /// </summary>
        /// <returns>A log detailing the removal actions.</returns>
        public Logging.Logs UninstallAll()
        {
            var startTime = DateTimeOffset.Now;
            var totalPackagesUninstalled = 0;
            // create install log
            var installLog = new Logging.Logs
            {
                // output initial log entries
                { Logging.LogEntryType.Information, LogSourceId, $"Uninstalling all packages." },
                { Logging.LogEntryType.Information, LogSourceId, $"Install Path={InstallationPath}" }
            };
            // output all currently installed packages
            WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            // remove all packages
            foreach (var item in InstalledPackages)
            {
                RemovePackage(installLog, item.Package);
                ++totalPackagesUninstalled;
            }
            // output total packages uninstalled
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Total packages processed: Removed={totalPackagesUninstalled}");
            // output all currently installed packages
            WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            // output completion message
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Completed uninstalling all packages");
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Elapsed Time={DateTimeOffset.Now - startTime}");
            // 
            return installLog;
        }
        /// <summary>
        /// Removes all installed packages where the <see cref="Packaging.IPackageConfiguration.IsEnabled"/> flag is <b>false</b>.
        /// </summary>
        /// <returns>A log detailing the removal actions.</returns>
        public Logging.Logs RemoveDisabledPackages() => RemoveDisabledPackages(out int count);
        /// <summary>
        /// Removes all installed packages where the <see cref="Packaging.IPackageConfiguration.IsEnabled"/> flag is <b>false</b>.
        /// </summary>
        /// <param name="totalPackagesRemoved">The total number of packages removed.</param>
        /// <returns>A log detailing the removal actions.</returns>
        public Logging.Logs RemoveDisabledPackages(out int totalPackagesRemoved)
        {
            var startTime = DateTimeOffset.Now;
            // output initial log entries
            var installLog = new Logging.Logs
            {
                { Logging.LogEntryType.Information, LogSourceId, $"Removing disabled packages." },
                { Logging.LogEntryType.Information, LogSourceId, $"Install Path={InstallationPath}" }
            };
            // output all currently installed packages
            WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            // remove disabled packages
            totalPackagesRemoved = 0;
            var startCount = InstalledPackages.Count();
            foreach (var installedPack in InstalledDisabledPackages)
            {
                installLog.Add(UninstallActual(installedPack, false, true));
            }
            totalPackagesRemoved = startCount - InstalledPackages.Count();
            // output total packages removed
            var displayStr = $"Total packages processed: Removed={totalPackagesRemoved}";
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, displayStr);
            // output all currently installed packages
            WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            // output completion message
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Completed removing disabled packages.");
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Elapsed Time={DateTimeOffset.Now - startTime}");
            // 
            return installLog;
        }
        /// <summary>
        /// Removes all installed packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b> and the package is not referenced by any other package. 
        /// </summary>
        /// <returns>A log detailing the removal actions.</returns>
        public Logging.Logs RemoveOrphanedPackages() => RemoveOrphanedPackages(out int count);
        /// <summary>
        /// Removes all installed packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b> and the package is not referenced by any other package. 
        /// </summary>
        /// <param name="totalPackagesRemoved">The total number of packages removed.</param>
        /// <returns>A log detailing the removal actions.</returns>
        public Logging.Logs RemoveOrphanedPackages(out int totalPackagesRemoved) => RemoveOrphanedPackagesActual(InstalledOrphanedDependencyPackages, false, out totalPackagesRemoved);
        /// <summary>
        /// Will read and deserialize the configuration file from the target <paramref name="installedPackage"/>.
        /// </summary>
        /// <param name="installedPackage">The installed package to read the configuration file from.</param>
        /// <returns>The target's package configuration.</returns>
        public Packaging.IPackageConfiguration ReadConfigurationFile(IInstalledPackage installedPackage)
        {
            var path = System.IO.Path.Combine(installedPackage.InstallPath, _ConfigurationFilename);
            return ReadConfigurationFile(path, _Serializer);
        }
        /// <summary>
        /// Will serialize and write the configuration file to the target <paramref name="installedPackage"/>.
        /// </summary>
        /// <param name="installedPackage">The installed package to write the configuration file to.</param>
        /// <param name="configuration">The package configuration to serialize and write to the configuration file.</param>
        public void WriteConfigurationFile(IInstalledPackage installedPackage,
                                           Packaging.IPackageConfiguration configuration)
        {
            var path = System.IO.Path.Combine(installedPackage.InstallPath, _ConfigurationFilename);
            System.IO.File.WriteAllText(path, _Serializer.Serialize(configuration.Configuration).ToString());
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
                var result = new Configuration.ConfigurationGroup("Installer");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // _InstallationRootPath
                result.Items.Add("InstallationRootPath", InstallationPath, InstallationPath.GetType());
                // _ConfigurationFilename
                result.Items.Add("ConfigurationFilename", _ConfigurationFilename, _ConfigurationFilename.GetType());
                // _Serializer
                result.Items.Add("ConfigurationFileSerializer", _Serializer.GetType(), typeof(Type));
                // LogSourceId
                result.Items.Add("LogSourceId", LogSourceId, LogSourceId.GetType());
                // 
                return result;
            }
        }
        #endregion
        #region Private Methods
        private Packaging.IPackageConfiguration ReadConfigurationFile(string configFilename, Configuration.IConfigurationSerializer<StringBuilder> serializer)
        {
            try
            {
                var config = serializer.Deserialize(new StringBuilder(System.IO.File.ReadAllText(configFilename)));
                return (Packaging.IPackageConfiguration)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(Packaging.PackageConfiguration), config);
            }
            catch
            {
                // swallow any exception; return null
                return null;
            }
        }
        private void WriteInstalledPackagesToInstallLog(Logging.Logs installLog, string logSourceId)
        {
            // build distinct list
            var packagesOk = new List<IInstalledPackage>();
            var packagesDisabled = new List<IInstalledPackage>();
            var packagesOrphaned = new List<IInstalledPackage>();
            foreach (var pack in InstalledPackages)
            {
                var foundDisabled = (from x in InstalledDisabledPackages
                                     where x.InstallPath == pack.InstallPath
                                     select x).FirstOrDefault() != null;
                if (foundDisabled)
                {
                    if (!packagesDisabled.Contains(pack))
                    {
                        packagesDisabled.Add(pack);
                    }
                }
                else
                {
                    var foundOrphaned = (from x in InstalledOrphanedDependencyPackages
                                         where x.InstallPath == pack.InstallPath
                                         select x).FirstOrDefault() != null;
                    if (foundOrphaned)
                    {
                        if (!packagesOrphaned.Contains(pack))
                        {
                            packagesOrphaned.Add(pack);
                        }
                    }
                    else
                    {
                        packagesOk.Add(pack);
                    }
                }
            }
            // output currently installed packages
            installLog.Add(Logging.LogEntryType.Information, logSourceId, $"Total installed packages={packagesOk.Count + packagesDisabled.Count + packagesOrphaned.Count}");
            //
            var count = 0; // *multi-use variable*
            var list = new List<string>();
            foreach (var item in packagesOk)
            {
                list.Add($"#{++count}={item.Package.FullyQualifiedPackageName}");
            }
            installLog.Add(Logging.LogEntryType.Information, logSourceId, $"Enabled packages={count}");
            foreach (var item in list)
            {
                installLog.Add(Logging.LogEntryType.Information, logSourceId, item);
            }
            // output currently installed packages
            count = 0;
            list = new List<string>();
            foreach (var item in packagesDisabled)
            {
                list.Add($"#{++count}={item.Package.FullyQualifiedPackageName}");
            }
            installLog.Add(Logging.LogEntryType.Information, logSourceId, $"Disabled packages={count}");
            foreach (var item in list)
            {
                installLog.Add(Logging.LogEntryType.Information, logSourceId, item);
            }
            // output orphan count and list
            count = 0;
            list = new List<string>();
            foreach (var item in packagesOrphaned)
            {
                list.Add($"#{++count}={item.Package.FullyQualifiedPackageName}");
            }
            installLog.Add(Logging.LogEntryType.Information, logSourceId, $"Orphaned packages={count}");
            foreach (var item in list)
            {
                installLog.Add(Logging.LogEntryType.Information, logSourceId, item);
            }
        }

        // NOTE: this method honors the IsEnabled flag
        //       this is accomplished by populating [allInstalledPacks] with only enabled packages
        //
        private void GetInstallInstructions(Dictionary<string, Tuple<string, Packaging.IPackage, InstallationSettings>> instructionSet,
                                            Packaging.IPackageProvider packProvider,
                                            Packaging.IPackage package,
                                            List<IInstalledPackage> allInstalledPacks,
                                            InstallationSettings installSettings,
                                            ref int totalPackagesSkipped,
                                            ref int totalPackagesAdd,
                                            ref int totalPackagesRemove,
                                            ref int totalPackagesUpdate)
        {
            // determine how to install the 'packageInformation' package
            var searchName = package.PackageConfiguration.Name;
            var searchVersion = package.PackageConfiguration.Version;
            // check if pack is installed by name
            var installedByNameSet = from x in allInstalledPacks
                                     where x.Package.PackageConfiguration.Name == searchName
                                     select x;
            if (installedByNameSet.Count() == 0)
            {
                // pack 'name' not found
                if (CreateInstruction(instructionSet, "add", package, installSettings))
                {
                    ++totalPackagesAdd;
                }
            }
            else
            {
                if (installSettings.InstallType == InstallType.SideBySide)
                {
                    // ######## InstallationType.SideBySide
                    // check if pack is installed by name && version
                    var installedByNameAndVersionSet = from x in allInstalledPacks
                                                       where ((x.Package.PackageConfiguration.Name == searchName) &&
                                                              (x.Package.PackageConfiguration.Version == searchVersion))
                                                       select x;
                    if (installedByNameAndVersionSet.Count() == 0)
                    {
                        // pack 'name, version' not found
                        if (CreateInstruction(instructionSet, "add", package, installSettings))
                        {
                            ++totalPackagesAdd;
                        }
                    }
                    else
                    {
                        // 'pack, version' found, allow control flags to determine actions
                        if (installSettings.CleanInstall)
                        {
                            CreateInstruction(instructionSet, "remove", package, installSettings);
                            if (CreateInstruction(instructionSet, "add", package, installSettings))
                            {
                                ++totalPackagesUpdate;
                            }
                        }
                        else if (installSettings.EnablePackageUpdates)
                        {
                            if (CreateInstruction(instructionSet, "update", package, installSettings))
                            {
                                ++totalPackagesUpdate;
                            }
                        }
                        else
                        {
                            if (CreateInstruction(instructionSet, "ok", package, installSettings))
                            {
                                ++totalPackagesSkipped;
                            }
                        }
                    }
                }
                else
                {
                    // ######## InstallationType.HighestVersionOnly
                    // search all packages for highest version
                    Packaging.IPackage highestVersion = null;
                    foreach (var packInfo in allInstalledPacks)
                    {
                        if (packInfo.Package.PackageConfiguration.Name == searchName)
                        {
                            if (highestVersion == null)
                            {
                                highestVersion = packInfo.Package;
                            }
                            else if (packInfo.Package.PackageConfiguration.Version > highestVersion.PackageConfiguration.Version)
                            {
                                highestVersion = packInfo.Package;
                            }
                        }
                    }
                    if (searchVersion > highestVersion.PackageConfiguration.Version)
                    {
                        if (CreateInstruction(instructionSet, "add", package, installSettings))
                        {
                            ++totalPackagesAdd;
                        }
                    }
                    else if (searchVersion == highestVersion.PackageConfiguration.Version)
                    {
                        if (installSettings.CleanInstall)
                        {
                            CreateInstruction(instructionSet, "remove", package, installSettings);
                            if (CreateInstruction(instructionSet, "add", package, installSettings))
                            {
                                ++totalPackagesUpdate;
                            }
                        }
                        else if (installSettings.EnablePackageUpdates)
                        {
                            if (CreateInstruction(instructionSet, "update", package, installSettings))
                            {
                                ++totalPackagesUpdate;
                            }
                        }
                        else
                        {
                            if (CreateInstruction(instructionSet, "ok", package, installSettings))
                            {
                                ++totalPackagesSkipped;
                            }
                        }
                    }
                    else
                    {
                        CreateInstruction(instructionSet,
                                          "error",
                                          new Packaging.Package($"{package.FullyQualifiedPackageName}",
                                                                $"{highestVersion.FullyQualifiedPackageName}",
                                                                new Packaging.PackageConfiguration($"Error:HighestVersionOnly rules will not allow installation of this package. Package {package.FullyQualifiedPackageName} version too low. Installed Package={highestVersion.FullyQualifiedPackageName}",
                                                                                                   package.PackageConfiguration.Version,
                                                                                                   false,
                                                                                                   package.PackageConfiguration.Priority,
                                                                                                   false)),
                                          installSettings);
                    }
                }
            }
            // process dependency packages
            // for each dependency pack: call (...self...recursively...self...)
            foreach (var dependencyPack in package.PackageConfiguration.DependencyPackages)
            {
                Packaging.IPackage packageToInstall = null;
                if (installSettings.InstallType == InstallType.SideBySide)
                {
                    // ######## InstallationType.SideBySide
                    if (dependencyPack.SpecificVersion != null)
                    {
                        packageToInstall = (from x in packProvider.Packages
                                            where ((x.PackageConfiguration.IsEnabled) &&
                                                   (x.PackageConfiguration.Name == dependencyPack.Name) &&
                                                   (x.PackageConfiguration.Version == dependencyPack.SpecificVersion))
                                            select x).FirstOrDefault();
                    }
                    if (packageToInstall == null)
                    {
                        packageToInstall = (from x in packProvider.PackagesEnabledHighestVersionsOnly
                                            where ((x.PackageConfiguration.IsEnabled) &&
                                                   (x.PackageConfiguration.Name == dependencyPack.Name) &&
                                                   (x.PackageConfiguration.Version >= dependencyPack.MinimumVersion))
                                            select x).FirstOrDefault();
                        if (packageToInstall == null)
                        {
                            packageToInstall = (from x in packProvider.Packages
                                                where ((x.PackageConfiguration.IsEnabled) &&
                                                       (x.PackageConfiguration.Name == dependencyPack.Name) &&
                                                       (x.PackageConfiguration.Version >= dependencyPack.MinimumVersion))
                                                select x).FirstOrDefault();
                        }
                    }
                }
                else
                {
                    // ######## InstallationType.HighestVersionOnly
                    packageToInstall = (from x in packProvider.PackagesEnabledHighestVersionsOnly
                                        where ((x.PackageConfiguration.IsEnabled) &&
                                               (x.PackageConfiguration.Name == dependencyPack.Name) &&
                                               (x.PackageConfiguration.Version >= dependencyPack.MinimumVersion))
                                        select x).FirstOrDefault();
                }
                //
                if (packageToInstall != null)
                {
                    GetInstallInstructions(instructionSet, packProvider, packageToInstall, allInstalledPacks, installSettings, ref totalPackagesSkipped, ref totalPackagesAdd, ref totalPackagesRemove, ref totalPackagesUpdate);
                }
                else
                {
                    var displayStr = $"{dependencyPack.Name}, v{dependencyPack.MinimumVersion}";
                    if (dependencyPack.SpecificVersion != null)
                    {
                        displayStr = displayStr + $", v{dependencyPack.SpecificVersion}";
                    }
                    CreateInstruction(instructionSet,
                                      "error",
                                      new Packaging.Package(displayStr,
                                                            "dodSON Software",
                                                            new Packaging.PackageConfiguration($"Error:Dependency Package [{displayStr}] not found.",
                                                                                               dependencyPack.MinimumVersion,
                                                                                               false,
                                                                                               package.PackageConfiguration.Priority,
                                                                                               false)),
                                      installSettings);
                }
            }
        }

        private bool CreateInstruction(Dictionary<string, Tuple<string, Packaging.IPackage, InstallationSettings>> instructionSet,
                                      string commandStr,
                                      Packaging.IPackage package,
                                      InstallationSettings installSettings)
        {
            var key = $"{package.PackageConfiguration.Id},{commandStr}";
            if (!instructionSet.ContainsKey(key))
            {
                instructionSet.Add(key, new Tuple<string, Packaging.IPackage, InstallationSettings>(commandStr, package, installSettings));
                return true;
            }
            return false;
        }

        private void ProcessInstallInstructions(Dictionary<string, Tuple<string, Packaging.IPackage, InstallationSettings>> instructionSet,
                                                Packaging.IPackageProvider packProvider,
                                                List<IInstalledPackage> allInstalledPacks,
                                                Logging.Logs installLog)
        {
            foreach (var instruction in instructionSet)
            {
                var command = instruction.Value.Item1;
                if (!string.IsNullOrWhiteSpace(command))
                {
                    var installPackInfo = instruction.Value.Item2;
                    switch (command)
                    {
                        case "ok":
                            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Skipping package {installPackInfo.FullyQualifiedPackageName}. Package already installed.");
                            break;
                        case "add":
                            AddPackage(installLog, packProvider, installPackInfo, allInstalledPacks, instruction.Value.Item3);
                            break;
                        case "remove":
                            RemovePackage(installLog, installPackInfo);
                            break;
                        case "update":
                            UpdatePackage(installLog, packProvider, installPackInfo, allInstalledPacks, instruction.Value.Item3);
                            break;
                        case "error":
                            installLog.Add(Logging.LogEntryType.Information, LogSourceId, instruction.Value.Item2.PackageConfiguration.Name);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    throw new FormatException($"Class::{nameof(Installer)}, Method::{nameof(Installer.Install)}, Badly formed instruction, instruction={instruction}");
                }
            }
        }

        private Logging.Logs UninstallActual(IInstalledPackage installedPackage,
                                              bool removeOrphanedPackages,
                                              bool suppressOutputtingSuperfluousInformation)
        {
            var startTime = DateTimeOffset.Now;
            var totalPackagesUninstalled = 1;
            var totalPackagesSkipped = 0;
            var totalPackagesMissing = 0;
            var totalErrors = 0;
            // create install log
            var installLog = new Logging.Logs
            {
                { Logging.LogEntryType.Information, LogSourceId, $"Uninstalling package and dependency chain for {installedPackage.Package.FullyQualifiedPackageName}" },
                { Logging.LogEntryType.Information, LogSourceId, $"Install Path={installedPackage.InstallPath}" }
            };
            if (!suppressOutputtingSuperfluousInformation)
            {
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Remove Orphaned Packages={removeOrphanedPackages}");
            }
            if (!suppressOutputtingSuperfluousInformation)
            {
                WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            }
            // get dependency chain
            var references = DependencyChain(installedPackage);
            // uninstall prime package
            RemovePackage(installLog, installedPackage.Package);
            // uninstall dependencies from the references
            foreach (var removeDependency in references)
            {
                // determine if removeDependency should be removed
                var removeIt = true;
                if (removeDependency.Package.Name.StartsWith("Missing Dependency"))
                {
                    // dependency is missing
                    removeIt = false;
                    // log it; build install log entries
                    // write install log
                    installLog.Add(Logging.LogEntryType.Information,
                                   LogSourceId,
                                   $"Missing package [{removeDependency.Package.RootFilename}]");
                }
                else
                {
                    // iterate all installed packages
                    foreach (var testPack in InstalledPackages)
                    {
                        // check if testPack is NOT the removeDependency && testPack is NOT in the references
                        if ((testPack.InstallPath != removeDependency.InstallPath) &&
                            (references.Where((x) => { return (x.InstallPath == testPack.InstallPath); }).Count() == 0))
                        {
                            var testChain = DependencyChain(testPack);
                            // check if removeDependency is in testChain
                            var findRemoveDependencyInTestChain = testChain.Where((x) => { return (x.InstallPath == removeDependency.InstallPath); }).Count() >= 1;
                            if (findRemoveDependencyInTestChain)
                            {
                                // the removeDependency is being used by another package that is not in references; do not remove it
                                removeIt = false;
                                // log it; build install log entries
                                ++totalPackagesSkipped;
                                var strList = new List<string>();
                                var referCounter = 0;
                                foreach (var refer in InstalledPackagesReferencingPackage(removeDependency))
                                {
                                    if (refer.InstallPath != removeDependency.InstallPath)
                                    {
                                        strList.Add($"#{++referCounter}={refer.Package.FullyQualifiedPackageName}");
                                    }
                                }
                                // write install log
                                installLog.Add(Logging.LogEntryType.Information,
                                               LogSourceId,
                                               $"Leaving package {removeDependency.Package.FullyQualifiedPackageName}. Package shared by {referCounter} other package{((referCounter == 1) ? "" : "s")}.");
                                foreach (var item in strList)
                                {
                                    installLog.Add(Logging.LogEntryType.Information, LogSourceId, item);
                                }
                                break;
                            }
                        }
                    }
                }
                // 
                if (removeIt)
                {
                    RemovePackage(installLog, removeDependency.Package);
                    ++totalPackagesUninstalled;
                }
            }
            // remove orphaned packages
            var totalRemoved = 0;
            if (removeOrphanedPackages)
            {
                foreach (var item in RemoveOrphanedPackages(out totalRemoved))
                {
                    installLog.Add(item);
                }
            }
            totalPackagesUninstalled += totalRemoved;
            // count errors (from log)
            foreach (var logEntry in installLog)
            {
                if (logEntry.Message.StartsWith("Error"))
                {
                    ++totalErrors;
                }
            }
            // count missing (from log)
            var missingDict = new List<string>();
            foreach (var logEntry in installLog)
            {
                if (logEntry.Message.StartsWith("Missing"))
                {
                    if (!missingDict.Contains(logEntry.Message))
                    {
                        missingDict.Add(logEntry.Message);
                    }
                }
            }
            totalPackagesMissing = missingDict.Count;
            // output total packages uninstalled
            if (!suppressOutputtingSuperfluousInformation)
            {
                var displayStr = new StringBuilder($"Total packages processed: Removed={totalPackagesUninstalled}, ");
                if (totalPackagesSkipped > 0)
                {
                    displayStr.Append($"Skipped={totalPackagesSkipped}, ");
                }
                if (totalPackagesMissing > 0)
                {
                    displayStr.Append($"Missing={totalPackagesMissing}, ");
                }
                if (totalErrors > 0)
                {
                    displayStr.Append($"Errors={totalErrors}, ");
                }
                displayStr.Length -= 2;
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, displayStr.ToString());
                // output all currently installed packages
                WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            }
            // output completion message
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Completed uninstall of package and dependency chain for {installedPackage.Package.FullyQualifiedPackageName}");
            if (!suppressOutputtingSuperfluousInformation)
            {
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Elapsed Time={DateTimeOffset.Now - startTime}");
            }
            // 
            return installLog;
        }

        private void AddPackage(Logging.Logs installLog,
                                Packaging.IPackageProvider packProvider,
                                Packaging.IPackage package,
                                List<IInstalledPackage> allInstalledPacks,
                                InstallationSettings installationSettings)
        {
            // update package before installing
            if (installationSettings.UpdatePackagesBeforeInstalling)
            {
                UpdateActualPackage(installLog, packProvider, package);
            }
            // add package
            var packRootPath = GetInstallPath(package);
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Adding files from package {package.FullyQualifiedPackageName}");
            // check for packRootPath existence
            if (!System.IO.Directory.Exists(packRootPath))
            {
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Creating directory={packRootPath}");
                FileStorage.FileStorageHelper.CreateDirectory(packRootPath);
            }
            else
            {
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Using existing directory={packRootPath}");
            }
            // extract package
            using (var pack = packProvider.Connect(package.RootFilename))
            {
                var fileCount = 0;
                pack.FileStore.ForEach((x) =>
                {
                    var extractedFilename = x.Extract(packRootPath, true);
                    if (System.IO.File.Exists(extractedFilename))
                    {
                        ++fileCount;
                        installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"{FormatActionVerb(FileStorage.CompareAction.New)} File={extractedFilename}");
                    }
                    else
                    {
                        installLog.Add(Logging.LogEntryType.Error, LogSourceId, $"Unable to extract file. File={x.RootFilename}");
                    }
                });
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Files: Added={fileCount}");
            }

            // add to all packs
            allInstalledPacks.Add(new InstalledPackage(packRootPath, package));
        }

        private void UpdatePackage(Logging.Logs installLog,
                                   Packaging.IPackageProvider packProvider,
                                   Packaging.IPackage package,
                                   List<IInstalledPackage> allInstalledPacks,
                                   InstallationSettings installationSettings)
        {
            // update package before installing
            if (installationSettings.UpdatePackagesBeforeInstalling)
            {
                UpdateActualPackage(installLog, packProvider, package);
            }
            // update package
            var packRootPath = GetInstallPath(package);

            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Updating files from package {package.FullyQualifiedPackageName}");

            // check for packRootPath existence
            if (!System.IO.Directory.Exists(packRootPath))
            {
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Creating directory={packRootPath}");
                FileStorage.FileStorageHelper.CreateDirectory(packRootPath);
            }
            else
            {
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Using existing directory={packRootPath}");
            }

            var reportInfo = new int[] { 0, 0, 0, 0, 0 };
            using (var connectedPackage = packProvider.Connect(package.RootFilename))
            {
                var cancelToken = new System.Threading.CancellationToken();
                var report = FileStorage.FileStorageHelper.Compare(connectedPackage.FileStore, packRootPath, cancelToken, (val) => { });
                foreach (var item in report)
                {
                    switch (item.Action)
                    {
                        case FileStorage.CompareAction.Unknown:
                            break;
                        case FileStorage.CompareAction.Ok:
                            if (item.ItemType == FileStorage.CompareType.File)
                            {
                                var path = item.DestinationFullPath;
                                if (string.IsNullOrWhiteSpace(path))
                                {
                                    path = System.IO.Path.Combine(InstallationPath, item.SourceFullPath);
                                }
                                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"{FormatActionVerb(item.Action)} {item.ItemType}={path}");
                                reportInfo[0]++;
                            }
                            break;
                        case FileStorage.CompareAction.New:
                            reportInfo[1]++;
                            break;
                        case FileStorage.CompareAction.Old:
                            // report all 'old' as 'ok'
                            reportInfo[0]++;
                            break;
                        case FileStorage.CompareAction.Update:
                            reportInfo[3]++;
                            break;
                        case FileStorage.CompareAction.Remove:
                            reportInfo[4]++;
                            break;
                        default:
                            break;
                    }
                }
                FileStorage.FileStorageHelper.MirrorSourceToDestination(report, connectedPackage.FileStore, packRootPath, (cr, val) =>
                {
                    var path = cr.DestinationFullPath;
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        path = System.IO.Path.Combine(InstallationPath, cr.SourceFullPath);
                    }
                    // output log entry
                    if (cr.DestinationLastModifiedTimeUtc > DateTime.MinValue)
                    {
                        installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"{FormatActionVerb(cr.Action)} {cr.ItemType}={path}, source={cr.SourceLastModifiedTimeUtc}, destination={cr.DestinationLastModifiedTimeUtc}, difference={cr.DestinationLastModifiedTimeUtc - cr.SourceLastModifiedTimeUtc}");
                    }
                    else
                    {
                        installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"{FormatActionVerb(cr.Action)} {cr.ItemType}={path}");
                    }
                });
            }
            var reportStr = new StringBuilder("Files: ");
            if (reportInfo[0] > 0)
            {
                reportStr.Append($"OK={reportInfo[0]}, ");
            }
            if (reportInfo[4] > 0)
            {
                reportStr.Append($"Removed={reportInfo[4]}, ");
            }
            if (reportInfo[1] > 0)
            {
                reportStr.Append($"Added={reportInfo[1]}, ");
            }
            if (reportInfo[2] > 0)
            {
                reportStr.Append($"Old={reportInfo[2]}, ");
            }
            if (reportInfo[3] > 0)
            {
                reportStr.Append($"Updated={reportInfo[3]}, ");
            }
            reportStr.Length -= 2;
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, reportStr.ToString());
        }

        private void RemovePackage(Logging.Logs installLog,
                                   Packaging.IPackage package)
        {
            var packRootPath = GetInstallPath(package);

            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Removing files and directory for package {package.FullyQualifiedPackageName}");

            if (System.IO.Directory.Exists(packRootPath))
            {
                installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Deleting directory={packRootPath}");
                // 
                foreach (var item in System.IO.Directory.EnumerateFiles(packRootPath, "*", System.IO.SearchOption.AllDirectories))
                {
                    System.IO.File.SetAttributes(item, System.IO.FileAttributes.Normal);
                    FileStorage.FileStorageHelper.DeleteFile(item);
                }
                //
                FileStorage.FileStorageHelper.DeleteDirectory(packRootPath);
                Threading.ThreadingHelper.Sleep(250);
            }
        }

        private void UpdateActualPackage(Logging.Logs installLog,
                                         Packaging.IPackageProvider packProvider,
                                         Packaging.IPackage package)
        {
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Updating Package {package.FullyQualifiedPackageName}");
            using (var connectedPackage = packProvider.Connect(package.RootFilename))
            {
                // update package
                connectedPackage.FileStore.Save(true);
            }
        }

        private string FormatActionVerb(FileStorage.CompareAction action)
        {
            switch (action)
            {
                case FileStorage.CompareAction.Unknown:
                    return "Unknown";
                case FileStorage.CompareAction.Ok:
                    return "OK";
                case FileStorage.CompareAction.New:
                    return "Adding";
                case FileStorage.CompareAction.Old:
                    return "Old";
                case FileStorage.CompareAction.Update:
                    return "Updating";
                case FileStorage.CompareAction.Remove:
                    return "Removing";
            }
            return "Unknown";
        }

        // NOTE: this method honors the IsEnabled flag
        private void DependencyChain2(IInstalledPackage searchPack,
                                      Dictionary<string, IInstalledPackage> list,
                                      bool ignoreIsEnabledFlag)
        {
            // iterate dependency packages for the searchPack
            foreach (var dependencyPack in searchPack.Package.PackageConfiguration.DependencyPackages)
            {
                // get installed package for dependency pack
                var installedSearchPack = FindProperDependencyPackageInInstalledPackages(dependencyPack, ignoreIsEnabledFlag);
                if (installedSearchPack != null)
                {
                    AddToList(installedSearchPack);
                    DependencyChain2(installedSearchPack, list, ignoreIsEnabledFlag);
                }
                else
                {
                    try
                    {

                        var displayStr = $"{dependencyPack.Name}, v{dependencyPack.MinimumVersion}";
                        if (dependencyPack.SpecificVersion != null)
                        {
                            displayStr = displayStr + $", v{dependencyPack.SpecificVersion}";
                        }
                        var val = new InstalledPackage("ERROR",
                                                       new Packaging.Package(displayStr,
                                                                             $"{searchPack.Package.PackageConfiguration.Name}, v{searchPack.Package.PackageConfiguration.Version}",
                                                                             new Packaging.PackageConfiguration($"Missing Dependency for {searchPack.Package.PackageConfiguration.Name}",
                                                                                                                searchPack.Package.PackageConfiguration.Version,
                                                                                                                false,
                                                                                                                searchPack.Package.PackageConfiguration.Priority,
                                                                                                                false)));
                        AddToList(val);
                    }
                    catch { }
                }
            }

            // ######## Internal Methods ########
            void AddToList(IInstalledPackage item)
            {
                var key = $"{item.InstallPath}, {item.Package.ConfigurationFilename}{item.Package.RootFilename}";
                if (!list.ContainsKey(key))
                {
                    list.Add(key, item);
                }
            }
        }

        // NOTE: this method honors the IsEnabled flag
        private IInstalledPackage FindProperDependencyPackageInInstalledPackages(Packaging.IDependencyPackage dependencyPackage,
                                                                                 bool ignoreIsEnabledFlag)
        {
            // if specific version supplied then look for name, specificVersion
            // else look in HIGHESTVERSIONPACKAGES for name, >= minimumVersion
            // else look in ALLPACKAGES for name, >= minimumVersion

            IInstalledPackage found = null;
            if (ignoreIsEnabledFlag)
            {
                if (dependencyPackage.SpecificVersion != null)
                {
                    found = (from x in InstalledPackages
                             where ((x.Package.PackageConfiguration.Name == dependencyPackage.Name) &&
                                    (x.Package.PackageConfiguration.Version == dependencyPackage.SpecificVersion))
                             select x).FirstOrDefault();
                }
                if (found == null)
                {
                    found = (from x in InstalledHighestVersionsOnlyPackages
                             where ((x.Package.PackageConfiguration.Name == dependencyPackage.Name) &&
                                    (x.Package.PackageConfiguration.Version >= dependencyPackage.MinimumVersion))
                             select x).FirstOrDefault();
                    if (found == null)
                    {
                        found = (from x in InstalledPackages
                                 where ((x.Package.PackageConfiguration.Name == dependencyPackage.Name) &&
                                        (x.Package.PackageConfiguration.Version >= dependencyPackage.MinimumVersion))
                                 select x).FirstOrDefault();
                    }
                }
            }
            else
            {
                if (dependencyPackage.SpecificVersion != null)
                {
                    found = (from x in InstalledPackages
                             where ((x.Package.PackageConfiguration.Name == dependencyPackage.Name) &&
                                    (x.Package.PackageConfiguration.Version == dependencyPackage.SpecificVersion)
                                    && (x.Package.PackageConfiguration.IsEnabled))
                             select x).FirstOrDefault();
                }
                if (found == null)
                {
                    found = (from x in InstalledHighestVersionsOnlyPackages
                             where ((x.Package.PackageConfiguration.Name == dependencyPackage.Name) &&
                                    (x.Package.PackageConfiguration.Version >= dependencyPackage.MinimumVersion)
                                     && (x.Package.PackageConfiguration.IsEnabled))
                             select x).FirstOrDefault();
                    if (found == null)
                    {
                        found = (from x in InstalledPackages
                                 where ((x.Package.PackageConfiguration.Name == dependencyPackage.Name) &&
                                        (x.Package.PackageConfiguration.Version >= dependencyPackage.MinimumVersion)
                                     && (x.Package.PackageConfiguration.IsEnabled))
                                 select x).FirstOrDefault();
                    }
                }
            }
            return found;
        }

        private Logging.Logs RemoveOrphanedPackagesActual(IEnumerable<IInstalledPackage> installedOrphanedDependencyPackages, bool suppressLists, out int totalPackagesRemoved)
        {
            var startTime = DateTimeOffset.Now;
            totalPackagesRemoved = 0;
            var installLog = new Logging.Logs
            {
                { Logging.LogEntryType.Information, LogSourceId, $"Removing orphaned packages." },
                { Logging.LogEntryType.Information, LogSourceId, $"Install Path={InstallationPath}" }
            };
            if (!suppressLists)
            {
                // output all currently installed packages
                WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            }
            //
            var counter = 0;
            var maxLoopCount = 3;
            var startCount = InstalledPackages.Count();
            var orphansCount = installedOrphanedDependencyPackages.Count();
            while (orphansCount > 0)
            {
                // increment counter
                ++counter;
                // check maxLoopCount
                if (counter >= maxLoopCount)
                {
                    installLog.Add(Logging.LogEntryType.Warning, LogSourceId, $"Error:Remove Orphaned Packages, Maximum Loop Iteration Reached, Max Iteration={maxLoopCount}");
                    break;
                }
                else
                {
                    installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Remove orphaned packages, pass #{counter}");
                    foreach (var installedPack in installedOrphanedDependencyPackages)
                    {
                        installLog.Add(UninstallActual(installedPack, false, true));
                    }
                }
                // get orphan count
                orphansCount = installedOrphanedDependencyPackages.Count();
            }
            totalPackagesRemoved = startCount - InstalledPackages.Count();
            // output total packages removed
            var displayStr = $"Total packages processed: Removed={totalPackagesRemoved}";
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, displayStr);
            if (!suppressLists)
            {
                // output all currently installed packages
                WriteInstalledPackagesToInstallLog(installLog, LogSourceId);
            }
            // output completion message
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Completed removing orphaned packages.");
            installLog.Add(Logging.LogEntryType.Information, LogSourceId, $"Elapsed Time={DateTimeOffset.Now - startTime}");
            // 
            return installLog;
        }

        private string GetInstallPath(Packaging.IPackage package) => System.IO.Path.Combine(InstallationPath, package.PackageConfiguration.Id);
    }
    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Installation
{
    /// <summary>
    /// Defines methods to install and uninstall packages, iterate installed packages, find installed packages and read and write installed package configuration files.
    /// </summary>
    public interface IInstaller
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        string LogSourceId { get; }
        /// <summary>
        /// The path the Installer will install packages.
        /// </summary>
        string InstallationPath { get; }
        /// <summary>
        /// Get or set whether superfluous information is output to the log during installation.
        /// </summary>
        bool WriteVerboseInstallationLogs { get; set; }
        /// <summary>
        /// Returns <see cref="Logging.Logs"/> with information listing the installed packages.
        /// </summary>
        /// <param name="logSourceId">The log source id string to use when generating logs.</param>
        /// <returns>Logs listing all of the installed packages.</returns>
        Logging.Logs LogInstalledPackages(string logSourceId);
        /// <summary>
        /// Gets a collection of all installed packages.
        /// </summary>
        IEnumerable<IInstalledPackage> InstalledPackages { get; }
        /// <summary>
        /// Gets a collection of only the highest versions of the installed packages.
        /// </summary>
        IEnumerable<IInstalledPackage> InstalledHighestVersionsOnlyPackages { get; }
        /// <summary>
        /// Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>false</b>.
        /// </summary>
        IEnumerable<IInstalledPackage> InstalledNonDependencyPackages { get; }
        /// <summary>
        /// Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b>.
        /// </summary>
        IEnumerable<IInstalledPackage> InstalledDependencyPackages { get; }

        // FYI: A package can create a dependency to any other package; it does not matter whether the package is a non-dependency packages or not.
        //      Nothing is stopping you from depending on a non-dependency package.
        //      The concept of an orphan, however, only applies to a dependency package that is not referenced by any other package.

        /// <summary>
        /// Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b> and the package is not referenced by any other package.
        /// </summary>
        IEnumerable<IInstalledPackage> InstalledOrphanedDependencyPackages { get; }
        /// <summary>
        /// Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsEnabled"/> flag is <b>false</b>.
        /// </summary>
        IEnumerable<IInstalledPackage> InstalledDisabledPackages { get; }
        /// <summary>
        /// Gets a collection containing packages that reference the given <paramref name="referencedPackage"/>.
        /// </summary>
        /// <param name="referencedPackage">The package to search for in other package's dependency packages list.</param>
        /// <returns>A collection containing packages that reference the given <paramref name="referencedPackage"/>.</returns>
        IEnumerable<IInstalledPackage> InstalledPackagesReferencingPackage(IInstalledPackage referencedPackage);
        /// <summary>
        /// Gets a collection containing packages that are missing dependency packages.
        /// </summary>
        IEnumerable<IInstalledPackage> InstalledPackagesWithMissingDependencies { get; }
        /// <summary>
        /// Gets a collection containing packages that are missing dependency packages, plus the referencing <see cref="Packaging.IDependencyPackage"/>.
        /// </summary>
        IEnumerable<Tuple<IInstalledPackage, Packaging.IDependencyPackage>> InstalledPackagesWithMissingDependenciesEx { get; }
        /// <summary>
        /// Gets a collection containing enabled packages that are required for the <paramref name="searchPackage"/>, based on it's package dependencies and the available enabled packages.  
        /// </summary>
        /// <param name="searchPackage">The package to generate the dependency package collection for.</param>
        /// <returns>A collection containing packages that are required for the <paramref name="searchPackage"/>.</returns>
        IEnumerable<IInstalledPackage> DependencyChain(IInstalledPackage searchPackage);
        /// <summary>
        /// Searches for the installed package that matches the given <paramref name="package"/>'s <see cref="Packaging.IPackageConfiguration.Name"/> and <see cref="Packaging.IPackageConfiguration.Version"/>.
        /// </summary>
        /// <param name="package">The package to use for the search.</param>
        /// <returns>The installed package that matches the given <paramref name="package"/>.</returns>
        IInstalledPackage FindInstalledPackage(Packaging.IPackage package);
        /// <summary>
        /// Searches for the installed package that matches the given <paramref name="name"/> and <paramref name="version"/>.
        /// </summary>
        /// <param name="name">The package name to search for.</param>
        /// <param name="version">The package version to search for.</param>
        /// <returns>The installed package that matches the given <paramref name="name"/> and <paramref name="version"/>.</returns>
        IInstalledPackage FindInstalledPackage(string name, Version version);
        /// <summary>
        /// Gets a collection containing all versions of packages where the <see cref="Packaging.IPackageConfiguration.Name"/> equals <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the package to search for.</param>
        /// <returns>A collection containing all versions of packages where the <see cref="Packaging.IPackageConfiguration.Name"/> equals <paramref name="name"/>.</returns>
        IEnumerable<IInstalledPackage> FindInstalledPackages(string name);
        /// <summary>
        /// Will install the <paramref name="package"/> using packages from the <paramref name="packageProvider"/> using the given parameters.
        /// </summary>
        /// <param name="package">The package to install.</param>
        /// <param name="packageProvider">The source of the package and any dependency packages.</param>
        /// <param name="installationSettings">Determines the installation actions.</param>
        /// <returns>A log detailing the installation actions.</returns>
        Logging.Logs Install(Packaging.IPackage package,
                              Packaging.IPackageProvider packageProvider,
                              InstallationSettings installationSettings);
        /// <summary>
        /// Will install the <paramref name="package"/> using packages from the <paramref name="packageProvider"/> using the given parameters and returns a (success/failure) flag.
        /// </summary>
        /// <param name="package">The package to install.</param>
        /// <param name="packageProvider">The source of the package and any dependency packages.</param>
        /// <param name="installationSettings">Determines the installation actions.</param>
        /// <param name="installLog">A log detailing the installation actions.</param>
        /// <returns><b>True</b> indicates the installation proceeded without error; other <b>false</b> indicates an error occurred. See the <paramref name="installLog"/> for details.</returns>
        bool TryInstall(Packaging.IPackage package,
                        Packaging.IPackageProvider packageProvider,
                        InstallationSettings installationSettings,
                        out Logging.Logs installLog);
        /// <summary>
        /// Will uninstall the <paramref name="installedPackage"/>.
        /// </summary>
        /// <param name="installedPackage">The installed package to uninstall.</param>
        /// <param name="removeOrphanedPackages">If <b>true</b> the installer will remove orphaned packages after uninstall is complete.</param>
        /// <returns>A log detailing the uninstall actions.</returns>
        Logging.Logs Uninstall(IInstalledPackage installedPackage,
                                bool removeOrphanedPackages);
        /// <summary>
        /// Will remove all packages.
        /// </summary>
        /// <returns>A log detailing the removal actions.</returns>
        Logging.Logs UninstallAll();
        /// <summary>
        /// Removes all installed packages where the <see cref="Packaging.IPackageConfiguration.IsEnabled"/> flag is <b>false</b>.
        /// </summary>
        /// <returns>A log detailing the removal actions.</returns>
        Logging.Logs RemoveDisabledPackages();
        /// <summary>
        /// Removes all installed packages where the <see cref="Packaging.IPackageConfiguration.IsEnabled"/> flag is <b>false</b>.
        /// </summary>
        /// <param name="totalPackagesRemoved">The total number of packages removed.</param>
        /// <returns>A log detailing the removal actions.</returns>
        Logging.Logs RemoveDisabledPackages(out int totalPackagesRemoved);
        /// <summary>
        /// Removes all installed packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b> and the package is not referenced by any other package. 
        /// </summary>
        /// <returns>A log detailing the removal actions.</returns>
        Logging.Logs RemoveOrphanedPackages();
        /// <summary>
        /// Removes all installed packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b> and the package is not referenced by any other package. 
        /// </summary>
        /// <param name="totalPackagesRemoved">The total number of packages removed.</param>
        /// <returns>A log detailing the removal actions.</returns>
        Logging.Logs RemoveOrphanedPackages(out int totalPackagesRemoved);
        /// <summary>
        /// Will read and deserialize the configuration file from the target <paramref name="installedPackage"/>.
        /// </summary>
        /// <param name="installedPackage">The installed package to read the configuration file from.</param>
        /// <returns>The target's package configuration.</returns>
        Packaging.IPackageConfiguration ReadConfigurationFile(IInstalledPackage installedPackage);
        /// <summary>
        /// Will serialize and write the configuration file to the target <paramref name="installedPackage"/>.
        /// </summary>
        /// <param name="installedPackage">The installed package to write the configuration file to.</param>
        /// <param name="configuration">The package configuration to serialize and write to the configuration file.</param>
        void WriteConfigurationFile(IInstalledPackage installedPackage, Packaging.IPackageConfiguration configuration);
    }
}

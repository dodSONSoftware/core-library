using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Defines methods to create and connect to packages, iterate packages, find packages and read and write package configuration files.
    /// </summary>
    public interface IPackageProvider
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Provides the proper <see cref="FileStorage.ICompressedFileStore"/> to open packages.
        /// </summary>
        IPackageFileStoreProvider PackageFileStoreProvider { get; }
        /// <summary>
        /// Gets a collection of all packages.
        /// </summary>
        IEnumerable<IPackage> Packages { get; }
        /// <summary>
        /// Gets a collection of only the enabled, highest versions of any one package by name.
        /// </summary>
        IEnumerable<IPackage> PackagesEnabledHighestVersionsOnly { get; }
        /// <summary>
        ///  Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>false</b>.
        /// </summary>
        IEnumerable<IPackage> NonDependencyPackages { get; }
        /// <summary>
        ///  Gets a collection containing packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b>.
        /// </summary>
        IEnumerable<IPackage> DependencyPackages { get; }
        /// <summary>
        /// Will search for a package.
        /// </summary>
        /// <param name="name">The package name to search for.</param>
        /// <param name="version">The version to search for.</param>
        /// <returns>An <see cref="IPackage"/> representing the desired package; otherwise a <b>null</b>.</returns>
        IPackage FindPackage(string name, Version version);
        /// <summary>
        /// Will search for a set of packages by name.
        /// </summary>
        /// <param name="name">The packages name to search for.</param>
        /// <returns>A collection of packages with the same name.</returns>
        IEnumerable<IPackage> FindPackages(string name);
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
        IConnectedPackage Create(string packageRootFilename, string configurationFilename, IPackageConfiguration packageConfiguration, bool overwrite);
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
        IConnectedPackage Create(string packageRootFilename, IPackageConfiguration packageConfiguration, bool overwrite);
        /// <summary>
        /// Will create a new package using the supplied <paramref name="package"/>.
        /// </summary>
        /// <param name="package">The package to use to create the package.</param>
        /// <param name="overwrite"><b>True</b> to write-over the package file if it already exists, otherwise, <b>false</b>.</param>
        /// <returns>An <see cref="IConnectedPackage"/>, which represents the desired package.</returns>
        /// <remarks>
        /// See <see cref="ConnectedPackage"/> concerning proper handling of the <see cref="ConnectedPackage"/>.
        /// </remarks>
        IConnectedPackage Create(IPackage package, bool overwrite);
        /// <summary>
        /// Will connect to an existing package.
        /// </summary>
        /// <param name="packageRootFilename">The package filename.</param>
        /// <returns>An <see cref="IConnectedPackage"/>, which represents the desired package.</returns>
        /// <remarks>
        /// See <see cref="ConnectedPackage"/> concerning proper handling of the <see cref="ConnectedPackage"/>.
        /// </remarks>
        IConnectedPackage Connect(string packageRootFilename);
        /// <summary>
        /// Will connect to an existing package.
        /// </summary>
        /// <param name="packageRootFilename">The package filename.</param>
        /// <param name="configurationFilename">The filename to use when reading the <paramref name="configurationFilename"/> from the package.</param>
        /// <returns>An <see cref="IConnectedPackage"/>, which represents the desired package.</returns>
        /// <remarks>
        /// See <see cref="ConnectedPackage"/> concerning proper handling of the <see cref="ConnectedPackage"/>.
        /// </remarks>
        IConnectedPackage Connect(string packageRootFilename, string configurationFilename);
        /// <summary>
        /// Will read a configuration file from a package.
        /// </summary>
        /// <param name="package">The package to read the configuration file from.</param>
        /// <returns>The contents of the package configuration file deserialized into an <see cref="IPackageConfiguration"/> type.</returns>
        IPackageConfiguration ReadConfigurationFile(IPackage package);
        /// <summary>
        /// Will write a configuration file to a package.
        /// </summary>
        /// <param name="package">The package to write the configuration file to.</param>
        /// <param name="configuration">The package configuration to write.</param>
        void WriteConfigurationFile(IPackage package, IPackageConfiguration configuration);
    }
}

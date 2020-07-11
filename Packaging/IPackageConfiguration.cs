using System;
using System.Collections.Generic;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Defines a package configuration.
    /// </summary>
    public interface IPackageConfiguration
        : Configuration.IConfigurable
    {
        /// <summary>
        /// The unique identification for this package.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// The package name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The package version.
        /// </summary>
        Version Version { get; set; }
        /// <summary>
        /// Determines the load order of packages. The lower this value the sooner it will be started.
        /// </summary>
        double Priority { get; set; }
        /// <summary>
        /// Indicates whether this package is enabled.
        /// </summary>
        bool IsEnabled { get; set; }
        /// <summary>
        /// Indicates whether this is a dependency package.
        /// </summary>
        bool IsDependencyPackage { get; set; }
        /// <summary>
        /// A collection of packages this package depends upon.
        /// </summary>
        IList<IDependencyPackage> DependencyPackages { get; }
    }
}

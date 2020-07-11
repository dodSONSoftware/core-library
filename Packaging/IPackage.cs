using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Defines a single package.
    /// </summary>
    public interface IPackage
    {
        /// <summary>
        /// <para>Returns the full name of the package.</para>
        /// <para>In the form: "PackageName, PackageVersion, PackagePath"</para>
        /// </summary>
        string FullyQualifiedPackageName { get; }
        /// <summary>
        /// The package name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The package version.
        /// </summary>
        Version Version { get; }
        /// <summary>
        /// The package filename.
        /// </summary>
        string RootFilename { get; }
        /// <summary>
        /// The configuration filename.
        /// </summary>
        string ConfigurationFilename { get; }
        /// <summary>
        /// The package configuration.
        /// </summary>
        IPackageConfiguration PackageConfiguration { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Represents a single package.
    /// </summary>
    [Serializable]
    public class Package
         : IPackage
    {
        //#region Static Methods
        ///// <summary>
        ///// <para>Will parse a string and attempt to interpret it as a fully qualified package name.</para>
        ///// <para>The expected form: "PackageName, PackageVersion{, PackagePath}"; PackagePath is optional.</para>
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns>
        ///// <para>A <see cref="Tuple{T1, T2, T3}"/> containing the parts of the fully qualified package name.</para>
        ///// <para>Item1 = Package Name</para>
        ///// <para>Item2 = Package Version</para>
        ///// <para>Item3 = <b>null</b> or Package Path. (Item3 is optional.)</para>
        ///// </returns>
        //public static Tuple<string, string, string> ParseFullyQualifiedPackageName(string value)
        //{
        //    if (string.IsNullOrWhiteSpace(value)) { throw new ArgumentNullException(nameof(value), $"Parameter {nameof(value)} cannot be null or empty."); }
        //    var validChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.' };
        //    var parts = value.Split(',').ToList();
        //    switch (parts.Count)
        //    {
        //        case 0:
        //        case 1:
        //            throw new Exception($"Parameter {nameof(value)} not correctly formed. {nameof(value)}=\"{value}\". The string should follow this basic pattern: \"PackageName, PackageVersion{{, PackagePath}}\"; packagePath is optional.");
        //        case 2:
        //            return new Tuple<string, string, string>(parts[0], CleanVersionString(parts[1]), "");
        //        default:
        //            return new Tuple<string, string, string>(parts[0], CleanVersionString(parts[1]), CleanVersionString(parts[2]));
        //    }

        //    // #### Internal Functions ####
        //    string CleanVersionString(string sourceStr)
        //    {
        //        var result = new StringBuilder(16);
        //        foreach (var ch in value)
        //        {
        //            if (validChars.Contains(ch)) { result.Append(ch); }
        //        }
        //        return result.ToString();
        //    }
        //}
        //#endregion
        #region Ctor
        private Package()
        {
        }
        /// <summary>
        /// Instantiates a new package.
        /// </summary>
        /// <param name="packageRootFilename">The filename for this package.</param>
        /// <param name="configurationFileName">The filename of the configuration file contained in this package.</param>
        /// <param name="configuration">This package's configuration.</param>
        public Package(string packageRootFilename,
                       string configurationFileName,
                       IPackageConfiguration configuration)
            : this()
        {
            if (string.IsNullOrWhiteSpace(packageRootFilename))
            {
                throw new ArgumentNullException(nameof(packageRootFilename));
            }
            RootFilename = packageRootFilename;
            if (string.IsNullOrWhiteSpace(configurationFileName))
            {
                throw new ArgumentNullException(nameof(configurationFileName));
            }
            ConfigurationFilename = configurationFileName;
            PackageConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        #endregion
        #region IPackageInformation Methods
        /// <summary>
        /// Returns the full name of the package.
        /// <para>In the form: "PackageName, PackageVersion, PackageFullPath"</para>
        /// </summary>
        public string FullyQualifiedPackageName => $"({PackageConfiguration.Name}, v{PackageConfiguration.Version}, {RootFilename})";
        /// <summary>
        /// The package name.
        /// </summary>
        public string Name => PackageConfiguration.Name;
        /// <summary>
        /// The package version.
        /// </summary>
        public Version Version => PackageConfiguration.Version;
        /// <summary>
        /// The package filename.
        /// </summary>
        public string RootFilename
        {
            get; private set;
        }
        /// <summary>
        /// The configuration filename.
        /// </summary>
        public string ConfigurationFilename
        {
            get; private set;
        }
        /// <summary>
        /// The package configuration.
        /// </summary>
        public IPackageConfiguration PackageConfiguration
        {
            get; private set;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// Returns the <see cref="FullyQualifiedPackageName"/> for this package.
        /// </summary>
        /// <returns>Returns the <see cref="FullyQualifiedPackageName"/> for this package.</returns>
        public override string ToString() => FullyQualifiedPackageName;
        #endregion
    }
}

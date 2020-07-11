using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Represents a dependency package.
    /// </summary>
    [Serializable()]
    public class DependencyPackage
        : IDependencyPackage
    {
        #region Ctor
        private DependencyPackage() { }
        /// <summary>
        /// Instantiates a new instance of dependency package.
        /// </summary>
        /// <param name="name">The name of the dependency package.</param>
        /// <param name="minimumVersion">The minimum version required for this dependency package.</param>
        /// <param name="specificVersion">The specific version desired. The <see cref="Installation"/> types will attempt to supply this version if available.</param>
        public DependencyPackage(string name,
                                 Version minimumVersion,
                                 Version specificVersion)
            : this()
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException(nameof(name)); }
            Name = name;
            MinimumVersion = minimumVersion ?? throw new ArgumentNullException(nameof(minimumVersion));
            SpecificVersion = specificVersion; // ?? throw new ArgumentNullException(nameof(specificVersion));
        }
        /// <summary>
        /// Instantiates a new instance of dependency package without a specific version.
        /// </summary>
        /// <param name="name">The name of the dependency package.</param>
        /// <param name="minimumVersion">The minimum version required for this dependency package.</param>
        public DependencyPackage(string name,
                                 Version minimumVersion)
            : this(name, minimumVersion, null) { }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public DependencyPackage(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (!configuration.Key.StartsWith("DependencyPackage")) { throw new ArgumentException($"Wrong configuration. Configuration Key must begin with \"DependencyPackage\". Configuration Key={configuration.Key}", nameof(configuration)); }
            // Name
            var temp1 = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Name", typeof(string)).Value;    
            if (string.IsNullOrWhiteSpace(temp1)) { throw new Exception($"Configuration invalid information. Configuration item: \"Name\" cannot be empty."); }
            Name = temp1;
            // MinimumVersion
            MinimumVersion = (Version)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "MinimumVersion", typeof(Version)).Value;   
            // SpecificVersion
            try { SpecificVersion = (Version)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "SpecificVersion", typeof(Version)).Value; }
            catch { }
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The dependency package name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The minimum version required for the dependency package.
        /// </summary>
        public Version MinimumVersion { get; }
        /// <summary>
        /// <para>The specific version desired.</para>
        /// <para>Optional, if supplied, the <see cref="Installation"/> types will attempt to supply this version if available.</para>
        /// </summary>
        /// <remarks>
        /// Can be used to patch into alternative packages, when a newer or different dependency package breaks your package. To install multiple packages with the same name, but different versions, you will need to install it <see cref="Installation.InstallType.SideBySide"/>; See the <see cref="Installation"/> namespace for more information.
        /// </remarks>
        public Version SpecificVersion { get; set; }
        #endregion
        /// <summary>
        /// Returns <b>PackageName, PackageVersion{, PackagePath}</b>; packagePath is optional.
        /// </summary>
        /// <returns>Returns <b>PackageName, PackageVersion{, PackagePath}</b>; packagePath is optional.</returns>
        public override string ToString()
        {
            if (SpecificVersion == null) { return $"{Name}, {MinimumVersion}"; }
            else { return $"{Name}, {MinimumVersion}, {SpecificVersion}"; }
        }
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("DependencyPackage");
                // Name
                result.Items.Add("Name", Name, Name.GetType());
                // MinimumVersion
                result.Items.Add("MinimumVersion", MinimumVersion, MinimumVersion.GetType());
                if (SpecificVersion != null)
                {
                    // SpecificVersion
                    result.Items.Add("SpecificVersion", SpecificVersion, SpecificVersion.GetType());
                }
                // 
                return result;
            }
        }
        #endregion
    }
}

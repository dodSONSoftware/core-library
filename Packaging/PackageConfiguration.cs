using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Represents a package configuration.
    /// </summary>
    [Serializable()]
    public class PackageConfiguration
        : IPackageConfiguration
    {
        #region Ctor
        private PackageConfiguration()
        {
        }
        /// <summary>
        /// Instantiates a new package configuration.
        /// </summary>
        /// <param name="name">The package name.</param>
        /// <param name="version">The package version.</param>
        /// <param name="isEnabled">Indicates whether the package is enabled or not.</param>
        /// <param name="priority">Determines the load order of packages. The lower this value the sooner it will be started.</param>
        /// <param name="isDependencyPackage">Indicates whether the package is a dependency package or not.</param>
        /// <param name="dependencyPackages">A collection of dependency packages this package depends upon.</param>
        public PackageConfiguration(string name,
                                    Version version,
                                    bool isEnabled,
                                    double priority,
                                    bool isDependencyPackage,
                                    IList<IDependencyPackage> dependencyPackages)
            : this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            IsEnabled = isEnabled;
            Priority = priority;
            IsDependencyPackage = isDependencyPackage;
            _Version = version ?? throw new ArgumentNullException(nameof(version));
            if (dependencyPackages != null)
            {
                DependencyPackages = new List<IDependencyPackage>(dependencyPackages);
            }
        }
        /// <summary>
        /// Instantiates a new package configuration without any dependency packages.
        /// </summary>
        /// <param name="name">The package name.</param>
        /// <param name="version">The package version.</param>
        /// <param name="isEnabled">Indicates whether the package is enabled or not.</param>
        /// <param name="priority">Determines the load order of packages. The lower this value the sooner it will be started.</param>
        /// <param name="isDependencyPackage">Indicates whether the package is a dependency package or not.</param>
        public PackageConfiguration(string name,
                                    Version version,
                                    bool isEnabled,
                                    double priority,
                                    bool isDependencyPackage)
            : this(name, version, isEnabled, priority, isDependencyPackage, null) { }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public PackageConfiguration(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "PackageConfiguration")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"PackageConfiguration\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // Name
            var temp1 = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Name", typeof(string)).Value; //    
            if (string.IsNullOrWhiteSpace(temp1))
            {
                throw new Exception($"Configuration invalid information. Configuration item: \"Name\" cannot be empty.");
            }
            Name = temp1;
            // Version
            Version = (Version)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Version", typeof(Version)).Value;
            // IsEnabled
            IsEnabled = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "IsEnabled", typeof(bool)).Value;
            // Priority
            Priority = (double)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Priority", typeof(double)).Value;
            // IsDependencyPackage
            IsDependencyPackage = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "IsDependencyPackage", typeof(bool)).Value;
            // group: DependencyPackages
            if (configuration.ContainsKey("DependencyPackages"))
            {
                foreach (var item in configuration["DependencyPackages"])
                {
                    ((Configuration.IConfigurationGroupAdvanced)item).SetKey("DependencyPackage");
                    DependencyPackages.Add(new DependencyPackage(item));
                }
            }
        }
        #endregion
        #region Private Fields
        private Version _Version = new Version();
        #endregion
        #region Public Methods
        /// <summary>
        /// The unique identification for this package.
        /// </summary>
        public string Id
        {
            get
            {
                return $"{Name}_v{Version}";
            }
        }
        /// <summary>
        /// The package name.
        /// </summary>
        public string Name
        {
            get; private set;
        }
        /// <summary>
        /// The package version.
        /// </summary>
        public Version Version
        {
            get => _Version;
            set => _Version = value ?? throw new ArgumentNullException(nameof(value));
        }
        /// <summary>
        /// Determines the load order of packages. The lower this value the sooner it will be started.
        /// </summary>
        public double Priority
        {
            get; set;
        }
        /// <summary>
        /// Indicates whether this package is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        /// <summary>
        /// Indicates whether this is a dependency package.
        /// </summary>
        public bool IsDependencyPackage { get; set; } = false;
        /// <summary>
        /// A collection of packages this package depends upon.
        /// </summary>
        public IList<IDependencyPackage> DependencyPackages { get; } = new List<IDependencyPackage>();
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("PackageConfiguration");
                // Id
                result.Items.Add("Name", Name, Name.GetType());
                // Version
                result.Items.Add("Version", Version, Version.GetType());
                // IsEnabled
                result.Items.Add("IsEnabled", IsEnabled, IsEnabled.GetType());
                // Priority
                result.Items.Add("Priority", Priority, Priority.GetType());
                // IsDependencyPackage
                result.Items.Add("IsDependencyPackage", IsDependencyPackage, IsDependencyPackage.GetType());
                // group: DependencyPackages
                var dependencies = result.Add("DependencyPackages");
                var count = 0;
                foreach (var item in DependencyPackages)
                {
                    // clone the item, change its key, save clone to dependencies list
                    // you do not want to change the key in the original item
                    var clone = Core.Configuration.XmlConfigurationSerializer.Clone(item.Configuration);
                    ((Configuration.IConfigurationGroupAdvanced)clone).SetKey($"DependencyPackage {++count}");
                    dependencies.Add(clone);
                }
                //
                return result;
            }
        }
        #endregion
    }
}

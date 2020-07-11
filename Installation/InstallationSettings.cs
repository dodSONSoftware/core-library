using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Installation
{
    /// <summary>
    /// Provides settings needed to determine how to install packages.
    /// </summary>
    public class InstallationSettings
        : Configuration.IConfigurable
    {
        #region Ctor
        private InstallationSettings() { }

        /// <summary>
        /// Instantiates a new <see cref="InstallationSettings"/>.
        /// </summary>
        /// <param name="installType">The type of installation to perform.</param>
        /// <param name="cleanInstall">If <b>true</b> the installer will attempt to delete the target path before installing; otherwise, if <b>false</b> it will leave it.</param>
        /// <param name="enablePackageUpdates">If <b>true</b> the installer will update installed packages to mirror the packages in the package provider.</param>
        /// <param name="removeOrphanedPackages">If <b>true</b> the installer will remove orphaned packages after installation is complete.</param>
        /// <param name="updatePackagesBeforeInstalling">If <b>True</b> the installer will attempt to update packages before installing them. <b>False</b> will leave packages as is.</param>
        public InstallationSettings(InstallType installType,
                                    bool cleanInstall,
                                    bool enablePackageUpdates,
                                    bool removeOrphanedPackages,
                                    bool updatePackagesBeforeInstalling)
            : this()
        {
            InstallType = installType;
            CleanInstall = cleanInstall;
            EnablePackageUpdates = enablePackageUpdates;
            RemoveOrphanedPackages = removeOrphanedPackages;
            UpdatePackagesBeforeInstalling = updatePackagesBeforeInstalling;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public InstallationSettings(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "InstallationSettings") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"InstallationSettings\". Configuration Key={configuration.Key}", nameof(configuration)); }
            // InstallType

            // TODO: add support for using a typeof(string) instead of a typeof(Installation.InstallType) 
            //       this would allow for a simpler configuration file item: it would not have the full type name of the ENUM, just SYSTEM.STRING
            InstallType = (Installation.InstallType)Enum.Parse(typeof(Installation.InstallType), 
                                                               (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "InstallType", typeof(Installation.InstallType)).Value, 
                                                               true);
            
            // CleanInstall
            CleanInstall = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "CleanInstall", typeof(bool)).Value;
            // EnablePackageUpdates
            EnablePackageUpdates = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "EnablePackageUpdates", typeof(bool)).Value;
            // RemoveOrphanedPackages
            RemoveOrphanedPackages = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "RemoveOrphanedPackages", typeof(bool)).Value;
            // UpdatePackagesBeforeInstalling
            UpdatePackagesBeforeInstalling = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "UpdatePackagesBeforeInstalling", typeof(bool)).Value;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The type of installation to perform.
        /// </summary>
        public Installation.InstallType InstallType { get; }
        /// <summary>
        /// If <b>true</b> the installer will attempt to delete the target path before installing; otherwise, if <b>false</b> it will leave it.
        /// </summary>
        public bool CleanInstall { get; }
        /// <summary>
        /// If <b>true</b> the installer will add, update or delete installed files, as needed, to mirror the package.
        /// </summary>
        public bool EnablePackageUpdates { get; }
        /// <summary>
        /// If <b>true</b> the installer will remove orphaned packages after installation is complete.
        /// </summary>
        public bool RemoveOrphanedPackages { get; }
        /// <summary>
        /// If <b>True</b> the installer will attempt to update the packages before installing them. <b>False</b> will leave the packages as is.
        /// </summary>
        public bool UpdatePackagesBeforeInstalling { get; }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("InstallationSettings");
                //result.Items.Add("Type", this.GetType());

                // InstallType
                result.Items.Add("InstallType", InstallType, InstallType.GetType());
                // CleanInstall
                result.Items.Add("CleanInstall", CleanInstall, CleanInstall.GetType());
                // EnablePackageUpdates
                result.Items.Add("EnablePackageUpdates", EnablePackageUpdates, EnablePackageUpdates.GetType());
                // RemoveOrphanedPackages
                result.Items.Add("RemoveOrphanedPackages", RemoveOrphanedPackages, RemoveOrphanedPackages.GetType());
                // UpdatePackagesBeforeInstalling
                result.Items.Add("UpdatePackagesBeforeInstalling", UpdatePackagesBeforeInstalling, UpdatePackagesBeforeInstalling.GetType());
                // 
                return result;
            }
        }
        #endregion
    }
}

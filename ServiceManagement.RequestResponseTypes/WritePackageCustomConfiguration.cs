using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Writes the custom configuration file to the specified installed package.
    /// </summary>
    [Serializable]
    public class WritePackageCustomConfiguration
    {
        #region Ctor
        private WritePackageCustomConfiguration()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="WritePackageCustomConfiguration"/> with the given name, version and custom configuration.
        /// </summary>
        /// <param name="name">The name of the package to write the custom configuration to.</param>
        /// <param name="version">The version of the package to write the custom configuration to.</param>
        /// <param name="configuration">The <see cref="Configuration.IConfigurationGroup"/> to write to the custom configuration file. Can be null; null will delete the custom configuration file.</param>
        public WritePackageCustomConfiguration(string name,
                                               Version version,
                                               Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            //
            Version = version ?? throw new ArgumentNullException(nameof(version));
            //
            Configuration = configuration;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The name of the package to write the custom configuration to.
        /// </summary>
        public string Name
        {
            get;
        }
        /// <summary>
        /// The version of the package to write the custom configuration to.
        /// </summary>
        public Version Version
        {
            get;
        }
        /// <summary>
        /// The <see cref="Configuration.IConfigurationGroup"/> to write to the custom configuration file. Can be null; null will delete the custom configuration file.
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get;
        }
        /// <summary>
        /// Indicates whether the <see cref="Configuration"/> is null or not. <b>True</b> indicates the <see cref="Configuration"/> has a value; otherwise, <b>false</b> means the <see cref="Configuration"/> is null.
        /// </summary>
        public bool HasConfiguration => (Configuration != null);
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Name={Name}, Version={Version}, Has Custom Configuration={HasConfiguration}";
        #endregion
    }
}
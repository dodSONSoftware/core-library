using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Reads the custom configuration file from the specified package.
    /// </summary>
    [Serializable]
    public class ReadPackageCustomConfiguration
    {
        #region Ctor
        private ReadPackageCustomConfiguration()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ReadPackageCustomConfiguration"/> with the given name and version.
        /// </summary>
        /// <param name="name">The name of the package to read the custom configuration from.</param>
        /// <param name="version">The version of the package to read the custom configuration from.</param>
        public ReadPackageCustomConfiguration(string name,
                                              Version version)
            : this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            //
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The name of the package to read the custom configuration from.
        /// </summary>
        public string Name
        {
            get;
        }
        /// <summary>
        /// The version of the package to read the custom configuration from.
        /// </summary>
        public Version Version
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Name={Name}, Version={Version}";
        #endregion
    }
}
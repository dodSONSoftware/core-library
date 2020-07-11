using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information to either enable or disable the specified package.
    /// </summary>
    [Serializable]
    public class EnableDisablePackage
    {
        #region Ctor
        private EnableDisablePackage()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="EnableDisablePackage"/> with the given name and version.
        /// </summary>
        /// <param name="isEnabled">The state of enable or disable to set the specified package. <b>True</b> will enable the specified package; otherwise, <b>false</b> will disable the specified package.</param>
        /// <param name="name">The name of the package to enable or disable.</param>
        /// <param name="version">The version of the package to enable or disable.</param>
        public EnableDisablePackage(bool isEnabled, 
                                    string name,
                                    Version version)
            : this()
        {
            IsEnabled = isEnabled;
            //
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
        /// The state of enable or disable to set the specified package.
        /// </summary>
        public bool IsEnabled
        {
            get; 
        }
        /// <summary>
        /// The name of the package to enable or disable.
        /// </summary>
        public string Name
        {
            get;
        }
        /// <summary>
        /// The version of the package to enable or disable.
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
        public override string ToString() => $"IsEnabled={IsEnabled}, Name={Name}, Version={Version}";
        #endregion
    }
}

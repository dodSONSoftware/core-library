using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains the custom configuration for the specified package.
    /// </summary>
    [Serializable]
    public class ReadPackageCustomConfigurationResponse
    {
        #region Ctor
        private ReadPackageCustomConfigurationResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ReadPackageCustomConfigurationResponse"/> with the given package and custom configuration.
        /// </summary>
        /// <param name="request">The package to get the custom configuration from.</param>
        /// <param name="configuration">The custom configuration for the package.</param>
        public ReadPackageCustomConfigurationResponse(ReadPackageCustomConfiguration request,
                                                      Configuration.IConfigurationGroup configuration)
            : this()
        {
            SourcePackage = request ?? throw new ArgumentNullException(nameof(request));
            Configuration = configuration;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The package to get the custom configuration from.
        /// </summary>
        public ReadPackageCustomConfiguration SourcePackage
        {
            get;
        }
        /// <summary>
        /// The custom configuration for the package.
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
        public override string ToString() => $"Name={SourcePackage.Name}, Version={SourcePackage.Version}, Has Custom Configuration={HasConfiguration}";
        #endregion
    }
}

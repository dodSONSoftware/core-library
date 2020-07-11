using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains the custom configuration for the specified installed package.
    /// </summary>
    [Serializable]
    public class ReadInstalledCustomConfigurationResponse
    {
        #region Ctor
        private ReadInstalledCustomConfigurationResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ReadInstalledCustomConfigurationResponse"/> with the given installed package and custom configuration.
        /// </summary>
        /// <param name="request">The installed package to get the custom configuration from.</param>
        /// <param name="configuration">The custom configuration for the installed package.</param>
        public ReadInstalledCustomConfigurationResponse(ReadInstalledCustomConfiguration request,
                                                        Configuration.IConfigurationGroup configuration)
            : this()
        {
            SourcePackage = request ?? throw new ArgumentNullException(nameof(request));
            Configuration = configuration;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The installed package to get the custom configuration from.
        /// </summary>
        public ReadInstalledCustomConfiguration SourcePackage
        {
            get;
        }
        /// <summary>
        /// The custom configuration for the installed package.
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

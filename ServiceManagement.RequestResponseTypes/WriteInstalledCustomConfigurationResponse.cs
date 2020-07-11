using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains the results of writing the custom configuration to the specified installed package.
    /// </summary>
    [Serializable]
    public class WriteInstalledCustomConfigurationResponse
    {
        #region Ctor
        private WriteInstalledCustomConfigurationResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="WriteInstalledCustomConfigurationResponse"/> with the given installed package and custom configuration.
        /// </summary>
        /// <param name="request">The installed package to write the custom configuration to.</param>
        /// <param name="results">Indicates whether the write operation was successful or not. <b>True</b> indicates the custom configuration file was written successfully; otherwise, <b>false</b> indicates a failure.</param>
        /// <param name="reason">The reason for the success or failure of the write operation.</param>
        public WriteInstalledCustomConfigurationResponse(WriteInstalledCustomConfiguration request,
                                                         bool results,
                                                         string reason)
            : this()
        {
            SourcePackage = request ?? throw new ArgumentNullException(nameof(request));
            Results = results;
            Reason = reason;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The installed package to write the custom configuration to.
        /// </summary>
        public WriteInstalledCustomConfiguration SourcePackage
        {
            get;
        }
        /// <summary>
        /// Indicates whether the write operation was successful or not. <b>True</b> indicates the custom configuration file was written successfully; otherwise, <b>false</b> indicates a failure.
        /// </summary>
        public bool Results
        {
            get;
        }
        /// <summary>
        /// The reason for the success or failure of the write operation.
        /// </summary>
        public string Reason
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Name={SourcePackage.Name}, Version={SourcePackage.Version}, Results={Results}, Reason={Reason}";
        #endregion
    }
}

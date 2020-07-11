using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains the results of writing the custom configuration to the specified package.
    /// </summary>
    [Serializable]
    public class WritePackageCustomConfigurationResponse
    {
        #region Ctor
        private WritePackageCustomConfigurationResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="WritePackageCustomConfigurationResponse"/> with the given package and custom configuration.
        /// </summary>
        /// <param name="request">The package to write the custom configuration to.</param>
        /// <param name="results">Indicates whether the write operation was successful or not. <b>True</b> indicates the custom configuration file was written successfully; otherwise, <b>false</b> indicates a failure.</param>
        /// <param name="reason">The reason for the success or failure of the write operation.</param>
        public WritePackageCustomConfigurationResponse(WritePackageCustomConfiguration request,
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
        /// The package to write the custom configuration to.
        /// </summary>
        public WritePackageCustomConfiguration SourcePackage
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Requests information about a specific folder.
    /// </summary>
    [Serializable]
    public class GetFolderInformationRequest
    {
        #region Ctor
        private GetFolderInformationRequest()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="GetFolderInformationRequest"/> object with the given parameters.
        /// </summary>
        /// <param name="folderName">The name of the folder to retrieve information about.</param>
        public GetFolderInformationRequest(string folderName)
            : this()
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }
            FolderName = folderName;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The name of the folder to retrieve information about.
        /// </summary>
        public string FolderName
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Folder={FolderName}";
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Conveys information about a specific folder.
    /// </summary>
    [Serializable]
    public class GetFolderInformationResponse
    {
        #region Ctor
        private GetFolderInformationResponse()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="GetFolderInformationResponse"/> object with the given parameters.
        /// </summary>
        /// <param name="folderName">The name of the folder this information is about.</param>
        /// <param name="folders">A collection of sub-folders in this <see cref="FolderName"/>.</param>
        /// <param name="files">A collection of files in this <see cref="FolderName"/>.</param>
        public GetFolderInformationResponse(string folderName,
                                            string[] folders,
                                            RequestResponseTypes.FileInformation[] files)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }
            FolderName = folderName;
            //
            if (folders != null)
            {
                Folders = folders;
            }
            else
            {
                Folders = new string[0];
            }
            //
            if (files != null)
            {
                Files = files;
            }
            else
            {
                Files = new FileInformation[0];
            }
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
        /// <summary>
        /// A collection of sub-folders in this <see cref="FolderName"/>.
        /// </summary>
        public IEnumerable<string> Folders
        {
            get;
        }
        /// <summary>
        /// A collection of files in this <see cref="FolderName"/>.
        /// </summary>
        public IEnumerable<RequestResponseTypes.FileInformation> Files
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"FolderName={FolderName}, Folders={Folders.Count()}, Files={Files.Count()}";
        #endregion
    }
}

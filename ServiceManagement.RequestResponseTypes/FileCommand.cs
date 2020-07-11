using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Attempts to copy, move or delete a file.
    /// </summary>
    [Serializable]
    public class FileCommand
    {
        #region Ctor
        private FileCommand()
        {
        }
        /// <summary>
        /// Instantiates a new instance with the given parameters.
        /// </summary>
        /// <param name="sourceFilename">The name of the file to perform the operation on.</param>
        /// <param name="destinationFilename">The name of the file created after a copy or move.</param>
        /// <param name="deleteSourceFile">Determines if the <see cref="SourceFilename"/> should be deleted.</param>
        public FileCommand(string sourceFilename,
                           string destinationFilename,
                           bool deleteSourceFile)
            : this()
        {
            if (string.IsNullOrWhiteSpace(sourceFilename))
            {
                throw new ArgumentNullException(nameof(sourceFilename));
            }
            SourceFilename = sourceFilename;
            //
            DestinationFilename = destinationFilename;
            //
            DeleteSourceFile = deleteSourceFile;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The name of the file to perform the operation on.
        /// </summary>
        public string SourceFilename
        {
            get;
        }
        /// <summary>
        /// The name of the file to copy or move to.
        /// </summary>
        public string DestinationFilename
        {
            get;
        }
        /// <summary>
        /// Determines if the <see cref="SourceFilename"/> should be deleted.
        /// </summary>
        public bool DeleteSourceFile
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"SourceFilename={SourceFilename}, DestinationFilename={DestinationFilename}, DeleteSourceFile={DeleteSourceFile}";
        #endregion
    }
}

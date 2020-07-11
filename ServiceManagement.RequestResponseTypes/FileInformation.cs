using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about a single file.
    /// </summary>
    [Serializable]
    public class FileInformation
    {
        #region Ctor
        private FileInformation()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="FileInformation"/> object with the given parameters.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <param name="length">The length of the file, in bytes.</param>
        /// <param name="dateCreatedUtc">The date the file was created.</param>
        /// <param name="dateLastModifiedUtc">The date the file was last modified.</param>
        public FileInformation(string name,
                               long length,
                               DateTimeOffset dateCreatedUtc,
                               DateTimeOffset dateLastModifiedUtc)
            : this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            //
            if (length < 0)
            {
                throw new ArgumentException($"Parameter length must be greater than zero.", nameof(length));
            }
            Length = length;
            //
            DateCreatedUtc = dateCreatedUtc;
            DateLastModifiedUtc = dateLastModifiedUtc;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string Name
        {
            get;
        }
        /// <summary>
        /// The length of the file, in bytes.
        /// </summary>
        public long Length
        {
            get;
        }
        /// <summary>
        /// The date the file was created.
        /// </summary>
        public DateTimeOffset DateCreatedUtc
        {
            get;
        }
        /// <summary>
        /// The date the file was last modified.
        /// </summary>
        public DateTimeOffset DateLastModifiedUtc
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Name={Name}, Length={Length}, DateCreatedUtc={DateCreatedUtc}, DateLastModifiedUtc={DateLastModifiedUtc}";
        #endregion
    }
}

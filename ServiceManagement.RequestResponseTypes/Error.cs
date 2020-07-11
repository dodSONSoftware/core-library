using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information related to a problem.
    /// </summary>
    [Serializable]
    public class Error
    {
        #region Ctor
        /// <summary>
        /// The default constructor.
        /// </summary>
        private Error()
        {
        }
        /// <summary>
        /// Creates a <see cref="Error"/> with the given parameters.
        /// </summary>
        /// <param name="description">Text describing the reason for the error.</param>
        public Error(string description)
            : this()
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }
            Description = description;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Text describing the reason for the error.
        /// </summary>
        public string Description
        {
            get; set;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"{Description}";
        #endregion
    }
}

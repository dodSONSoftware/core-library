using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information related to canceling an operation.
    /// </summary>
    [Serializable]
    public class Cancel
    {
        #region Ctor
        /// <summary>
        /// The default constructor.
        /// </summary>
        private Cancel()
        {
        }
        /// <summary>
        /// Creates a <see cref="Cancel"/> with the given parameters.
        /// </summary>
        /// <param name="description">Text describing the reason for the cancellation.</param>
        public Cancel(string description)
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
        /// Text describing the reason for the cancellation.
        /// </summary>
        public string Description
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Cancel, {Description}";
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Indicates whether the Put File Command was successful of not.
    /// </summary>
    [Serializable]
    public class PutFileComplete
    {
        #region Ctor
        private PutFileComplete()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="PutFileComplete"/> object with the given parameters.
        /// </summary>
        /// <param name="transferSuccessful">Indicates whether the file transfer was successful or not.</param>
        /// <param name="description">Details the results of the file transfer.</param>
        /// <param name="elapsedTime">The amount of time spent transferring the file.</param>
        public PutFileComplete(bool transferSuccessful,
                               string description,
                               TimeSpan elapsedTime)
            : this()
        {
            TransferSuccessful = transferSuccessful;
            Description = description;
            ElapsedTime = elapsedTime;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Indicates whether the file transfer was successful or not.
        /// </summary>
        public bool TransferSuccessful
        {
            get;
        }
        /// <summary>
        /// Details the results of the file transfer.
        /// </summary>
        public string Description
        {
            get;
        }
        /// <summary>
        /// The amount of time spent transferring the file.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"TransferSuccessful={TransferSuccessful}, Description={Description}, ElapsedTime={ElapsedTime}";
        #endregion
    }
}

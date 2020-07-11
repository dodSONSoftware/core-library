using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Used to search for <see cref="IComponent"/>s which supports a <see cref="IComponent.ComponentDesignation"/>.
    /// </summary>
    [Serializable]
    public class SupportsComponentDesignationRequest
    {
        #region Ctor
        private SupportsComponentDesignationRequest()
        {
        }
        /// <summary>
        /// Creates a new <see cref="SupportsComponentDesignationRequest"/> with the given <paramref name="componentDesignation"/> to search for.
        /// </summary>
        /// <param name="componentDesignation">The <see cref="IComponent.ComponentDesignation"/> to search for.</param>
        /// <param name="processId">The unique id for this operation.</param>
        public SupportsComponentDesignationRequest(string componentDesignation,
                                                   string processId)
            : this()
        {
            if (string.IsNullOrWhiteSpace(componentDesignation))
            {
                throw new ArgumentNullException(componentDesignation);
            }
            ComponentDesignation = componentDesignation;
            //
            if (string.IsNullOrWhiteSpace(processId))
            {
                throw new ArgumentNullException(processId);
            }
            ProcessId = processId;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The <see cref="IComponent.ComponentDesignation"/> to search for.
        /// </summary>
        public string ComponentDesignation
        {
            get;
        }
        /// <summary>
        /// The unique id for this operation.
        /// </summary>
        public string ProcessId
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"ComponentDesignation={ComponentDesignation}";
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Returns information about <see cref="IComponent"/>s that support the given <see cref="IComponent.ComponentDesignation"/>.
    /// </summary>
    [Serializable]
    public class SupportsComponentDesignationResponse
    {
        #region Ctor
        private SupportsComponentDesignationResponse()
        {
        }
        /// <summary>
        /// Creates a new <see cref="SupportsComponentDesignationResponse"/> with the given <paramref name="componentDesignation"/> that was searched for.
        /// </summary>
        /// <param name="componentDesignation">The <see cref="IComponent.ComponentDesignation"/> that was searched for.</param>
        /// <param name="componentName">The human-friendly name for this component. It should be unique for each <see cref="IComponent"/> to help differentiated between <see cref="IComponent"/>s with the same <paramref name="componentDesignation"/>.</param>
        /// <param name="componentId">The <see cref="IComponent.Id"/> for this component.</param>
        /// <param name="processId">The unique id for this operation.</param>
        public SupportsComponentDesignationResponse(string componentDesignation,
                                                    string componentName,
                                                    string componentId,
                                                    string processId)
            : this()
        {
            if (string.IsNullOrWhiteSpace(componentDesignation))
            {
                throw new ArgumentNullException(componentDesignation);
            }
            ComponentDesignation = componentDesignation;
            //
            if (string.IsNullOrWhiteSpace(componentName))
            {
                throw new ArgumentNullException(componentName);
            }
            ComponentName = componentName;
            //
            if (string.IsNullOrWhiteSpace(componentId))
            {
                throw new ArgumentNullException(componentId);
            }
            ComponentId = componentId;
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
        /// The <see cref="IComponent.ComponentDesignation"/> that was searched for.
        /// </summary>
        public string ComponentDesignation
        {
            get;
        }
        /// <summary>
        /// The human-friendly name for this component. It should be unique for each <see cref="IComponent"/> to help differentiated between <see cref="IComponent"/>s with the same <see cref="ComponentDesignation"/>.
        /// </summary>
        public string ComponentName
        {
            get;
        }
        /// <summary>
        /// The <see cref="IComponent.Id"/> for this component.
        /// </summary>
        public string ComponentId
        {
            get;
        }
        /// <summary>
        /// The client id of the <see cref="IComponent"/> that supports the <see cref="ComponentDesignation"/>.
        /// </summary>
        public string ClientId
        {
            get;
            // see [ dodSON.Core.ComponentManagement.ComponentExtensionBase.OnStart() ] and [ dodSON.Core.ComponentManagement.ComponentPluginBase.OnStart() ] for code that sets this value.
            internal set;
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
        public override string ToString() => $"ComponentDesignation={ComponentDesignation}, ComponentName={ComponentName}, ComponentId={ComponentId}, ClientId={ClientId}";
        #endregion
    }
}

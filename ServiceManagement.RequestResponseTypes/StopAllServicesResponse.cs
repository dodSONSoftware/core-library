using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about stopping all services.
    /// </summary>
    [Serializable]
    public class StopAllServicesResponse
    {
        #region Ctor
        private StopAllServicesResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="StopAllServicesResponse"/> with the given information.
        /// </summary>
        /// <param name="beforeExtensionCount">The number of extensions running before stopping.</param>
        /// <param name="beforePluginCount">The number of plugins running before stopping.</param>
        /// <param name="afterExtensionCount">The number of extensions running after stopping.</param>
        /// <param name="afterPluginCount">The number of plugins running after stopping.</param>
        public StopAllServicesResponse(int beforeExtensionCount,
                                       int beforePluginCount,
                                       int afterExtensionCount,
                                       int afterPluginCount)
            : this()
        {
            BeforeExtensionCount = beforeExtensionCount;
            BeforePluginCount = beforePluginCount;
            AfterExtensionCount = afterExtensionCount;
            AfterPluginCount = afterPluginCount;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The number of extensions running before stopping.
        /// </summary>
        public int BeforeExtensionCount
        {
            get;
        }
        /// <summary>
        /// The number of extensions running after stopping.
        /// </summary>
        public int AfterExtensionCount
        {
            get;
        }
        /// <summary>
        /// The number of plugins running before stopping.
        /// </summary>
        public int BeforePluginCount
        {
            get;
        }
        /// <summary>
        /// The number of plugins running after stopping.
        /// </summary>
        public int AfterPluginCount
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Extension Count: Before={BeforeExtensionCount}, After={AfterExtensionCount}, Plugin Count: Before={BeforePluginCount}, After={AfterPluginCount}";
        #endregion
    }
}

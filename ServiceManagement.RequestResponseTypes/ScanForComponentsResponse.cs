using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about scanning and starting new extensions and plugins.
    /// </summary>
    [Serializable]
    public class ScanForComponentsResponse
    {
        #region Ctor
        private ScanForComponentsResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ScanForComponentsResponse"/> with the given information.
        /// </summary>
        /// <param name="beforeExtensionCount">The number of extensions running before scanning.</param>
        /// <param name="beforePluginCount">The number of plugins running before scanning.</param>
        /// <param name="afterExtensionCount">The number of extensions running after scanning.</param>
        /// <param name="afterPluginCount">The number of plugins running after scanning.</param>
        public ScanForComponentsResponse(int beforeExtensionCount,
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
        /// The number of extensions running before scanning.
        /// </summary>
        public int BeforeExtensionCount
        {
            get;
        }
        /// <summary>
        /// The number of extensions running after scanning.
        /// </summary>
        public int AfterExtensionCount
        {
            get;
        }
        /// <summary>
        /// The number of plugins running before scanning.
        /// </summary>
        public int BeforePluginCount
        {
            get;
        }
        /// <summary>
        /// The number of plugins running after scanning.
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

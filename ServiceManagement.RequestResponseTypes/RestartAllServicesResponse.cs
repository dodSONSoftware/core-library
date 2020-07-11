using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about restarting all services.
    /// </summary>
    [Serializable]
    public class RestartAllServicesResponse
    {
        #region Ctor
        private RestartAllServicesResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="RestartAllServicesResponse"/> with the given information.
        /// </summary>
        /// <param name="totalExtensions">The total number of extensions.</param>
        /// <param name="totalPlugins">The total number of plugins.</param>
        /// <param name="beforeExtensionCount">The number of extensions running before restarting all.</param>
        /// <param name="beforePluginCount">The number of plugins running before restarting all.</param>
        /// <param name="afterExtensionCount">The number of extensions running after restarting all.</param>
        /// <param name="afterPluginCount">The number of plugins running after restarting all.</param>
        public RestartAllServicesResponse(int totalExtensions,
                                          int totalPlugins,
                                          int beforeExtensionCount,
                                          int beforePluginCount,
                                          int afterExtensionCount,
                                          int afterPluginCount)
            : this()
        {
            TotalExtensionsCount = totalExtensions;
            TotalPluginsCount = totalPlugins;
            BeforeExtensionCount = beforeExtensionCount;
            BeforePluginCount = beforePluginCount;
            AfterExtensionCount = afterExtensionCount;
            AfterPluginCount = afterPluginCount;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The total number of extensions.
        /// </summary>
        public int TotalExtensionsCount
        {
            get;
        }
        /// <summary>
        /// The total number of plugins.
        /// </summary>
        public int TotalPluginsCount
        {
            get;
        }
        /// <summary>
        /// The number of extensions running before restarting all.
        /// </summary>
        public int BeforeExtensionCount
        {
            get;
        }
        /// <summary>
        /// The number of extensions running after restarting all.
        /// </summary>
        public int AfterExtensionCount
        {
            get;
        }
        /// <summary>
        /// The number of plugins running before restarting all.
        /// </summary>
        public int BeforePluginCount
        {
            get;
        }
        /// <summary>
        /// The number of plugins running after restarting all.
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
        public override string ToString() => $"Extension Count: Total={TotalExtensionsCount}, Before={BeforeExtensionCount}, After={AfterExtensionCount}, Plugin Count: Total={TotalPluginsCount}, Before={BeforePluginCount}, After={AfterPluginCount}";
        #endregion
    }
}

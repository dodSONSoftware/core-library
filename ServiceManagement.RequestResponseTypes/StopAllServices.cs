using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about the services being stopped.
    /// </summary>
    [Serializable]
    public class StopAllServices
    {
        #region Ctor
        private StopAllServices()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="StopAllServices"/> with the given information.
        /// </summary>
        /// <param name="includeExtensions">Indicates whether extensions should be stopped. <b>True</b> will attempt to stop all extensions; otherwise <b>false</b> will leave the extensions alone.</param>
        /// <param name="includePlugins">Indicates whether plugins should be stopped. <b>True</b> will attempt to stop all plugins; otherwise <b>false</b> will leave the plugins alone.</param>
        public StopAllServices(bool includeExtensions,
                               bool includePlugins)
            : this()
        {
            IncludeExtensions = includeExtensions;
            IncludePlugins = includePlugins;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Indicates whether extensions should be stopped. <b>True</b> will attempt to stop all extensions; otherwise <b>false</b> will leave the extensions alone.
        /// </summary>
        public bool IncludeExtensions
        {
            get;
        }
        /// <summary>
        /// Indicates whether plugins should be stopped. <b>True</b> will attempt to stop all plugins; otherwise <b>false</b> will leave the plugins alone.
        /// </summary>
        public bool IncludePlugins
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"IncludeExtensions={IncludeExtensions}, IncludePlugins={IncludePlugins}";
        #endregion
    }
}


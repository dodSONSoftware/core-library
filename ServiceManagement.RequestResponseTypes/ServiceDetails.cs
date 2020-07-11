using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains detailed information about a specific <see cref="IService"/>.
    /// </summary>
    [Serializable]
    public class ServiceDetails
    {
        #region Ctor
        private ServiceDetails()
        {
        }
        /// <summary>
        /// Creates a new <see cref="ServiceDetails"/> using the given <see cref="IService"/>.
        /// </summary>
        /// <param name="service">The <see cref="IService"/> to extract information from.</param>
        public ServiceDetails(IService service)
            : this()
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            // extract information
            Id = service.Id;
            FullyQualifiedName = service.FullyQualifiedName;
            PackageConfiguration = service.PackageConfiguration;
            ClientConfiguration = service.ClientConfiguration;
            CustomConfiguration = service.CustomConfiguration;
            DateLastStarted = service.DateLastStarted;
            DateLastStopped = service.DateLastStopped;
            LastRunDuration = service.LastRunDuration;
            OverallRunDuration = service.OverallRunDuration;
            StartCount = service.StartCount;
            StopCount = service.StopCount;
            InstallPath = service.InstallPath;
            IsRunning = service.IsRunning;
            LogSourceId = service.LogSourceId;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The id for this <see cref="IService"/>. This value should be unique.
        /// </summary>
        public string Id
        {
            get;
        }
        /// <summary>
        /// The fully-qualified name for this <see cref="IService"/>. This value should be unique. 
        /// </summary>
        public string FullyQualifiedName
        {
            get;
        }
        /// <summary>
        /// The <see cref="Packaging.IPackageConfiguration"/> for the package this component was located in.
        /// </summary>
        public Packaging.IPackageConfiguration PackageConfiguration
        {
            get;
        }
        /// <summary>
        /// The <see cref="Networking.IClientConfiguration"/> for this component.
        /// </summary>
        public Networking.IClientConfiguration ClientConfiguration
        {
            get;
        }
        /// <summary>
        /// A custom <see cref="Configuration.IConfigurationGroup"/> found in the installed package.
        /// </summary>
        public Configuration.IConfigurationGroup CustomConfiguration
        {
            get;
        }
        /// <summary>
        /// The date the <see cref="IService"/> was started.
        /// </summary>
        public DateTime DateLastStarted
        {
            get;
        }
        /// <summary>
        /// The date the <see cref="IService"/> was stopped.
        /// </summary>
        public DateTime DateLastStopped
        {
            get;
        }
        /// <summary>
        /// Returns whether the <see cref="IService"/> is running. Returns <b>true</b> if the <see cref="IService"/> is running; otherwise, <b>false</b>.
        /// </summary>
        public bool IsRunning
        {
            get;
        }
        /// <summary>
        /// Returns the total duration the <see cref="IService"/> has been running.
        /// </summary>
        /// <seealso cref="LastRunDuration"/>
        public TimeSpan OverallRunDuration
        {
            get;
        }
        /// <summary>
        /// Returns the duration the <see cref="IService"/> has been running since it was last started.
        /// </summary>
        /// <seealso cref="OverallRunDuration"/>
        public TimeSpan LastRunDuration
        {
            get;
        }
        /// <summary>
        /// Returns the number of times the <see cref="IService"/> has been started.
        /// </summary>
        public int StartCount
        {
            get;
        }
        /// <summary>
        /// Returns the number of times the <see cref="IService"/> has been stopped.
        /// </summary>
        public int StopCount
        {
            get;
        }
        /// <summary>
        /// The install directory path where this component's package has been installed.
        /// </summary>
        public string InstallPath
        {
            get;
        }
        /// <summary>
        /// The value used as the source id when creating log entries.
        /// </summary>
        public string LogSourceId
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Id={Id}, IsRunning={IsRunning}, DateLastStarted={DateLastStarted}, InstallPath={InstallPath}";
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Facilitates communication by servicing System Requests, querying <see cref="IService"/>s and returning System Responses.
    /// </summary>
    public interface IServiceManager
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Fired when a message is received.
        /// </summary>
        event EventHandler<Networking.MessageEventArgs> MessageBus;
        //
        /// <summary>
        /// A value which uniquely identifies a <see cref="IServiceManager"/>.
        /// </summary>
        string ServiceManagerId
        {
            get;
        }
        //
        /// <summary>
        /// Challenge evidence required to login and access a <see cref="IServiceManager"/>.
        /// </summary>
        byte[] LoginEvidence
        {
            get; set;
        }
        /// <summary>
        /// The maximum number of clients allowed to log in at any one time.
        /// </summary>
        int MaximumLoginsAllowed
        {
            get; set;
        }
        //
        /// <summary>
        /// The amount of time to wait before checking the cache for any items that can be purged.
        /// </summary>
        TimeSpan RequestsPurgeInterval
        {
            get;
        }
        /// <summary>
        /// The amount of time a cached item will remain in the cache before timing out and being purged.
        /// </summary>
        TimeSpan RequestsResponseTimeout
        {
            get;
        }
        /// <summary>
        /// The amount of time a session will remain in the cache, without activity, before timing out.
        /// </summary>
        TimeSpan SessionCacheTimeLimit
        {
            get;
        }
        /// <summary>
        /// If <b>true</b> all remaining cached requests will have their <see cref="Cache.ICacheProcessorItem.Process"/> executed; otherwise, setting this to <b>false</b> will terminate all remaining cached requests without executing their <see cref="Cache.ICacheProcessorItem.Process"/>.
        /// </summary>
        bool ExecuteRemainingRequestsOnStop
        {
            get; set;
        }
        //
        /// <summary>
        /// The <see cref="ComponentManagement.IComponentManager"/> to be serviced by this <see cref="IServiceManager"/>.
        /// </summary>
        ComponentManagement.IComponentManager ComponentManager
        {
            get;
        }
        //
        /// <summary>
        /// The amount of time to wait after, and between, starting services, stopping services, scanning for new services and restarting services.
        /// </summary>
        TimeSpan ServicesRestartDelay
        {
            get;
        }
        //
        /// <summary>
        /// Indicates the operating state of the <see cref="IServiceManager"/>.
        /// </summary>
        bool IsRunning
        {
            get;
        }
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="IServiceManager"/> started.
        /// </summary>
        DateTimeOffset StartTime
        {
            get;
        }
        /// <summary>
        /// Returns the amount of time this <see cref="IServiceManager"/> has been running.
        /// </summary>
        TimeSpan RunTime
        {
            get;
        }
        /// <summary>
        /// Will start the <see cref="ComponentManager"/>, if it is not already running, and connect to it's communication system.
        /// </summary>
        /// <param name="clearLogBeforeStarting">Determines if the logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        /// <param name="clearArchivedLogsBeforeStarting">Determines if the archived logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        /// <param name="exception">If <b>false</b> is returned, this value will hold the <see cref="Exception"/> that occurred.</param>
        /// <returns><b>True</b> if the operation was successful; otherwise <b>false</b>.</returns>
        bool TryStart(bool clearLogBeforeStarting, bool clearArchivedLogsBeforeStarting, out Exception exception);
        /// <summary>
        /// Will stop and restart the <see cref="ComponentManagement.IComponentManager"/>.
        /// </summary>
        /// <param name="restartDelay">The time to wait after stopping before starting again.</param>
        /// <param name="clearLogBeforeStarting">Determines if the logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        /// <param name="clearArchivedLogsBeforeStarting">Determines if the archived logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        /// <param name="exception">If <b>false</b> is returned, this value will hold the <see cref="Exception"/> that occurred.</param>
        /// <returns><b>True</b> if the operation was successful; otherwise <b>false</b>.</returns>
        bool TryRestart(TimeSpan restartDelay, bool clearLogBeforeStarting, bool clearArchivedLogsBeforeStarting, out Exception exception);
        /// <summary>
        /// Will disconnect from the <see cref="ComponentManager"/>'s communication system and stop the <see cref="ComponentManager"/>, if it is running.
        /// </summary>
        /// <param name="exception">If <b>false</b> is returned, this value will hold the <see cref="Exception"/> that occurred.</param>
        /// <returns><b>True</b> if the operation was successful; otherwise <b>false</b>.</returns>
        bool TryStop(out Exception exception);
        //
        /// <summary>
        /// The number of requests received.
        /// </summary>
        long ReceivedRequests
        {
            get;
        }
        /// <summary>
        /// The number of requests that have been processed.
        /// </summary>
        long ProcessedRequests
        {
            get;
        }
        /// <summary>
        /// The number of requests currently being managed by the <see cref="IServiceManager"/>.
        /// </summary>
        long WorkingRequests
        {
            get;
        }
        // 
        /// <summary>
        /// Determines if the service manager will allow logs to be downloaded.
        /// </summary>
        bool AllowLogsDownload
        {
            get; set;
        }
        /// <summary>
        /// The number of <see cref="Logging.ILogEntry"/>s to include with each chunk of <see cref="Logging.Logs"/> sent.
        /// </summary>
        int LogsDownloadChunkCount
        {
            get; set;
        }
        /// <summary>
        /// The amount of time to wait between downloading log chunks.
        /// </summary>
        TimeSpan LogsDownloadInterstitialDelay
        {
            get; set;
        }
        /// <summary>
        /// The number of clients currently logged into the service manager.
        /// </summary>
        int LoginCount
        {
            get;
        }
        //
        /// <summary>
        /// Determines if this service manager will allow files to be transfered to its <see cref="FileTransferRootPath"/>.
        /// </summary>
        bool AllowFileUpload
        {
            get; set;
        }
        /// <summary>
        /// Determines if this service manager will allow files to be downloaded.
        /// </summary>
        bool AllowFileDownload
        {
            get; set;
        }
        /// <summary>
        /// The root directory path to write uploaded files to.
        /// </summary>
        string FileTransferRootPath
        {
            get;
        }
        /// <summary>
        /// The type of <see cref="System.Security.Cryptography.HashAlgorithm"/> used to create the verification hash when transferring a file.
        /// </summary>
        string FileTransferHashAlgorithmType
        {
            get; set;
        }
        //
        /// <summary>
        /// The length, in bytes, that each file segment will be split into.
        /// </summary>
        long FileTransferSegmentLength
        {
            get; set;
        }
        /// <summary>
        /// the amount of time to wait between sending and receiving file segments during a file transfer.
        /// </summary>
        TimeSpan FileTransferInterstitialDelay
        {
            get; set;
        }
        //
        /// <summary>
        /// The number of background threads created.
        /// </summary>
        long TotalThreads
        {
            get;
        }
        /// <summary>
        /// The number of background threads currently waiting to be executed.
        /// </summary>
        long WaitingThreads
        {
            get;
        }
        /// <summary>
        /// The number of background threads currently executing.
        /// </summary>
        long ActiveThreads
        {
            get;
        }
        //
        /// <summary>
        /// Allows logs to be written to the <see cref="ComponentManager"/>'s log.
        /// </summary>
        Logging.ILogWriter LogWriter
        {
            get;
        }
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        string LogSourceId
        {
            get;
        }
        /// <summary>
        /// Gets or sets whether messages will be audited in the logs.
        /// </summary>
        bool AuditMessages
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets whether debug information will be audited in the logs.
        /// </summary>
        bool AuditDebugMessages
        {
            get; set;
        }
    }
}

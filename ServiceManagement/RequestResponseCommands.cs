using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Contains all of the coded Request and Response commands.
    /// </summary>
    public enum RequestResponseCommands
    {
        /// <summary>
        /// Contains information related to a problem.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.Error"/>
        /// </summary>
        Error = 0,
        /// <summary>
        /// Cancels an ongoing operation.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.Cancel"/>
        /// </summary>
        Cancel,

        /// <summary>
        /// Will execute the received <see cref="RequestResponseTypes.Ping.MarkReceivedTime"/> function and return the <see cref="RequestResponseTypes.Ping"/> object to the original sender. 
        /// <br/>
        /// Be sure to execute the <see cref="RequestResponseTypes.Ping.MarkEndTime"/> function immediately upon receiving the <see cref="RequestResponseTypes.Ping"/> object.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.Ping"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.Ping"/>
        /// </summary>
        Ping,
        /// <summary>
        /// Indicates whether the given session is alive.
        /// <br/>
        /// Input  = String ( Session Id )
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.IsSessionAlive"/>
        /// </summary>
        IsSessionAlive,

        /// <summary>
        /// Initiates the login process between the calling client and an <see cref="IServiceManager"/>.
        /// <br/>
        /// Input = null
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.LoginRequestResponse"/> (encrypted)
        /// </summary>
        LoginRequest,
        /// <summary>
        /// The response from the <see cref="IServiceManager"/> to the <see cref="LoginRequest"/>.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.LoginRequestResponse"/> (encrypted)
        /// </summary>
        LoginRequestResponse,
        /// <summary>
        /// Contains <see cref="IServiceManager"/> login information encrypted with the public key from the response to the <see cref="LoginRequest"/> command.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.Login"/> (encrypted)
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.LoginResponse"/> (encrypted)
        /// </summary>
        Login,
        /// <summary>
        /// The response from the <see cref="IServiceManager"/> to the <see cref="Login"/>.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.LoginResponse"/> (encrypted)
        /// </summary>
        LoginResponse,

        /// <summary>
        /// If the specified session is active, will terminate the session.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.Logout"/>
        /// <br/>
        /// Output = none
        /// </summary>
        Logout,

        /// <summary>
        /// Contains detailed information about an <see cref="IServiceManager"/>.
        /// <br/>
        /// Input  = ( null )
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.ServiceManagerDetails"/>
        /// </summary>
        ServiceManagerDetails,

        /// <summary>
        /// Contains information about transporting file segments.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.FileTransferThrottle"/>
        /// <br/>
        /// Output = none
        /// </summary>
        FileTransferThrottle,
        /// <summary>
        /// Contains part of a file being transfered between the <see cref="IServiceManager"/> and a client.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.FileSegment"/>
        /// </summary>
        FileSegment,

        /// <summary>
        /// Begins the process of sending a file to the service manager.
        /// <br/>
        /// Input = String ( Client's Public Key )
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.PutFileRequestResponse"/> (encrypted)
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.FileSegment"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.PutFileComplete"/>
        /// </summary>
        PutFileRequest,
        /// <summary>
        /// The response to a <see cref="PutFileRequest"/> command.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.PutFileRequestResponse"/> (encrypted)
        /// </summary>
        PutFileRequestResponse,
        /// <summary>
        /// Conveys information about the file being transfered to the service manager.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.PutFileInformation"/> (encrypted)
        /// <br/>
        /// Output = null
        /// <br/>
        /// Command = <see cref="PutFileReady"/>
        /// </summary>
        PutFileInformation,
        /// <summary>
        /// Indicates that the <see cref="IServiceManager"/> is ready to receive <see cref="FileSegment"/>s.
        /// <br/>
        /// Data = ( null )
        /// </summary>
        PutFileReady,
        /// <summary>
        /// Indicates the end of a file upload operation. 
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.PutFileComplete"/>
        /// </summary>
        PutFileComplete,

        /// <summary>
        /// Begins the process of receiving a file from the service manager.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.GetFileRequest"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.GetFileRequestResponse"/> (encrypted)
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.FileSegment"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.GetFileComplete"/>
        /// <br/>
        /// Command = <see cref="GetFileRequestResponse"/>
        /// </summary>
        GetFileRequest,
        /// <summary>
        /// The response to a <see cref="GetFileRequest"/> command. Contains all the information about the file to download.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.GetFileRequestResponse"/> (encrypted)
        /// </summary>
        GetFileRequestResponse,
        /// <summary>
        /// Indicates that the client is ready to receive, download, <see cref="FileSegment"/>s from the Service Manager.
        /// <br/>
        /// Data = ( null )
        /// </summary>
        GetFileReady,

        /// <summary>
        /// Requests information pertaining to the designated folder.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.GetFolderInformationRequest"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.GetFolderInformationResponse"/>
        /// <br/>
        /// Command = <see cref="GetFolderInformationResponse"/>
        /// </summary>
        GetFolderInformation,
        /// <summary>
        /// The response for the <see cref="GetFolderInformation"/> request. Contains information about the desired folder.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.GetFolderInformationResponse"/>
        /// </summary>
        GetFolderInformationResponse,

        /// <summary>
        /// Requests the service manager to copy, move or delete a file.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.FileCommand"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.FileCommand"/>
        /// <br/>
        /// Command = <see cref="FileCommandResponse"/>
        /// </summary>
        FileCommand,
        /// <summary>
        /// The response for the <see cref="FileCommand"/> request.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.FileCommand"/>
        /// </summary>
        FileCommandResponse,

        /// <summary>
        /// Requests logs from the service manager logging system.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.GetLogs"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.GetLogsResponse"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.GetLogsComplete"/>
        /// <br/>
        /// Command = <see cref="GetLogsResponse"/>
        /// </summary>
        GetLogs,
        /// <summary>
        /// Contains a portion of the logs requested by the <see cref="GetLogs"/> command.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.GetLogsResponse"/>
        /// </summary>
        GetLogsResponse,
        /// <summary>
        /// Indicates that the last <see cref="GetLogs"/> command has complete.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.GetLogsComplete"/>
        /// </summary>
        GetLogsComplete,

        /// <summary>
        /// Requests information about the services the <see cref="IServiceManager"/> is managing.
        /// <br/>
        /// Input = null
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.ListServicesResponse"/>
        /// <br/>
        /// Command = <see cref="ListServicesResponse"/>
        /// </summary>
        ListServices,
        /// <summary>
        /// Contains information about the services the <see cref="IServiceManager"/> is managing.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.ListServicesResponse"/>
        /// </summary>
        ListServicesResponse,

        /// <summary>
        /// Requests information about a specific service by id.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.GetServiceInformation"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.ServiceInformation"/>
        /// <br/>
        /// Command = <see cref="GetServiceInformationResponse"/>
        /// </summary>
        GetServiceInformation,
        /// <summary>
        /// Contains information about a specific service.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.ServiceInformation"/>
        /// </summary>
        GetServiceInformationResponse,

        /// <summary>
        /// Commands the specified service to perform an action.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.ServiceCommand"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.ServiceCommandResponse"/>
        /// <br/>
        /// Command = <see cref="ServiceCommandResponse"/>
        /// </summary>
        ServiceCommand,
        /// <summary>
        /// Indicates a service command has been executed.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.ServiceCommandResponse"/>
        /// </summary>
        ServiceCommandResponse,

        /// <summary>
        /// Requests the service manager to stop all services.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.StopAllServices"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.StopAllServicesResponse"/>
        /// <br/>
        /// Command = <see cref="StopAllServicesResponse"/>
        /// </summary>
        StopAllServices,
        /// <summary>
        /// Contains information about stopping all services.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.StopAllServicesResponse"/>
        /// </summary>
        StopAllServicesResponse,
        /// <summary>
        /// Requests the service manager to start all stopped services.
        /// <br/>
        /// Input = null
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.StartAllServicesResponse"/>
        /// <br/>
        /// Command = <see cref="StartAllServicesResponse"/>
        /// </summary>
        StartAllServices,
        /// <summary>
        /// Contains information about starting all stopped services.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.StartAllServicesResponse"/>
        /// </summary>
        StartAllServicesResponse,
        /// <summary>
        /// Request the service manager to stop and start all extensions and plugins.
        /// <br/>
        /// Input = null
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.RestartAllServicesResponse"/>
        /// <br/>
        /// Command = <see cref="RestartAllServicesResponse"/>
        /// </summary>
        RestartAllServices,
        /// <summary>
        /// Contains information about the <see cref="RestartAllServices"/> command.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.RestartAllServicesResponse"/>
        /// </summary>
        RestartAllServicesResponse,

        /// <summary>
        /// Scan for new extensions and plugins and start them.
        /// <br/>
        /// Input = null
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.ScanForComponentsResponse"/>
        /// <br/>
        /// Command = <see cref="ScanForComponentsResponse"/>
        /// </summary>
        ScanForComponents,
        /// <summary>
        /// Contains information about scanning and starting new extensions and plugins.
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.ScanForComponentsResponse"/>
        /// </summary>
        ScanForComponentsResponse,

        /// <summary>
        /// Request information about the packages in the package folder.
        /// <br/>
        /// Input = null
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.ListPackagesResponse"/>
        /// <br/>
        /// Command = <see cref="ListPackagesResponse"/>
        /// </summary>
        ListPackages,
        /// <summary>
        /// Contains information about the packages in the package folder.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.ListPackagesResponse"/>
        /// </summary>
        ListPackagesResponse,
        /// <summary>
        /// Request information about the installed packages in the installation folder.
        /// <br/>
        /// Input = null
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.ListInstalledPackagesResponse"/>
        /// <br/>
        /// Command = <see cref="ListInstalledPackagesResponse"/>
        /// </summary>
        ListInstalledPackages,
        /// <summary>
        /// Contains information about the installed packaged in the installation folder.
        /// </summary>
        ListInstalledPackagesResponse,

        /// <summary>
        /// Requests the service manager to uninstall the specified package.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.UnInstallPackage"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.UnInstallPackageResponse"/>
        /// <br/>
        /// Command = <see cref="UnInstallPackageResponse"/>
        /// </summary>
        UnInstallPackage,
        /// <summary>
        /// Contains information about the <see cref="UnInstallPackage"/> request.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.UnInstallPackageResponse"/>
        /// </summary>
        UnInstallPackageResponse,
        /// <summary>
        /// Request the service manager to install the specified package.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.InstallPackage"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.InstallPackageResponse"/>
        /// <br/>
        /// Command = <see cref="InstallPackageResponse"/>
        /// </summary>
        InstallPackage,
        /// <summary>
        /// Contains information about the <see cref="InstallPackage"/> request.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.InstallPackageResponse"/>
        /// </summary>
        InstallPackageResponse,
        /// <summary>
        /// Requests the service manager to uninstall all installed packages.
        /// <br/>
        /// Input = null
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.UnInstallAllPackagesResponse"/>
        /// <br/>
        /// Command = <see cref="UnInstallAllPackagesResponse"/>
        /// </summary>
        UnInstallAllPackages,
        /// <summary>
        /// Contains information about the <see cref="UnInstallAllPackages"/> request. 
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.UnInstallAllPackagesResponse"/>
        /// </summary>
        UnInstallAllPackagesResponse,

        /// <summary>
        /// Requests the service manager to either enable or disable the specified package.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.EnableDisablePackage"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.EnableDisablePackageResponse"/>
        /// <br/>
        /// Command = <see cref="EnableDisablePackageResponse"/>
        /// </summary>
        EnableDisablePackage,
        /// <summary>
        /// Contains information about enabling or disabling a package.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.EnableDisablePackageResponse"/>
        /// </summary>
        EnableDisablePackageResponse,

        /// <summary>
        /// Request the service manager to either enable or disable the specified installed package.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.EnableDisableInstalledPackage"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.EnableDisableInstalledPackageResponse"/>
        /// <br/>
        /// Command = <see cref="EnableDisableInstalledPackageResponse"/>
        /// </summary>
        EnableDisableInstalledPackage,
        /// <summary>
        /// Contains information about enabling or disabling an installed package.
        /// </summary>
        EnableDisableInstalledPackageResponse,

        /// <summary>
        /// Requests the dependency chain for the specified installed package.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.InstalledPackageDependencyChain"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.InstalledPackageDependencyChainResponse"/>
        /// <br/>
        /// Command = <see cref="InstalledPackageDependencyChainResponse"/>
        /// </summary>
        InstalledPackageDependencyChain,
        /// <summary>
        /// Contains the dependency chain information for the specified installed package.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.InstalledPackageDependencyChainResponse"/>
        /// </summary>
        InstalledPackageDependencyChainResponse,

        /// <summary>
        /// Requests a list of installed packages which references the specified installed package.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.InstalledPackageReferencedBy"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.InstalledPackageReferencedByResponse"/>
        /// <br/>
        /// Command = <see cref="InstalledPackageReferencedByResponse"/>
        /// </summary>
        InstalledPackageReferencedBy,
        /// <summary>
        /// Contains a list of installed packages which references the specified installed package.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.InstalledPackageReferencedByResponse"/>
        /// </summary>
        InstalledPackageReferencedByResponse,

        /// <summary>
        /// Requests information about the communication system.
        /// <br/>
        /// Input = null
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.CommunicationInformationResponse"/>
        /// <br/>
        /// Command = <see cref="CommunicationInformationResponse"/>
        /// </summary>
        CommunicationInformation,
        /// <summary>
        /// Contains information about the communication system. 
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.CommunicationInformationResponse"/>
        /// </summary>
        CommunicationInformationResponse,

        /// <summary>
        /// Requests the custom configuration for the specified installed package.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.ReadInstalledCustomConfiguration"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.ReadInstalledCustomConfigurationResponse"/>
        /// <br/>
        /// Command = <see cref="ReadInstalledCustomConfigurationResponse"/>
        /// </summary>
        ReadInstalledCustomConfiguration,
        /// <summary>
        /// Contains the custom configuration for a specific installed package.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.ReadInstalledCustomConfigurationResponse"/>
        /// </summary>
        ReadInstalledCustomConfigurationResponse,
        /// <summary>
        /// Requests the supplied custom configuration be written to the specified installed package's custom configuration file.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.WriteInstalledCustomConfiguration"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.WriteInstalledCustomConfigurationResponse"/>
        /// <br/>
        /// Command = <see cref="WriteInstalledCustomConfigurationResponse"/>
        /// </summary>
        WriteInstalledCustomConfiguration,
        /// <summary>
        /// Contains the results of writing the supplied custom configuration to the specific installed package.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.WriteInstalledCustomConfigurationResponse"/>
        /// </summary>
        WriteInstalledCustomConfigurationResponse,

        /// <summary>
        /// Requests the custom configuration for the specified package.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.ReadPackageCustomConfiguration"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.ReadPackageCustomConfigurationResponse"/>
        /// <br/>
        /// Command = <see cref="ReadPackageCustomConfigurationResponse"/>
        /// </summary>      
        ReadPackageCustomConfiguration,
        /// <summary>
        /// Contains the custom configuration for a specific package.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.ReadPackageCustomConfigurationResponse"/>
        /// </summary>
        ReadPackageCustomConfigurationResponse,
        /// <summary>
        /// Requests the supplied custom configuration be written to the specified package's custom configuration file.
        /// <br/>
        /// Input = <see cref="RequestResponseTypes.WritePackageCustomConfiguration"/>
        /// <br/>
        /// Output = <see cref="RequestResponseTypes.WritePackageCustomConfigurationResponse"/>
        /// <br/>
        /// Command = <see cref="WritePackageCustomConfigurationResponse"/>
        /// </summary>
        WritePackageCustomConfiguration,
        /// <summary>
        /// Contains the results of writing the supplied custom configuration to the specific package.
        /// <br/>
        /// Data = <see cref="RequestResponseTypes.WritePackageCustomConfigurationResponse"/>
        /// </summary>
        WritePackageCustomConfigurationResponse
    }
}

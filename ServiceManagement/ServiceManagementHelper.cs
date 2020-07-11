using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Common, and standardizing, methods used throughout the dodSON.Core.ServiceManagement namespace.
    /// </summary>
    public static class ServiceManagementHelper
    {
        #region Public Fields
        /// <summary>
        /// The minimum length, in bytes, that a file segment will be separated into.
        /// </summary>
        public static readonly int MinimumFileTransferChunkSize = (int)Common.ByteCountHelper.FromKilobytes(50);
        /// <summary>
        /// The minimum amount of time allowed to wait between sending file segments.
        /// </summary>
        public static readonly TimeSpan MinimumFileTransferInterstitialDelay = TimeSpan.FromMilliseconds(1);
        #endregion
        #region Public Methods

        // TODO: use the TIMEOUT parameter

        /// <summary>
        /// Performs all of the logic to log in to a <see cref="IServiceManager"/>.
        /// </summary>
        /// <param name="loginClient">The <see cref="Networking.IClient"/> to use to communicate with the <see cref="IServiceManager"/>; this <see cref="Networking.IClient"/> will be the only one able to send commands to the <see cref="IServiceManager"/>, using a valid SessionId, after a successful login.</param>
        /// <param name="serviceManagerClientId">The ClientId of the <see cref="IServiceManager"/> to log in to.</param>
        /// <param name="loginEvidence">The byte array used to prove you have permission to access the <see cref="IServiceManager"/>.</param>
        /// <param name="requestTimeout">The length of time to wait before aborting a request and failing the login process.</param>
        /// <param name="cancellationToken">Allows the log in process to be canceled.</param>
        /// <param name="logSourceId">The log source id to use when writing logs.</param>
        /// <param name="logWriter">The <see cref="Logging.ILogWriter"/> to write logs to.</param>
        /// <param name="serviceManagerId">A value which uniquely identifies a <see cref="IServiceManager"/>.</param>
        /// <param name="resultsDescription">Details the success or failure of the log in attempt.</param>
        /// <param name="loginResults">If return value is <b>true</b>, a <see cref="RequestResponseTypes.LoginResponse"/>; otherwise, if return value is <b>false</b> this value will be null.</param>
        /// <returns>Returns whether the log in attempt was successful or not. <b>True</b> indicates a successful log in; <b>false</b> indicates a log in failure. See the <paramref name="resultsDescription"/> for details.</returns>
        public static bool Login(Networking.IClient loginClient,
                                 string serviceManagerClientId,
                                 byte[] loginEvidence,
                                 TimeSpan requestTimeout,
                                 System.Threading.CancellationToken cancellationToken,
                                 string logSourceId,
                                 Logging.ILogWriter logWriter,
                                 out string serviceManagerId,
                                 out string resultsDescription,
                                 out RequestResponseTypes.LoginResponse loginResults)
        {
            ServiceResponse localClientResponse = null;
            string groupId = Guid.NewGuid().ToString("N");
            var loginStartTimeUtc = DateTimeOffset.UtcNow;
            // 
            try
            {
                // #### connect to the message bus, temporarily
                loginClient.MessageBus += LocalMessageBus;

                // ######## LOGIN REQUEST COMMAND

                // #### send and wait for "LoginRequest"
                // send "LoginRequest" message to specific Service Manager
                WriteLog(Logging.LogEntryType.Information, $"Sending Login Request, GroupKey={groupId}");
                SendServiceRequest(loginClient, serviceManagerClientId, new ServiceRequest("", groupId, RequestResponseCommands.LoginRequest, "", null));
                // wait for response
                localClientResponse = null;
                while (true)
                {
                    // ######## check for cancellation
                    if (IsCancellationRequested(loginStartTimeUtc, out RequestResponseTypes.Error loginRequestCancelResults, out serviceManagerId))
                    {
                        // CANCELED
                        resultsDescription = $"Login Request, {loginRequestCancelResults}";
                        loginResults = new RequestResponseTypes.LoginResponse(false, resultsDescription, serviceManagerId, "");
                        WriteLog(Logging.LogEntryType.Error, resultsDescription);
                        return false;
                    }
                    if ((localClientResponse != null) && (localClientResponse.Data != null))
                    {
                        // ######## LOGIN COMMAND

                        // get login data
                        var loginRequestData = (localClientResponse.Data as RequestResponseTypes.LoginRequestResponse);
                        if (loginRequestData != null)
                        {
                            // #### create "Login"
                            // create public/private keys
                            var clientPublicPrivateKeys = Cryptography.CryptographyHelper.GeneratePublicPrivateKeys();
                            // create "Login"
                            var loginRequest = new RequestResponseTypes.Login(clientPublicPrivateKeys.Item1, loginEvidence);
                            // convert "Login" to byte[]
                            var loginRequestByteArray = (new Converters.TypeSerializer<RequestResponseTypes.Login>()).ToByteArray(loginRequest);
                            // *** prepare for "tunneling" transport
                            // prepare "Login" for secure transport
                            var transportData = Cryptography.CryptographyHelper.PrepareForTransport(loginRequestByteArray, loginRequestData.XmlPublicKey);
                            // #### send "Login" message to specific Service Manager
                            WriteLog(Logging.LogEntryType.Information, $"Sending Login Information (encrypted), GroupKey={groupId}");
                            SendServiceRequest(loginClient, serviceManagerClientId, new ServiceRequest("", groupId, RequestResponseCommands.Login, "", transportData));
                            // #### wait for response
                            localClientResponse = null;
                            while (true)
                            {
                                // ######## check for cancellation
                                if (IsCancellationRequested(loginStartTimeUtc, out RequestResponseTypes.Error loginCancelResults, out serviceManagerId))
                                {
                                    // CANCELED
                                    resultsDescription = $"Login, {loginCancelResults}";
                                    loginResults = new RequestResponseTypes.LoginResponse(false, resultsDescription, serviceManagerId, "");
                                    WriteLog(Logging.LogEntryType.Error, resultsDescription);
                                    return false;
                                }
                                if ((localClientResponse != null) && (localClientResponse.Data != null))
                                {
                                    if (localClientResponse.Data is List<byte[]>)
                                    {
                                        // #### received tunnel encrypted data
                                        // decrypt, using the private key, and restore the list of byte arrays to a single byte array 
                                        var restoredData = Cryptography.CryptographyHelper.RestoreFromTransport((List<byte[]>)localClientResponse.Data, clientPublicPrivateKeys.Item2);
                                        // convert the decrypted and restored byte array into a 'ServiceManagerLoginResponse'
                                        loginResults = (new Converters.TypeSerializer<RequestResponseTypes.LoginResponse>()).FromByteArray(restoredData);
                                        // ######## LOGIN COMMAND COMPLETE
                                        serviceManagerId = loginResults.ServiceManagerId;
                                        resultsDescription = $"{loginResults.Description}";
                                        WriteLog(Logging.LogEntryType.Information, resultsDescription);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                // #### disconnect from the message bus
                loginClient.MessageBus -= LocalMessageBus;
            }

            // ########################
            // internal methods
            // ########################

            TimeSpan ElapsedTime(DateTimeOffset startTimeUtc) => (DateTimeOffset.UtcNow - startTimeUtc);

            void WriteLog(Logging.LogEntryType entryType, string message) => logWriter.Write(entryType, logSourceId, message);

            bool IsCancellationRequested(DateTimeOffset icrStartTime, out RequestResponseTypes.Error icrResults, out string ircServiceManagerId)
            {
                var details = $", Elapsed Time={ElapsedTime(icrStartTime)}";
                // check for cancellation
                if (cancellationToken.IsCancellationRequested)
                {
                    icrResults = new RequestResponseTypes.Error($"Canceled by Client{details}");
                    ircServiceManagerId = "";
                    return true;
                }
                if (localClientResponse != null)
                {
                    // check for service manager error or cancel
                    if (localClientResponse.Command == RequestResponseCommands.Error)
                    {
                        // #### error from ServiceManager
                        icrResults = new RequestResponseTypes.Error($"Error sent from ServiceManager: {(localClientResponse.Data as RequestResponseTypes.Error)?.Description}{details}");
                        ircServiceManagerId = localClientResponse.ServiceManagerId;
                        return true;
                    }
                    else if (localClientResponse.Command == RequestResponseCommands.Cancel)
                    {
                        // #### cancel from ServiceManager
                        icrResults = new RequestResponseTypes.Error($"Canceled by ServiceManager: {(localClientResponse.Data as RequestResponseTypes.Cancel)?.Description}{details}");
                        ircServiceManagerId = localClientResponse.ServiceManagerId;
                        return true;
                    }
                }
                //
                icrResults = null;
                ircServiceManagerId = "";
                return false;
            }

            void LocalMessageBus(object sender, Networking.MessageEventArgs e)
            {
                var response = e.Message.PayloadMessage<ServiceResponse>();
                if (response != null)
                {
                    // only work with these commands
                    if ((response.Command == RequestResponseCommands.LoginRequestResponse) ||
                        (response.Command == RequestResponseCommands.LoginResponse) ||
                        (response.Command == RequestResponseCommands.Cancel) ||
                        (response.Command == RequestResponseCommands.Error))
                    {
                        localClientResponse = response;
                    }
                }
            }
        }

        // ################################

        /// <summary>
        /// Performs all the logic  to upload a file to a <see cref="IServiceManager"/>.
        /// </summary>
        /// <param name="sourceFilename">The source file to upload.</param>
        /// <param name="destinationFilename">The name of the file when uploaded; any path information will be ignored.</param>
        /// <param name="loggedInClient">The <see cref="Networking.IClient"/> used to login and send commands to the <see cref="IServiceManager"/>.</param>
        /// <param name="sessionId">The session id returned from the <see cref="IServiceManager"/> login.</param>
        /// <param name="groupKey">A group id connecting this file upload operation. Use this value to cancel and throttle this specific file upload operation.</param>
        /// <param name="serviceManagerClientId">The <see cref="Networking.IClient"/> id for the <see cref="IServiceManager"/>.</param>
        /// <param name="serviceManagerId">The id for the <see cref="IServiceManager"/>.</param>
        /// <param name="fileTransferCompletionWaitTime">The amount of time to wait for a file upload completed message from the <see cref="IServiceManager"/> before aborting the file upload.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> used to cancel this file upload operation.</param>
        /// <param name="logSourceId">The log source id to use when writing logs.</param>
        /// <param name="logWriter">The <see cref="Logging.ILogWriter"/> to write logs to.</param>
        /// <param name="uploadFeedback">A function returning file transmission information.</param>
        /// <param name="results">A <see cref="RequestResponseTypes.PutFileComplete"/> which indicates the success for failure of the file upload operation</param>
        /// <returns>Whether the file upload was succeeded, <b>true</b>, or failed, <b>false</b>.</returns>
        public static bool SendFileToServiceManager(string sourceFilename,
                                                    string destinationFilename,
                                                    Networking.IClient loggedInClient,
                                                    string sessionId,
                                                    string groupKey,
                                                    string serviceManagerClientId,
                                                    string serviceManagerId,
                                                    TimeSpan fileTransferCompletionWaitTime,
                                                    System.Threading.CancellationToken cancellationToken,
                                                    string logSourceId,
                                                    Logging.ILogWriter logWriter,
                                                    Action<RequestResponseTypes.FileTransferFeedback> uploadFeedback,
                                                    out RequestResponseTypes.PutFileComplete results)
        {

            // TODO: test with zero-length file
            // TODO: test uploadFeedback




            // validate parameters
            if (uploadFeedback == null)
            {
                throw new ArgumentNullException(nameof(uploadFeedback));
            }
            //
            ServiceRequest localClientRequest;
            ServiceResponse localClientResponse;
            var destFilename = System.IO.Path.GetFileName(destinationFilename);
            long fileLength = 0;
            long sentBytes = 0;
            // 
            try
            {
                // #### connect to the message bus, temporarily
                loggedInClient.MessageBus += LocalMessageBus;

                // ######## PUT FILE REQUEST

                // ################################################################
                // Command = ServiceManager_PutFileRequest
                // Data = <string> Client Public Key
                // ################################################################
                // Response = ERROR
                //          = ServiceManager_PutFileRequestResponse (encrypted)
                // ################################################################

                // send "PutFileRequest" message to specific Service Manager                
                fileLength = new System.IO.FileInfo(sourceFilename).Length;
                WriteLog(Logging.LogEntryType.Information, $"Sending File Upload Request, SessionId={sessionId}, GroupKey={groupKey}");
                var publicPrivateKeys = Cryptography.CryptographyHelper.GeneratePublicPrivateKeys();
                loggedInClient.SendMessage(serviceManagerClientId, new ServiceRequest(sessionId, groupKey, RequestResponseCommands.PutFileRequest, serviceManagerId, publicPrivateKeys.Item1));
                // wait for response
                localClientResponse = null;
                var putFileRequestStartTimeUtc = DateTimeOffset.UtcNow;
                while (true)
                {
                    // ######## check for cancellation
                    if (IsCancellationRequested(putFileRequestStartTimeUtc, out RequestResponseTypes.PutFileComplete putFileRequestCancelResults))
                    {
                        // CANCELED
                        results = putFileRequestCancelResults;
                        WriteLog(Logging.LogEntryType.Error, $"{results.Description}, SessionId={sessionId}, GroupKey={groupKey}");
                        return false;
                    }
                    if (localClientResponse != null)
                    {
                        // ######## check for response
                        if (localClientResponse.Data is List<byte[]>)
                        {
                            // #### file transfer information from the service manager
                            // decrypt response
                            var decryptedData = Cryptography.CryptographyHelper.RestoreFromTransport((localClientResponse.Data as List<byte[]>), publicPrivateKeys.Item2);
                            var restoredData = (new Converters.TypeSerializer<RequestResponseTypes.PutFileRequestResponse>()).FromByteArray(decryptedData);
                            // 
                            long serviceChunkTransferChunkLength = restoredData.ChunkLength;
                            TimeSpan serviceChunkTransferInterstitialDelay = restoredData.ChunkTransferInterstitialDelay;
                            // compute file hash
                            byte[] fileHash;
                            using (var hasher = restoredData.HashAlgorithm)
                            {
                                fileHash = Common.ChecksumHelper.ComputeFile(sourceFilename, hasher);
                            }
                            // create PutFileInformation
                            var putFileInfo = new RequestResponseTypes.PutFileInformation(destFilename, fileLength, fileHash);
                            // convert "Login" to byte[]
                            var putFileInfoByteArray = (new Converters.TypeSerializer<RequestResponseTypes.PutFileInformation>()).ToByteArray(putFileInfo);
                            // encrypt PutFileInformation
                            var transportData = Cryptography.CryptographyHelper.PrepareForTransport(putFileInfoByteArray, restoredData.XmlPublicKey);

                            // ################################################################
                            // Command = ServiceManager_PutFileInformation
                            // Data = ( encrypted for tunneling: ServiceManager_PutFileInformation )
                            // ################################################################
                            // Response = ERROR
                            //          = ServiceManager_PutFileReady
                            // ################################################################

                            // send PutFileInformation to service manager
                            WriteLog(Logging.LogEntryType.Information, $"Sending File Upload Information, (encrypted), SessionId={sessionId}, GroupKey={groupKey}");
                            loggedInClient.SendMessage(serviceManagerClientId, new ServiceRequest(sessionId, groupKey, RequestResponseCommands.PutFileInformation, serviceManagerId, transportData));
                            // wait for response
                            localClientResponse = null;
                            var putFileInformationStartTimeUtc = DateTimeOffset.UtcNow;
                            while (true)
                            {
                                // ######## check for cancellation
                                if (IsCancellationRequested(putFileInformationStartTimeUtc, out RequestResponseTypes.PutFileComplete putFileInformationCancelResults))
                                {
                                    // CANCELED
                                    results = putFileInformationCancelResults;
                                    WriteLog(Logging.LogEntryType.Error, $"File Upload Information {results.Description}, SessionId={sessionId}, GroupKey={groupKey}");
                                    return false;
                                }
                                if (localClientResponse != null)
                                {
                                    // ######## check for response
                                    if (localClientResponse.Command == RequestResponseCommands.PutFileReady)
                                    {
                                        // #### service manager is ready to receive file
                                        // #### loop: send the file to the service manager in chunks
                                        // create byte[] buffer
                                        var bytes = new byte[serviceChunkTransferChunkLength];
                                        // get file's total length
                                        var totalBytes = new System.IO.FileInfo(sourceFilename).Length;
                                        localClientResponse = null;
                                        localClientRequest = null;
                                        var putFileStartTimeUtc = DateTimeOffset.UtcNow;
                                        long lastBytesSent = 0;
                                        // open source file stream
                                        WriteLog(Logging.LogEntryType.Information, $"Starting File Upload, Source Filename={sourceFilename}, Destination Filename={destFilename}, File Size={Common.ByteCountHelper.ToString(fileLength)} ({fileLength:N0}), File Segment Length={Common.ByteCountHelper.ToString(serviceChunkTransferChunkLength)} ({serviceChunkTransferChunkLength:N0}), Interstitial Delay={serviceChunkTransferInterstitialDelay}, SessionId={sessionId}, GroupKey={groupKey}");
                                        using (var fs = System.IO.File.OpenRead(sourceFilename))
                                        {
                                            // ***************************   LOOP   ***************************
                                            // ****************************************************************

                                            // ################################################################
                                            // Command = ServiceManager_PutFile
                                            // Data = ServiceManager_PutFile
                                            // ################################################################
                                            // Response = ERROR
                                            //          = CANCEL
                                            // ################################################################

                                            // #### FEEDBACK
                                            uploadFeedback(new RequestResponseTypes.FileTransferFeedback(sourceFilename, destFilename, lastBytesSent, totalBytes, 0));

                                            // read the source file in chunks
                                            for (long i = 0; i < totalBytes; i += serviceChunkTransferChunkLength)
                                            {
                                                // ######## check for cancellation
                                                if (IsCancellationRequested(putFileStartTimeUtc, out RequestResponseTypes.PutFileComplete putFileCancelResults))
                                                {
                                                    // CANCELED
                                                    results = putFileCancelResults;
                                                    WriteLog(Logging.LogEntryType.Error, $"File Upload {results.Description}, SessionId={sessionId}, GroupKey={groupKey}");
                                                    // #### FEEDBACK
                                                    return false;
                                                }
                                                // ######## check for throttle command
                                                if ((localClientRequest != null) && (localClientRequest.Command == RequestResponseCommands.FileTransferThrottle))
                                                {
                                                    // update these values
                                                    serviceChunkTransferChunkLength = (localClientRequest.Data as RequestResponseTypes.FileTransferThrottle).SegmentLength;
                                                    if (serviceChunkTransferChunkLength < MinimumFileTransferChunkSize)
                                                    {
                                                        serviceChunkTransferChunkLength = MinimumFileTransferChunkSize;
                                                    }
                                                    serviceChunkTransferInterstitialDelay = (localClientRequest.Data as RequestResponseTypes.FileTransferThrottle).SegmentTransferInterstitialDelay;
                                                    if (serviceChunkTransferInterstitialDelay < MinimumFileTransferInterstitialDelay)
                                                    {
                                                        serviceChunkTransferInterstitialDelay = MinimumFileTransferInterstitialDelay;
                                                    }
                                                    // reset localClientRequest
                                                    localClientRequest = null;
                                                    // resize working array
                                                    bytes = new byte[serviceChunkTransferChunkLength];
                                                    //
                                                    WriteLog(Logging.LogEntryType.Information, $"File Upload Throttled, File Segment Length={Common.ByteCountHelper.ToString(serviceChunkTransferChunkLength)} ({serviceChunkTransferChunkLength:N0}), Interstitial Delay={serviceChunkTransferInterstitialDelay}, Source Filename={sourceFilename}, Destination Filename={destFilename}, SessionId={sessionId}, GroupKey={groupKey}");
                                                }
                                                // read file segment
                                                lastBytesSent = fs.Read(bytes, 0, (int)serviceChunkTransferChunkLength);
                                                // test if resize needed
                                                if (lastBytesSent != serviceChunkTransferChunkLength)
                                                {
                                                    // resize byte[]
                                                    Array.Resize(ref bytes, (int)lastBytesSent);
                                                }
                                                // create PutFileSegment object
                                                sentBytes = i + lastBytesSent;
                                                var dude = new RequestResponseTypes.FileSegment(DateTimeOffset.UtcNow.Ticks, totalBytes, sentBytes, bytes);

                                                // ################################################################
                                                // Command = ServiceManager_PutFile
                                                // Data = ServiceManager_PutFile
                                                // ################################################################
                                                // Response = ERROR
                                                //          = CANCEL
                                                //          = PUT FILE COMPLETE
                                                // ################################################################

                                                // send PutFile to service manager
                                                loggedInClient.SendMessage(serviceManagerClientId, new ServiceRequest(sessionId, groupKey, RequestResponseCommands.FileSegment, serviceManagerId, dude));
                                                // #### FEEDBACK
                                                uploadFeedback(new RequestResponseTypes.FileTransferFeedback(sourceFilename, destFilename, lastBytesSent, totalBytes, sentBytes));
                                                // wait 
                                                Threading.ThreadingHelper.Sleep(serviceChunkTransferInterstitialDelay);
                                            }
                                        }
                                        // wait for OK or ERROR response
                                        localClientResponse = null;
                                        var transferCompleteStartTimeUtc = DateTimeOffset.UtcNow;
                                        while (true)
                                        {
                                            // ######## check for timeout
                                            if (ElapsedTime(transferCompleteStartTimeUtc) > fileTransferCompletionWaitTime)
                                            {
                                                // TIMEOUT
                                                var strMessage = $"File Upload Timed Out, Source Filename={sourceFilename}, Destination Filename={destFilename}, File Size={Common.ByteCountHelper.ToString(fileLength)} ({fileLength:N0}), SessionId={sessionId}, GroupKey={groupKey}";
                                                results = new RequestResponseTypes.PutFileComplete(false, strMessage, ElapsedTime(transferCompleteStartTimeUtc));
                                                WriteLog(Logging.LogEntryType.Error, strMessage);
                                                // #### FEEDBACK
                                                return false;
                                            }
                                            // ######## check for cancellation
                                            if (IsCancellationRequested(transferCompleteStartTimeUtc, out RequestResponseTypes.PutFileComplete putFileCompleteCancel))
                                            {
                                                // CANCELED
                                                results = putFileCompleteCancel;
                                                WriteLog(Logging.LogEntryType.Error, $"File Upload {results.Description}");
                                                // #### FEEDBACK
                                                return false;
                                            }
                                            // ######## check for incoming messages
                                            if (localClientResponse != null)
                                            {
                                                if (localClientResponse.Command == RequestResponseCommands.PutFileComplete)
                                                {
                                                    // SUCCESS or FAILURE
                                                    if (localClientResponse.Data is RequestResponseTypes.PutFileComplete)
                                                    {
                                                        results = (localClientResponse.Data as RequestResponseTypes.PutFileComplete);
                                                        var infoStr = $"{results.Description}";
                                                        if (results.TransferSuccessful)
                                                        {
                                                            WriteLog(Logging.LogEntryType.Information, infoStr);
                                                        }
                                                        else
                                                        {
                                                            WriteLog(Logging.LogEntryType.Error, infoStr);
                                                        }
                                                        // #### FEEDBACK
                                                        return results.TransferSuccessful;
                                                    }
                                                }
                                            }
                                        } // while (true) 
                                    } // else if (localClientResponse.Data is RequestResponseTypes.ServiceManager_PutFileReady)
                                } // if (localClientResponse != null)
                            } // while (true)
                        } // else if (localClientResponse.Data is List<byte[]>)
                    } // if (localClientResponse != null)
                } // while (true)
            }
            finally
            {
                // #### disconnect from the message bus
                loggedInClient.MessageBus -= LocalMessageBus;
            }

            // ########################
            // internal methods
            // ########################

            TimeSpan ElapsedTime(DateTimeOffset startTimeUtc) => (DateTimeOffset.UtcNow - startTimeUtc);

            void WriteLog(Logging.LogEntryType logEntryType, string message) => logWriter.Write(logEntryType, logSourceId, message);

            bool IsCancellationRequested(DateTimeOffset icrStartTime, out RequestResponseTypes.PutFileComplete icrResults)
            {
                var details = $", Source Filename={sourceFilename}, Destination Filename={destFilename}, Uploaded={((double)sentBytes / fileLength) * 100.0}% {Common.ByteCountHelper.ToString(sentBytes)} ({sentBytes:N0}), Elapsed Time={ElapsedTime(icrStartTime)}";
                // check for cancellation
                if (cancellationToken.IsCancellationRequested)
                {
                    icrResults = new RequestResponseTypes.PutFileComplete(false, $"Canceled by Client{details}", ElapsedTime(icrStartTime));
                    return true;
                }
                if (localClientResponse != null)
                {
                    // check for service manager error or cancel
                    if (localClientResponse.Command == RequestResponseCommands.Error)
                    {
                        // #### error from ServiceManager
                        icrResults = new RequestResponseTypes.PutFileComplete(false, $"{(localClientResponse.Data as RequestResponseTypes.Error)?.Description}{details}", ElapsedTime(icrStartTime));
                        return true;
                    }
                    else if (localClientResponse.Command == RequestResponseCommands.Cancel)
                    {
                        // #### cancel from ServiceManager
                        icrResults = new RequestResponseTypes.PutFileComplete(false, $"Canceled by Service Manager: {(localClientResponse.Data as RequestResponseTypes.Cancel)?.Description}{details}", ElapsedTime(icrStartTime));
                        return true;
                    }
                }
                //
                icrResults = null;
                return false;
            }

            void LocalMessageBus(object sender, Networking.MessageEventArgs e)
            {
                // ######## check for service manager responses
                var response = e.Message.PayloadMessage<ServiceResponse>();
                if (response != null)
                {
                    if (response.GroupKey == groupKey)
                    {
                        // only work with these commands
                        if ((response.Command == RequestResponseCommands.PutFileRequestResponse) ||
                            (response.Command == RequestResponseCommands.PutFileReady) ||
                            (response.Command == RequestResponseCommands.PutFileComplete) ||
                            (response.Command == RequestResponseCommands.Cancel) ||
                            (response.Command == RequestResponseCommands.Error))
                        {
                            localClientResponse = response;
                        }
                    }
                }
                else
                {
                    // ######## check for service manager requests
                    var request = e.Message.PayloadMessage<ServiceRequest>();
                    if (request != null)
                    {
                        if (request.GroupKey == groupKey)
                        {
                            // only work with these commands
                            if (request.Command == RequestResponseCommands.FileTransferThrottle)
                            {
                                localClientRequest = request;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Performs all the logic to download a file from the Service Manager.
        /// </summary>
        /// <param name="sendFilename">The name of the file to download.</param>
        /// <param name="privateWorkingRootDirectory">The root directory to write downloaded file segments.</param>
        /// <param name="destinationFilename">The final filename for the download file.</param>
        /// <param name="overwriteExistingFile">If <b>true</b> will overwrite the <paramref name="destinationFilename"/> if it already exists, otherwise, if it is <b>false</b> and the <paramref name="destinationFilename"/> exists, it will throw an exception.</param>
        /// <param name="loggedInClient">The <see cref="Networking.IClient"/> used to login and send commands to the <see cref="IServiceManager"/>.</param>
        /// <param name="sessionId">The session id returned from the <see cref="IServiceManager"/> login.</param>
        /// <param name="groupKey">A group id connecting this file download operation. Use this value to cancel and throttle this specific file download operation.</param>
        /// <param name="serviceManagerClientId">The <see cref="Networking.IClient"/> id for the <see cref="IServiceManager"/>.</param>
        /// <param name="serviceManagerId">The id for the <see cref="IServiceManager"/>.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> used to cancel this file download operation.</param>
        /// <param name="logSourceId">The log source id to use when writing logs.</param>
        /// <param name="logWriter">The <see cref="Logging.ILogWriter"/> to write logs to.</param>
        /// <param name="downloadFeedback">A function returning file transmission information.</param>
        /// <param name="results">A <see cref="RequestResponseTypes.GetFileComplete"/> which indicates the success for failure of the file download operation</param>
        /// <returns>Whether the file download was succeeded, <b>true</b>, or failed, <b>false</b>.</returns>
        public static bool GetFileFromServiceManager(string sendFilename,
                                                     string privateWorkingRootDirectory,
                                                     string destinationFilename,
                                                     bool overwriteExistingFile,
                                                     Networking.IClient loggedInClient,
                                                     string sessionId,
                                                     string groupKey,
                                                     string serviceManagerClientId,
                                                     string serviceManagerId,
                                                     CancellationToken cancellationToken,
                                                     string logSourceId,
                                                     Logging.ILogWriter logWriter,
                                                     Action<RequestResponseTypes.FileTransferFeedback> downloadFeedback,
                                                     out RequestResponseTypes.GetFileComplete results)
        {

            // TODO: test with zero-length file
            // TODO: test downloadFeedback




            // validate parameters
            if (!System.IO.Directory.Exists(privateWorkingRootDirectory))
            {
                throw new ArgumentNullException(nameof(privateWorkingRootDirectory));
            }
            //
            if (downloadFeedback == null)
            {
                throw new ArgumentNullException(nameof(downloadFeedback));
            }
            // check if file already exists
            if (System.IO.File.Exists(destinationFilename))
            {
                if (!overwriteExistingFile)
                {
                    results = new RequestResponseTypes.GetFileComplete(false, $"File already exists. Filename={destinationFilename}, SessionId={sessionId}, GroupKey={groupKey}", TimeSpan.Zero);
                    WriteLog(Logging.LogEntryType.Error, $"File Download. {results.Description}");
                    return false;
                }
            }
            //
            ServiceResponse localClientResponse;
            long fileLength = 0;
            long receivedBytes = 0;
            var startTimeUtc = DateTimeOffset.UtcNow;
            var privateWorkingDirectory = System.IO.Path.Combine(privateWorkingRootDirectory, "DOWNLOAD" + Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(privateWorkingDirectory);
            // 
            try
            {
                // #### connect to the message bus, temporarily
                loggedInClient.MessageBus += LocalMessageBus;

                // ######## GET FILE REQUEST

                // ################################################################
                // Command = GetFileRequest
                // Data = GetFileRequest
                // ################################################################
                // Response = ERROR
                //          = GetFileRequestResponse (encrypted)
                // ################################################################

                // send "GetFileRequest" message to specific Service Manager
                WriteLog(Logging.LogEntryType.Information, $"Sending File Download Request, SessionId={sessionId}, GroupKey={groupKey}");
                var publicPrivateKeys = Cryptography.CryptographyHelper.GeneratePublicPrivateKeys();
                loggedInClient.SendMessage(serviceManagerClientId, new ServiceRequest(sessionId, groupKey, RequestResponseCommands.GetFileRequest, serviceManagerId, new RequestResponseTypes.GetFileRequest(publicPrivateKeys.Item1, sendFilename)));

                // wait for response
                localClientResponse = null;
                while (true)
                {
                    // ######## check for cancellation
                    if (IsCancellationRequested(startTimeUtc, out RequestResponseTypes.GetFileComplete getFileRequestCancelResults))
                    {
                        // CANCELED
                        results = getFileRequestCancelResults;
                        WriteLog(Logging.LogEntryType.Error, $"{results.Description}");
                        // send service manager a CANCEL command
                        loggedInClient.SendMessage(serviceManagerClientId, new ServiceRequest(sessionId, groupKey, RequestResponseCommands.Cancel, serviceManagerId, new RequestResponseTypes.Cancel(results.Description)));
                        // clean up
                        CleanUp();
                        return false;
                    }
                    if (localClientResponse != null)
                    {
                        // ######## check for response
                        if (localClientResponse.Data is List<byte[]>)
                        {
                            // #### file transfer information from the service manager
                            // decrypt response
                            var decryptedData = Cryptography.CryptographyHelper.RestoreFromTransport((localClientResponse.Data as List<byte[]>), publicPrivateKeys.Item2);
                            var restoredData = (new Converters.TypeSerializer<RequestResponseTypes.GetFileRequestResponse>()).FromByteArray(decryptedData);

                            // ################################################################
                            // Command = GetFileReady
                            // Data = null
                            // ################################################################
                            // Response = ERROR, CANCEL
                            //          = FileSegments, ...
                            // ################################################################

                            // send "GetFileReady" message to specific Service Manager
                            WriteLog(Logging.LogEntryType.Information, $"Starting File Download, Source Filename={restoredData.Filename}, Destination Filename{destinationFilename}, File Length={Common.ByteCountHelper.ToString(restoredData.FileLength)} ({restoredData.FileLength:N0}), SessionId={sessionId}, GroupKey={groupKey}");
                            loggedInClient.SendMessage(serviceManagerClientId, new ServiceRequest(sessionId, groupKey, RequestResponseCommands.GetFileReady, serviceManagerId, null));
                            // #### FEEDBACK
                            downloadFeedback(new RequestResponseTypes.FileTransferFeedback(sendFilename, destinationFilename, 0, restoredData.FileLength, 0));

                            // wait for response
                            localClientResponse = null;
                            long accumulatedBytesSent = 0;
                            long lastBytesTransfered = 0;
                            while (true)
                            {
                                // ######## check for cancellation
                                if (IsCancellationRequested(startTimeUtc, out RequestResponseTypes.GetFileComplete getFileCancelResults))
                                {
                                    // CANCELED
                                    results = getFileCancelResults;
                                    WriteLog(Logging.LogEntryType.Error, $"{results.Description}");
                                    // send service manager a CANCEL command
                                    loggedInClient.SendMessage(serviceManagerClientId, new ServiceRequest(sessionId, groupKey, RequestResponseCommands.Cancel, serviceManagerId, new RequestResponseTypes.Cancel(results.Description)));
                                    // clean up
                                    CleanUp();
                                    return false;
                                }
                                if (localClientResponse != null)
                                {
                                    // ######## check for response
                                    if (localClientResponse.Data != null)
                                    {
                                        if (localClientResponse.Data is RequestResponseTypes.FileSegment fileSegment)
                                        {
                                            localClientResponse = null;
                                            //
                                            lastBytesTransfered = fileSegment.Payload.LongLength;
                                            accumulatedBytesSent += lastBytesTransfered;
                                            var segmentFilename = System.IO.Path.Combine(privateWorkingDirectory, $"{fileSegment.Index}");
                                            // save file segment
                                            System.IO.File.WriteAllBytes(segmentFilename, fileSegment.Payload);

                                            // #### FEEDBACK
                                            downloadFeedback(new RequestResponseTypes.FileTransferFeedback(sendFilename, destinationFilename, lastBytesTransfered, restoredData.FileLength, accumulatedBytesSent));

                                            // ################################################################################################

                                            // ######## check to see if all segments have been received  
                                            if (accumulatedBytesSent == fileSegment.TotalBytes)
                                            {
                                                // check if file already exists, already checked for permission above
                                                if (System.IO.File.Exists(destinationFilename))
                                                {
                                                    System.IO.File.Delete(destinationFilename);
                                                }
                                                // combine file segments into single file
                                                long actualLength = 0;
                                                // open destination stream
                                                using (var outputStream = System.IO.File.OpenWrite(destinationFilename))
                                                {
                                                    // iterate through all files in the local file transfer directory in alphabetical order (each filename is the current Ticks, according to the {FileSegment.Index} when the file was written)
                                                    foreach (var fileSegmentName in System.IO.Directory.GetFiles(privateWorkingDirectory).OrderBy(x => x))
                                                    {
                                                        // add length of actual files
                                                        actualLength += (new System.IO.FileInfo(fileSegmentName)).Length;
                                                        // open source stream
                                                        using (var fileSegmentStream = System.IO.File.OpenRead(fileSegmentName))
                                                        {
                                                            // copy source stream into destination stream
                                                            fileSegmentStream.CopyTo(outputStream);
                                                        }
                                                        // delete source file (file segment)
                                                        System.IO.File.Delete(fileSegmentName);
                                                    }
                                                    // flush and close (redundant, i know. but it gives me emotional security.)
                                                    outputStream.Flush();
                                                    outputStream.Close();
                                                }

                                                // TODO: research this
                                                // in theory, this will allow time for the system to release file handles before attempting to reading the newly written file.
                                                Threading.ThreadingHelper.Sleep(TimeSpan.FromSeconds(2));

                                                // delete working directory
                                                System.IO.Directory.Delete(privateWorkingDirectory, true);

                                                // compute the new file's hash
                                                using (var restoredDataHashAlgorithm = restoredData.HashAlgorithm)
                                                {
                                                    var match = Common.ChecksumHelper.CompareFile(destinationFilename, restoredData.Hash, restoredDataHashAlgorithm);
                                                    // send a success or error message
                                                    if (match)
                                                    {
                                                        results = new RequestResponseTypes.GetFileComplete(true, $"File Download Successful, Filename={restoredData.Filename}, File Length={Common.ByteCountHelper.ToString(restoredData.FileLength)} ({restoredData.FileLength:N0}), Elapsed Time={ElapsedTime(startTimeUtc)}, SessionId={sessionId}, GroupKey={groupKey}", DateTimeOffset.UtcNow - startTimeUtc);
                                                        WriteLog(Logging.LogEntryType.Information, $"{results.Description}");
                                                        return true;
                                                    }
                                                    else
                                                    {
                                                        // delete download file
                                                        FileStorage.FileStorageHelper.DeleteFile(destinationFilename);
                                                        //
                                                        results = new RequestResponseTypes.GetFileComplete(false, $"File Download Failed, File Integrity Compromised, Filename={restoredData.Filename}, File Length={Common.ByteCountHelper.ToString(restoredData.FileLength)} ({restoredData.FileLength:N0}), Elapsed Time={ElapsedTime(startTimeUtc)}, SessionId={sessionId}, GroupKey={groupKey}", DateTimeOffset.UtcNow - startTimeUtc);
                                                        WriteLog(Logging.LogEntryType.Error, $"{results.Description}");
                                                        return false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                // disconnect from the message bus
                loggedInClient.MessageBus -= LocalMessageBus;
            }

            // ########################
            // internal methods
            // ########################

            TimeSpan ElapsedTime(DateTimeOffset etStartTimeUtc) => (DateTimeOffset.UtcNow - etStartTimeUtc);

            void WriteLog(Logging.LogEntryType logEntryType, string message) => logWriter.Write(logEntryType, logSourceId, message);

            bool IsCancellationRequested(DateTimeOffset icrStartTime, out RequestResponseTypes.GetFileComplete icrResults)
            {
                // no NANs
                long ircReceivedBytes = receivedBytes > 0 ? receivedBytes : 0;
                double ircReceivedPercentage = (ircReceivedBytes == 0) ? 0 : ((double)ircReceivedBytes / fileLength) * 100.0;
                var ircFileDetails = (ircReceivedBytes == 0) ? "" : $", Sent={ircReceivedPercentage}% {Common.ByteCountHelper.ToString(ircReceivedBytes)} ({ircReceivedBytes:N0})";
                var ircDetails = $", Source Filename={sendFilename}, Destination Filename={destinationFilename}{ircFileDetails}, Elapsed Time={ElapsedTime(icrStartTime)}, SessionId={sessionId}, GroupKey={groupKey}";
                // check for cancellation
                if (cancellationToken.IsCancellationRequested)
                {
                    icrResults = new RequestResponseTypes.GetFileComplete(false, $"File Download Canceled by Client{ircDetails}", ElapsedTime(icrStartTime));
                    return true;
                }
                if (localClientResponse != null)
                {
                    // check for service manager error or cancel
                    if (localClientResponse.Command == RequestResponseCommands.Error)
                    {
                        // #### error from ServiceManager
                        icrResults = new RequestResponseTypes.GetFileComplete(false, $"File Download {(localClientResponse.Data as RequestResponseTypes.Error)?.Description}{ircDetails}", ElapsedTime(icrStartTime));
                        return true;
                    }
                    else if (localClientResponse.Command == RequestResponseCommands.Cancel)
                    {
                        // #### cancel from ServiceManager
                        icrResults = new RequestResponseTypes.GetFileComplete(false, $"File Download Canceled by Service Manager: {(localClientResponse.Data as RequestResponseTypes.Cancel)?.Description}{ircDetails}", ElapsedTime(icrStartTime));
                        return true;
                    }
                }
                //
                icrResults = null;
                return false;
            }

            void LocalMessageBus(object sender, Networking.MessageEventArgs e)
            {
                // ######## check for service manager responses
                var response = e.Message.PayloadMessage<ServiceResponse>();
                if (response != null)
                {
                    if (response.GroupKey == groupKey)
                    {
                        // only work with these commands
                        if ((response.Command == RequestResponseCommands.GetFileRequestResponse) ||
                            (response.Command == RequestResponseCommands.FileSegment) ||
                            (response.Command == RequestResponseCommands.Cancel) ||
                            (response.Command == RequestResponseCommands.Error))
                        {
                            localClientResponse = response;
                        }
                    }
                }
            }

            void CleanUp()
            {
                FileStorage.FileStorageHelper.DeleteDirectory(privateWorkingDirectory);
            }
        }
        #endregion
        #region Private Methods
        private static void SendServiceRequest(Networking.IClient client, string serviceManagerClientId, ServiceRequest serviceRequest) => client.SendMessage(serviceManagerClientId, serviceRequest);
        #endregion
    }
}

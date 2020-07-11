using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Common, and standardizing, methods used throughout the dodSON.Core.ComponentManagement namespace.
    /// </summary>
    public static class ComponentManagementHelper
    {
        /// <summary>
        /// Will broadcast a <see cref="SupportsComponentDesignationRequest"/> message and wait up to <paramref name="timeout"/> for <see cref="SupportsComponentDesignationResponse"/> messages.
        /// </summary>
        /// <param name="client">The <see cref="Networking.IClient"/> used to communicate with the network.</param>
        /// <param name="componentDesignationSearch">The <see cref="IComponent.ComponentDesignation"/> to search for.</param>
        /// <param name="timeout">The total amount of time to wait for <see cref="SupportsComponentDesignationResponse"/> messages.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the search.</param>
        /// <param name="wasCancelled">Indicates whether the search was canceled. <b>True</b> indicates that the search was canceled; otherwise, <b>false</b> indicates that the search timed out.</param>
        /// <param name="resultsDescription">Describes the results of the search.</param>
        /// <returns>Returns a list of <see cref="SupportsComponentDesignationResponse"/>s for all of the components that responded.</returns>
        public static List<SupportsComponentDesignationResponse> DiscoverComponents(Networking.IClient client,
                                                                                    string componentDesignationSearch,
                                                                                    TimeSpan timeout,
                                                                                    CancellationToken cancellationToken,
                                                                                    out bool wasCancelled,
                                                                                    out string resultsDescription)
        {
            // ######## check parameters
            if ((client == null) || (client.State != Networking.ChannelStates.Open))
            {
                throw new ArgumentException($"Communication client is either null or not open.", nameof(client));
            }
            if (string.IsNullOrWhiteSpace(componentDesignationSearch))
            {
                throw new ArgumentNullException(nameof(componentDesignationSearch));
            }
            if ((timeout == null) || (timeout < TimeSpan.Zero))
            {
                throw new ArgumentNullException(nameof(timeout), $"The {nameof(timeout)} parameter cannot be null or less than {TimeSpan.Zero}; value={timeout}");
            }
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }
            // 
            DateTimeOffset startTimeUtc = DateTimeOffset.UtcNow;
            var processId = Guid.NewGuid().ToString("N");
            List<SupportsComponentDesignationResponse> results = new List<SupportsComponentDesignationResponse>();
            // 
            try
            {
                // ######## connect to the message bus, temporarily
                client.MessageBus += LocalMessageBus;
                // ######## send a SupportsComponentDesignationRequest message to all clients
                client.SendMessage("", new SupportsComponentDesignationRequest(componentDesignationSearch, processId));
                // ######## wait for either the timeout or cancellation request
                while (true)
                {
                    // ######## check for cancellation
                    if (cancellationToken.IsCancellationRequested)
                    {
                        wasCancelled = true;
                        resultsDescription = $"Canceled by Client, Elapsed Time={DateTimeOffset.UtcNow - startTimeUtc}";
                        return results;
                    }
                    // ######## check for timeout
                    if ((DateTimeOffset.UtcNow - startTimeUtc) > timeout)
                    {
                        wasCancelled = false;
                        resultsDescription = $"Operation Timed Out, Elapsed Time={DateTimeOffset.UtcNow - startTimeUtc}";
                        return results;
                    }
                    // ######## wait a bit... (will keep this method from hammering the CPU).
                    Threading.ThreadingHelper.Sleep(50);
                }
            }
            finally
            {
                // #### disconnect from the message bus
                client.MessageBus -= LocalMessageBus;
            }

            // ########################
            // internal methods
            // ########################

            void LocalMessageBus(object sender, Networking.MessageEventArgs e)
            {
                var response = e.Message.PayloadMessage<SupportsComponentDesignationResponse>();
                if ((response != null) &&
                    (response.ComponentDesignation.Equals(componentDesignationSearch, StringComparison.InvariantCultureIgnoreCase)) &&
                    (response.ProcessId.Equals(processId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    response.ClientId = e.Message.ClientId;
                    results.Add(response);
                }
            }
        }
    }
}

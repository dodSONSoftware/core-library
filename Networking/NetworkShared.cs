using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Networking
{
    internal static class NetworkShared
    {
        // Facilitates the Network namespace by providing shared code to log registration information, including optional details.
        internal static void LogClientConfigurationDetails(Logging.Logs logs, string serverLogsSourceId, IClientConfiguration clientConfiguration, bool showRegistrationDetails)
        {
            logs.Add(dodSON.Core.Logging.LogEntryType.Information, serverLogsSourceId, $"Registering Client={clientConfiguration.Id}");
            logs.Add(dodSON.Core.Logging.LogEntryType.Information, serverLogsSourceId, $"Receive Own Messages={clientConfiguration.ReceiveSelfSentMessages}");
            if (showRegistrationDetails)
            {
                var clientRecievableTypesCount = clientConfiguration.ReceivableTypesFilter.Count();
                logs.Add(dodSON.Core.Logging.LogEntryType.Information, serverLogsSourceId, $"Receivable Types: ({clientRecievableTypesCount})");
                if (clientRecievableTypesCount > 0)
                {
                    int count = 0;
                    foreach (var item in clientConfiguration.ReceivableTypesFilter) { logs.Add(dodSON.Core.Logging.LogEntryType.Information, serverLogsSourceId, $"#{++count}={item.TypeName}"); }
                }
                var clientTransmittableTypesCount = clientConfiguration.TransmittableTypesFilter.Count();
                logs.Add(dodSON.Core.Logging.LogEntryType.Information, serverLogsSourceId, $"Transmittable Types: ({clientTransmittableTypesCount})");
                if (clientTransmittableTypesCount > 0)
                {
                    int count = 0;
                    foreach (var item in clientConfiguration.TransmittableTypesFilter) { logs.Add(dodSON.Core.Logging.LogEntryType.Information, serverLogsSourceId, $"#{++count}={item.TypeName}"); }
                }
            }
            logs.Add(dodSON.Core.Logging.LogEntryType.Information, serverLogsSourceId, $"Completed Registering Client={clientConfiguration.Id}");
        }
    }
}

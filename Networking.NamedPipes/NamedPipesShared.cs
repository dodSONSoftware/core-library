using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking.NamedPipes
{
    internal static class NamedPipesShared
    {
        internal static string NamedPipeFullChannelName(IChannel channel)
        {
            return string.Format("{0}-{1}", channel.Address.Name, channel.Address.Port);
        }
    }
}

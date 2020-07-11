using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    internal class CachedServiceRequest
        : Cache.CacheProcessorItemBase
    {
        #region Ctor

        public CachedServiceRequest(string key,
                                    Cache.ICacheValidater<Cache.ICacheProcessorItem> validater,
                                    RequestResponseCommands command,
                                    string clientId,
                                    object data,
                                    Action<Cache.ICacheProcessorItem> process) : base(key, validater, process)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            Command = command;
            ClientId = clientId;
            Data = data;
        }
        #endregion
        #region Public Methods

        public string ClientId
        {
            get;
        }

        public RequestResponseCommands Command
        {
            get;
        }

        public object Data
        {
            get; set;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this <see cref="CachedServiceRequest"/>.
        /// </summary>
        /// <returns>The string representation of this <see cref="CachedServiceRequest"/>.</returns>
        public override string ToString() => base.ToString() + $", Command={Command}, ClientId={ClientId}, HasData={Data != null}";
        #endregion
    }
}

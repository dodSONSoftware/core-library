using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.AppDomain
{
    /// <summary>
    /// Facilitates the lifetime lease sponsorship and lease renewal of remote <see cref="Type"/>s in separate <see cref="System.AppDomain"/>s.
    /// </summary>
    public interface ITypeProxySponsorHelper
    {
        /// <summary>
        /// Returns <b>true</b> if the type proxy is currently loaded into the remote <see cref="System.AppDomain"/>; otherwise, <b>false</b>.
        /// </summary>
        bool IsLoaded { get; }
    }
}

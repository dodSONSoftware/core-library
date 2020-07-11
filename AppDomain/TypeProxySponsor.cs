using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.AppDomain
{
    /// <summary>
    /// Facilitates the lifetime lease sponsorship and lease renewal of remote <see cref="Type"/>s in separate <see cref="System.AppDomain"/>s.
    /// </summary>
    [Serializable()]
    public class TypeProxySponsor
        : System.Runtime.Remoting.Lifetime.ISponsor
    {
        #region Ctor
        private TypeProxySponsor() { }
        /// <summary>
        /// Initializes a new instance of TypeProxySponsor.
        /// </summary>
        /// <param name="sponsorHelper">The remote proxy which lifetime service sponsorship is required.</param>
        public TypeProxySponsor(ITypeProxySponsorHelper sponsorHelper)
            : this()
        {
            _SponsorHelper = sponsorHelper ?? throw new ArgumentNullException(nameof(sponsorHelper));
        }
        #endregion
        #region Private Fields
        private ITypeProxySponsorHelper _SponsorHelper = null;
        #endregion
        #region System.Runtime.Remoting.Lifetime.ISponsor Methods
        /// <summary>
        /// Renews the lease only if the remote proxy is loaded.
        /// </summary>
        /// <param name="lease">The <see cref="System.Runtime.Remoting.Lifetime.ILease"/> requesting renewal.</param>
        /// <returns>The new expiration time of the lease.</returns>
        public TimeSpan Renewal(System.Runtime.Remoting.Lifetime.ILease lease)
        {
            if (_SponsorHelper.IsLoaded)
            {
                return lease.RenewOnCallTime;
            }
            return TimeSpan.Zero;
        }
        #endregion
    }
}

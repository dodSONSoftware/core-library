using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines properties and methods used to control access to the communication system.
    /// </summary>
    public interface IChallengeController
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Will examine the <paramref name="evidence"/> and return whether it passes the challenge or not.
        /// </summary>
        /// <param name="evidence">The evidence needed to pass the challenge.</param>
        /// <returns><b>True</b> if the challenge succeeded; otherwise, <b>false</b>.</returns>
        bool Challenge(byte[] evidence);
        /// <summary>
        /// The challenge evidence.
        /// </summary>
        byte[] Evidence { get; }
    }
}

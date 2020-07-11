using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking.ChallengeControllers
{
    /// <summary>
    /// Provides a null, or empty, challenge controller. A challenge controller which always return <b>true</b>.
    /// </summary>
    [Serializable]
    public class NullChallengeController
        : IChallengeController
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="NullChallengeController"/>.
        /// </summary>
        public NullChallengeController() { }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public NullChallengeController(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "ChallengeController") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"ChallengeController\". Configuration Key={configuration.Key}", nameof(configuration)); }
        }
        #endregion
        #region IChallengeController Methods
        /// <summary>
        /// Will examine the <paramref name="evidence"/> and return whether it passes the challenge or not.
        /// </summary>
        /// <param name="evidence">The evidence needed to pass the challenge.</param>
        /// <returns><b>True</b> if the challenge succeeded; otherwise, <b>false</b>.</returns>
        public bool Challenge(byte[] evidence) { return true; }
        /// <summary>
        /// The challenge evidence.
        /// </summary>
        public byte[] Evidence { get { return new byte[0]; } }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("ChallengeController");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                return result;
            }
        }
        #endregion
    }
}

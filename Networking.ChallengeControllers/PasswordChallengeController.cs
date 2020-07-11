using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking.ChallengeControllers
{
    /// <summary>
    /// Provides a challenge controller which will pass only on matching byte arrays.
    /// </summary>
    [Serializable]
    public class PasswordChallengeController
        : IChallengeController
    {
        #region Ctor
        private PasswordChallengeController() { }
        /// <summary>
        /// Instantiates a new <see cref="PasswordChallengeController"/>.
        /// </summary>
        /// <param name="actualEvidence">A byte[] representing the evidence needed to pass the challenge.</param>
        public PasswordChallengeController(byte[] actualEvidence)
            : this()
        {
            if (actualEvidence == null) { throw new ArgumentNullException("actualEvidence"); }
            if (actualEvidence.Length == 0) { throw new ArgumentException("actualEvidence", "actualEvidence cannot be empty."); }
            Evidence = actualEvidence;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public PasswordChallengeController(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "ChallengeController") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"ChallengeController\". Configuration Key={configuration.Key}", nameof(configuration)); }
            // Evidence
            Evidence = (byte[])Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Evidence", typeof(byte[])).Value;
        }
        #endregion
        #region IChallengeController Methods
        /// <summary>
        /// Will examine the <paramref name="evidence"/> and return whether it passes the challenge or not.
        /// </summary>
        /// <param name="evidence">The evidence needed to pass the challenge.</param>
        /// <returns><b>True</b> if the challenge succeeded; otherwise, <b>false</b>.</returns>
        public bool Challenge(byte[] evidence)
        {
            if ((evidence == null) || (evidence.Length == 0)) { return false; }
            return Evidence.SequenceEqual(evidence);
        }
        /// <summary>
        /// The challenge evidence.
        /// </summary>
        public byte[] Evidence { get; }
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
                result.Items.Add("Evidence", Evidence, Evidence.GetType());
                return result;
            }
        }
        #endregion
    }
}

using System;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Represents the elements required to create a connection in the communication system.
    /// </summary>
    [Serializable]
    public class ChannelAddress
        : IChannelAddress
    {
        #region Ctor
        private ChannelAddress() { }
        /// <summary>
        /// Instantiates a new <see cref="ChannelAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The Internet Protocol (IP) address in string form.</param>
        /// <param name="port">The port number for the <see cref="IPAddress"/>.</param>
        /// <param name="name">The name of the connection.</param>
        public ChannelAddress(string ipAddress,
                              int port,
                              string name)
            : this()
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) { ipAddress = "localhost"; }
            if ((port < NetworkingHelper.MinumumPortValue) || (port > NetworkingHelper.MaximumPortValue))
            {
                throw new ArgumentOutOfRangeException("port", string.Format("port out of range. ({0} >= port <= {1})", NetworkingHelper.MinumumPortValue, NetworkingHelper.MaximumPortValue));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                var parts = Guid.NewGuid().ToString().Split('-');
                name = string.Format("{0}-{1}-{2}-{3}-{4}{5}{6}", parts[0], parts[1], parts[2], parts[3], parts[4].Substring(0, 3), new string(new char[] { 'd' }), new string(new char[] { 'o', 'd', 'S' }), new string(new char[] { 'O', 'N' }), parts[4].Substring(4, 3));
            }
            IPAddress = ipAddress;
            Port = port;
            Name = name;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public ChannelAddress(Configuration.IConfigurationGroup configuration)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "ChannelAddress") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"ChannelAddress\". Configuration Key={configuration.Key}", nameof(configuration)); }
            // IPAddress
            IPAddress = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "IPAddress", typeof(string)).Value;
            // Name
            Name = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Name", typeof(string)).Value;
            // Port
            Port = (int)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Port", typeof(int)).Value;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The Internet Protocol (IP) address in string form.
        /// </summary>
        public string IPAddress { get; }
        /// <summary>
        /// The port number for the <see cref="IPAddress"/>.
        /// </summary>
        public int Port { get; }
        /// <summary>
        /// The name of the connection.
        /// </summary>
        public string Name { get; }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("ChannelAddress");
                result.Items.Add("IPAddress", IPAddress, IPAddress.GetType());
                result.Items.Add("Name", Name, Name.GetType());
                result.Items.Add("Port", Port, Port.GetType());
                //
                return result;
            }
        }
        #endregion
    }
}

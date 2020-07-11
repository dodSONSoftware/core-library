using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Represents information about the payload being transported in an <see cref="IMessage"/>.
    /// </summary>
    [Serializable]
    public class PayloadTypeInfo
        : IPayloadTypeInfo
    {
        #region Public Static Factory Methods
        /// <summary>
        /// Gets an empty <see cref="PayloadTypeInfo"/>.
        /// </summary>
        public static PayloadTypeInfo Empty => new PayloadTypeInfo();
        #endregion
        #region Ctor
        private PayloadTypeInfo()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="PayloadTypeInfo"/> using the <paramref name="typeName"/>.
        /// </summary>
        /// <param name="typeName">The name of the type this payload contains. Preferably a <see cref="Type.AssemblyQualifiedName"/>.</param>
        public PayloadTypeInfo(string typeName)
            : this()
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }
            TypeName = typeName;
        }
        /// <summary>
        /// Instantiates a new <see cref="PayloadTypeInfo"/> using the <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type this payload contains.</param>
        public PayloadTypeInfo(Type type)
            : this()
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            TypeName = type.AssemblyQualifiedName;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public PayloadTypeInfo(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "PayloadTypeInfo")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"PayloadTypeInfo\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // TypeName
            TypeName = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "TypeName", typeof(string)).Value;
        }
        #endregion
        #region IPayloadTypeMetadata Methods
        /// <summary>
        /// Defines the type; usually by returning the <see cref="Type.AssemblyQualifiedName"/> of the <see cref="Type"/> in the <see cref="IMessage.Payload"/>.
        /// </summary>
        public string TypeName { get; private set; } = "";
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("PayloadTypeInfo");
                // TypeName
                result.Items.Add("TypeName", TypeName, TypeName.GetType());
                //
                return result;
            }
        }
        #endregion
    }
}

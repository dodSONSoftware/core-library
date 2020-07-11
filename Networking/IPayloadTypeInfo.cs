using System;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines information about the payload being transported in an <see cref="IMessage"/>.
    /// </summary>
    public interface IPayloadTypeInfo
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Defines the type; usually by returning the <see cref="Type.AssemblyQualifiedName"/> of the <see cref="Type"/> in the <see cref="IMessage.Payload"/>.
        /// </summary>
        string TypeName { get; }
    }
}

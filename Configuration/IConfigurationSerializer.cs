using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Defines methods to convert <see cref="IConfigurationItem"/>s and <see cref="IConfigurationGroup"/>s to and from other types.
    /// </summary>
    /// <typeparam name="T">The type to convert to and from.</typeparam>
    public interface IConfigurationSerializer<T>
        : IConfigurable
    {
        /// <summary>
        /// Converts the specified configuration to the type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="configuration">The configuration to convert.</param>
        /// <returns>The configuration converted into type <typeparamref name="T"/>.</returns>
        T Serialize(IConfigurationGroup configuration);
        /// <summary>
        /// Converts the specified <paramref name="source"/>, of type <typeparamref name="T"/>, to an <see cref="IConfigurationGroup"/>.
        /// </summary>
        /// <param name="source">The object, of type <typeparamref name="T"/>, to convert into an <see cref="IConfigurationGroup"/>.</param>
        /// <returns>The object converted into an <see cref="IConfigurationGroup"/>.</returns>
        IConfigurationGroup Deserialize(T source);
    }
}

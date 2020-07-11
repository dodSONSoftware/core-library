using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Converters
{
    /// <summary>
    /// Core functionality used by the types in the dodSON.Core.Converters namespace.
    /// </summary>
    public static class ConvertersHelper
    {
        /// <summary>
        /// Returns a deep clone of the <paramref name="source"/> using binary serialization.
        /// </summary>
        /// <typeparam name="U">The <see cref="Type"/> to clone.</typeparam>
        /// <param name="source">The instance to clone.</param>
        /// <returns>A deep-cloned instance.</returns>
        public static U Clone<U>(U source)
        {
            var converter = new TypeSerializer<U>();
            return converter.FromByteArray(converter.ToByteArray(source));
        }
    }
}

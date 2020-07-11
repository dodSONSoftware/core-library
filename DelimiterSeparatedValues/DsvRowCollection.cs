using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.DelimiterSeparatedValues
{
    /// <summary>
    /// Represents a collection of <see cref="DsvRow"/> objects for a <see cref="DsvTable"/>.
    /// </summary>
    public sealed class DsvRowCollection
        : System.Collections.ObjectModel.Collection<DsvRow>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvRowCollection"/> class.
        /// </summary>
        internal DsvRowCollection()
            : base() { }
        #endregion
    }
}

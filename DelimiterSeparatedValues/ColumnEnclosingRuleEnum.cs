using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.DelimiterSeparatedValues
{
    /// <summary>
    /// Rules guiding how the delimiter separated values are closed when written.
    /// </summary>
    public enum ColumnEnclosingRuleEnum
    {
        /// <summary>
        /// Will enclose according to the <see cref="DsvColumn.Enclosed"/> property for the column it is in.
        /// </summary>
        UseColumnDefault = 0,
        /// <summary>
        /// Will never enclose.
        /// </summary>
        EncloseNone,
        /// <summary>
        /// Will always enclose.
        /// </summary>
        EncloseAll,
        /// <summary>
        /// Will enclose only when the data contains at least one <see cref="DsvSettings.RowSeperator"/>, <see cref="DsvSettings.ColumnSeperator"/> or <see cref="DsvSettings.Enclosure"/>.
        /// </summary>
        EncloseMinimal
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.DelimiterSeparatedValues
{
    /// <summary>
    /// Represents the schema of a <see cref="DsvColumn"/> in a <see cref="DsvTable"/>.
    /// </summary>
    public class DsvColumn
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvColumn"/> class.
        /// </summary>
        private DsvColumn() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvColumn"/> class.
        /// </summary>
        /// <param name="parent">The <see cref="DsvTable"/> which this instance belongs.</param>
        public DsvColumn(DsvTable parent)
            : this()
        {
            Parent = parent;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvColumn"/> class.
        /// </summary>
        /// <param name="parent">The <see cref="DsvTable"/> which this instance belongs.</param>
        /// <param name="columnName">The name of the <see cref="DsvColumn"/>.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public DsvColumn(DsvTable parent,
                         string columnName)
            : this(parent)
        {
            ColumnName = columnName;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvColumn"/> class.
        /// </summary>
        /// <param name="parent">The <see cref="DsvTable"/> which this instance belongs.</param>
        /// <param name="columnName">The name of the <see cref="DsvColumn"/>.</param>
        /// <param name="enclosed">Determines whether the <see cref="DsvColumn"/> is enclosed or not. <b>True</b> to enclose the <see cref="DsvColumn"/>; <b>False</b> to not enclose it.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public DsvColumn(DsvTable parent,
                         string columnName,
                         bool enclosed)
            : this(parent, columnName)
        {
            Enclosed = enclosed;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvColumn"/> class.
        /// </summary>
        /// <param name="parent">The <see cref="DsvTable"/> which this instance belongs.</param>
        /// <param name="columnName">The name of the <see cref="DsvColumn"/>.</param>
        /// <param name="enclosed">Determines whether the <see cref="DsvColumn"/> is enclosed or not. <b>True</b> to enclose the <see cref="DsvColumn"/>; <b>False</b> to not enclose it.</param>
        /// <param name="formatString">When writing values, the format string that specifies the format of the corresponding <see cref="Type"/>.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public DsvColumn(DsvTable parent,
                         string columnName,
                         bool enclosed,
                         string formatString)
            : this(parent, columnName, enclosed)
        {
            FormatString = formatString;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvColumn"/> class.
        /// </summary>
        /// <param name="parent">The <see cref="DsvTable"/> which this instance belongs.</param>
        /// <param name="columnName">The name of the <see cref="DsvColumn"/>.</param>
        /// <param name="enclosed">Determines whether the <see cref="DsvColumn"/> is enclosed or not. <b>True</b> to enclose the <see cref="DsvColumn"/>; <b>False</b> to not enclose it.</param>
        /// <param name="formatString">When writing values, the format string that specifies the format of the corresponding <see cref="Type"/>.</param>
        /// <param name="defaultValue">When writing values, if the given object is null, or <b>Nothing</b> in VB.NET, this will be the value of the object.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public DsvColumn(DsvTable parent,
                         string columnName,
                         bool enclosed,
                         string formatString,
                         object defaultValue)
            : this(parent, columnName, enclosed, formatString)
        {
            DefaultValue = defaultValue;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvColumn"/> class.
        /// </summary>
        /// <param name="parent">The <see cref="DsvTable"/> which this instance belongs.</param>
        /// <param name="columnName">The name of the <see cref="DsvColumn"/>.</param>
        /// <param name="enclosed">Determines whether the <see cref="DsvColumn"/> is enclosed or not. <b>True</b> to enclose the <see cref="DsvColumn"/>; <b>False</b> to not enclose it.</param>
        /// <param name="formatString">When writing values, the format string that specifies the format of the corresponding <see cref="Type"/>.</param>
        /// <param name="defaultValue">When writing values, if the given object is null, or <b>Nothing</b> in VB.NET, this will be the value of the object.</param>
        /// <param name="assumedType">The <see cref="Type"/> of object assumed to be in this <see cref="DsvColumn"/>.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public DsvColumn(DsvTable parent,
                         string columnName,
                         bool enclosed,
                         string formatString,
                         object defaultValue,
                         Type assumedType)
            : this(parent, columnName, enclosed, formatString, defaultValue)
        {
            AssumedType = assumedType;
        }

        #endregion
        #region Public Methods
        /// <summary>
        /// Gets the <see cref="DsvTable"/> to which the <see cref="DsvColumn"/> belongs to.
        /// </summary>
        /// <value>The <see cref="DsvTable"/> to which the <see cref="DsvColumn"/> belongs to.</value>
        public DsvTable Parent { get; } = null;
        /// <summary>
        /// Gets or sets the name of the <see cref="DsvColumn"/> in the <see cref="DsvColumnCollection"/>.
        /// </summary>
        /// <value>The name of the <see cref="DsvColumn"/>.</value>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public string ColumnName { get; set; } = "";
        /// <summary>
        /// Gets or sets whether the <see cref="DsvColumn"/> is enclosed or not.
        /// <value><b>True</b> to enclose the <see cref="DsvColumn"/>; <b>False</b> to not enclose it.</value>
        /// </summary>
        public bool Enclosed { get; set; } = false;
        /// <summary>
        /// Gets or sets the format string that specifies the format of the corresponding <see cref="Type"/> when writing values.
        /// <value>The format string that specifying the format of the corresponding <see cref="Type"/> when writing values.</value>
        /// </summary>
        public string FormatString { get; set; } = "";
        /// <summary>
        /// Gets or sets the value of the object to be written if the object is null, or <b>Nothing</b> in VB.NET.
        /// </summary>
        public object DefaultValue { get; set; } = null;
        /// <summary>
        /// Gets or sets the <see cref="Type"/> of object assumed to be in this <see cref="DsvColumn"/>.
        /// </summary>
        public Type AssumedType { get; set; } = typeof(string);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.DelimiterSeparatedValues
{
    /// <summary>
    /// Represents a row of data in a <see cref="DsvRowCollection"/> of a <see cref="DsvTable"/>.
    /// </summary>
    public class DsvRow
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvRow"/> class.
        /// </summary>
        private DsvRow()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvRow"/> class.
        /// </summary>
        /// <param name="parent">The <see cref="DsvTable"/> to which this instance belongs.</param>
        internal DsvRow(DsvTable parent)
            : this()
        {
            Parent = parent;
        }
        #endregion
        #region Private Fields
        private List<object> _Data = new List<object>();
        #endregion
        #region Public Methods
        /// <summary>
        /// Gets the <see cref="DsvTable"/> which this <see cref="DsvRow"/> belongs to.
        /// </summary>
        public DsvTable Parent { get; } = null;
        /// <summary>
        /// Gets or sets the data stored in the <see cref="DsvRow"/> specified by column index.
        /// </summary>
        /// <param name="index">The index of the <see cref="DsvColumn"/>.</param>
        /// <returns>An object that contains the data for this column.</returns>
        public object this[int index]
        {
            get
            {
                if (index < _Data.Count)
                {
                    return _Data[index];
                }
                throw new ArgumentOutOfRangeException("index");
            }
        }
        /// <summary>
        /// Gets or sets the data stored in the <see cref="DsvRow"/> specified by column name.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>An object that contains the data for this column.</returns>
        /// <remarks>If there exists more than one column with the same name, the first column with the column name will be the target of this property. Best practice would dictate that column names be unique.</remarks>
        public object this[string columnName]
        {
            get
            {
                var index = Parent.Columns.IndexOf(columnName);
                if (index != -1)
                {
                    return this[index];
                }
                throw new ArgumentOutOfRangeException("columnName", string.Format("Cannot find a DsvColumn named ({0}).", columnName));
            }
        }
        /// <summary>
        /// Gets or sets the data stored in the specified <see cref="DsvColumn"/>.
        /// </summary>
        /// <param name="column">An instance of a <see cref="DsvColumn"/>.</param>
        /// <returns>An object that contains the data for this column.</returns>
        public object this[DsvColumn column]
        {
            get
            {
                if (column == null)
                {
                    throw new ArgumentNullException("column", "Parameter column cannot be null.");
                }
                return this[column.ColumnName];
            }
        }
        /// <summary>
        /// Gets or sets all of the values for this <see cref="DsvRow"/> through an array.
        /// </summary>
        /// <value>An object that contains the data for this <see cref="DsvRow"/>.</value>
        /// <remarks>You can use this property to set or get values for this row through an array. If you use this property to set values, any objects beyond the number of columns will be ignored.</remarks>
        public object[] ItemArray
        {
            get
            {
                return _Data.ToArray();
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (i < _Data.Count)
                    {
                        _Data[i] = value[i];
                    }
                }
            }
        }
        /// <summary>
        /// Gets a value that indicates whether the column at the specified index contains a null value.
        /// </summary>
        /// <param name="index">The index of the column.</param>
        /// <returns><b>True</b> If the column contains a null value; otherwise <b>false</b>.</returns>
        public bool IsNull(int index)
        {
            if (index < _Data.Count)
            {
                return (_Data[index] == null);
            }
            throw new ArgumentOutOfRangeException("index");
        }
        /// <summary>
        /// Gets a value that indicates whether the column with the specified name contains a null value.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns><b>True</b> If the column contains a null value; otherwise <b>false</b>.</returns>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.
        /// <para>If there are more than one column with the same name, only the first column will every be accessed by name.</para></remarks>
        public bool IsNull(string columnName)
        {
            var index = Parent.Columns.IndexOf(columnName);
            if (index != -1)
            {
                return IsNull(index);
            }
            throw new ArgumentOutOfRangeException("columnName", string.Format("Cannot find a DsvColumn named ({0}).", columnName));
        }
        /// <summary>
        /// Gets a value that indicates whether the specified <see cref="DsvColumn"/> contains the null value.
        /// </summary>
        /// <param name="column">An instance of a <see cref="DsvColumn"/>.</param>
        /// <returns><b>True</b> If the column contains a null value; otherwise <b>false</b>.</returns>
        public bool IsNull(DsvColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column", "Parameter column cannot be null.");
            }
            return (this[column.ColumnName] == null);
        }
        #endregion
        #region Internal Methods
        /// <summary>
        /// Adds an object to the end of the internal list.
        /// </summary>
        /// <param name="value">The object to add.</param>
        internal void Add(object value) => _Data.Add(value);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.DelimiterSeparatedValues
{
    /// <summary>
    /// Represents a collection of <see cref="DsvColumn"/> objects for a <see cref="DsvTable"/>.
    /// </summary>
    public sealed class DsvColumnCollection
        : System.Collections.ObjectModel.Collection<DsvColumn>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvColumnCollection"/> class.
        /// </summary>
        private DsvColumnCollection()
            : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvColumnCollection"/> class.
        /// </summary>
        /// <param name="parent">The <see cref="DsvTable"/> to which this instance belongs.</param>
        internal DsvColumnCollection(DsvTable parent)
            : this()
        {
            _Parent = parent;
        }
        #endregion
        #region Private Fields
        private readonly DsvTable _Parent = null;
        #endregion
        #region Public Methods
        /// <summary>
        /// Creates and adds a <see cref="DsvColumn"/> object that has the specified name to the <see cref="DsvColumnCollection"/>.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <remarks>If null or an empty string ("") is passed in for the columnName, a default name ("Column0", "Column1", "Column2", and so on) is given to the column.
        /// <para>Use the <see cref="Contains"/> method to determine whether a column with the proposed name already exists in the collection.</para>
        /// <para>Unique column names are not enforced. However, best practice would dictate that column names be unique.</para></remarks>
        public void Add(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                columnName = DetermineOrdinalColumnName();
            }
            Add(new DsvColumn(_Parent, columnName));
        }
        /// <summary>
        /// Creates and adds a <see cref="DsvColumn"/> object that has the specified name to the <see cref="DsvColumnCollection"/>.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="enclosed">Determines whether the column is enclosed or not. <b>True</b> to enclose the column; <b>False</b> to not enclose it.</param>
        /// <remarks>If null or an empty string ("") is passed in for the columnName, a default name ("Column0", "Column1", "Column2", and so on) is given to the column.
        /// <para>Use the <see cref="Contains"/> method to determine whether a column with the proposed name already exists in the collection.</para>
        /// <para>Unique column names are not enforced. However, best practice would dictate that column names be unique.</para></remarks>
        public void Add(string columnName,
                        bool enclosed)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                columnName = DetermineOrdinalColumnName();
            }
            Add(new DsvColumn(_Parent, columnName, enclosed));
        }
        /// <summary>
        /// Creates and adds a <see cref="DsvColumn"/> object that has the specified name to the <see cref="DsvColumnCollection"/>.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="enclosed">Determines whether the column is enclosed or not. <b>True</b> to enclose the column; <b>False</b> to not enclose it.</param>
        /// <param name="formatString">When writing values, the format string that specifies the format of the corresponding <see cref="Type"/>.</param>
        /// <remarks>If null or an empty string ("") is passed in for the columnName, a default name ("Column0", "Column1", "Column2", and so on) is given to the column.
        /// <para>Use the <see cref="Contains"/> method to determine whether a column with the proposed name already exists in the collection.</para>
        /// <para>Unique column names are not enforced. However, best practice would dictate that column names be unique.</para></remarks>
        public void Add(string columnName,
                        bool enclosed,
                        string formatString)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                columnName = DetermineOrdinalColumnName();
            }
            Add(new DsvColumn(_Parent, columnName, enclosed, formatString));
        }
        /// <summary>
        /// Creates and adds a <see cref="DsvColumn"/> object that has the specified name to the <see cref="DsvColumnCollection"/>.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="enclosed">Determines whether the column is enclosed or not. <b>True</b> to enclose the column; <b>False</b> to not enclose it.</param>
        /// <param name="formatString">When writing values, the format string that specifies the format of the corresponding <see cref="Type"/>.</param>
        /// <param name="defaultValue">When writing values, if the given object is null, or <b>Nothing</b> in VB.NET, this will be the value of the object.</param>
        /// <remarks>If null or an empty string ("") is passed in for the columnName, a default name ("Column0", "Column1", "Column2", and so on) is given to the column.
        /// <para>Use the <see cref="Contains"/> method to determine whether a column with the proposed name already exists in the collection.</para>
        /// <para>Unique column names are not enforced. However, best practice would dictate that column names be unique.</para></remarks>
        public void Add(string columnName,
                        bool enclosed,
                        string formatString,
                        object defaultValue)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                columnName = DetermineOrdinalColumnName();
            }
            Add(new DsvColumn(_Parent, columnName, enclosed, formatString, defaultValue));
        }
        /// <summary>
        /// Creates and adds a <see cref="DsvColumn"/> object that has the specified name to the <see cref="DsvColumnCollection"/>.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="enclosed">Determines whether the column is enclosed or not. <b>True</b> to enclose the column; <b>False</b> to not enclose it.</param>
        /// <param name="formatString">When writing values, the format string that specifies the format of the corresponding <see cref="Type"/>.</param>
        /// <param name="defaultValue">When writing values, if the given object is null, or <b>Nothing</b> in VB.NET, this will be the value of the object.</param>
        /// <param name="assumedType">The <see cref="Type"/> of object assumed to be in this column.</param>
        /// <remarks>If null or an empty string ("") is passed in for the columnName, a default name ("Column0", "Column1", "Column2", and so on) is given to the column.
        /// <para>Use the <see cref="Contains"/> method to determine whether a column with the proposed name already exists in the collection.</para>
        /// <para>Unique column names are not enforced. However, best practice would dictate that column names be unique.</para></remarks>
        public void Add(string columnName,
                        bool enclosed,
                        string formatString,
                        object defaultValue,
                        Type assumedType)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                columnName = DetermineOrdinalColumnName();
            }
            Add(new DsvColumn(_Parent, columnName, enclosed, formatString, defaultValue, assumedType));
        }
        /// <summary>
        /// Checks whether the collection contains a <see cref="DsvColumn"/> with the specified name.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns><b>True</b> if a column exists with this name; otherwise, <b>False</b>.</returns>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.
        /// <para>If there are more than one column with the same name, only the first column will every be accessed by name.</para></remarks>
        public bool Contains(string columnName)
        {
            foreach (var item in this)
            {
                if (item.ColumnName.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Gets the index of a <see cref="DsvColumn"/> specified by name.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The index of the <see cref="DsvColumn"/> specified by columnName if it is found; otherwise, -1.</returns>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.
        /// <para>If there are more than one column with the same name, only the first column will every be accessed by name.</para></remarks>
        public int IndexOf(string columnName)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].ColumnName.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// Inserts a new <see cref="DsvColumn"/>, specified by name, into this instance at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the <see cref="DsvColumn"/> should be inserted.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public void Insert(int index,
                           string columnName) => Insert(index, new DsvColumn(_Parent, columnName));
        /// <summary>
        /// Inserts a new <see cref="DsvColumn"/>, specified by name, into this instance at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the <see cref="DsvColumn"/> should be inserted.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="enclosed">Determines whether the column is enclosed or not. <b>True</b> to enclose the column; <b>False</b> to not enclose it.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public void Insert(int index,
                           string columnName,
                           bool enclosed) => Insert(index, new DsvColumn(_Parent, columnName, enclosed));
        /// <summary>
        /// Inserts a new <see cref="DsvColumn"/>, specified by name, into this instance at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the <see cref="DsvColumn"/> should be inserted.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="enclosed">Determines whether the column is enclosed or not. <b>True</b> to enclose the column; <b>False</b> to not enclose it.</param>
        /// <param name="formatString">When writing values, the format string that specifies the format of the corresponding <see cref="Type"/>.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public void Insert(int index,
                           string columnName,
                           bool enclosed,
                           string formatString) => Insert(index, new DsvColumn(_Parent, columnName, enclosed, formatString));
        /// <summary>
        /// Inserts a new <see cref="DsvColumn"/>, specified by name, into this instance at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the <see cref="DsvColumn"/> should be inserted.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="enclosed">Determines whether the column is enclosed or not. <b>True</b> to enclose the column; <b>False</b> to not enclose it.</param>
        /// <param name="formatString">When writing values, the format string that specifies the format of the corresponding <see cref="Type"/>.</param>
        /// <param name="defaultValue">When writing values, if the given object is null, or <b>Nothing</b> in VB.NET, this will be the value of the object.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public void Insert(int index,
                           string columnName,
                           bool enclosed,
                           string formatString,
                           object defaultValue) => Insert(index, new DsvColumn(_Parent, columnName, enclosed, formatString, defaultValue));
        /// <summary>
        /// Inserts a new <see cref="DsvColumn"/>, specified by name, into this instance at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the <see cref="DsvColumn"/> should be inserted.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="enclosed">Determines whether the column is enclosed or not. <b>True</b> to enclose the column; <b>False</b> to not enclose it.</param>
        /// <param name="formatString">When writing values, the format string that specifies the format of the corresponding <see cref="Type"/>.</param>
        /// <param name="defaultValue">When writing values, if the given object is null, or <b>Nothing</b> in VB.NET, this will be the value of the object.</param>
        /// <param name="assumedType">The <see cref="Type"/> of object assumed to be in this column.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.</remarks>
        public void Insert(int index,
                           string columnName,
                           bool enclosed,
                           string formatString,
                           object defaultValue,
                           Type assumedType) => Insert(index, new DsvColumn(_Parent, columnName, enclosed, formatString, defaultValue, assumedType));
        /// <summary>
        /// Gets the <see cref="DsvColumn"/> from the collection with the specified Column Name.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The <see cref="DsvColumn"/> in the collection with the specified Column Name; otherwise a null value if the <see cref="DsvColumn"/> does not exist.</returns>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.
        /// <para>If there are more than one column with the same name, only the first column will every be accessed by name.</para></remarks>
        public DsvColumn this[string columnName]
        {
            get
            {
                var index = IndexOf(columnName);
                if (index != -1)
                {
                    return this[index];
                }
                return null;
            }
        }
        /// <summary>
        /// Removes the <see cref="DsvColumn"/> object that has the specified name from the collection.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <remarks>Unique column names are not enforced. However, best practice would dictate that column names be unique.
        /// <para>If there are more than one column with the same name, only the first column will every be accessed by name.</para></remarks>
        public void Remove(string columnName)
        {
            var index = IndexOf(columnName);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }
        #endregion
        #region Private Methods
        private string DetermineOrdinalColumnName()
        {
            var candidateName = string.Format("Column{0}", Count.ToString());
            if (Contains(candidateName))
            {
                var index = 0;
                var workingName = string.Format("{0}_{1}", candidateName, index);
                while (Contains(workingName))
                {
                    ++index;
                    workingName = string.Format("{0}_{1}", candidateName, index);
                }
                candidateName = workingName;
            }
            return candidateName;
        }
        #endregion
    }
}

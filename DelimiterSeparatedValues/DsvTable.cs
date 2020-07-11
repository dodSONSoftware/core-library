using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.DelimiterSeparatedValues
{
    /// <summary>
    /// Represents a table of delimiter separated values.
    /// </summary>
    /// <example>
    /// The following code will demonstrate how to create a delimiter separated values table, define it's columns, 
    /// populated it and output that data into a properly formatted string, based on the defined DsvSettings.
    /// Next, it will create a second table, using a clone of the original settings, and read the original's
    /// output string, change it's settings to 'rather' bizarre values, for demonstration purposes only, I hope,
    /// output that data into a properly formatted string, albeit bizarre. It will then create a third
    /// table, using a clone of the second table's settings, the bizarre ones, and read in the second table's output string.
    /// Finally, it will change the third table's settings back to the original table's settings and output
    /// that data into a properly formatted string. Lastly, it will compare all the values in the original table
    /// with the values in the third table; there should be no errors.
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // create delimiter separated values settings
    ///     var columnSeperator = ",";
    ///     var rowSeperator = Environment.NewLine;
    ///     var enclosure = "\"";
    ///     var reader_FirstRowIsHeader = true;
    ///     var reader_ParseHeaderRowAsColumns = true;
    ///     var reader_TrimWhitespace = true;
    ///     var writer_IncludeHeaderRow = true;
    ///     var writer_ColumnEnclosingRule = dodSON.Core.DelimiterSeperatedValues.ColumnEnclosingRuleEnum.UseColumnDefault;
    ///     var settings = new dodSON.Core.DelimiterSeperatedValues.DsvSettings(rowSeperator,
    ///                                                                                   columnSeperator,
    ///                                                                                   enclosure,
    ///                                                                                   reader_FirstRowIsHeader,
    ///                                                                                   reader_ParseHeaderRowAsColumns,
    ///                                                                                   reader_TrimWhitespace,
    ///                                                                                   writer_IncludeHeaderRow,
    ///                                                                                   writer_ColumnEnclosingRule);
    ///     
    ///     // create delimiter separated values table
    ///     var originalTable = new dodSON.Core.DelimiterSeperatedValues.DsvTable(settings);
    ///     
    ///     // create columns
    ///     originalTable.Columns.Add("Id", true, "", "", typeof(string));
    ///     originalTable.Columns.Add("Integer", false, "", 0, typeof(Int32));
    ///     originalTable.Columns.Add("Date", true, "", DateTime.MinValue.ToString(), typeof(DateTime));
    ///     originalTable.Columns.Add("Double", false, "", 0.0, typeof(double));
    ///     
    ///     // populate table with rows
    ///     for (int i = 0; i &lt; 16; i++)
    ///     {
    ///         var myIntegerValue = (int)Math.Pow(2, i);
    ///         var row = originalTable.NewRow(string.Format("#{0:X4}", myIntegerValue),
    ///                                        myIntegerValue,
    ///                                        DateTime.Now.AddMinutes(myIntegerValue),
    ///                                        i * 3.1415927);
    ///         originalTable.Rows.Add(row);
    ///     }
    ///     
    ///     // write table to string
    ///     var originalTableString = originalTable.WriteString();
    ///     
    ///     // create table2 with the same settings as the original table
    ///     var settings2 = dodSON.Core.Converters.TypeSerializer&lt;dodSON.Core.DelimiterSeperatedValues.DsvSettings&gt;.Clone(settings);
    ///     var table2 = new dodSON.Core.DelimiterSeperatedValues.DsvTable(settings2);
    ///     
    ///     // read original table's string into table2 (using the settings as defined by the original table)
    ///     table2.ReadString(originalTableString);
    ///     
    ///     // change some of the settings for table2
    ///     table2.Settings.Enclosure = "[]";
    ///     table2.Settings.ColumnSeperator = "+^+";
    ///     table2.Settings.RowSeperator = " +=end-of-column=+ " + Environment.NewLine;
    ///     table2.Settings.Writer_ColumnEnclosingRule = dodSON.Core.DelimiterSeperatedValues.ColumnEnclosingRuleEnum.EncloseAll;
    ///     
    ///     // write table2 to string
    ///     var table2String = table2.WriteString();
    ///     
    ///     // create a third table with the same settings as table2
    ///     var settings3 = dodSON.Core.Converters.TypeSerializer&lt;dodSON.Core.DelimiterSeperatedValues.DsvSettings&gt;.Clone(settings2);
    ///     var table3 = new dodSON.Core.DelimiterSeperatedValues.DsvTable(settings3);
    ///     // read table2 string into table3
    ///     table3.ReadString(table2String);
    ///     
    ///     // restore original settings
    ///     table3.Settings = originalTable.Settings;
    ///     
    ///     // display data
    ///     Console.WriteLine("Original: Enclosure      ='{0}'", originalTable.Settings.Enclosure);
    ///     Console.WriteLine("Original: ColumnSeperator='{0}'", originalTable.Settings.ColumnSeperator);
    ///     Console.WriteLine("Original: RowSeperator   ='{0}'", originalTable.Settings.RowSeperator);
    ///     Console.WriteLine("Original: {0}", originalTable.Settings.Writer_ColumnEnclosingRule);
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine(originalTableString);
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///     Console.WriteLine("table2: Enclosure      ='{0}'", table2.Settings.Enclosure);
    ///     Console.WriteLine("table2: ColumnSeperator='{0}'", table2.Settings.ColumnSeperator);
    ///     Console.WriteLine("table2: RowSeperator   ='{0}'", table2.Settings.RowSeperator);
    ///     Console.WriteLine("Original: {0}", table2.Settings.Writer_ColumnEnclosingRule);
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine(table2.WriteString());
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///     Console.WriteLine("table3: Enclosure      ='{0}'", table3.Settings.Enclosure);
    ///     Console.WriteLine("table3: ColumnSeperator='{0}'", table3.Settings.ColumnSeperator);
    ///     Console.WriteLine("table3: RowSeperator   ='{0}'", table3.Settings.RowSeperator);
    ///     Console.WriteLine("Original: {0}", table3.Settings.Writer_ColumnEnclosingRule);
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine(table3.WriteString());
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///     
    ///     // test the original data against table3
    ///     Console.WriteLine("Testing Original Table against Table3...");
    ///     Console.WriteLine("No errors 'should' be displayed here...");
    ///     for (int i = 0; i &lt; originalTable.Rows.Count; i++)
    ///     {
    ///         if (originalTable.Rows[i].ItemArray[0].ToString() != table3.Rows[i].ItemArray[0].ToString()) { Console.WriteLine(string.Format("     Table copy error. Row={0}; Item=0", i)); }
    ///         if (originalTable.Rows[i].ItemArray[1].ToString() != table3.Rows[i].ItemArray[1].ToString()) { Console.WriteLine(string.Format("     Table copy error. Row={0}; Item=1", i)); }
    ///         if (originalTable.Rows[i].ItemArray[2].ToString() != table3.Rows[i].ItemArray[2].ToString()) { Console.WriteLine(string.Format("     Table copy error. Row={0}; Item=2", i)); }
    ///         if (originalTable.Rows[i].ItemArray[3].ToString() != table3.Rows[i].ItemArray[3].ToString()) { Console.WriteLine(string.Format("     Table copy error. Row={0}; Item=3", i)); }
    ///     }
    ///     
    ///     //
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    ///
    /// // Original: Enclosure      ='"'
    /// // Original: ColumnSeperator=','
    /// // Original: RowSeperator   ='
    /// // '
    /// // Original: UseColumnDefault
    /// // ------------------------------------
    /// // "Id",Integer,"Date",Double
    /// // "#0001",1,"12/11/2015 10:56:37 PM",0
    /// // "#0002",2,"12/11/2015 10:57:37 PM",3.1415927
    /// // "#0004",4,"12/11/2015 10:59:37 PM",6.2831854
    /// // "#0008",8,"12/11/2015 11:03:37 PM",9.4247781
    /// // "#0010",16,"12/11/2015 11:11:37 PM",12.5663708
    /// // "#0020",32,"12/11/2015 11:27:37 PM",15.7079635
    /// // "#0040",64,"12/11/2015 11:59:37 PM",18.8495562
    /// // "#0080",128,"12/12/2015 1:03:37 AM",21.9911489
    /// // "#0100",256,"12/12/2015 3:11:37 AM",25.1327416
    /// // "#0200",512,"12/12/2015 7:27:37 AM",28.2743343
    /// // "#0400",1024,"12/12/2015 3:59:37 PM",31.415927
    /// // "#0800",2048,"12/13/2015 9:03:37 AM",34.5575197
    /// // "#1000",4096,"12/14/2015 7:11:37 PM",37.6991124
    /// // "#2000",8192,"12/17/2015 3:27:37 PM",40.8407051
    /// // "#4000",16384,"12/23/2015 7:59:37 AM",43.9822978
    /// // "#8000",32768,"1/3/2016 5:03:37 PM",47.1238905
    /// // ------------------------------------
    /// // 
    /// // table2: Enclosure      ='[]'
    /// // table2: ColumnSeperator='+^+'
    /// // table2: RowSeperator   =' +=end-of-column=+
    /// // '
    /// // Original: EncloseAll
    /// // ------------------------------------
    /// // []Id[]+^+[]Integer[]+^+[]Date[]+^+[]Double[] +=end-of-column=+
    /// // []#0001[]+^+[]1[]+^+[]12/11/2015 10:56:37 PM[]+^+[]0[] +=end-of-column=+
    /// // []#0002[]+^+[]2[]+^+[]12/11/2015 10:57:37 PM[]+^+[]3.1415927[] +=end-of-column=+
    /// // []#0004[]+^+[]4[]+^+[]12/11/2015 10:59:37 PM[]+^+[]6.2831854[] +=end-of-column=+
    /// // []#0008[]+^+[]8[]+^+[]12/11/2015 11:03:37 PM[]+^+[]9.4247781[] +=end-of-column=+
    /// // []#0010[]+^+[]16[]+^+[]12/11/2015 11:11:37 PM[]+^+[]12.5663708[] +=end-of-column=+
    /// // []#0020[]+^+[]32[]+^+[]12/11/2015 11:27:37 PM[]+^+[]15.7079635[] +=end-of-column=+
    /// // []#0040[]+^+[]64[]+^+[]12/11/2015 11:59:37 PM[]+^+[]18.8495562[] +=end-of-column=+
    /// // []#0080[]+^+[]128[]+^+[]12/12/2015 1:03:37 AM[]+^+[]21.9911489[] +=end-of-column=+
    /// // []#0100[]+^+[]256[]+^+[]12/12/2015 3:11:37 AM[]+^+[]25.1327416[] +=end-of-column=+
    /// // []#0200[]+^+[]512[]+^+[]12/12/2015 7:27:37 AM[]+^+[]28.2743343[] +=end-of-column=+
    /// // []#0400[]+^+[]1024[]+^+[]12/12/2015 3:59:37 PM[]+^+[]31.415927[] +=end-of-column=+
    /// // []#0800[]+^+[]2048[]+^+[]12/13/2015 9:03:37 AM[]+^+[]34.5575197[] +=end-of-column=+
    /// // []#1000[]+^+[]4096[]+^+[]12/14/2015 7:11:37 PM[]+^+[]37.6991124[] +=end-of-column=+
    /// // []#2000[]+^+[]8192[]+^+[]12/17/2015 3:27:37 PM[]+^+[]40.8407051[] +=end-of-column=+
    /// // []#4000[]+^+[]16384[]+^+[]12/23/2015 7:59:37 AM[]+^+[]43.9822978[] +=end-of-column=+
    /// // []#8000[]+^+[]32768[]+^+[]1/3/2016 5:03:37 PM[]+^+[]47.1238905[]
    /// // ------------------------------------
    /// // 
    /// // table3: Enclosure      ='"'
    /// // table3: ColumnSeperator=','
    /// // table3: RowSeperator   ='
    /// // '
    /// // Original: UseColumnDefault
    /// // ------------------------------------
    /// // "Id","Integer","Date","Double"
    /// // "#0001","1","12/11/2015 10:56:37 PM","0"
    /// // "#0002","2","12/11/2015 10:57:37 PM","3.1415927"
    /// // "#0004","4","12/11/2015 10:59:37 PM","6.2831854"
    /// // "#0008","8","12/11/2015 11:03:37 PM","9.4247781"
    /// // "#0010","16","12/11/2015 11:11:37 PM","12.5663708"
    /// // "#0020","32","12/11/2015 11:27:37 PM","15.7079635"
    /// // "#0040","64","12/11/2015 11:59:37 PM","18.8495562"
    /// // "#0080","128","12/12/2015 1:03:37 AM","21.9911489"
    /// // "#0100","256","12/12/2015 3:11:37 AM","25.1327416"
    /// // "#0200","512","12/12/2015 7:27:37 AM","28.2743343"
    /// // "#0400","1024","12/12/2015 3:59:37 PM","31.415927"
    /// // "#0800","2048","12/13/2015 9:03:37 AM","34.5575197"
    /// // "#1000","4096","12/14/2015 7:11:37 PM","37.6991124"
    /// // "#2000","8192","12/17/2015 3:27:37 PM","40.8407051"
    /// // "#4000","16384","12/23/2015 7:59:37 AM","43.9822978"
    /// // "#8000","32768","1/3/2016 5:03:37 PM","47.1238905"
    /// // ------------------------------------
    /// // 
    /// // Testing Original Table against Table3...
    /// // No errors 'should' be displayed here...
    /// // ------------------------------------
    /// // 
    /// // press anykey...
    /// </code>
    /// </example>    
    public class DsvTable
    {
        #region Events
        /// <summary>
        /// Occurs before a row is processed, allowing a receiver to cancel the addition of the row, or terminate processing the entire table.
        /// </summary>
        public event EventHandler<RowProcessedEventArgs> RowProcessed;
        /// <summary>
        /// Will raise the <see cref="RowProcessed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="RowProcessedEventArgs"/> to pass to the <see cref="RowProcessed"/> event.</param>
        protected void RaiseRowProcessedEvent(RowProcessedEventArgs e)
        {
            RowProcessed?.Invoke(this, e);
        }
        #endregion
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvTable"/> class using default settings.
        /// </summary>
        public DsvTable()
        {
            Columns = new DsvColumnCollection(this);
            Rows = new DsvRowCollection();
            _Settings = new DsvSettings();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvTable"/> class.
        /// </summary>
        /// <param name="settings">An instance of the <see cref="DsvSettings"/> to use as the initial settings values.</param>
        public DsvTable(DsvSettings settings)
            : this()
        {
            _Settings = settings ?? throw new ArgumentNullException("settings", "Parameter settings cannot be null.");
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvTable"/> class.
        /// </summary>
        /// <param name="settings">An instance of the <see cref="DsvSettings"/> to use as the initial settings value.</param>
        /// <param name="columns">An instance of the <see cref="DsvColumnCollection"/> containing the columns for this table.</param>
        internal DsvTable(DsvSettings settings,
                          DsvColumnCollection columns)
            : this(settings)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns", "Parameter columns cannot be null.");
            }
            foreach (var column in columns)
            {
                Columns.Add(column.ColumnName, column.Enclosed, column.FormatString, column.DefaultValue, column.AssumedType);
            }
        }

        #endregion
        #region Private Fields
        private DsvSettings _Settings = null;
        #endregion
        #region Public Methods
        /// <summary>
        /// Gets the collection of columns that belong to this table.
        /// </summary>
        /// <value>A <see cref="DsvColumnCollection"/> that contains the collection of <see cref="DsvColumn"/> objects for the table. An empty collection is returned if no <see cref="DsvColumn"/> objects exist.</value>
        public DsvColumnCollection Columns { get; private set; } = null;
        /// <summary>
        /// Gets the collection of <see cref="DsvRow"/>s that belong to this table.
        /// </summary>
        /// <value>A <see cref="DsvRowCollection"/> that contains <see cref="DsvRow"/> objects; otherwise an empty collection is returned if no <see cref="DsvRow"/> objects exist.</value>
        public DsvRowCollection Rows { get; } = null;
        /// <summary>
        /// Formatting parameters used to direct the reading and writing of delimiter separated values for this <see cref="DsvTable"/> instance.
        /// </summary>
        /// <value>A <see cref="DsvSettings"/> object for this <see cref="DsvTable"/> instance.</value>
        public DsvSettings Settings
        {
            get
            {
                return _Settings;
            }
            set
            {
                _Settings = value ?? throw new ArgumentNullException("settings", "Parameter settings cannot be null.");
            }
        }
        /// <summary>
        /// Creates a new <see cref="DsvRow"/> with the same schema as the <see cref="DsvTable"/>.
        /// </summary>
        /// <returns>A <see cref="DsvRow"/> with the same schema as the <see cref="DsvTable"/>.</returns>
        /// <remarks>You must use one of the NewRow methods to create new <see cref="DsvRow"/> objects with the same schema as the <see cref="DsvTable"/>. After creating a <see cref="DsvRow"/>, you can add it to the <see cref="DsvRowCollection"/>, through the <see cref="DsvTable"/> object's <see cref="Rows"/> property.</remarks>
        public DsvRow NewRow()
        {
            var row = new DsvRow(this);
            foreach (var item in Columns)
            {
                row.Add(item.DefaultValue);
            }
            return row;
        }
        /// <summary>
        /// Creates a new <see cref="DsvRow"/> with the same schema as the <see cref="DsvTable"/> and populates it with the given values.
        /// </summary>
        /// <param name="values">A list of values to populate the new <see cref="DsvRow"/> with.</param>
        /// <returns>A <see cref="DsvRow"/> with the same schema as the <see cref="DsvTable"/> and populated it with the given values.</returns>
        /// <remarks>You must use one of the NewRow methods to create new <see cref="DsvRow"/> objects with the same schema as the <see cref="DsvTable"/>. After creating a <see cref="DsvRow"/>, you can add it to the <see cref="DsvRowCollection"/>, through the <see cref="DsvTable"/> object's <see cref="Rows"/> property.</remarks>
        public DsvRow NewRow(params object[] values)
        {
            var row = new DsvRow(this);
            for (int i = 0; i < Columns.Count; i++)
            {
                if (i < values.Length)
                {
                    row.Add(values[i]);
                }
                else
                {
                    row.Add(Columns[i].DefaultValue);
                }
            }
            return row;
        }
        /// <summary>
        /// Gets the header row for this instance of the <see cref="DsvTable"/>.
        /// </summary>
        /// <returns>The header row for this instance of the <see cref="DsvTable"/>.</returns>
        public string HeaderRow() => HeaderRow(_Settings.Writer_ColumnEnclosingRule, false);
        /// <summary>
        /// Gets the header row for this instance of the <see cref="DsvTable"/>. The supplied parameters will override any settings in the <see cref="DsvTable"/>'s <see cref="DsvSettings"/>.
        /// </summary>
        /// <param name="columnEnclosingRule">Informs the writer how to enclose data. See <see cref="ColumnEnclosingRuleEnum"/>.</param>
        /// <returns>The header row for this instance of the <see cref="DsvTable"/>.</returns>
        public string HeaderRow(ColumnEnclosingRuleEnum columnEnclosingRule) => HeaderRow(columnEnclosingRule, false);
        /// <summary>
        /// Gets the header row for this instance of the <see cref="DsvTable"/>. The supplied parameters will override any settings in the <see cref="DsvTable"/>'s <see cref="DsvSettings"/>.
        /// </summary>
        /// <param name="includeRowSeperator">If <b>true</b>, the row separator will be appended to the end of the returned string; otherwise, if <b>false</b>, nothing will be appended to the end of the string.</param>
        /// <returns>The header row for this instance of the <see cref="DsvTable"/>.</returns>
        public string HeaderRow(bool includeRowSeperator) => HeaderRow(_Settings.Writer_ColumnEnclosingRule, includeRowSeperator);
        /// <summary>
        /// Gets the header row for this instance of the <see cref="DsvTable"/>. The supplied parameters will override any settings in the <see cref="DsvTable"/>'s <see cref="DsvSettings"/>.
        /// </summary>
        /// <param name="columnEnclosingRule">Informs the writer how to enclose data. See <see cref="ColumnEnclosingRuleEnum"/>.</param>
        /// <param name="includeRowSeperator">If <b>true</b>, the row separator will be appended to the end of the returned string; otherwise, if <b>false</b>, nothing will be appended to the end of the string.</param>
        /// <returns>The header row for this instance of the <see cref="DsvTable"/>.</returns>
        public string HeaderRow(ColumnEnclosingRuleEnum columnEnclosingRule, bool includeRowSeperator)
        {
            var results = new StringBuilder(2560);
            foreach (var item in Columns)
            {
                if (columnEnclosingRule == ColumnEnclosingRuleEnum.UseColumnDefault)
                {
                    // enclose default
                    results.AppendFormat("{0}{1}{0}{2}", ((item.Enclosed) ? _Settings.Enclosure : ""), item.ColumnName, _Settings.ColumnSeperator);
                }
                else if (columnEnclosingRule == ColumnEnclosingRuleEnum.EncloseNone)
                {
                    // enclose none
                    results.AppendFormat("{0}{1}", item.ColumnName, _Settings.ColumnSeperator);
                }
                else if (columnEnclosingRule == ColumnEnclosingRuleEnum.EncloseAll)
                {
                    // enclose all
                    results.AppendFormat("{0}{1}{0}{2}", _Settings.Enclosure, item.ColumnName, _Settings.ColumnSeperator);
                }
                else
                {
                    // enclose minimal
                    if (ContainsSeparatorsOrEnclosures(item.ColumnName))
                    {
                        // enclose it
                        results.AppendFormat("{0}{1}{0}{2}", _Settings.Enclosure, item.ColumnName, _Settings.ColumnSeperator);
                    }
                    else
                    {
                        // do not enclose it
                        results.AppendFormat("{0}{1}", item.ColumnName, _Settings.ColumnSeperator);
                    }
                }
            }
            if (results.Length > 0)
            {
                results.Length -= _Settings.ColumnSeperator.Length;
            }
            if (includeRowSeperator)
            {
                results.Append(_Settings.RowSeperator);
            }
            return results.ToString();
        }
        // ******** Read, Write, ToDataTable Methods
        /// <summary>
        /// Writes the delimiter separated values to a string.
        /// </summary>
        /// <returns>A string containing the delimiter separated values.</returns>
        public string WriteString() => WriteString(_Settings.Writer_IncludeHeaderRow, _Settings.Writer_ColumnEnclosingRule);
        /// <summary>
        /// Writes the delimiter separated values to a string.
        /// </summary>
        /// <param name="includeHeaderRow">Informs the writer to include the header row.</param>
        /// <param name="columnEnclosingRule">Informs the writer how to enclose data when writing the limited separated values. See <see cref="ColumnEnclosingRuleEnum"/>.</param>
        /// <returns>A string containing the delimiter separated values.</returns>
        public string WriteString(bool includeHeaderRow,
                                  ColumnEnclosingRuleEnum columnEnclosingRule)
        {
            var result = new StringBuilder(2560);
            if (includeHeaderRow)
            {
                result.Append(HeaderRow(columnEnclosingRule, includeHeaderRow));
            }
            foreach (var row in Rows)
            {
                result.Append(ProcessRowIntoString(row, columnEnclosingRule, true));
            }
            if (result.Length >= _Settings.RowSeperator.Length)
            {
                result.Length -= _Settings.RowSeperator.Length;
            }
            return result.ToString();
        }
        /// <summary>
        /// Writes the delimiter separated values to a byte array.
        /// </summary>
        /// <returns>A byte array containing the delimiter separated values.</returns>
        public byte[] WriteByteArray() => System.Text.Encoding.Unicode.GetBytes(WriteString());
        /// <summary>
        /// Writes the delimiter separated values to a byte array.
        /// </summary>
        /// <param name="includeHeaderRow">Informs the writer to include the header row.</param>
        /// <param name="columnEnclosingRule">Informs the writer how to enclose data when writing the limited separated values. See <see cref="ColumnEnclosingRuleEnum"/>.</param>
        /// <returns>A byte array containing the delimiter separated values.</returns>
        public byte[] WriteByteArray(bool includeHeaderRow,
                                     ColumnEnclosingRuleEnum columnEnclosingRule) => System.Text.Encoding.Unicode.GetBytes(WriteString(includeHeaderRow, columnEnclosingRule));
        /// <summary>
        /// Writes the delimiter separated values to a stream.
        /// </summary>
        /// <param name="stream">The stream to write the delimiter separated values to.</param>
        /// <returns>An integer representing the number of bytes written to the stream.</returns>
        public int WriteStream(System.IO.Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream", "Parameter stream cannot be null.");
            }
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Parameter stream is not a writable stream.", "stream");
            }
            var bytArr = WriteByteArray();
            if (bytArr != null)
            {
                stream.Write(bytArr, 0, bytArr.Length);
                return bytArr.Length;
            }
            return 0;
        }
        /// <summary>
        /// Writes the delimiter separated values to a stream.
        /// </summary>
        /// <param name="stream">The stream to write the delimiter separated values to.</param>
        /// <param name="includeHeaderRow">Informs the writer to include the header row.</param>
        /// <param name="columnEnclosingRule">Informs the writer how to enclose data when writing the limited separated values. See <see cref="ColumnEnclosingRuleEnum"/>.</param>
        /// <returns>An integer representing the number of bytes written to the stream.</returns>
        public int WriteStream(System.IO.Stream stream,
                               bool includeHeaderRow,
                               ColumnEnclosingRuleEnum columnEnclosingRule)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream", "Parameter stream cannot be null.");
            }
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Parameter stream is not a writable stream.", "stream");
            }
            var bytArr = WriteByteArray(includeHeaderRow, columnEnclosingRule);
            if (bytArr != null)
            {
                stream.Write(bytArr, 0, bytArr.Length);
                return bytArr.Length;
            }
            return 0;
        }
        /// <summary>
        /// Writes the delimiter separated values to a file.
        /// </summary>
        /// <param name="filename">The name of the file to write the delimiter separated values to.</param>
        public void WriteFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename", "Parameter filename cannot be null or empty.");
            }
            using (var sw = new System.IO.StreamWriter(filename, false))
            {

                sw.Write(WriteString());
                sw.Flush();
                sw.Close();
            }
        }
        /// <summary>
        /// Writes the delimiter separated values to a file.
        /// </summary>
        /// <param name="filename">The name of the file to write the delimiter separated values to.</param>
        /// <param name="includeHeaderRow">Informs the writer to include the header row.</param>
        /// <param name="columnEnclosingRule">Informs the writer how to enclose data when writing the limited separated values. See <see cref="ColumnEnclosingRuleEnum"/>.</param>
        public void WriteFile(string filename,
                              bool includeHeaderRow,
                              ColumnEnclosingRuleEnum columnEnclosingRule)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename", "Parameter filename cannot be null or empty.");
            }
            using (var sw = new System.IO.StreamWriter(filename, false))
            {

                sw.Write(WriteString(includeHeaderRow, columnEnclosingRule));
                sw.Flush();
                sw.Close();
            }
        }
        /// <summary>
        /// Parses the source string, reading column data and row data into this instance.
        /// </summary>
        /// <param name="source">The string containing the delimited separated values.</param>
        public void ReadString(string source) => ReadString(source, _Settings.Reader_FirstRowIsHeader, _Settings.Reader_ParseHeaderRowAsColumns, _Settings.Reader_TrimWhitespace);
        /// <summary>
        /// Parses the source string, reading column data and row data into this instance.
        /// </summary>
        /// <param name="source">The string containing the delimited separated values.</param>
        /// <param name="firstRowIsHeaderRow">Informs the reader that the first row is a header row.</param>
        /// <param name="parseHeaderRowAsColumns">Informs the reader to parse the first row into a <see cref="DsvColumnCollection"/> of <see cref="DsvColumn"/>s.</param>
        /// <param name="trimWhiteSpace">Informs the reader to leave or remove any whitespace after reading the data. <b>True</b> will remove the white space; <b>false</b> will leave it.</param>
        public void ReadString(string source,
                               bool firstRowIsHeaderRow,
                               bool parseHeaderRowAsColumns,
                               bool trimWhiteSpace)
        {
            // check for errors
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullException("source", "Parameter source cannot be null.");
            }
            if ((!firstRowIsHeaderRow) && parseHeaderRowAsColumns)
            {
                throw new Exception("Incongruent; cannot parse the first row into columns if it is not a header row.");
            }
            // clear existing rows
            Rows.Clear();
            int index = 0;
            if (Columns.Count == 0)
            {
                if (firstRowIsHeaderRow)
                {
                    // skip index past first row
                    DsvColumnCollection columnHolder = null;
                    index = ParseHeaderColumns(source, index, trimWhiteSpace, ref columnHolder);
                    // parse first row into columns
                    if (parseHeaderRowAsColumns)
                    {
                        Columns = columnHolder;
                    }
                }
                else
                {
                    // auto determine columns from first row
                    DsvColumnCollection possibleColumnData = null;
                    var indexOfFirstRow = ParseHeaderColumns(source, index, trimWhiteSpace, ref possibleColumnData);
                    // add correct number of columns
                    if (possibleColumnData != null)
                    {
                        Columns = new DsvColumnCollection(this);
                        for (int possibleColumnIndex = 0; possibleColumnIndex < possibleColumnData.Count; possibleColumnIndex++)
                        {
                            Columns.Add(string.Format("Column{0}", possibleColumnIndex.ToString()),
                                                       possibleColumnData[possibleColumnIndex].Enclosed,
                                                       possibleColumnData[possibleColumnIndex].FormatString,
                                                       possibleColumnData[possibleColumnIndex].DefaultValue,
                                                       possibleColumnData[possibleColumnIndex].AssumedType);
                        }
                    }
                }
            }
            else
            {
                if (firstRowIsHeaderRow)
                {
                    // skip index past first row
                    DsvColumnCollection columnHolder = null;
                    index = ParseHeaderColumns(source, index, trimWhiteSpace, ref columnHolder);
                    // test for the correct number and names of columns
                    string errStr = "";
                    if ((columnHolder != null) && (columnHolder.Count == Columns.Count))
                    {
                        for (int i = 0; i < Columns.Count; i++)
                        {
                            if (!Columns[i].ColumnName.Equals(columnHolder[i].ColumnName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                errStr = string.Format("Source columns names mismatch with existing table columns names.");
                                break;
                            }
                        }
                    }
                    else
                    {
                        errStr = "Source columns mismatch with existing table columns.";
                    }
                    if (!string.IsNullOrWhiteSpace(errStr))
                    {
                        throw new Exception(errStr);
                    }
                }
            }
            // process rows from the source string
            do
            {
                var data = ParseRow(source, ref index, trimWhiteSpace);
                if ((data != null) && (data.Length > 0))
                {
                    var list = new List<object>();
                    for (int listIndex = 0; listIndex < Columns.Count; listIndex++)
                    {
                        object objValue = null;
                        if (listIndex < data.Length)
                        {
                            objValue = data[listIndex]._Value;
                        }
                        if (Columns[listIndex].AssumedType != typeof(string))
                        {
                            try
                            {
                                objValue = Convert.ChangeType(objValue, Columns[listIndex].AssumedType);
                            }
                            catch { }
                        }
                        list.Add(objValue);
                    }
                    var row = NewRow(list.ToArray());
                    var args = new RowProcessedEventArgs(row, Rows.Count + 1, (double)((double)index / (double)source.Length), false, false);
                    RaiseRowProcessedEvent(args);
                    if (args.CancelTableProcessing)
                    {
                        break;
                    }
                    if (!args.CancelRowAddition)
                    {
                        Rows.Add(row);
                    }
                }
            } while (index < source.Length);
        }
        /// <summary>
        /// Parses the byte array, reading column data and row data into this instance.
        /// </summary>
        /// <param name="source">The byte array containing the delimited separated values.</param>
        public void ReadByteArray(byte[] source) => ReadString(System.Text.Encoding.Unicode.GetString(source));
        /// <summary>
        /// Parses the byte array, reading column data and row data into this instance.
        /// </summary>
        /// <param name="source">The byte array containing the delimited separated values.</param>
        /// <param name="firstRowisHeaderRow">Informs the reader that the first row is a header row.</param>
        /// <param name="parseHeaderRowAsColumns">Informs the reader to parse the first row into a <see cref="DsvColumnCollection"/> of <see cref="DsvColumn"/>s.</param>
        /// <param name="trimWhiteSpace">Informs the reader to leave or remove any whitespace after reading the data. <b>True</b> will remove the white space; <b>false</b> will leave it.</param>
        public void ReadByteArray(byte[] source,
                                  bool firstRowisHeaderRow,
                                  bool parseHeaderRowAsColumns,
                                  bool trimWhiteSpace) => ReadString(System.Text.Encoding.Unicode.GetString(source), firstRowisHeaderRow, parseHeaderRowAsColumns, trimWhiteSpace);
        /// <summary>
        /// Parses the stream, reading column data and row data into this instance.
        /// </summary>
        /// <param name="stream">The stream containing the delimited separated values.</param>
        /// <param name="length">The number of bytes to read from the stream.</param>
        public void ReadStream(System.IO.Stream stream,
                               int length)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream", "Parameter stream cannot be null.");
            }
            if (!stream.CanRead)
            {
                throw new ArgumentException("Parameter stream is not a readable stream.", "stream");
            }
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);
            ReadByteArray(buffer);
        }
        /// <summary>
        /// Parses the stream, reading column data and row data into this instance.
        /// </summary>
        /// <param name="stream">The stream containing the delimited separated values.</param>
        /// <param name="length">The number of bytes to read from the stream.</param>
        /// <param name="firstRowisHeaderRow">Informs the reader that the first row is a header row.</param>
        /// <param name="parseHeaderRowAsColumns">Informs the reader to parse the first row into a <see cref="DsvColumnCollection"/> of <see cref="DsvColumn"/>s.</param>
        /// <param name="trimWhiteSpace">Informs the reader to leave or remove any whitespace after reading the data. <b>True</b> will remove the white space; <b>false</b> will leave it.</param>
        public void ReadStream(System.IO.Stream stream,
                               int length,
                               bool firstRowisHeaderRow,
                               bool parseHeaderRowAsColumns,
                               bool trimWhiteSpace)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream", "Parameter stream cannot be null.");
            }
            if (!stream.CanRead)
            {
                throw new ArgumentException("Parameter stream is not a readable stream.", "stream");
            }
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);
            ReadByteArray(buffer, firstRowisHeaderRow, parseHeaderRowAsColumns, trimWhiteSpace);
        }
        /// <summary>
        /// Opens the specified file, reading column data and row data into this instance.
        /// </summary>
        /// <param name="filename">The name of the file containing the delimited separated values.</param>
        public void ReadFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename", "Parameter filename cannot be null or empty.");
            }
            if (!System.IO.File.Exists(filename))
            {
                throw new System.IO.FileNotFoundException(string.Format("The file ({0}) cannot be found.", filename), "filename");
            }
            using (var sr = new System.IO.StreamReader(filename))
            {
                ReadString(sr.ReadToEnd());
            }
        }
        /// <summary>
        /// Opens the specified file, reading column data and row data into this instance.
        /// </summary>
        /// <param name="filename">The name of the file containing the delimited separated values.</param>
        /// <param name="firstRowisHeaderRow">Informs the reader that the first row is a header row.</param>
        /// <param name="parseHeaderRowAsColumns">Informs the reader to parse the first row into a <see cref="DsvColumnCollection"/> of <see cref="DsvColumn"/>s.</param>
        /// <param name="trimWhiteSpace">Informs the reader to leave or remove any whitespace after reading the data. <b>True</b> will remove the white space; <b>false</b> will leave it.</param>
        public void ReadFile(string filename,
                             bool firstRowisHeaderRow,
                             bool parseHeaderRowAsColumns,
                             bool trimWhiteSpace)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename", "Parameter filename cannot be null or empty.");
            }
            if (!System.IO.File.Exists(filename))
            {
                throw new System.IO.FileNotFoundException(string.Format("The file ({0}) cannot be found.", filename), "filename");
            }
            using (var sr = new System.IO.StreamReader(filename))
            {
                ReadString(sr.ReadToEnd(), firstRowisHeaderRow, parseHeaderRowAsColumns, trimWhiteSpace);
            }
        }
        #endregion
        #region Private Methods
        private ParseResults[] ParseRow(string source,
                                        ref int index,
                                        bool trimWhiteSpace)
        {
            // do nothing if remaining string(index) is empty
            if (string.IsNullOrWhiteSpace(source.Substring(index)))
            {
                index = source.Length;
                return null;
            }
            // parse row from source(index)
            var result = new List<ParseResults>();
            do
            {
                // parse column
                if (!((index >= source.Length) || (GetSubString(source, index, _Settings.RowSeperator.Length).StartsWith(_Settings.RowSeperator))))
                {
                    var wasEnclosed = false;
                    var columnStr = ParseColumn(source, ref index, ref wasEnclosed);
                    if (trimWhiteSpace && !string.IsNullOrWhiteSpace(columnStr))
                    {
                        columnStr = columnStr.Trim();
                    }
                    result.Add(new ParseResults(wasEnclosed, columnStr));
                }
            } while ((index < source.Length) && !(GetSubString(source, index, _Settings.RowSeperator.Length).StartsWith(_Settings.RowSeperator)));



            // skip all row separators
            if ((index < (source.Length - _Settings.RowSeperator.Length)) && (GetSubString(source, index, _Settings.RowSeperator.Length).StartsWith(_Settings.RowSeperator)))
            {
                index += _Settings.RowSeperator.Length;
            }
            return result.ToArray();
        }
        private string GetSubString(string source, int index, int length)
        {
            if ((index + length) < source.Length)
            {
                return source.Substring(index, length);
            }
            return "";
        }
        private string ParseColumn(string source,
                                   ref int index,
                                   ref bool wasEnclosed)
        {
            wasEnclosed = false;
            // check for null entry
            if ((index < source.Length) && (GetSubString(source, index, _Settings.ColumnSeperator.Length).StartsWith(_Settings.ColumnSeperator)))
            {
                index += _Settings.ColumnSeperator.Length;
                return null;
            }
            // parse column
            var result = new StringBuilder(2560);
            // determine action plan
            int workingIndex = index;
            int startIndex = -1;
            int endIndex = -1;
            while ((workingIndex < source.Length) &&
                   (!GetSubString(source, workingIndex, _Settings.ColumnSeperator.Length).StartsWith(_Settings.ColumnSeperator)) &&
                   (!GetSubString(source, workingIndex, _Settings.RowSeperator.Length).StartsWith(_Settings.RowSeperator)))
            {
                if (GetSubString(source, workingIndex, _Settings.Enclosure.Length).StartsWith(_Settings.Enclosure))
                {
                    startIndex = workingIndex + _Settings.Enclosure.Length;
                    wasEnclosed = true;
                    break;
                }
                ++workingIndex;
            }
            // work action plan
            if (startIndex == -1)
            {
                // search for the end of the column/row/string
                startIndex = index;
                endIndex = startIndex;
                while ((endIndex < source.Length) &&
                       (!GetSubString(source, endIndex, _Settings.ColumnSeperator.Length).StartsWith(_Settings.ColumnSeperator)) &&
                       (!GetSubString(source, endIndex, _Settings.RowSeperator.Length).StartsWith(_Settings.RowSeperator)))
                {
                    ++endIndex;
                }
                // set index
                index = endIndex;
                // move past the separator
                if ((GetSubString(source, index, _Settings.ColumnSeperator.Length).StartsWith(_Settings.ColumnSeperator)))
                {
                    index += _Settings.ColumnSeperator.Length;
                }
            }
            else
            {
                // search for ending enclosure
                endIndex = startIndex;
                bool foundEnd = false;
                int subIndex = 0;
                while (endIndex < source.Length)
                {
                    if (GetSubString(source, endIndex, _Settings.Enclosure.Length).StartsWith(_Settings.Enclosure))
                    {
                        subIndex = endIndex + _Settings.Enclosure.Length;
                        while (subIndex < source.Length)
                        {
                            if ((GetSubString(source, subIndex, _Settings.ColumnSeperator.Length).StartsWith(_Settings.ColumnSeperator)) ||
                                (GetSubString(source, subIndex, _Settings.RowSeperator.Length).StartsWith(_Settings.RowSeperator)))
                            {
                                // found end
                                foundEnd = true;
                                break;
                            }
                            else
                            {
                                // continue looking
                                endIndex = subIndex;
                                break;
                            }
                        }
                        if (foundEnd || (subIndex == source.Length))
                        {
                            break;
                        }
                    }
                    ++endIndex;
                }
                // set index
                if (subIndex == 0)
                {
                    // move past the separator
                    if (GetSubString(source, index - _Settings.ColumnSeperator.Length, _Settings.ColumnSeperator.Length).StartsWith(_Settings.ColumnSeperator))
                    {
                        index += _Settings.ColumnSeperator.Length;
                    }
                    //startIndex = index - _Settings.Enclosure.Length;
                    if (source.Substring(source.Length - _Settings.Enclosure.Length).StartsWith(_Settings.Enclosure))
                    {
                        endIndex -= _Settings.Enclosure.Length;
                    }
                    index = source.Length;
                }
                else
                {
                    index = subIndex;
                    // move past the separator
                    if (GetSubString(source, index, _Settings.ColumnSeperator.Length).StartsWith(_Settings.ColumnSeperator))
                    {
                        index += _Settings.ColumnSeperator.Length;
                    }
                }
            }
            // get column string
            return source.Substring(startIndex, endIndex - startIndex);
        }
        private int ParseHeaderColumns(string source,
                                       int index,
                                       bool trimWhiteSpace,
                                       ref DsvColumnCollection columns)
        {
            // clear columns
            columns = new DsvColumnCollection(this);
            // parse new columns
            var data = ParseRow(source, ref index, trimWhiteSpace);
            if ((data != null) && (data.Length > 0))
            {
                foreach (var columnData in data)
                {
                    if (columnData._WasEnclosed)
                    {
                        columns.Add(columnData._Value, true, "", null);
                    }
                    else
                    {
                        columns.Add(columnData._Value, false, "", null);
                    }
                }
            }
            return index;
        }
        private string ProcessRowIntoString(DsvRow row,
                                            ColumnEnclosingRuleEnum columnEnclosureRule,
                                            bool includeRowSeperator)
        {
            var result = new StringBuilder(2560);
            for (int index = 0; index < row.ItemArray.Length; index++)
            {
                object value = row[index];
                if (value == null)
                {
                    value = row.Parent.Columns[index].DefaultValue;
                }
                string valueStr = Convert.ToString(value);
                if (Columns[index].AssumedType != typeof(string))
                {
                    string formatString = string.Format("{{0:{0}}}", Columns[index].FormatString);
                    valueStr = string.Format(formatString, Convert.ChangeType(value, Columns[index].AssumedType));
                    //valueStr = string.Format(Columns[index].FormatString, Convert.ChangeType(value, Columns[index].AssumedType));
                }
                if (columnEnclosureRule == ColumnEnclosingRuleEnum.UseColumnDefault)
                {
                    // column default
                    result.AppendFormat("{0}{1}{0}{2}", (Columns[index].Enclosed ? _Settings.Enclosure : ""),
                                                        valueStr,
                                                        _Settings.ColumnSeperator);
                }
                else if (columnEnclosureRule == ColumnEnclosingRuleEnum.EncloseNone)
                {
                    // enclose none
                    result.AppendFormat("{0}{1}", valueStr,
                                                  _Settings.ColumnSeperator);
                }
                else if (columnEnclosureRule == ColumnEnclosingRuleEnum.EncloseAll)
                {
                    // enclose all
                    result.AppendFormat("{0}{1}{0}{2}", _Settings.Enclosure,
                                                        valueStr,
                                                        _Settings.ColumnSeperator);
                }
                else
                {
                    // enclose minimal
                    if (ContainsSeparatorsOrEnclosures(valueStr))
                    {
                        // enclose all
                        result.AppendFormat("{0}{1}{0}{2}", _Settings.Enclosure,
                                                            valueStr,
                                                            _Settings.ColumnSeperator);
                    }
                    else
                    {
                        // enclose none
                        result.AppendFormat("{0}{1}", valueStr,
                                                      _Settings.ColumnSeperator);
                    }
                }
            }
            if (result.Length >= _Settings.ColumnSeperator.Length)
            {
                result.Length -= _Settings.ColumnSeperator.Length;
            }
            if (includeRowSeperator)
            {
                result.Append(_Settings.RowSeperator);
            }
            return result.ToString();
        }
        private bool ContainsSeparatorsOrEnclosures(string source) => (source.Contains(_Settings.Enclosure) || source.Contains(_Settings.ColumnSeperator) || source.Contains(_Settings.RowSeperator));
        #endregion
        #region Private Class: ParseResults
        private class ParseResults
        {
            #region Ctor
            private ParseResults()
            {
            }
            public ParseResults(bool wasEnclosed, string value)
                : this()
            {
                _WasEnclosed = wasEnclosed;
                _Value = value;
            }
            #endregion
            #region Public Fields
            public bool _WasEnclosed = false;
            public string _Value = "";
            #endregion
            #region Overrides
            public override string ToString() => _Value;
            #endregion
        }
        #endregion
    }
}

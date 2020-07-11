using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.DelimiterSeparatedValues
{
    /// <summary>
    /// Provides data for the dodSON.Core.DelimiterSeperatedValues.RowProcessed event.
    /// </summary>
    /// <example>
    /// The following example will create a string containing a properly formatted comma-delimited values table; it will
    /// then create a Delimiter Separated Values table, register with the RowProcessed event, process the comma-delimited values
    /// string and display the results. Finally, it will select a few rows to display at random.
    /// <para/>
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     var rnd = new Random();
    ///     
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
    ///     var table = new dodSON.Core.DelimiterSeperatedValues.DsvTable(settings);
    ///     
    ///     // register with the table's RowProcessed event 
    ///     table.RowProcessed += new EventHandler&lt;dodSON.Core.DelimiterSeperatedValues.RowProcessedEventArgs&gt;(
    ///                             (object s, dodSON.Core.DelimiterSeperatedValues.RowProcessedEventArgs e) =&gt;
    ///                             {
    ///                                 Console.WriteLine("Processing Row# {0,2:0} {1,5:0}% complete", e.RowsProcessedCount, e.PercentComplete * 100);
    ///                             });
    ///     
    ///     // create data
    ///     var sourceDataBuilder = new StringBuilder(1024);
    ///     sourceDataBuilder.AppendFormat("Alpha{1}Beta{1}{0}Gamma{0}{1}Delta{1}Epsilon{2}", settings.Enclosure, 
    ///                                                                                       settings.ColumnSeperator, 
    ///                                                                                       settings.RowSeperator);
    ///     var totalRows = 16;
    ///     for (int i = 0; i &lt; totalRows; i++)
    ///     {
    ///         int alpha = i;
    ///         double beta = i * 3.1415927;
    ///         string gamma = string.Format("#{0:X5}", (int)(alpha * beta));
    ///         double delta = Math.Round(((double)alpha * beta), 3, MidpointRounding.AwayFromZero);
    ///         int epsilon = totalRows - i - 1;
    ///         sourceDataBuilder.AppendFormat("{0}{1}", alpha, settings.ColumnSeperator); // Alpha column
    ///         sourceDataBuilder.AppendFormat("{0}{1}", beta, settings.ColumnSeperator); // Beta column
    ///         sourceDataBuilder.AppendFormat("{1}{0}{1}{2}", gamma, settings.Enclosure, settings.ColumnSeperator); // Gamma column
    ///         sourceDataBuilder.AppendFormat("{0}{1}", delta, settings.ColumnSeperator); // Delta column
    ///         sourceDataBuilder.AppendFormat("{0}", epsilon); // Epsilon column
    ///         sourceDataBuilder.AppendFormat(settings.RowSeperator); // end of column
    ///     }
    ///     var sourceData = sourceDataBuilder.ToString();
    ///     
    ///     // use table to read generated string
    ///     table.ReadString(sourceData);
    ///     
    ///     // display results
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///     Console.WriteLine(sourceData);
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///     Console.WriteLine(table.WriteString());
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///     var randomRow = rnd.Next(0, totalRows);
    ///     Console.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}", table.Rows[randomRow]["Alpha"],
    ///                                                                table.Rows[randomRow]["Beta"],
    ///                                                                table.Rows[randomRow]["Gamma"],
    ///                                                                table.Rows[randomRow]["Delta"],
    ///                                                                table.Rows[randomRow]["Epsilon"]));
    ///     randomRow = rnd.Next(0, totalRows);
    ///     Console.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}", table.Rows[randomRow][0],
    ///                                                                table.Rows[randomRow][1],
    ///                                                                table.Rows[randomRow][2],
    ///                                                                table.Rows[randomRow][3],
    ///                                                                table.Rows[randomRow][4]));
    ///     randomRow = rnd.Next(0, totalRows);
    ///     Console.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}", table.Rows[randomRow]["Alpha"],
    ///                                                                table.Rows[randomRow]["Beta"],
    ///                                                                table.Rows[randomRow]["Gamma"],
    ///                                                                table.Rows[randomRow]["Delta"],
    ///                                                                table.Rows[randomRow]["Epsilon"]));
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // Processing Row#  1    10% complete
    /// // Processing Row#  2    15% complete
    /// // Processing Row#  3    21% complete
    /// // Processing Row#  4    27% complete
    /// // Processing Row#  5    33% complete
    /// // Processing Row#  6    39% complete
    /// // Processing Row#  7    45% complete
    /// // Processing Row#  8    51% complete
    /// // Processing Row#  9    57% complete
    /// // Processing Row# 10    63% complete
    /// // Processing Row# 11    69% complete
    /// // Processing Row# 12    75% complete
    /// // Processing Row# 13    81% complete
    /// // Processing Row# 14    88% complete
    /// // Processing Row# 15    94% complete
    /// // Processing Row# 16   100% complete
    /// // 
    /// // ------------------------------------
    /// // 
    /// // Alpha,Beta,"Gamma",Delta,Epsilon
    /// // 0,0,"#00000",0,15
    /// // 1,3.1415927,"#00003",3.142,14
    /// // 2,6.2831854,"#0000C",12.566,13
    /// // 3,9.4247781,"#0001C",28.274,12
    /// // 4,12.5663708,"#00032",50.265,11
    /// // 5,15.7079635,"#0004E",78.54,10
    /// // 6,18.8495562,"#00071",113.097,9
    /// // 7,21.9911489,"#00099",153.938,8
    /// // 8,25.1327416,"#000C9",201.062,7
    /// // 9,28.2743343,"#000FE",254.469,6
    /// // 10,31.415927,"#0013A",314.159,5
    /// // 11,34.5575197,"#0017C",380.133,4
    /// // 12,37.6991124,"#001C4",452.389,3
    /// // 13,40.8407051,"#00212",530.929,2
    /// // 14,43.9822978,"#00267",615.752,1
    /// // 15,47.1238905,"#002C2",706.858,0
    /// // 
    /// // 
    /// // ------------------------------------
    /// // 
    /// // Alpha,Beta,"Gamma",Delta,Epsilon
    /// // 0,0,"#00000",0,15
    /// // 1,3.1415927,"#00003",3.142,14
    /// // 2,6.2831854,"#0000C",12.566,13
    /// // 3,9.4247781,"#0001C",28.274,12
    /// // 4,12.5663708,"#00032",50.265,11
    /// // 5,15.7079635,"#0004E",78.54,10
    /// // 6,18.8495562,"#00071",113.097,9
    /// // 7,21.9911489,"#00099",153.938,8
    /// // 8,25.1327416,"#000C9",201.062,7
    /// // 9,28.2743343,"#000FE",254.469,6
    /// // 10,31.415927,"#0013A",314.159,5
    /// // 11,34.5575197,"#0017C",380.133,4
    /// // 12,37.6991124,"#001C4",452.389,3
    /// // 13,40.8407051,"#00212",530.929,2
    /// // 14,43.9822978,"#00267",615.752,1
    /// // 15,47.1238905,"#002C2",706.858,0
    /// // 
    /// // ------------------------------------
    /// // 
    /// // 3, 9.4247781, #0001C, 28.274, 12
    /// // 14, 43.9822978, #00267, 615.752, 1
    /// // 7, 21.9911489, #00099, 153.938, 8
    /// // 
    /// // ------------------------------------
    /// // press anykey...
    /// </code>
    /// </example>    
    public class RowProcessedEventArgs
        : EventArgs
    {
        #region Ctor
        private RowProcessedEventArgs()
            : base() { }
        /// <summary>
        /// Initializes a new instance of the RowProcessedEventArgs class.
        /// </summary>
        /// <param name="row">The processed row, about to be added to the table.</param>
        /// <param name="rowsProcessedCount">The number of rows processed.</param>
        /// <param name="percentComplete">A value between 0.0 and 1.0 representing the how much of the table has been processed.</param>
        /// <param name="cancelRowAddition">Cancels adding the <paramref name="row"/> to the table. <b>True</b> will cancel adding the row, <b>false</b> will add the row.</param>
        /// <param name="cancelTableProcessing">Cancels processing the table. <b>True</b> will cancel processing the table, <b>false</b> will continue processing the table.</param>
        public RowProcessedEventArgs(DsvRow row,
                                     int rowsProcessedCount,
                                     double percentComplete,
                                     bool cancelRowAddition,
                                     bool cancelTableProcessing)
            : this()
        {
            if (rowsProcessedCount < 0) { throw new ArgumentOutOfRangeException("rowsProcessedCount", "rowsProcessedCount must be 1 or greater. (x >= 1)"); }
            if ((percentComplete < 0) || (percentComplete > 1)) { throw new ArgumentOutOfRangeException("percentComplete", "percentComplete must be between 0 and 1, inclusively."); }
            Row = row ?? throw new ArgumentNullException("row");
            RowsProcessedCount = rowsProcessedCount;
            PercentComplete = percentComplete;
            CancelRowAddition = cancelRowAddition;
            CancelTableProcessing = cancelTableProcessing;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Cancels processing the table. <b>True</b> will cancel processing the table, <b>false</b> will continue processing the table.
        /// </summary>
        public bool CancelTableProcessing { get; set; } = false;
        /// <summary>
        /// Cancels adding the <see cref="Row"/> to the table. <b>True</b> will cancel adding the row, <b>false</b> will add the row.
        /// </summary>
        public bool CancelRowAddition { get; set; } = false;
        /// <summary>
        /// The processed row, about to be added to the table.
        /// </summary>
        public DsvRow Row { get; } = null;
        /// <summary>
        /// The number of rows processed.
        /// </summary>
        public int RowsProcessedCount { get; } = 0;
        /// <summary>
        /// A value between 0.0 and 1.0 representing the how much of the table has been processed.
        /// </summary>
        public double PercentComplete { get; } = 0.0;
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.DelimiterSeparatedValues
{
    /// <summary>
    /// Formatting parameters used to direct the reading and writing of delimiter separated values.
    /// </summary>
    [Serializable()]
    public class DsvSettings
        : Configuration.IConfigurable
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvSettings"/> class.
        /// </summary>
        public DsvSettings() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvSettings"/> class.
        /// </summary>
        /// <param name="rowSeperator">The string which separates the rows.</param>
        /// <param name="columnSeperator">The string which separates the columns.</param>
        /// <param name="enclosure">The string which encloses the columns.</param>
        /// <param name="reader_FirstRowIsHeader">Informs the reader that the first row is a header row.</param>
        /// <param name="reader_ParseHeaderRowAsColumns">Informs the reader to parse the first row into a <see cref="DsvColumnCollection"/> of <see cref="DsvColumn"/>s.</param>
        /// <param name="reader_TrimWhitespace">Informs the reader to leave or remove any whitespace after reading the data. <b>True</b> will remove the whitespace; <b>false</b> will leave it.</param>
        /// <param name="writer_IncludeHeaderRow">Informs the writer to include the header row when writing the limited separated values.</param>
        /// <param name="writer_ColumnEnclosingRule">Informs the writer have to enclose data when writing delimited separated values. See <see cref="ColumnEnclosingRuleEnum"/>.</param>
        public DsvSettings(string rowSeperator,
                           string columnSeperator,
                           string enclosure,
                           bool reader_FirstRowIsHeader,
                           bool reader_ParseHeaderRowAsColumns,
                           bool reader_TrimWhitespace,
                           bool writer_IncludeHeaderRow,
                           ColumnEnclosingRuleEnum writer_ColumnEnclosingRule)
            : this()
        {
            RowSeperator = rowSeperator;
            ColumnSeperator = columnSeperator;
            Enclosure = enclosure;
            Reader_FirstRowIsHeader = reader_FirstRowIsHeader;
            Reader_ParseHeaderRowAsColumns = reader_ParseHeaderRowAsColumns;
            Reader_TrimWhitespace = reader_TrimWhitespace;
            Writer_IncludeHeaderRow = writer_IncludeHeaderRow;
            Writer_ColumnEnclosingRule = writer_ColumnEnclosingRule;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvSettings"/> class.
        /// </summary>
        /// <param name="reader_FirstRowIsHeader">Informs the reader that the first row is a header row.</param>
        /// <param name="reader_ParseHeaderRowAsColumns">Informs the reader to parse the first row into a <see cref="DsvColumnCollection"/> of <see cref="DsvColumn"/>s.</param>
        /// <param name="reader_TrimWhitespace">Informs the reader to leave or remove any whitespace after reading the data. <b>True</b> will remove the whitespace; <b>false</b> will leave it.</param>
        /// <param name="writer_IncludeHeaderRow">Informs the writer to include the header row when writing the limited separated values.</param>
        /// <param name="writer_ColumnEnclosingRule">Informs the writer have to enclose data when writing delimited separated values. See <see cref="ColumnEnclosingRuleEnum"/>.</param>
        public DsvSettings(bool reader_FirstRowIsHeader,
                           bool reader_ParseHeaderRowAsColumns,
                           bool reader_TrimWhitespace,
                           bool writer_IncludeHeaderRow,
                           ColumnEnclosingRuleEnum writer_ColumnEnclosingRule)
            : this()
        {
            Reader_FirstRowIsHeader = reader_FirstRowIsHeader;
            Reader_ParseHeaderRowAsColumns = reader_ParseHeaderRowAsColumns;
            Reader_TrimWhitespace = reader_TrimWhitespace;
            Writer_IncludeHeaderRow = writer_IncludeHeaderRow;
            Writer_ColumnEnclosingRule = writer_ColumnEnclosingRule;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DsvSettings"/> class.
        /// </summary>
        /// <param name="settings">A <see cref="DsvSettings"/> instance whose value will be copied to this instance.</param>
        public DsvSettings(DsvSettings settings)
            : this()
        {
            RowSeperator = settings.RowSeperator;
            ColumnSeperator = settings.ColumnSeperator;
            Enclosure = settings.Enclosure;
            Reader_FirstRowIsHeader = settings.Reader_FirstRowIsHeader;
            Reader_ParseHeaderRowAsColumns = settings.Reader_ParseHeaderRowAsColumns;
            Reader_TrimWhitespace = settings.Reader_TrimWhitespace;
            Writer_IncludeHeaderRow = settings.Writer_IncludeHeaderRow;
            Writer_ColumnEnclosingRule = settings.Writer_ColumnEnclosingRule;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public DsvSettings(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "DsvSettings") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"DsvSettings\". Configuration Key={configuration.Key}", nameof(configuration)); }
            // settings
            RowSeperator = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "RowSeperator", typeof(string)).Value;
            ColumnSeperator = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ColumnSeperator", typeof(string)).Value;
            Enclosure = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Enclosure", typeof(string)).Value;
            Reader_FirstRowIsHeader = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Reader_FirstRowIsHeader", typeof(bool)).Value;
            Reader_ParseHeaderRowAsColumns = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Reader_ParseHeaderRowAsColumns", typeof(bool)).Value;
            Reader_TrimWhitespace = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Reader_TrimWhitespace", typeof(bool)).Value;
            Writer_IncludeHeaderRow = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Writer_IncludeHeaderRow", typeof(bool)).Value;
            Writer_ColumnEnclosingRule = (ColumnEnclosingRuleEnum)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Writer_ColumnEnclosingRule", typeof(ColumnEnclosingRuleEnum)).Value;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The string which separates the rows.
        /// </summary>
        public string RowSeperator { get; set; } = System.Environment.NewLine;
        /// <summary>
        /// The string which separates the columns.
        /// </summary>
        public string ColumnSeperator { get; set; } = ",";
        /// <summary>
        /// The string which encloses the columns.
        /// </summary>
        public string Enclosure { get; set; } = "\"";
        /// <summary>
        /// Informs the reader that the first row is a header row.
        /// </summary>
        public bool Reader_FirstRowIsHeader { get; set; } = true;
        /// <summary>
        /// Informs the reader to parse the first row into a <see cref="DsvColumnCollection"/> of <see cref="DsvColumn"/>s.
        /// </summary>
        public bool Reader_ParseHeaderRowAsColumns { get; set; } = true;
        /// <summary>
        /// Informs the reader to leave or remove any whitespace after reading the data. <b>True</b> will remove the whitespace; <b>false</b> will leave it.
        /// </summary>
        public bool Reader_TrimWhitespace { get; set; } = false;
        /// <summary>
        /// Informs the writer to include the header row when writing the delimited separated values.
        /// </summary>
        public bool Writer_IncludeHeaderRow { get; set; } = true;
        /// <summary>
        /// Informs the writer how to enclose data when writing delimited separated values. See <see cref="ColumnEnclosingRuleEnum"/>.
        /// </summary>
        public ColumnEnclosingRuleEnum Writer_ColumnEnclosingRule { get; set; } = ColumnEnclosingRuleEnum.UseColumnDefault;
        #endregion
        #region Configuration.IConfigurable Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("DsvSettings");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                result.Items.Add("RowSeperator", RowSeperator, RowSeperator.GetType());
                result.Items.Add("ColumnSeperator", ColumnSeperator, ColumnSeperator.GetType());
                result.Items.Add("Enclosure", Enclosure, Enclosure.GetType());
                result.Items.Add("Reader_FirstRowIsHeader", Reader_FirstRowIsHeader, Reader_FirstRowIsHeader.GetType());
                result.Items.Add("Reader_ParseHeaderRowAsColumns", Reader_ParseHeaderRowAsColumns, Reader_ParseHeaderRowAsColumns.GetType());
                result.Items.Add("Reader_TrimWhitespace", Reader_TrimWhitespace, Reader_TrimWhitespace.GetType());
                result.Items.Add("Writer_IncludeHeaderRow", Writer_IncludeHeaderRow, Writer_IncludeHeaderRow.GetType());
                result.Items.Add("Writer_ColumnEnclosingRule", Writer_ColumnEnclosingRule, Writer_ColumnEnclosingRule.GetType());
                return result;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Configuration
{

    // FYI: see https://en.wikipedia.org/wiki/INI_file


    // TODO: Fully support the INI File Format.
    //
    // ----------------
    //
    // These two settings may need to be added to the Shared/Helper classes:
    // 
    // Duplicate Groups should be Merged/Ignored/Replaced/Added (name-###)/Throw Exception (setting)
    // Duplicate Names should Ignored/Replaced/Added (name-###)/Throw Exception (setting)
    //
    // ----------------
    //
    // Ignore blank lines
    // Be sure that Group and Key comparisons are not case sensitive
    // Add support for comments
    //          Customizable Comment Character (setting)
    //          single-line implementation only (i.e. the comment character must be the first character in the line.)
    // Add support for customizable separators
    //          Element Separator (setting)
    //          bool SupportGroups (setting)
    //          Group Start Character (setting)
    //          Group Separator Character (setting)
    //          Group Stop Character (setting)
    // Add support for quoted Values
    //          That is Values with "  !explicit sting values!  ", i.e. whitespace and separators
    //          bool QuotedValues (setting)
    //          Start Quote Character (setting)
    //          Stop Quote Character (setting)
    // Duplicate key within a group
    //          Support either [ Throw Exception, Overwrite, Discard ]
    // Test that Duplicate Groups have their items merged
    // Add support for Escape Characters
    //          A Dictionary containing the <EscapeSequence> and the <ActualString>


    /// <summary>
    /// Defines methods to convert <see cref="IConfigurationGroup"/>s to and from a <see cref="StringBuilder"/> using basic INI style formatting semantics.
    /// </summary>
    /// <remarks>
    /// <para>The following shows how an INI file will be interpreted.</para>
    /// <para>The first file will be read by the system; the second file will be written.</para>
    /// <para><b>INPUT:</b></para>
    /// <code>
    /// DateCreated=System.DateTime=2016-11-11 17:01
    /// [01 AutoDetecting]
    ///         Type01=TRUE
    ///         Type02=False
    ///         Type03=X
    ///         Type04=Hello
    ///         Type05=-1024
    ///         Type06=314159265359
    ///         Type07=3.14159265359
    ///         Type08=11/11/2016 5:01pm
    ///         Type09=5A809246-1DF0-499F-A5C5-27080E7B7B62
    /// [02 AutoDetecting System^Version]
    ///         Type01=1
    ///         Type02=1.2
    ///         Type03=1.2.3
    ///         Type04=1.2.3.4
    /// [03 System^Version]
    ///         Type01=System.Version=1
    ///         Type02=System.Version=1.2
    ///         Type03=System.Version=1.2.3
    ///         Type04=System.Version=1.2.3.4
    /// [04 C# Style]
    ///         Type01=bool=True
    ///         Type02=boolean=FALSE
    ///         Type03=char=X
    ///         Type04=string=Hello
    ///         Type05=byte=5
    ///         Type06=sbyte=6
    ///         Type07=short=7
    ///         Type08=int16=8
    ///         Type09=ushort=9
    ///         Type10=uint16=10
    ///         Type11=int=11
    ///         Type12=int32=12
    ///         Type13=uint=13
    ///         Type14=uint32=14
    ///         Type15=long=15
    ///         Type16=int64=16
    ///         Type17=ulong=17
    ///         Type18=uint64=18
    ///         Type19=float=19
    ///         Type20=single=20
    ///         Type21=double=21
    ///         Type22=decimal=22
    ///         Type23=version=23
    ///         Type24=datetime=11/11/2016 17:01
    ///         Type25=datetimeoffset=11/11/2016 17:01
    ///         Type26=guid=4AA1DDD47CAE400189769780EE4A794B
    /// [05 ^NET Framework Types]
    ///         Type01=System.Boolean=True
    ///         Type02=System.Byte=2
    ///         Type03=System.SByte=3
    ///         Type04=System.Char=X
    ///         Type05=System.Decimal=5
    ///         Type06=System.Double=6
    ///         Type07=System.Single=7
    ///         Type08=System.Int32=8
    ///         Type09=System.UInt32=9
    ///         Type10=System.Int64=10
    ///         Type11=System.UInt64=11
    ///         Type12=System.Int16=12
    ///         Type13=System.UInt16=13
    ///         Type14=System.String=Hello
    ///         Type15=System.Version=15
    ///         Type16=System.DateTime=Nov 11, 2016 5:01 pm
    ///         Type17=System.DateTimeOffset=Nov 11, 2016 5:01 pm
    ///         Type18=System.Guid={0x1301f67d, 0xf210, 0x4e4d, {0xa9, 0xd8, 0x58, 0x9c, 0xa5, 0xe9, 0x6b, 0x61}}
    /// </code>
    /// <para><b>OUTPUT:</b></para>
    /// <code>
    /// DateCreated=System.DateTime=2016-11-11 5:01:00 PM
    /// [01 AutoDetecting]
    ///         Type01=System.Boolean=True
    ///         Type02=System.Boolean=False
    ///         Type03=System.String=X
    ///         Type04=System.String=Hello
    ///         Type05=System.Int32=-1024
    /// 	    Type06=System.Int64=314159265359
    /// 	    Type07=System.Double=3.14159265359
    /// 	    Type08=System.DateTimeOffset=2016-11-11 5:01:00 PM -06:00
    ///         Type09=System.Guid=5a809246-1df0-499f-a5c5-27080e7b7b62
    /// [02 AutoDetecting System ^ Version]
    ///         Type01=System.Int32=1
    /// 	    Type02=System.Double=1.2
    /// 	    Type03=System.Version=1.2.3
    /// 	    Type04=System.Version=1.2.3.4
    /// [03 System^Version]
    ///         Type01=System.Version=1.0
    /// 	    Type02=System.Version=1.2
    /// 	    Type03=System.Version=1.2.3
    /// 	    Type04=System.Version=1.2.3.4
    /// [04 C# Style]
    ///     	Type01=System.Boolean=True
    ///         Type02=System.Boolean=False
    ///         Type03=System.Char=X
    ///         Type04=System.String=Hello
    ///         Type05=System.Byte=5
    ///         Type06=System.SByte=6
    ///         Type07=System.Int16=7
    ///         Type08=System.Int16=8
    ///         Type09=System.UInt16=9
    ///         Type10=System.UInt16=10
    ///         Type11=System.Int32=11
    ///         Type12=System.Int32=12
    ///         Type13=System.UInt32=13
    ///         Type14=System.UInt32=14
    ///         Type15=System.Int64=15
    ///         Type16=System.Int64=16
    ///         Type17=System.UInt64=17
    ///         Type18=System.UInt64=18
    ///         Type19=System.Single=19
    ///         Type20=System.Single=20
    ///         Type21=System.Double=21
    ///         Type22=System.Decimal=22
    ///         Type23=System.Version=23.0
    ///         Type24=System.DateTime=2016-11-11 5:01:00 PM
    ///         Type25=System.DateTimeOffset=2016-11-11 5:01:00 PM -06:00
    ///         Type26=System.Guid=4aa1ddd4-7cae-4001-8976-9780ee4a794b
    /// [05 ^NET Framework Types]
    ///         Type01=System.Boolean=True
    ///         Type02=System.Byte=2
    ///     	Type03=System.SByte=3
    ///     	Type04=System.Char=X
    ///         Type05=System.Decimal=5
    ///     	Type06=System.Double=6
    ///     	Type07=System.Single=7
    ///     	Type08=System.Int32=8
    ///     	Type09=System.UInt32=9
    ///     	Type10=System.Int64=10
    ///     	Type11=System.UInt64=11
    ///     	Type12=System.Int16=12
    ///     	Type13=System.UInt16=13
    ///     	Type14=System.String=Hello
    ///     	Type15=System.Version=15.0
    ///     	Type16=System.DateTime=2016-11-11 5:01:00 PM
    ///     	Type17=System.DateTimeOffset=2016-11-11 5:01:00 PM -06:00
    ///     	Type18=System.Guid=1301f67d-f210-4e4d-a9d8-589ca5e96b61
    /// </code>
    /// </remarks>
    /// <example>
    /// <para>The following example will create an INI Serializer, load a file containing the INPUT text from above and deserialize it into an <see cref="IConfigurationGroup"/>; finally it will create an XML Serializer, convert the <see cref="IConfigurationGroup"/> into XML and output it.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// // load INI text from file
    /// // ##### be sure to change this to the proper file on your system #####
    /// string sourceINIFilename = @"C:\(WORKING)\Dev\ConfigurationSystem\ConfigurationTest.input.ini.txt";
    /// StringBuilder iniSource = new StringBuilder(System.IO.File.ReadAllText(sourceINIFilename));
    /// 
    /// // output INI
    /// Console.WriteLine("--------------------------------");
    /// Console.WriteLine(iniSource);
    /// 
    /// // create INI converter
    /// dodSON.Core.Configuration.IniConfigurationSerializer iniConverter = new dodSON.Core.Configuration.IniConfigurationSerializer();
    /// 
    /// // deserialize INI
    /// dodSON.Core.Configuration.IConfigurationGroup configuration = iniConverter.DeserializeGroup(iniSource);
    /// 
    /// // create XML serializer
    /// dodSON.Core.Configuration.XmlConfigurationSerializer xmlSerializer = new dodSON.Core.Configuration.XmlConfigurationSerializer();
    /// 
    /// // serialize into XML
    /// StringBuilder xmlString = xmlSerializer.SerializeGroup(configuration);
    /// 
    /// // output XML
    /// Console.WriteLine("--------------------------------");
    /// Console.WriteLine(xmlString);
    /// 
    /// // ----
    /// Console.WriteLine("--------------------------------");
    /// Console.WriteLine("press anykey&gt;");
    /// Console.ReadKey(true);
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// /// // --------------------------------
    /// // DateCreated=System.DateTime=2016-11-11 17:01
    /// // [01 AutoDetecting]
    /// //         Type01=TRUE
    /// //         Type02=False
    /// //         Type03=X
    /// //         Type04=Hello
    /// //         Type05=-1024
    /// //         Type06=314159265359
    /// //         Type07=3.14159265359
    /// //         Type08=11/11/2016 5:01pm
    /// //         Type09=5A809246-1DF0-499F-A5C5-27080E7B7B62
    /// // [02 AutoDetecting System^Version]
    /// //         Type01=1
    /// //         Type02=1.2
    /// //         Type03=1.2.3
    /// //         Type04=1.2.3.4
    /// // [03 System^Version]
    /// //         Type01=System.Version=1
    /// //         Type02=System.Version=1.2
    /// //         Type03=System.Version=1.2.3
    /// //         Type04=System.Version=1.2.3.4
    /// // [04 C# Style]
    /// //         Type01=bool=True
    /// //         Type02=boolean=FALSE
    /// //         Type03=char=X
    /// //         Type04=string=Hello
    /// //         Type05=byte=5
    /// //         Type06=sbyte=6
    /// //         Type07=short=7
    /// //         Type08=int16=8
    /// //         Type09=ushort=9
    /// //         Type10=uint16=10
    /// //         Type11=int=11
    /// //         Type12=int32=12
    /// //         Type13=uint=13
    /// //         Type14=uint32=14
    /// //         Type15=long=15
    /// //         Type16=int64=16
    /// //         Type17=ulong=17
    /// //         Type18=uint64=18
    /// //         Type19=float=19
    /// //         Type20=single=20
    /// //         Type21=double=21
    /// //         Type22=decimal=22
    /// //         Type23=version=23
    /// //         Type24=datetime=11=11=2016 17:01
    /// //         Type25=datetimeoffset=11/11/2016 17:01
    /// //         Type26=guid=4AA1DDD47CAE400189769780EE4A794B
    /// // [05 ^NET Framework Types]
    /// //         Type01=System.Boolean=True
    /// //         Type02=System.Byte=2
    /// //         Type03=System.SByte=3
    /// //         Type04=System.Char=X
    /// //         Type05=System.Decimal=5
    /// //         Type06=System.Double=6
    /// //         Type07=System.Single=7
    /// //         Type08=System.Int32=8
    /// //         Type09=System.UInt32=9
    /// //         Type10=System.Int64=10
    /// //         Type11=System.UInt64=11
    /// //         Type12=System.Int16=12
    /// //         Type13=System.UInt16=13
    /// //         Type14=System.String=Hello
    /// //         Type15=System.Version=15
    /// //         Type16=System.DateTime=Nov 11, 2016 5:01 pm
    /// //         Type17=System.DateTimeOffset=Nov 11, 2016 5:01 pm
    /// //         Type18=System.Guid={0x1301f67d, 0xf210, 0x4e4d, {0xa9, 0xd8, 0x58, 0x9c, 0xa5, 0xe9, 0x6b, 0x61}}
    /// // --------------------------------
    /// // &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// // &lt;group key=""&gt;
    /// //   &lt;items&gt;
    /// //     &lt;item key="DateCreated" type="System.DateTime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    /// //   &lt;/items&gt;
    /// //   &lt;groups&gt;
    /// //     &lt;group key="01 AutoDetecting"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="Type01" type="System.Boolean"&gt;True&lt;/item&gt;
    /// //         &lt;item key="Type02" type="System.Boolean"&gt;False&lt;/item&gt;
    /// //         &lt;item key="Type03" type="System.String"&gt;X&lt;/item&gt;
    /// //         &lt;item key="Type04" type="System.String"&gt;Hello&lt;/item&gt;
    /// //         &lt;item key="Type05" type="System.Int32"&gt;-1024&lt;/item&gt;
    /// //         &lt;item key="Type06" type="System.Int64"&gt;314159265359&lt;/item&gt;
    /// //         &lt;item key="Type07" type="System.Double"&gt;3.14159265359&lt;/item&gt;
    /// //         &lt;item key="Type08" type="System.DateTimeOffset"&gt;2016-11-11 5:01:00 PM -06:00&lt;/item&gt;
    /// //         &lt;item key="Type09" type="System.Guid"&gt;5a809246-1df0-499f-a5c5-27080e7b7b62&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //     &lt;group key="02 AutoDetecting System^Version"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="Type01" type="System.Int32"&gt;1&lt;/item&gt;
    /// //         &lt;item key="Type02" type="System.Double"&gt;1.2&lt;/item&gt;
    /// //         &lt;item key="Type03" type="System.Version"&gt;1.2.3&lt;/item&gt;
    /// //         &lt;item key="Type04" type="System.Version"&gt;1.2.3.4&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //     &lt;group key="03 System^Version"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="Type01" type="System.Version"&gt;1.0&lt;/item&gt;
    /// //         &lt;item key="Type02" type="System.Version"&gt;1.2&lt;/item&gt;
    /// //         &lt;item key="Type03" type="System.Version"&gt;1.2.3&lt;/item&gt;
    /// //         &lt;item key="Type04" type="System.Version"&gt;1.2.3.4&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //     &lt;group key="04 C# Style"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="Type01" type="System.Boolean"&gt;True&lt;/item&gt;
    /// //         &lt;item key="Type02" type="System.Boolean"&gt;False&lt;/item&gt;
    /// //         &lt;item key="Type03" type="System.Char"&gt;X&lt;/item&gt;
    /// //         &lt;item key="Type04" type="System.String"&gt;Hello&lt;/item&gt;
    /// //         &lt;item key="Type05" type="System.Byte"&gt;5&lt;/item&gt;
    /// //         &lt;item key="Type06" type="System.SByte"&gt;6&lt;/item&gt;
    /// //         &lt;item key="Type07" type="System.Int16"&gt;7&lt;/item&gt;
    /// //         &lt;item key="Type08" type="System.Int16"&gt;8&lt;/item&gt;
    /// //         &lt;item key="Type09" type="System.UInt16"&gt;9&lt;/item&gt;
    /// //         &lt;item key="Type10" type="System.UInt16"&gt;10&lt;/item&gt;
    /// //         &lt;item key="Type11" type="System.Int32"&gt;11&lt;/item&gt;
    /// //         &lt;item key="Type12" type="System.Int32"&gt;12&lt;/item&gt;
    /// //         &lt;item key="Type13" type="System.UInt32"&gt;13&lt;/item&gt;
    /// //         &lt;item key="Type14" type="System.UInt32"&gt;14&lt;/item&gt;
    /// //         &lt;item key="Type15" type="System.Int64"&gt;15&lt;/item&gt;
    /// //         &lt;item key="Type16" type="System.Int64"&gt;16&lt;/item&gt;
    /// //         &lt;item key="Type17" type="System.UInt64"&gt;17&lt;/item&gt;
    /// //         &lt;item key="Type18" type="System.UInt64"&gt;18&lt;/item&gt;
    /// //         &lt;item key="Type19" type="System.Single"&gt;19&lt;/item&gt;
    /// //         &lt;item key="Type20" type="System.Single"&gt;20&lt;/item&gt;
    /// //         &lt;item key="Type21" type="System.Double"&gt;21&lt;/item&gt;
    /// //         &lt;item key="Type22" type="System.Decimal"&gt;22&lt;/item&gt;
    /// //         &lt;item key="Type23" type="System.Version"&gt;23.0&lt;/item&gt;
    /// //         &lt;item key="Type24" type="System.DateTime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    /// //         &lt;item key="Type25" type="System.DateTimeOffset"&gt;2016-11-11 5:01:00 PM -06:00&lt;/item&gt;
    /// //         &lt;item key="Type26" type="System.Guid"&gt;4aa1ddd4-7cae-4001-8976-9780ee4a794b&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //     &lt;group key="05 ^NET Framework Types"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="Type01" type="System.Boolean"&gt;True&lt;/item&gt;
    /// //         &lt;item key="Type02" type="System.Byte"&gt;2&lt;/item&gt;
    /// //         &lt;item key="Type03" type="System.SByte"&gt;3&lt;/item&gt;
    /// //         &lt;item key="Type04" type="System.Char"&gt;X&lt;/item&gt;
    /// //         &lt;item key="Type05" type="System.Decimal"&gt;5&lt;/item&gt;
    /// //         &lt;item key="Type06" type="System.Double"&gt;6&lt;/item&gt;
    /// //         &lt;item key="Type07" type="System.Single"&gt;7&lt;/item&gt;
    /// //         &lt;item key="Type08" type="System.Int32"&gt;8&lt;/item&gt;
    /// //         &lt;item key="Type09" type="System.UInt32"&gt;9&lt;/item&gt;
    /// //         &lt;item key="Type10" type="System.Int64"&gt;10&lt;/item&gt;
    /// //         &lt;item key="Type11" type="System.UInt64"&gt;11&lt;/item&gt;
    /// //         &lt;item key="Type12" type="System.Int16"&gt;12&lt;/item&gt;
    /// //         &lt;item key="Type13" type="System.UInt16"&gt;13&lt;/item&gt;
    /// //         &lt;item key="Type14" type="System.String"&gt;Hello&lt;/item&gt;
    /// //         &lt;item key="Type15" type="System.Version"&gt;15.0&lt;/item&gt;
    /// //         &lt;item key="Type16" type="System.DateTime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    /// //         &lt;item key="Type17" type="System.DateTimeOffset"&gt;2016-11-11 5:01:00 PM -06:00&lt;/item&gt;
    /// //         &lt;item key="Type18" type="System.Guid"&gt;1301f67d-f210-4e4d-a9d8-589ca5e96b61&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //   &lt;/groups&gt;
    /// // &lt;/group&gt;
    /// // --------------------------------
    /// // press anykey&gt;
    /// </code>
    /// </example>
    public class IniConfigurationSerializer
        : IConfigurationSerializer<StringBuilder>
    {
        #region Public Static Methods
        /// <summary>
        /// Creates a clone of the <paramref name="source"/> using <see cref="IniConfigurationSerializer"/>.
        /// </summary>
        /// <param name="source">The <see cref="IConfigurationGroup"/> to clone.</param>
        /// <returns>A clone of the <paramref name="source"/> using <see cref="IniConfigurationSerializer"/>.</returns>
        public static IConfigurationGroup Clone(IConfigurationGroup source)
        {
            var serializer = new IniConfigurationSerializer();
            return serializer.Deserialize(serializer.Serialize(source));
        }
        /// <summary>
        /// Will load the <paramref name="filename"/> and try to deserialize it using an <see cref="IniConfigurationSerializer"/>.
        /// </summary>
        /// <param name="filename">The file to load and deserialize.</param>
        /// <returns>The <see cref="IConfigurationGroup"/> deserialized from the text in the <paramref name="filename"/>.</returns>
        public static IConfigurationGroup LoadFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException(nameof(filename)); }
            if (!System.IO.File.Exists(filename)) { throw new System.IO.FileNotFoundException($"File not found.", filename); }
            //
            var source = new StringBuilder(System.IO.File.ReadAllText(filename));
            return (new IniConfigurationSerializer()).Deserialize(source);
        }
        /// <summary>
        /// Will serialize the <paramref name="source"/> using an <see cref="IniConfigurationSerializer"/> and save it to the <paramref name="filename"/>.
        /// </summary>
        /// <param name="source">The <see cref="IConfigurationGroup"/> to serialize and save.</param>
        /// <param name="filename">The path and filename to save the <paramref name="source"/> to.</param>
        /// <param name="overwrite">Determines if the file should be written over if it already exists. <b>True</b> will overwrite the file; <b>false</b> will throw an exception.</param>
        /// <returns>Whether the save operation was successful. <b>True</b> indicates the save operation succeeded; <b>false</b> will be expressed by throwing an exception.</returns>
        public static void SaveFile(IConfigurationGroup source,
                                    string filename,
                                    bool overwrite)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException(nameof(filename)); }
            if (!overwrite && System.IO.File.Exists(filename)) { throw new ArgumentException($"File already exists.", nameof(overwrite)); }
            //
            var dude = (new IniConfigurationSerializer()).Serialize(source);
            System.IO.File.WriteAllText(filename, dude.ToString());
        }
        #endregion
        #region Public Constants
        /// <summary>
        /// The character used to separate the elements of an INI entry. 
        /// <para>Value = '='</para>
        /// <para><b>Example:</b> Key=Value  -or-  Key=DataType=Value</para>
        /// </summary>
        public static readonly char _IniElementSeparator_ = '=';
        /// <summary>
        /// The character used to separate the group names.
        /// <para>Value = '.'</para>
        /// <para><b>Example:</b> [root.sub1.level1]</para>
        /// </summary>
        public static readonly char _IniGroupNameSeparator_ = ConfigurationHelper.GroupNameSeparator;
        /// <summary>
        /// The character used to indicate the beginning of a group name.
        /// <para>Value = '['</para>
        /// <para><b>Example:</b> [root]</para>
        /// </summary>
        public static readonly string _IniGroupNameStartCharacter_ = "[";
        /// <summary>
        /// The character used to indicate the end of a group name.
        /// <para>Value = ']'</para>
        /// <para><b>Example:</b> [root]</para>
        /// </summary>
        public static readonly string _IniGroupNameStopCharacter_ = "]";
        /// <summary>
        /// The default number of spaces to add before each serialized item inside a group.
        /// <para>Value = 4</para>
        /// </summary>
        public static readonly int _IniDefaultIndentionDepth = 4;
        #endregion
        #region Ctor
        /// <summary>
        /// <para>Initializes a new instance of the IniConfigurationSerializer class.</para>
        /// </summary>
        /// <remarks>
        /// <para>Using this constructor will default to using a <see cref="dodSON.Core.Compression.DeflateStreamCompressor"/> for compression and decompression tasks.</para>
        /// <para>Type which cannot be serialized into strings will be converted into a compressed byte array then converted into a base 64 string. The deserializer will expect to reverse that process, by converting the string from a base 64 string and decompressing the resultant byte array; then it will convert that byte array into the expected type.</para>
        /// </remarks>
        /// <seealso cref="dodSON.Core.Compression">dodSON.Core.Compression Namespace</seealso>
        /// <seealso cref="dodSON.Core.Compression.DeflateStreamCompressor"/>
        public IniConfigurationSerializer()
        {
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public IniConfigurationSerializer(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "Serializer")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Serializer\". Configuration Key={configuration.Key}", nameof(configuration));
            }
        }
        #endregion
        #region Private Fields
        private readonly Compression.ICompressor _Compressor = new Compression.DeflateStreamCompressor();
        #endregion
        #region IConfigurationSerializer<string> Methods
        /// <summary>
        /// Converts the specified group to a <see cref="StringBuilder"/> type.
        /// </summary>
        /// <param name="configuration">The group to convert.</param>
        /// <returns>The group converted into a <see cref="StringBuilder"/> type.</returns>
        public StringBuilder Serialize(IConfigurationGroup configuration) => Serialize(configuration, true, _IniDefaultIndentionDepth);
        /// <summary>
        /// Converts the specified group to a <see cref="StringBuilder"/> type.
        /// </summary>
        /// <param name="configuration">The group to convert.</param>
        /// <param name="includeTypeInformation">Determines if the serialized items will contain <see cref="Type"/> information. <b>False</b> produces classic INI-style formatting.<br/><b>True</b>: {key}={type}={value}<br/><b>False</b>: {key}={value}</param>
        /// <param name="indentionDepth">The number of spaces to add before each serialized item inside a group.</param>
        /// <returns>The group converted into a <see cref="StringBuilder"/> type.</returns>
        public StringBuilder Serialize(IConfigurationGroup configuration, bool includeTypeInformation, int indentionDepth)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (indentionDepth < 0)
            {
                indentionDepth = 0;
            }
            var buffer = new StringBuilder(2560);
            InternalSerializeGroup("", configuration, buffer, includeTypeInformation, indentionDepth);
            while ((buffer.Length >= 2) &&
                   (buffer.ToString(buffer.Length - 2, 2).EndsWith(Environment.NewLine)))
            {
                buffer.Length -= 2;
            }
            return buffer;
        }
        /// <summary>
        /// Converts the specified group to a <see cref="StringBuilder"/> type.
        /// </summary>
        /// <param name="configuration">The group to convert.</param>
        /// <param name="includeTypeInformation">Determines if the serialized items will contain <see cref="Type"/> information. <b>False</b> produces classic INI-style formatting.<br/><b>True</b>: {key}={type}={value}<br/><b>False</b>: {key}={value}</param>
        /// <returns>The group converted into a <see cref="StringBuilder"/> type.</returns>
        public StringBuilder Serialize(IConfigurationGroup configuration, bool includeTypeInformation) => Serialize(configuration, includeTypeInformation, _IniDefaultIndentionDepth);
        /// <summary>
        /// Converts the specified <paramref name="source"/>, a <see cref="StringBuilder"/> type, to an <see cref="IConfigurationGroup"/>.
        /// </summary>
        /// <param name="source">The object, a <see cref="StringBuilder"/> type, to convert into an <see cref="IConfigurationGroup"/>.</param>
        /// <returns>The object converted into an <see cref="IConfigurationGroup"/>.</returns>
        public IConfigurationGroup Deserialize(StringBuilder source) => Deserialize(source, true, false);
        /// <summary>
        /// Converts the specified <paramref name="source"/>, a <see cref="StringBuilder"/> type, to an <see cref="IConfigurationGroup"/>.
        /// </summary>
        /// <param name="source">The object, a <see cref="StringBuilder"/> type, to convert into an <see cref="IConfigurationGroup"/>.</param>
        /// <param name="includeSubgroups">True to scan group names for the Group Name Separator and create sub groups; otherwise, false will ignore the Group Name Separator and use the entire string as the group name.</param>
        /// <param name="isClassicIni">if <b>true</b> the <paramref name="source"/> format will be interpreted as classic INI, {key}={value}; otherwise, <b>false</b> will interpret the file using the custom triple entry INI format, {key}={type}={value}</param>
        /// <returns>The object converted into an <see cref="IConfigurationGroup"/>.</returns>
        public IConfigurationGroup Deserialize(StringBuilder source, bool includeSubgroups, bool isClassicIni)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source.Length == 0)
            {
                throw new ArgumentNullException(nameof(source), $"Parameter {nameof(source)} cannot be an empty string.");
            }
            IConfigurationGroup rootGroup = new ConfigurationGroup("");
            IConfigurationGroup currentGroup = rootGroup;
            string candidateRootGroupName = null;
            foreach (var line in from x in source.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                 select x.Trim())
            {
                if (line.StartsWith(_IniGroupNameStartCharacter_) && line.EndsWith(_IniGroupNameStopCharacter_))
                {
                    // process group
                    var groupFullName = line.Substring(1, line.Length - 2);
                    if (includeSubgroups)
                    {
                        var possibleRootGroup = groupFullName;
                        if (possibleRootGroup.Contains(_IniGroupNameSeparator_))
                        {
                            possibleRootGroup = possibleRootGroup.Substring(0, line.IndexOf(_IniGroupNameSeparator_) - 1);
                        }
                        if (candidateRootGroupName == null)
                        {
                            candidateRootGroupName = possibleRootGroup;
                        }
                        else if (candidateRootGroupName != possibleRootGroup)
                        {
                            candidateRootGroupName = "";
                        }
                        // 
                        currentGroup = ConfigurationShared.FindGroup(rootGroup, groupFullName, _IniGroupNameSeparator_);
                    }
                    else
                    {
                        var newGroup = new ConfigurationGroup(groupFullName, !includeSubgroups);
                        rootGroup.Add(newGroup);
                        currentGroup = newGroup;
                    }
                    if (currentGroup == null)
                    {
                        currentGroup = rootGroup;
                    }
                }
                else
                {
                    // process item
                    if (currentGroup == null)
                    {
                        throw new InvalidOperationException($"There is no current configuration group. Somehow the group name was missed. Check the parameter {nameof(source)} for corruption or bad form.");
                    }
                    currentGroup.Items.Add(DeserializeItem(new StringBuilder(line), isClassicIni));
                }
            }
            if (!string.IsNullOrWhiteSpace(candidateRootGroupName))
            {
                ((IConfigurationGroupAdvanced)rootGroup).SetKey(candidateRootGroupName);
            }
            // check for subgroup with same name as root group
            var foundSubGroup = (from x in rootGroup
                                 where x.Key == rootGroup.Key
                                 select x).FirstOrDefault();
            if (foundSubGroup != null)
            {
                rootGroup = foundSubGroup;
                (rootGroup as IConfigurationGroupAdvanced).Parent = null;
            }
            //
            if ((rootGroup.Count == 1) && (rootGroup.ContainsKey(rootGroup.Key)) && (rootGroup[rootGroup.Key].Key == rootGroup.Key))
            {
            }
            //
            rootGroup.ClearDirty();
            rootGroup.ClearNew();
            return rootGroup;
        }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("Serializer");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                return result;
            }
        }
        #endregion
        #region Private Methods
        private void InternalSerializeGroup(string parentGroupAddress, IConfigurationGroup group, StringBuilder buffer, bool includeTypeInformation, int indentionDepth)
        {
            // get local INI Group Address
            var address = group.Key;
            if (!string.IsNullOrWhiteSpace(parentGroupAddress))
            {
                address = parentGroupAddress + _IniGroupNameSeparator_ + group.Key;
            }
            // create group header
            if (!string.IsNullOrWhiteSpace(address))
            {
                // group name
                buffer.AppendLine($"{_IniGroupNameStartCharacter_}{address}{_IniGroupNameStopCharacter_}");
            }
            // process items
            if (group.Items.Count > 0)
            {
                // serialize items
                var prepend = (string.IsNullOrWhiteSpace(address)) ? "" : (new string(' ', indentionDepth));
                foreach (var item in group.Items)
                {
                    buffer.AppendLine(prepend + InternalSerializeItem(item, includeTypeInformation));
                }
            }
            // process groups
            if (group.Count > 0)
            {
                foreach (var subGroup in group)
                {
                    InternalSerializeGroup(address, subGroup, buffer, includeTypeInformation, indentionDepth);
                }
            }
        }
        private StringBuilder InternalSerializeItem(IConfigurationItem item, bool includeTypeInformation)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (includeTypeInformation)
            {
                // full format
                var typeStr = ConfigurationShared.ConvertItemTypeToString(item);
                if (typeStr.Contains(_IniElementSeparator_))
                {
                    typeStr = "\"" + typeStr + "\"";
                }
                return new StringBuilder($"{item.Key}{_IniElementSeparator_}{typeStr}{_IniElementSeparator_}{ConfigurationShared.ConvertItemObjectToString(item, _Compressor)}");
            }
            else
            {
                // classic style
                return new StringBuilder($"{item.Key}{_IniElementSeparator_}{ConfigurationShared.ConvertItemObjectToString(item, _Compressor)}");
            }
        }
        /// <summary>
        /// Converts the specified <paramref name="source"/>, a <see cref="StringBuilder"/> type, to an <see cref="IConfigurationItem"/>.
        /// </summary>
        /// <param name="source">The object, a <see cref="StringBuilder"/> type, to convert into an <see cref="IConfigurationItem"/>.</param>
        /// <param name="isClassicIni">if <b>true</b> the <paramref name="source"/> format will be interpreted as classic INI, {key}={value}; otherwise, <b>false</b> will interpret the file using the custom triple entry INI format, {key}={type}={value}</param>
        /// <returns>The object converted into an <see cref="IConfigurationItem"/>.</returns>
        private IConfigurationItem DeserializeItem(StringBuilder source, bool isClassicIni)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source.Length == 0)
            {
                throw new ArgumentNullException(nameof(source), $"Parameter {nameof(source)} cannot be an empty string.");
            }
            if (!source.ToString().Contains(_IniElementSeparator_))
            {
                throw new ArgumentException($"Parameter {nameof(source)} must contain at least one {nameof(_IniElementSeparator_)}. ({nameof(_IniElementSeparator_)}='{_IniElementSeparator_}'); (source='{source}')", nameof(source));
            }
            var parts = new List<string>();
            var index = 0;
            parts.Add("");
            var insideQuote = false;
            foreach (var ch in source.ToString())
            {
                if (insideQuote)
                {
                    if (ch == '\"')
                    {
                        insideQuote = false;
                    }
                    else
                    {
                        parts[index] = parts[index] + ch;
                    }
                }
                else
                {
                    if (ch == '\"')
                    {
                        insideQuote = true;
                    }
                    else
                    {
                        if ((ch == _IniElementSeparator_) && (index < 2))
                        {
                            ++index;
                            parts.Add("");
                        }
                        else
                        {
                            parts[index] = parts[index] + ch;
                        }
                    }
                }
            }
            if (isClassicIni)
            {

                // FYI: classic INI has a problem with correctly auto-detecting the proper types.
                //      specifically, it thinks TYPEs are STRINGs and this is a problem for the Configuration Instantiation functions.


                // {key}={value}
                string key = parts[0];
                string content = "";
                if (parts?.Count == 2)
                {
                    content = parts[1];
                }
                if (parts?.Count == 3)
                {
                    content = parts[1] + parts[2];
                }
                // **** AUTO-DETECT
                return ConfigurationShared.CreateConfigurationItem(key, content);
            }
            else
            {
                if (parts?.Count == 3)
                {
                    // **** TYPE PROVIDED
                    // contains pattern: {key}={type}={value}
                    string key = parts[0];
                    string valueTypeName = parts[1];
                    string content = parts[2];
                    return ConfigurationShared.CreateConfigurationItem(key, valueTypeName, content, _Compressor);
                }
                else if (parts?.Count == 2)
                {
                    // **** AUTO-DETECT
                    // contains pattern: {key}={value}
                    string key = parts[0];
                    string content = parts[1];
                    return ConfigurationShared.CreateConfigurationItem(key, content);
                }
            }
            throw new ArgumentException($"Parameter {nameof(source)} format error. (source='{source}')", nameof(source));
        }
        #endregion
    }
}

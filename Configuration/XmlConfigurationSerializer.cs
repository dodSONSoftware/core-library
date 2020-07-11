using System;
using System.Text;
using System.Xml;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Defines methods to convert <see cref="IConfigurationGroup"/>s to and from a <see cref="StringBuilder"/> using standard XML formatting.
    /// </summary>
    /// <remarks>
    /// <para>The following shows how an XML file will be interpreted.</para>
    /// <para>The first file will be read by the system; the second file will be written.</para>
    /// <para><b>INPUT:</b></para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key=""&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="DateCreated" type="System.DateTime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="01 AutoDetecting"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type01"&gt;True&lt;/item&gt;
    ///         &lt;item key="Type02"&gt;FALSE&lt;/item&gt;
    ///         &lt;item key="Type03"&gt;X&lt;/item&gt;
    ///         &lt;item key="Type04"&gt;Hello&lt;/item&gt;
    ///         &lt;item key="Type05"&gt;-1024&lt;/item&gt;
    ///         &lt;item key="Type06"&gt;314159265359&lt;/item&gt;
    ///         &lt;item key="Type07"&gt;3.14159265359&lt;/item&gt;
    ///         &lt;item key="Type08"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    ///         &lt;item key="Type09"&gt;5A809246-1DF0-499F-A5C5-27080E7B7B62&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="02 AutoDetecting System^Version"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type01"&gt;1&lt;/item&gt;
    ///         &lt;item key="Type02"&gt;1.2&lt;/item&gt;
    ///         &lt;item key="Type03"&gt;1.2.3&lt;/item&gt;
    ///         &lt;item key="Type04"&gt;1.2.3.4&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="03 System^Version"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type01" type="System.Version"&gt;1&lt;/item&gt;
    ///         &lt;item key="Type02" type="System.Version"&gt;1.2&lt;/item&gt;
    ///         &lt;item key="Type03" type="System.Version"&gt;1.2.3&lt;/item&gt;
    ///         &lt;item key="Type04" type="System.Version"&gt;1.2.3.4&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="04 C# Style"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type01" type="bool"&gt;True&lt;/item&gt;
    ///         &lt;item key="Type02" type="boolean"&gt;False&lt;/item&gt;
    ///         &lt;item key="Type03" type="char"&gt;X&lt;/item&gt;
    ///         &lt;item key="Type04" type="string"&gt;Hello&lt;/item&gt;
    ///         &lt;item key="Type05" type="byte"&gt;5&lt;/item&gt;
    ///         &lt;item key="Type06" type="sbyte"&gt;6&lt;/item&gt;
    ///         &lt;item key="Type07" type="short"&gt;7&lt;/item&gt;
    ///         &lt;item key="Type08" type="int16"&gt;8&lt;/item&gt;
    ///         &lt;item key="Type09" type="ushort"&gt;9&lt;/item&gt;
    ///         &lt;item key="Type10" type="uint16"&gt;10&lt;/item&gt;
    ///         &lt;item key="Type11" type="int"&gt;11&lt;/item&gt;
    ///         &lt;item key="Type12" type="int32"&gt;12&lt;/item&gt;
    ///         &lt;item key="Type13" type="uint"&gt;13&lt;/item&gt;
    ///         &lt;item key="Type14" type="uint32"&gt;14&lt;/item&gt;
    ///         &lt;item key="Type15" type="long"&gt;15&lt;/item&gt;
    ///         &lt;item key="Type16" type="int64"&gt;16&lt;/item&gt;
    ///         &lt;item key="Type17" type="ulong"&gt;17&lt;/item&gt;
    ///         &lt;item key="Type18" type="uint64"&gt;18&lt;/item&gt;
    ///         &lt;item key="Type19" type="float"&gt;19&lt;/item&gt;
    ///         &lt;item key="Type20" type="single"&gt;20&lt;/item&gt;
    ///         &lt;item key="Type21" type="double"&gt;21&lt;/item&gt;
    ///         &lt;item key="Type22" type="decimal"&gt;22&lt;/item&gt;
    ///         &lt;item key="Type23" type="version"&gt;23&lt;/item&gt;
    ///         &lt;item key="Type24" type="datetime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    ///         &lt;item key="Type25" type="datetimeoffset"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    ///         &lt;item key="Type26" type="guid"&gt;4AA1DDD47CAE400189769780EE4A794B&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="05 ^NET Framework Types"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type01" type="System.Boolean"&gt;True&lt;/item&gt;
    ///         &lt;item key="Type02" type="System.Byte"&gt;2&lt;/item&gt;
    ///         &lt;item key="Type03" type="System.SByte"&gt;3&lt;/item&gt;
    ///         &lt;item key="Type04" type="System.Char"&gt;X&lt;/item&gt;
    ///         &lt;item key="Type05" type="System.Decimal"&gt;5&lt;/item&gt;
    ///         &lt;item key="Type06" type="System.Double"&gt;6&lt;/item&gt;
    ///         &lt;item key="Type07" type="System.Single"&gt;7&lt;/item&gt;
    ///         &lt;item key="Type08" type="System.Int32"&gt;8&lt;/item&gt;
    ///         &lt;item key="Type09" type="System.UInt32"&gt;9&lt;/item&gt;
    ///         &lt;item key="Type10" type="System.Int64"&gt;10&lt;/item&gt;
    ///         &lt;item key="Type11" type="System.UInt64"&gt;11&lt;/item&gt;
    ///         &lt;item key="Type12" type="System.Int16"&gt;12&lt;/item&gt;
    ///         &lt;item key="Type13" type="System.UInt16"&gt;13&lt;/item&gt;
    ///         &lt;item key="Type14" type="System.String"&gt;Hello&lt;/item&gt;
    ///         &lt;item key="Type15" type="System.Version"&gt;15&lt;/item&gt;
    ///         &lt;item key="Type16" type="System.DateTime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    ///         &lt;item key="Type17" type="System.DateTimeOffset"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    ///         &lt;item key="Type18" type="System.Guid"&gt;{0x1301f67d, 0xf210, 0x4e4d, {0xa9, 0xd8, 0x58, 0x9c, 0xa5, 0xe9, 0x6b, 0x61}}&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// <para><b>OUTPUT:</b></para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key=""&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="DateCreated" type="System.DateTime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="01 AutoDetecting"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type01" type="System.Boolean"&gt;True&lt;/item&gt;
    ///         &lt;item key="Type02" type="System.Boolean"&gt;False&lt;/item&gt;
    ///         &lt;item key="Type03" type="System.String"&gt;X&lt;/item&gt;
    ///         &lt;item key="Type04" type="System.String"&gt;Hello&lt;/item&gt;
    ///         &lt;item key="Type05" type="System.Int32"&gt;-1024&lt;/item&gt;
    ///         &lt;item key="Type06" type="System.Int64"&gt;314159265359&lt;/item&gt;
    ///         &lt;item key="Type07" type="System.Double"&gt;3.14159265359&lt;/item&gt;
    ///         &lt;item key="Type08" type="System.DateTimeOffset"&gt;2016-11-11 5:01:00 PM -06:00&lt;/item&gt;
    ///         &lt;item key="Type09" type="System.Guid"&gt;5a809246-1df0-499f-a5c5-27080e7b7b62&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="02 AutoDetecting System^Version"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type01" type="System.Int32"&gt;1&lt;/item&gt;
    ///         &lt;item key="Type02" type="System.Double"&gt;1.2&lt;/item&gt;
    ///         &lt;item key="Type03" type="System.Version"&gt;1.2.3&lt;/item&gt;
    ///         &lt;item key="Type04" type="System.Version"&gt;1.2.3.4&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="03 System^Version"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type01" type="System.Version"&gt;1.0&lt;/item&gt;
    ///         &lt;item key="Type02" type="System.Version"&gt;1.2&lt;/item&gt;
    ///         &lt;item key="Type03" type="System.Version"&gt;1.2.3&lt;/item&gt;
    ///         &lt;item key="Type04" type="System.Version"&gt;1.2.3.4&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="04 C# Style"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type01" type="System.Boolean"&gt;True&lt;/item&gt;
    ///         &lt;item key="Type02" type="System.Boolean"&gt;False&lt;/item&gt;
    ///         &lt;item key="Type03" type="System.Char"&gt;X&lt;/item&gt;
    ///         &lt;item key="Type04" type="System.String"&gt;Hello&lt;/item&gt;
    ///         &lt;item key="Type05" type="System.Byte"&gt;5&lt;/item&gt;
    ///         &lt;item key="Type06" type="System.SByte"&gt;6&lt;/item&gt;
    ///         &lt;item key="Type07" type="System.Int16"&gt;7&lt;/item&gt;
    ///         &lt;item key="Type08" type="System.Int16"&gt;8&lt;/item&gt;
    ///         &lt;item key="Type09" type="System.UInt16"&gt;9&lt;/item&gt;
    ///         &lt;item key="Type10" type="System.UInt16"&gt;10&lt;/item&gt;
    ///         &lt;item key="Type11" type="System.Int32"&gt;11&lt;/item&gt;
    ///         &lt;item key="Type12" type="System.Int32"&gt;12&lt;/item&gt;
    ///         &lt;item key="Type13" type="System.UInt32"&gt;13&lt;/item&gt;
    ///         &lt;item key="Type14" type="System.UInt32"&gt;14&lt;/item&gt;
    ///         &lt;item key="Type15" type="System.Int64"&gt;15&lt;/item&gt;
    ///         &lt;item key="Type16" type="System.Int64"&gt;16&lt;/item&gt;
    ///         &lt;item key="Type17" type="System.UInt64"&gt;17&lt;/item&gt;
    ///         &lt;item key="Type18" type="System.UInt64"&gt;18&lt;/item&gt;
    ///         &lt;item key="Type19" type="System.Single"&gt;19&lt;/item&gt;
    ///         &lt;item key="Type20" type="System.Single"&gt;20&lt;/item&gt;
    ///         &lt;item key="Type21" type="System.Double"&gt;21&lt;/item&gt;
    ///         &lt;item key="Type22" type="System.Decimal"&gt;22&lt;/item&gt;
    ///         &lt;item key="Type23" type="System.Version"&gt;23.0&lt;/item&gt;
    ///         &lt;item key="Type24" type="System.DateTime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    ///         &lt;item key="Type25" type="System.DateTimeOffset"&gt;2016-11-11 5:01:00 PM -06:00&lt;/item&gt;
    ///         &lt;item key="Type26" type="System.Guid"&gt;4aa1ddd4-7cae-4001-8976-9780ee4a794b&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="05 ^NET Framework Types"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type01" type="System.Boolean"&gt;True&lt;/item&gt;
    ///         &lt;item key="Type02" type="System.Byte"&gt;2&lt;/item&gt;
    ///         &lt;item key="Type03" type="System.SByte"&gt;3&lt;/item&gt;
    ///         &lt;item key="Type04" type="System.Char"&gt;X&lt;/item&gt;
    ///         &lt;item key="Type05" type="System.Decimal"&gt;5&lt;/item&gt;
    ///         &lt;item key="Type06" type="System.Double"&gt;6&lt;/item&gt;
    ///         &lt;item key="Type07" type="System.Single"&gt;7&lt;/item&gt;
    ///         &lt;item key="Type08" type="System.Int32"&gt;8&lt;/item&gt;
    ///         &lt;item key="Type09" type="System.UInt32"&gt;9&lt;/item&gt;
    ///         &lt;item key="Type10" type="System.Int64"&gt;10&lt;/item&gt;
    ///         &lt;item key="Type11" type="System.UInt64"&gt;11&lt;/item&gt;
    ///         &lt;item key="Type12" type="System.Int16"&gt;12&lt;/item&gt;
    ///         &lt;item key="Type13" type="System.UInt16"&gt;13&lt;/item&gt;
    ///         &lt;item key="Type14" type="System.String"&gt;Hello&lt;/item&gt;
    ///         &lt;item key="Type15" type="System.Version"&gt;15.0&lt;/item&gt;
    ///         &lt;item key="Type16" type="System.DateTime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    ///         &lt;item key="Type17" type="System.DateTimeOffset"&gt;2016-11-11 5:01:00 PM -06:00&lt;/item&gt;
    ///         &lt;item key="Type18" type="System.Guid"&gt;1301f67d-f210-4e4d-a9d8-589ca5e96b61&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// </remarks>
    /// <example>
    /// <para>The following example will create an XML Serializer, load a file containing the INPUT text from above and deserialize it into an <see cref="IConfigurationGroup"/>; finally it will create an INI Serializer, convert the <see cref="IConfigurationGroup"/> into INI and output it.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// // load XML text from file
    /// string sourceXMLFilename = @"C:\(WORKING)\Dev\ConfigurationSystem\ConfigurationTest.input.xml.txt";
    /// StringBuilder xmlSource = new StringBuilder(System.IO.File.ReadAllText(sourceXMLFilename));
    /// 
    /// // output original XML
    /// Console.WriteLine("--------------------------------");
    /// Console.WriteLine(xmlSource);
    /// 
    /// // create XML converter
    /// dodSON.Core.Configuration.XmlConfigurationSerializer xmlConverter = new dodSON.Core.Configuration.XmlConfigurationSerializer();
    /// 
    /// // deserialize XML
    /// dodSON.Core.Configuration.IConfigurationGroup configuration = xmlConverter.DeserializeGroup(xmlSource);
    /// 
    /// // create INI serializer
    /// dodSON.Core.Configuration.IniConfigurationSerializer iniSerializer = new dodSON.Core.Configuration.IniConfigurationSerializer();
    /// 
    /// // serialize into INI
    /// StringBuilder iniString = iniSerializer.SerializeGroup(configuration);
    /// 
    /// // output INI
    /// Console.WriteLine("--------------------------------");
    /// Console.WriteLine(iniString);
    /// 
    /// // ----
    /// Console.WriteLine("--------------------------------");
    /// Console.WriteLine("press anykey&gt;");
    /// Console.ReadKey(true);
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // --------------------------------
    /// // &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// // &lt;group key=""&gt;
    /// //   &lt;items&gt;
    /// //     &lt;item key="DateCreated" type="System.DateTime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    /// //   &lt;/items&gt;
    /// //   &lt;groups&gt;
    /// //     &lt;group key="01 AutoDetecting"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="Type01"&gt;True&lt;/item&gt;
    /// //         &lt;item key="Type02"&gt;FALSE&lt;/item&gt;
    /// //         &lt;item key="Type03"&gt;X&lt;/item&gt;
    /// //         &lt;item key="Type04"&gt;Hello&lt;/item&gt;
    /// //         &lt;item key="Type05"&gt;-1024&lt;/item&gt;
    /// //         &lt;item key="Type06"&gt;314159265359&lt;/item&gt;
    /// //         &lt;item key="Type07"&gt;3.14159265359&lt;/item&gt;
    /// //         &lt;item key="Type08"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    /// //         &lt;item key="Type09"&gt;5A809246-1DF0-499F-A5C5-27080E7B7B62&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //     &lt;group key="02 AutoDetecting System^Version"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="Type01"&gt;1&lt;/item&gt;
    /// //         &lt;item key="Type02"&gt;1.2&lt;/item&gt;
    /// //         &lt;item key="Type03"&gt;1.2.3&lt;/item&gt;
    /// //         &lt;item key="Type04"&gt;1.2.3.4&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //     &lt;group key="03 System^Version"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="Type01" type="System.Version"&gt;1&lt;/item&gt;
    /// //         &lt;item key="Type02" type="System.Version"&gt;1.2&lt;/item&gt;
    /// //         &lt;item key="Type03" type="System.Version"&gt;1.2.3&lt;/item&gt;
    /// //         &lt;item key="Type04" type="System.Version"&gt;1.2.3.4&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //     &lt;group key="04 C# Style"&gt;
    /// //       &lt;items&gt;
    /// //         &lt;item key="Type01" type="bool"&gt;True&lt;/item&gt;
    /// //         &lt;item key="Type02" type="boolean"&gt;False&lt;/item&gt;
    /// //         &lt;item key="Type03" type="char"&gt;X&lt;/item&gt;
    /// //         &lt;item key="Type04" type="string"&gt;Hello&lt;/item&gt;
    /// //         &lt;item key="Type05" type="byte"&gt;5&lt;/item&gt;
    /// //         &lt;item key="Type06" type="sbyte"&gt;6&lt;/item&gt;
    /// //         &lt;item key="Type07" type="short"&gt;7&lt;/item&gt;
    /// //         &lt;item key="Type08" type="int16"&gt;8&lt;/item&gt;
    /// //         &lt;item key="Type09" type="ushort"&gt;9&lt;/item&gt;
    /// //         &lt;item key="Type10" type="uint16"&gt;10&lt;/item&gt;
    /// //         &lt;item key="Type11" type="int"&gt;11&lt;/item&gt;
    /// //         &lt;item key="Type12" type="int32"&gt;12&lt;/item&gt;
    /// //         &lt;item key="Type13" type="uint"&gt;13&lt;/item&gt;
    /// //         &lt;item key="Type14" type="uint32"&gt;14&lt;/item&gt;
    /// //         &lt;item key="Type15" type="long"&gt;15&lt;/item&gt;
    /// //         &lt;item key="Type16" type="int64"&gt;16&lt;/item&gt;
    /// //         &lt;item key="Type17" type="ulong"&gt;17&lt;/item&gt;
    /// //         &lt;item key="Type18" type="uint64"&gt;18&lt;/item&gt;
    /// //         &lt;item key="Type19" type="float"&gt;19&lt;/item&gt;
    /// //         &lt;item key="Type20" type="single"&gt;20&lt;/item&gt;
    /// //         &lt;item key="Type21" type="double"&gt;21&lt;/item&gt;
    /// //         &lt;item key="Type22" type="decimal"&gt;22&lt;/item&gt;
    /// //         &lt;item key="Type23" type="version"&gt;23&lt;/item&gt;
    /// //         &lt;item key="Type24" type="datetime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    /// //         &lt;item key="Type25" type="datetimeoffset"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    /// //         &lt;item key="Type26" type="guid"&gt;4AA1DDD47CAE400189769780EE4A794B&lt;/item&gt;
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
    /// //         &lt;item key="Type15" type="System.Version"&gt;15&lt;/item&gt;
    /// //         &lt;item key="Type16" type="System.DateTime"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    /// //         &lt;item key="Type17" type="System.DateTimeOffset"&gt;2016-11-11 5:01:00 PM&lt;/item&gt;
    /// //         &lt;item key="Type18" type="System.Guid"&gt;{0x1301f67d, 0xf210, 0x4e4d, {0xa9, 0xd8, 0x58, 0x9c, 0xa5, 0xe9, 0x6b, 0x61}}&lt;/item&gt;
    /// //       &lt;/items&gt;
    /// //     &lt;/group&gt;
    /// //   &lt;/groups&gt;
    /// // &lt;/group&gt;
    /// // --------------------------------
    /// // DateCreated=System.DateTime=2016-11-11 5:01:00 PM
    /// // [01 AutoDetecting]
    /// //         Type01=System.Boolean=True
    /// //         Type02=System.Boolean=False
    /// //         Type03=System.String=X
    /// //         Type04=System.String=Hello
    /// //         Type05=System.Int32=-1024
    /// //         Type06=System.Int64=314159265359
    /// //         Type07=System.Double=3.14159265359
    /// //         Type08=System.DateTimeOffset=2016-11-11 5:01:00 PM -06:00
    /// //         Type09=System.Guid=5a809246-1df0-499f-a5c5-27080e7b7b62
    /// // [02 AutoDetecting System^Version]
    /// //         Type01=System.Int32=1
    /// //         Type02=System.Double=1.2
    /// //         Type03=System.Version=1.2.3
    /// //         Type04=System.Version=1.2.3.4
    /// // [03 System^Version]
    /// //         Type01=System.Version=1.0
    /// //         Type02=System.Version=1.2
    /// //         Type03=System.Version=1.2.3
    /// //         Type04=System.Version=1.2.3.4
    /// // [04 C# Style]
    /// //         Type01=System.Boolean=True
    /// //         Type02=System.Boolean=False
    /// //         Type03=System.Char=X
    /// //         Type04=System.String=Hello
    /// //         Type05=System.Byte=5
    /// //         Type06=System.SByte=6
    /// //         Type07=System.Int16=7
    /// //         Type08=System.Int16=8
    /// //         Type09=System.UInt16=9
    /// //         Type10=System.UInt16=10
    /// //         Type11=System.Int32=11
    /// //         Type12=System.Int32=12
    /// //         Type13=System.UInt32=13
    /// //         Type14=System.UInt32=14
    /// //         Type15=System.Int64=15
    /// //         Type16=System.Int64=16
    /// //         Type17=System.UInt64=17
    /// //         Type18=System.UInt64=18
    /// //         Type19=System.Single=19
    /// //         Type20=System.Single=20
    /// //         Type21=System.Double=21
    /// //         Type22=System.Decimal=22
    /// //         Type23=System.Version=23.0
    /// //         Type24=System.DateTime=2016-11-11 5:01:00 PM
    /// //         Type25=System.DateTimeOffset=2016-11-11 5:01:00 PM -06:00
    /// //         Type26=System.Guid=4aa1ddd4-7cae-4001-8976-9780ee4a794b
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
    /// //         Type15=System.Version=15.0
    /// //         Type16=System.DateTime=2016-11-11 5:01:00 PM
    /// //         Type17=System.DateTimeOffset=2016-11-11 5:01:00 PM -06:00
    /// //         Type18=System.Guid=1301f67d-f210-4e4d-a9d8-589ca5e96b61
    /// // --------------------------------
    /// // press anykey&gt;
    /// </code>
    /// </example>
    public class XmlConfigurationSerializer
        : IConfigurationSerializer<StringBuilder>
    {
        #region Public Static Methods
        /// <summary>
        /// Creates a clone of the <paramref name="source"/> using <see cref="XmlConfigurationSerializer"/>.
        /// </summary>
        /// <param name="source">The <see cref="IConfigurationGroup"/> to clone.</param>
        /// <returns>A clone of the <paramref name="source"/> using <see cref="XmlConfigurationSerializer"/>.</returns>
        public static IConfigurationGroup Clone(IConfigurationGroup source)
        {
            var serializer = new XmlConfigurationSerializer();
            return serializer.Deserialize(serializer.Serialize(source));
        }
        /// <summary>
        /// Will load the <paramref name="filename"/> and try to deserialize it using an <see cref="XmlConfigurationSerializer"/>.
        /// </summary>
        /// <param name="filename">The file to load and deserialize.</param>
        /// <returns>The <see cref="IConfigurationGroup"/> deserialized from the text in the <paramref name="filename"/>.</returns>
        public static IConfigurationGroup LoadFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException(nameof(filename)); }
            if (!System.IO.File.Exists(filename)) { throw new System.IO.FileNotFoundException($"File not found.", filename); }
            //
            var source = new StringBuilder(System.IO.File.ReadAllText(filename));
            return (new XmlConfigurationSerializer()).Deserialize(source);
        }
        /// <summary>
        /// Will serialize the <paramref name="source"/> using an <see cref="XmlConfigurationSerializer"/> and save it to the <paramref name="filename"/>.
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
            var dude = (new XmlConfigurationSerializer()).Serialize(source);
            System.IO.File.WriteAllText(filename, dude.ToString());
        }
        #endregion
        #region Public Constants
        /// <summary>
        /// The name used to indicate a group element.
        /// <para>Value = 'group'</para>
        /// </summary>
        public static readonly string _XmlElementGroupName_ = "group";
        /// <summary>
        /// The character used to separate the group names.
        /// <para>Value = '.'</para>
        /// </summary>
        public static readonly char _XmlElementGroupKeyNameSeparator_ = ConfigurationHelper.GroupNameSeparator;
        /// <summary>
        /// The name of the group's key attribute.
        /// <para>Value = 'key'</para>
        /// </summary>
        public static readonly string _XmlElementGroupKeyName_ = "key";
        /// <summary>
        /// The name used to indicate a items element.
        /// <para>Value = 'items'</para>
        /// </summary>
        public static readonly string _XmlElementGroupItemsName_ = "items";
        /// <summary>
        /// The name used to indicate a groups element.
        /// <para>Value = 'groups'</para>
        /// </summary>
        public static readonly string _XmlElementGroupsName_ = "groups";
        /// <summary>
        /// The name used to indicate a item element.
        /// <para>Value = 'item'</para>
        /// </summary>
        public static readonly string _XmlElementItemName_ = "item";
        /// <summary>
        /// The name of the item's key attribute.
        /// <para>Value = 'key'</para>
        /// </summary>
        public static readonly string _XmlElementItemKeyName_ = "key";
        /// <summary>
        /// The name of the item's type attribute.
        /// <para>Value = 'type'</para>
        /// </summary>
        public static readonly string _XmlElementItemTypeName_ = "type";
        #endregion
        #region Ctor
        /// <summary>
        /// <para>Initializes a new instance of the XmlConfigurationSerializer class.</para>
        /// </summary>
        /// <remarks>
        /// <para>Using this constructor will default to using a <see cref="dodSON.Core.Compression.DeflateStreamCompressor"/> for compression and decompression tasks.</para>
        /// <para>Type which cannot be serialized into strings will be converted into a compressed byte array then converted into a base 64 string. The deserializer will expect to reverse that process, by converting the string from a base 64 string and decompressing the resultant byte array; then it will convert that byte array into the expected type.</para>
        /// </remarks>
        /// <seealso cref="dodSON.Core.Compression">dodSON.Core.Compression Namespace</seealso>
        /// <seealso cref="dodSON.Core.Compression.DeflateStreamCompressor"/>
        public XmlConfigurationSerializer() { }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public XmlConfigurationSerializer(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "Serializer") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Serializer\". Configuration Key={configuration.Key}", nameof(configuration)); }
        }
        #endregion
        #region Private Fields
        private readonly Compression.ICompressor _Compressor = new Compression.DeflateStreamCompressor();
        #endregion
        #region IConfigurationSerializer<StringBuilder> Methods
        /// <summary>
        /// Converts the specified group to a <see cref="StringBuilder"/> type.
        /// </summary>
        /// <param name="configuration">The group to convert.</param>
        /// <returns>The group converted into a <see cref="StringBuilder"/> type.</returns>
        public StringBuilder Serialize(IConfigurationGroup configuration)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            var buffer = new StringBuilder(2560);
            InternalSerializeGroup(configuration, buffer);
            return buffer;
        }
        /// <summary>
        /// Converts the specified <paramref name="source"/>, a <see cref="StringBuilder"/> type, to an <see cref="IConfigurationGroup"/>.
        /// </summary>
        /// <param name="source">The object, a <see cref="StringBuilder"/> type, to convert into an <see cref="IConfigurationGroup"/>.</param>
        /// <returns>The object converted into an <see cref="IConfigurationGroup"/>.</returns>
        public IConfigurationGroup Deserialize(StringBuilder source)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length == 0) { throw new ArgumentNullException(nameof(source), $"Parameter {nameof(source)} cannot be an empty string."); }
            IConfigurationGroup rootGroup = null;
            using (var sr = new System.IO.StringReader(source.ToString()))
            {
                using (var xr = System.Xml.XmlReader.Create(sr))
                {
                    while (xr.Read())
                    {
                        if (xr.IsStartElement(_XmlElementGroupName_))
                        {
                            // process group
                            var key = xr.GetAttribute(_XmlElementGroupKeyName_);
                            if (rootGroup == null)
                            {
                                rootGroup = new ConfigurationGroup(key);
                                rootGroup.ClearDirty();
                                rootGroup.ClearNew();
                            }
                            ProcessGroup(xr, rootGroup, rootGroup);
                        }
                    }
                }
            }
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
        private void InternalSerializeGroup(IConfigurationGroup group, StringBuilder buffer)
        {
            using (var xw = System.Xml.XmlWriter.Create(buffer, new System.Xml.XmlWriterSettings() { Indent = true, CheckCharacters = true, OmitXmlDeclaration = false }))
            {
                xw.WriteStartDocument();
                //
                RecursiveSerializeGroup(group, xw);
                //
                xw.WriteEndDocument();
            }
            // fix for changing UTF-16 to UTF-8
            buffer.Replace(System.Text.Encoding.Unicode.WebName, System.Text.Encoding.UTF8.WebName, 0, 56);
        }
        private void RecursiveSerializeGroup(IConfigurationGroup rootGroup, XmlWriter xw)
        {
            xw.WriteStartElement(_XmlElementGroupName_);
            xw.WriteAttributeString(_XmlElementGroupKeyName_, rootGroup.Key);
            if (rootGroup.Items.Count > 0)
            {
                xw.WriteStartElement(_XmlElementGroupItemsName_);
                foreach (var item in rootGroup.Items)
                {
                    xw.WriteStartElement(_XmlElementItemName_);
                    xw.WriteAttributeString(_XmlElementItemKeyName_, item.Key);
                    xw.WriteAttributeString(_XmlElementItemTypeName_, ConfigurationShared.ConvertItemTypeToString(item));
                    xw.WriteString(ConfigurationShared.ConvertItemObjectToString(item, _Compressor));
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
            }
            if (rootGroup.Count > 0)
            {
                xw.WriteStartElement(_XmlElementGroupsName_);
                foreach (var subGroup in rootGroup)
                {
                    RecursiveSerializeGroup(subGroup, xw);
                }
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }
        // --------
        private void ProcessGroup(XmlReader xr, IConfigurationGroup rootGroup, IConfigurationGroup currentGroup)
        {
            while (xr.Read())
            {
                if (xr.IsStartElement(_XmlElementGroupItemsName_))
                {
                    while (xr.Read())
                    {
                        if (xr.IsStartElement(_XmlElementItemName_))
                        {
                            currentGroup.Items.Add(ProcessItemSegment(xr));
                        }
                        else { break; }
                    }
                }
                else if (xr.IsStartElement(_XmlElementGroupsName_))
                {
                    while (xr.Read())
                    {
                        if (xr.IsStartElement(_XmlElementGroupName_))
                        {
                            var key = xr.GetAttribute(_XmlElementGroupKeyName_);
                            if (xr.IsEmptyElement) { currentGroup.Add(key); }
                            else { ProcessGroup(xr, rootGroup, ConfigurationShared.FindGroup(currentGroup, key, _XmlElementGroupKeyNameSeparator_)); }
                        }
                        else { break; }
                    }
                }
                else { break; }
            }
        }
        private IConfigurationItem ProcessItemSegment(XmlReader xr)
        {
            var key = xr.GetAttribute(_XmlElementItemKeyName_);
            var valueTypeName = xr.GetAttribute(_XmlElementItemTypeName_);
            var content = xr.ReadElementContentAsString();
            if (string.IsNullOrWhiteSpace(valueTypeName))
            {
                // **** AUTO-DETECT
                return ConfigurationShared.CreateConfigurationItem(key, content);
            }
            // **** TYPE PROVIDED
            return ConfigurationShared.CreateConfigurationItem(key, valueTypeName, content, _Compressor);
        }
        #endregion
    }
}

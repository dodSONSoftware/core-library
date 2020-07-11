using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Defines methods to convert <see cref="IConfigurationGroup"/>s to and from a <see cref="StringBuilder"/> using basic Comma-Separated-Values style formatting semantics.
    /// </summary>
    /// <remarks>
    /// <para>The following shows how an .CSV file will be interpreted.</para>
    /// <para>The first file will be read by the system; the second file will be written.</para>
    /// <para><b>INPUT:</b></para>
    /// <code>
    /// "Group","Item","Type","Value"
    /// "","DateCreated","System.DateTime","9/27/2017 5:01 PM"
    /// "01 AutoDetecting","Type01","","True"
    /// "01 AutoDetecting","Type02","","False"
    /// "01 AutoDetecting","Type03","","X"
    /// "01 AutoDetecting","Type04","","Hello"
    /// "01 AutoDetecting","Type05","","-1024"
    /// "01 AutoDetecting","Type06","","314159265359"
    /// "01 AutoDetecting","Type07","","3.14159265359"
    /// "01 AutoDetecting","Type08","","2017/09/27 17:01"
    /// "01 AutoDetecting","Type09","","5a809246-1df0-499f-a5c5-27080e7b7b62"
    /// "02 AutoDetecting System^Version","Type01","","1"
    /// "02 AutoDetecting System^Version","Type02","","1.2"
    /// "02 AutoDetecting System^Version","Type03","","1.2.3"
    /// "02 AutoDetecting System^Version","Type04","","1.2.3.4"
    /// "03 System^Version","Type01","System.Version","1.0"
    /// "03 System^Version","Type02","System.Version","1.2"
    /// "03 System^Version","Type03","System.Version","1.2.3"
    /// "03 System^Version","Type04","System.Version","1.2.3.4"
    /// "04 C# Style","Type01","bool","True"
    /// "04 C# Style","Type02","boolean","False"
    /// "04 C# Style","Type03","char","X"
    /// "04 C# Style","Type04","string","Hello"
    /// "04 C# Style","Type05","byte","5"
    /// "04 C# Style","Type06","sbyte","6"
    /// "04 C# Style","Type07","short","7"
    /// "04 C# Style","Type08","int16","8"
    /// "04 C# Style","Type09","ushort","9"
    /// "04 C# Style","Type10","uint16","10"
    /// "04 C# Style","Type11","int","11"
    /// "04 C# Style","Type12","int32","12"
    /// "04 C# Style","Type13","uint","13"
    /// "04 C# Style","Type14","uint32","14"
    /// "04 C# Style","Type15","long","15"
    /// "04 C# Style","Type16","int64","16"
    /// "04 C# Style","Type17","ulong","17"
    /// "04 C# Style","Type18","uint64","18"
    /// "04 C# Style","Type19","float","19"
    /// "04 C# Style","Type20","single","20"
    /// "04 C# Style","Type21","double","21"
    /// "04 C# Style","Type22","decimal","22"
    /// "04 C# Style","Type23","version","23.0"
    /// "04 C# Style","Type24","datetime","9/27/2017 17:01:00"
    /// "04 C# Style","Type25","datetimeoffset","2017-09-27 17:01"
    /// "04 C# Style","Type26","guid","4AA1DDD47CAE400189769780EE4A794B"
    /// "05 ^NET Framework Types","Type01","System.Boolean","True"
    /// "05 ^NET Framework Types","Type02","System.Byte","2"
    /// "05 ^NET Framework Types","Type03","System.SByte","3"
    /// "05 ^NET Framework Types","Type04","System.Char","X"
    /// "05 ^NET Framework Types","Type05","System.Decimal","5"
    /// "05 ^NET Framework Types","Type06","System.Double","6"
    /// "05 ^NET Framework Types","Type07","System.Single","7"
    /// "05 ^NET Framework Types","Type08","System.Int32","8"
    /// "05 ^NET Framework Types","Type09","System.UInt32","9"
    /// "05 ^NET Framework Types","Type10","System.Int64","10"
    /// "05 ^NET Framework Types","Type11","System.UInt64","11"
    /// "05 ^NET Framework Types","Type12","System.Int16","12"
    /// "05 ^NET Framework Types","Type13","System.UInt16","13"
    /// "05 ^NET Framework Types","Type14","System.String","Hello"
    /// "05 ^NET Framework Types","Type15","System.Version","15.0"
    /// "05 ^NET Framework Types","Type16","System.DateTime","9/27/2017 5:01 Pm"
    /// "05 ^NET Framework Types","Type17","System.DateTimeOffset","2017-09-27 17:01"
    /// "05 ^NET Framework Types","Type18","System.Guid","{0x1301f67d, 0xf210, 0x4e4d, {0xa9, 0xd8, 0x58, 0x9c, 0xa5, 0xe9, 0x6b, 0x61}}"
    /// </code>
    /// <para><b>OUTPUT:</b></para>
    /// <code>
    /// "Group","Item","Type","Value"
    /// "","DateCreated","System.DateTime","2017-09-27 5:01:00 PM"
    /// "01 AutoDetecting","Type01","System.Boolean","True"
    /// "01 AutoDetecting","Type02","System.Boolean","False"
    /// "01 AutoDetecting","Type03","System.String","X"
    /// "01 AutoDetecting","Type04","System.String","Hello"
    /// "01 AutoDetecting","Type05","System.Int32","-1024"
    /// "01 AutoDetecting","Type06","System.Int64","314159265359"
    /// "01 AutoDetecting","Type07","System.Double","3.14159265359"
    /// "01 AutoDetecting","Type08","System.DateTimeOffset","2017-09-27 5:01:00 PM -05:00"
    /// "01 AutoDetecting","Type09","System.Guid","5a809246-1df0-499f-a5c5-27080e7b7b62"
    /// "02 AutoDetecting System^Version","Type01","System.Int32","1"
    /// "02 AutoDetecting System^Version","Type02","System.Double","1.2"
    /// "02 AutoDetecting System^Version","Type03","System.Version","1.2.3"
    /// "02 AutoDetecting System^Version","Type04","System.Version","1.2.3.4"
    /// "03 System^Version","Type01","System.Version","1.0"
    /// "03 System^Version","Type02","System.Version","1.2"
    /// "03 System^Version","Type03","System.Version","1.2.3"
    /// "03 System^Version","Type04","System.Version","1.2.3.4"
    /// "04 C# Style","Type01","System.Boolean","True"
    /// "04 C# Style","Type02","System.Boolean","False"
    /// "04 C# Style","Type03","System.Char","X"
    /// "04 C# Style","Type04","System.String","Hello"
    /// "04 C# Style","Type05","System.Byte","5"
    /// "04 C# Style","Type06","System.SByte","6"
    /// "04 C# Style","Type07","System.Int16","7"
    /// "04 C# Style","Type08","System.Int16","8"
    /// "04 C# Style","Type09","System.UInt16","9"
    /// "04 C# Style","Type10","System.UInt16","10"
    /// "04 C# Style","Type11","System.Int32","11"
    /// "04 C# Style","Type12","System.Int32","12"
    /// "04 C# Style","Type13","System.UInt32","13"
    /// "04 C# Style","Type14","System.UInt32","14"
    /// "04 C# Style","Type15","System.Int64","15"
    /// "04 C# Style","Type16","System.Int64","16"
    /// "04 C# Style","Type17","System.UInt64","17"
    /// "04 C# Style","Type18","System.UInt64","18"
    /// "04 C# Style","Type19","System.Single","19"
    /// "04 C# Style","Type20","System.Single","20"
    /// "04 C# Style","Type21","System.Double","21"
    /// "04 C# Style","Type22","System.Decimal","22"
    /// "04 C# Style","Type23","System.Version","23.0"
    /// "04 C# Style","Type24","System.DateTime","2017-09-27 5:01:00 PM"
    /// "04 C# Style","Type25","System.DateTimeOffset","2017-09-27 5:01:00 PM -05:00"
    /// "04 C# Style","Type26","System.Guid","4aa1ddd4-7cae-4001-8976-9780ee4a794b"
    /// "05 ^NET Framework Types","Type01","System.Boolean","True"
    /// "05 ^NET Framework Types","Type02","System.Byte","2"
    /// "05 ^NET Framework Types","Type03","System.SByte","3"
    /// "05 ^NET Framework Types","Type04","System.Char","X"
    /// "05 ^NET Framework Types","Type05","System.Decimal","5"
    /// "05 ^NET Framework Types","Type06","System.Double","6"
    /// "05 ^NET Framework Types","Type07","System.Single","7"
    /// "05 ^NET Framework Types","Type08","System.Int32","8"
    /// "05 ^NET Framework Types","Type09","System.UInt32","9"
    /// "05 ^NET Framework Types","Type10","System.Int64","10"
    /// "05 ^NET Framework Types","Type11","System.UInt64","11"
    /// "05 ^NET Framework Types","Type12","System.Int16","12"
    /// "05 ^NET Framework Types","Type13","System.UInt16","13"
    /// "05 ^NET Framework Types","Type14","System.String","Hello"
    /// "05 ^NET Framework Types","Type15","System.Version","15.0"
    /// "05 ^NET Framework Types","Type16","System.DateTime","2017-09-27 5:01:00 PM"
    /// "05 ^NET Framework Types","Type17","System.DateTimeOffset","2017-09-27 5:01:00 PM -05:00"
    /// "05 ^NET Framework Types","Type18","System.Guid","1301f67d-f210-4e4d-a9d8-589ca5e96b61"
    /// </code>
    /// </remarks>
    /// <example>
    /// <para>The following example will create an <see cref="CsvConfigurationSerializer"/>, load a file containing the INPUT text from above and deserialize it into an <see cref="IConfigurationGroup"/>; 
    /// finally it will create other serializers and convert the <see cref="IConfigurationGroup"/> into various forms.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // load CSV text from file
    ///     // ##### be sure to change this to the proper file on your system #####
    ///     string sourceCsvFilename = @"C:\(WORKING)\Dev\SerializerTests 2\ConfigurationTest.input.csv.txt";
    ///     StringBuilder csvSource = new StringBuilder(System.IO.File.ReadAllText(sourceCsvFilename));
    /// 
    ///     // output original CSV
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine(csvSource);
    /// 
    ///     // create CSV serializer
    ///     dodSON.Core.Configuration.CsvConfigurationSerializer csvSerializer = new dodSON.Core.Configuration.CsvConfigurationSerializer();
    /// 
    ///     // deserialize CSV
    ///     dodSON.Core.Configuration.IConfigurationGroup configuration = csvSerializer.DeserializeGroup(csvSource);
    /// 
    ///     // serialize/output CSV
    ///     StringBuilder csvString = csvSerializer.SerializeGroup(configuration);
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine(csvString);
    /// 
    ///     // serialize/output INI
    ///     var iniSerializer = new dodSON.Core.Configuration.IniConfigurationSerializer();
    ///     StringBuilder iniString = iniSerializer.SerializeGroup(configuration);
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine(iniString);
    /// 
    ///     // serialize/output XML
    ///     var xmlSerializer = new dodSON.Core.Configuration.XmlConfigurationSerializer();
    ///     StringBuilder xmlString = xmlSerializer.SerializeGroup(configuration);
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine(xmlString);
    /// 
    ///     // ----
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine("press anykey&gt;");
    ///     Console.ReadKey(true);
    /// 
    ///     // This code produces output similar to the following:
    /// 
    ///     // --------------------------------
    ///     // "Group","Item","Type","Value"
    ///     // "","DateCreated","System.DateTime","9/27/2017 5:01 PM"
    ///     // "01 AutoDetecting","Type01","","True"
    ///     // "01 AutoDetecting","Type02","","False"
    ///     // "01 AutoDetecting","Type03","","X"
    ///     // "01 AutoDetecting","Type04","","Hello"
    ///     // "01 AutoDetecting","Type05","","-1024"
    ///     // "01 AutoDetecting","Type06","","314159265359"
    ///     // "01 AutoDetecting","Type07","","3.14159265359"
    ///     // "01 AutoDetecting","Type08","","2017/09/27 17:01"
    ///     // "01 AutoDetecting","Type09","","5a809246-1df0-499f-a5c5-27080e7b7b62"
    ///     // "02 AutoDetecting System^Version","Type01","","1"
    ///     // "02 AutoDetecting System^Version","Type02","","1.2"
    ///     // "02 AutoDetecting System^Version","Type03","","1.2.3"
    ///     // "02 AutoDetecting System^Version","Type04","","1.2.3.4"
    ///     // "03 System^Version","Type01","System.Version","1.0"
    ///     // "03 System^Version","Type02","System.Version","1.2"
    ///     // "03 System^Version","Type03","System.Version","1.2.3"
    ///     // "03 System^Version","Type04","System.Version","1.2.3.4"
    ///     // "04 C# Style","Type01","bool","True"
    ///     // "04 C# Style","Type02","boolean","False"
    ///     // "04 C# Style","Type03","char","X"
    ///     // "04 C# Style","Type04","string","Hello"
    ///     // "04 C# Style","Type05","byte","5"
    ///     // "04 C# Style","Type06","sbyte","6"
    ///     // "04 C# Style","Type07","short","7"
    ///     // "04 C# Style","Type08","int16","8"
    ///     // "04 C# Style","Type09","ushort","9"
    ///     // "04 C# Style","Type10","uint16","10"
    ///     // "04 C# Style","Type11","int","11"
    ///     // "04 C# Style","Type12","int32","12"
    ///     // "04 C# Style","Type13","uint","13"
    ///     // "04 C# Style","Type14","uint32","14"
    ///     // "04 C# Style","Type15","long","15"
    ///     // "04 C# Style","Type16","int64","16"
    ///     // "04 C# Style","Type17","ulong","17"
    ///     // "04 C# Style","Type18","uint64","18"
    ///     // "04 C# Style","Type19","float","19"
    ///     // "04 C# Style","Type20","single","20"
    ///     // "04 C# Style","Type21","double","21"
    ///     // "04 C# Style","Type22","decimal","22"
    ///     // "04 C# Style","Type23","version","23.0"
    ///     // "04 C# Style","Type24","datetime","9/27/2017 17:01:00"
    ///     // "04 C# Style","Type25","datetimeoffset","2017-09-27 17:01"
    ///     // "04 C# Style","Type26","guid","4AA1DDD47CAE400189769780EE4A794B"
    ///     // "05 ^NET Framework Types","Type01","System.Boolean","True"
    ///     // "05 ^NET Framework Types","Type02","System.Byte","2"
    ///     // "05 ^NET Framework Types","Type03","System.SByte","3"
    ///     // "05 ^NET Framework Types","Type04","System.Char","X"
    ///     // "05 ^NET Framework Types","Type05","System.Decimal","5"
    ///     // "05 ^NET Framework Types","Type06","System.Double","6"
    ///     // "05 ^NET Framework Types","Type07","System.Single","7"
    ///     // "05 ^NET Framework Types","Type08","System.Int32","8"
    ///     // "05 ^NET Framework Types","Type09","System.UInt32","9"
    ///     // "05 ^NET Framework Types","Type10","System.Int64","10"
    ///     // "05 ^NET Framework Types","Type11","System.UInt64","11"
    ///     // "05 ^NET Framework Types","Type12","System.Int16","12"
    ///     // "05 ^NET Framework Types","Type13","System.UInt16","13"
    ///     // "05 ^NET Framework Types","Type14","System.String","Hello"
    ///     // "05 ^NET Framework Types","Type15","System.Version","15.0"
    ///     // "05 ^NET Framework Types","Type16","System.DateTime","9/27/2017 5:01 Pm"
    ///     // "05 ^NET Framework Types","Type17","System.DateTimeOffset","2017-09-27 17:01"
    ///     // "05 ^NET Framework Types","Type18","System.Guid","{0x1301f67d, 0xf210, 0x4e4d, {0xa9, 0xd8, 0x58, 0x9c, 0xa5, 0xe9, 0x6b, 0x61}}"
    ///     // --------------------------------
    ///     // "Group","Item","Type","Value"
    ///     // "","DateCreated","System.DateTime","2017-09-27 5:01:00 PM"
    ///     // "01 AutoDetecting","Type01","System.Boolean","True"
    ///     // "01 AutoDetecting","Type02","System.Boolean","False"
    ///     // "01 AutoDetecting","Type03","System.String","X"
    ///     // "01 AutoDetecting","Type04","System.String","Hello"
    ///     // "01 AutoDetecting","Type05","System.Int32","-1024"
    ///     // "01 AutoDetecting","Type06","System.Int64","314159265359"
    ///     // "01 AutoDetecting","Type07","System.Double","3.14159265359"
    ///     // "01 AutoDetecting","Type08","System.DateTimeOffset","2017-09-27 5:01:00 PM -05:00"
    ///     // "01 AutoDetecting","Type09","System.Guid","5a809246-1df0-499f-a5c5-27080e7b7b62"
    ///     // "02 AutoDetecting System^Version","Type01","System.Int32","1"
    ///     // "02 AutoDetecting System^Version","Type02","System.Double","1.2"
    ///     // "02 AutoDetecting System^Version","Type03","System.Version","1.2.3"
    ///     // "02 AutoDetecting System^Version","Type04","System.Version","1.2.3.4"
    ///     // "03 System^Version","Type01","System.Version","1.0"
    ///     // "03 System^Version","Type02","System.Version","1.2"
    ///     // "03 System^Version","Type03","System.Version","1.2.3"
    ///     // "03 System^Version","Type04","System.Version","1.2.3.4"
    ///     // "04 C# Style","Type01","System.Boolean","True"
    ///     // "04 C# Style","Type02","System.Boolean","False"
    ///     // "04 C# Style","Type03","System.Char","X"
    ///     // "04 C# Style","Type04","System.String","Hello"
    ///     // "04 C# Style","Type05","System.Byte","5"
    ///     // "04 C# Style","Type06","System.SByte","6"
    ///     // "04 C# Style","Type07","System.Int16","7"
    ///     // "04 C# Style","Type08","System.Int16","8"
    ///     // "04 C# Style","Type09","System.UInt16","9"
    ///     // "04 C# Style","Type10","System.UInt16","10"
    ///     // "04 C# Style","Type11","System.Int32","11"
    ///     // "04 C# Style","Type12","System.Int32","12"
    ///     // "04 C# Style","Type13","System.UInt32","13"
    ///     // "04 C# Style","Type14","System.UInt32","14"
    ///     // "04 C# Style","Type15","System.Int64","15"
    ///     // "04 C# Style","Type16","System.Int64","16"
    ///     // "04 C# Style","Type17","System.UInt64","17"
    ///     // "04 C# Style","Type18","System.UInt64","18"
    ///     // "04 C# Style","Type19","System.Single","19"
    ///     // "04 C# Style","Type20","System.Single","20"
    ///     // "04 C# Style","Type21","System.Double","21"
    ///     // "04 C# Style","Type22","System.Decimal","22"
    ///     // "04 C# Style","Type23","System.Version","23.0"
    ///     // "04 C# Style","Type24","System.DateTime","2017-09-27 5:01:00 PM"
    ///     // "04 C# Style","Type25","System.DateTimeOffset","2017-09-27 5:01:00 PM -05:00"
    ///     // "04 C# Style","Type26","System.Guid","4aa1ddd4-7cae-4001-8976-9780ee4a794b"
    ///     // "05 ^NET Framework Types","Type01","System.Boolean","True"
    ///     // "05 ^NET Framework Types","Type02","System.Byte","2"
    ///     // "05 ^NET Framework Types","Type03","System.SByte","3"
    ///     // "05 ^NET Framework Types","Type04","System.Char","X"
    ///     // "05 ^NET Framework Types","Type05","System.Decimal","5"
    ///     // "05 ^NET Framework Types","Type06","System.Double","6"
    ///     // "05 ^NET Framework Types","Type07","System.Single","7"
    ///     // "05 ^NET Framework Types","Type08","System.Int32","8"
    ///     // "05 ^NET Framework Types","Type09","System.UInt32","9"
    ///     // "05 ^NET Framework Types","Type10","System.Int64","10"
    ///     // "05 ^NET Framework Types","Type11","System.UInt64","11"
    ///     // "05 ^NET Framework Types","Type12","System.Int16","12"
    ///     // "05 ^NET Framework Types","Type13","System.UInt16","13"
    ///     // "05 ^NET Framework Types","Type14","System.String","Hello"
    ///     // "05 ^NET Framework Types","Type15","System.Version","15.0"
    ///     // "05 ^NET Framework Types","Type16","System.DateTime","2017-09-27 5:01:00 PM"
    ///     // "05 ^NET Framework Types","Type17","System.DateTimeOffset","2017-09-27 5:01:00 PM -05:00"
    ///     // "05 ^NET Framework Types","Type18","System.Guid","1301f67d-f210-4e4d-a9d8-589ca5e96b61"
    ///     // --------------------------------
    ///     // DateCreated=System.DateTime=2017-09-27 5:01:00 PM
    ///     // [01 AutoDetecting]
    ///     //         Type01=System.Boolean=True
    ///     //         Type02=System.Boolean=False
    ///     //         Type03=System.Char=X
    ///     //         Type04=System.String=Hello
    ///     //         Type05=System.Int32=-1024
    ///     //         Type06=System.Int64=314159265359
    ///     //         Type07=System.Double=3.14159265359
    ///     //         Type08=System.DateTimeOffset=2017-09-27 5:01:00 PM -05:00
    ///     //         Type09=System.Guid=5a809246-1df0-499f-a5c5-27080e7b7b62
    ///     // [02 AutoDetecting System^Version]
    ///     //         Type01=System.Int32=1
    ///     //         Type02=System.Double=1.2
    ///     //         Type03=System.Version=1.2.3
    ///     //         Type04=System.Version=1.2.3.4
    ///     // [03 System^Version]
    ///     //         Type01=System.Version=1.0
    ///     //         Type02=System.Version=1.2
    ///     //         Type03=System.Version=1.2.3
    ///     //         Type04=System.Version=1.2.3.4
    ///     // [04 C# Style]
    ///     //         Type01=System.Boolean=True
    ///     //         Type02=System.Boolean=False
    ///     //         Type03=System.Char=X
    ///     //         Type04=System.String=Hello
    ///     //         Type05=System.Byte=5
    ///     //         Type06=System.SByte=6
    ///     //         Type07=System.Int16=7
    ///     //         Type08=System.Int16=8
    ///     //         Type09=System.UInt16=9
    ///     //         Type10=System.UInt16=10
    ///     //         Type11=System.Int32=11
    ///     //         Type12=System.Int32=12
    ///     //         Type13=System.UInt32=13
    ///     //         Type14=System.UInt32=14
    ///     //         Type15=System.Int64=15
    ///     //         Type16=System.Int64=16
    ///     //         Type17=System.UInt64=17
    ///     //         Type18=System.UInt64=18
    ///     //         Type19=System.Single=19
    ///     //         Type20=System.Single=20
    ///     //         Type21=System.Double=21
    ///     //         Type22=System.Decimal=22
    ///     //         Type23=System.Version=23.0
    ///     //         Type24=System.DateTime=2017-09-27 5:01:00 PM
    ///     //         Type25=System.DateTimeOffset=2017-09-27 5:01:00 PM -05:00
    ///     //         Type26=System.Guid=4aa1ddd4-7cae-4001-8976-9780ee4a794b
    ///     // [05 ^NET Framework Types]
    ///     //         Type01=System.Boolean=True
    ///     //         Type02=System.Byte=2
    ///     //         Type03=System.SByte=3
    ///     //         Type04=System.Char=X
    ///     //         Type05=System.Decimal=5
    ///     //         Type06=System.Double=6
    ///     //         Type07=System.Single=7
    ///     //         Type08=System.Int32=8
    ///     //         Type09=System.UInt32=9
    ///     //         Type10=System.Int64=10
    ///     //         Type11=System.UInt64=11
    ///     //         Type12=System.Int16=12
    ///     //         Type13=System.UInt16=13
    ///     //         Type14=System.String=Hello
    ///     //         Type15=System.Version=15.0
    ///     //         Type16=System.DateTime=2017-09-27 5:01:00 PM
    ///     //         Type17=System.DateTimeOffset=2017-09-27 5:01:00 PM -05:00
    ///     //         Type18=System.Guid=1301f67d-f210-4e4d-a9d8-589ca5e96b61
    ///     // --------------------------------
    ///     // &lt;?xml version="1.0" encoding="utf-8"?&gt;
    ///     // &lt;group key=""&gt;
    ///     //   &lt;items&gt;
    ///     //     &lt;item key="DateCreated" type="System.DateTime"&gt;2017-09-27 5:01:00 PM&lt;/item&gt;
    ///     //   &lt;/items&gt;
    ///     //   &lt;groups&gt;
    ///     //     &lt;group key="01 AutoDetecting"&gt;
    ///     //       &lt;items&gt;
    ///     //         &lt;item key="Type01" type="System.Boolean"&gt;True&lt;/item&gt;
    ///     //         &lt;item key="Type02" type="System.Boolean"&gt;False&lt;/item&gt;
    ///     //         &lt;item key="Type03" type="System.String"&gt;X&lt;/item&gt;
    ///     //         &lt;item key="Type04" type="System.String"&gt;Hello&lt;/item&gt;
    ///     //         &lt;item key="Type05" type="System.Int32"&gt;-1024&lt;/item&gt;
    ///     //         &lt;item key="Type06" type="System.Int64"&gt;314159265359&lt;/item&gt;
    ///     //         &lt;item key="Type07" type="System.Double"&gt;3.14159265359&lt;/item&gt;
    ///     //         &lt;item key="Type08" type="System.DateTimeOffset"&gt;2017-09-27 5:01:00 PM -05:00&lt;/item&gt;
    ///     //         &lt;item key="Type09" type="System.Guid"&gt;5a809246-1df0-499f-a5c5-27080e7b7b62&lt;/item&gt;
    ///     //       &lt;/items&gt;
    ///     //     &lt;/group&gt;
    ///     //     &lt;group key="02 AutoDetecting System^Version"&gt;
    ///     //       &lt;items&gt;
    ///     //         &lt;item key="Type01" type="System.Int32"&gt;1&lt;/item&gt;
    ///     //         &lt;item key="Type02" type="System.Double"&gt;1.2&lt;/item&gt;
    ///     //         &lt;item key="Type03" type="System.Version"&gt;1.2.3&lt;/item&gt;
    ///     //         &lt;item key="Type04" type="System.Version"&gt;1.2.3.4&lt;/item&gt;
    ///     //       &lt;/items&gt;
    ///     //     &lt;/group&gt;
    ///     //     &lt;group key="03 System^Version"&gt;
    ///     //       &lt;items&gt;
    ///     //         &lt;item key="Type01" type="System.Version"&gt;1.0&lt;/item&gt;
    ///     //         &lt;item key="Type02" type="System.Version"&gt;1.2&lt;/item&gt;
    ///     //         &lt;item key="Type03" type="System.Version"&gt;1.2.3&lt;/item&gt;
    ///     //         &lt;item key="Type04" type="System.Version"&gt;1.2.3.4&lt;/item&gt;
    ///     //       &lt;/items&gt;
    ///     //     &lt;/group&gt;
    ///     //     &lt;group key="04 C# Style"&gt;
    ///     //       &lt;items&gt;
    ///     //         &lt;item key="Type01" type="System.Boolean"&gt;True&lt;/item&gt;
    ///     //         &lt;item key="Type02" type="System.Boolean"&gt;False&lt;/item&gt;
    ///     //         &lt;item key="Type03" type="System.Char"&gt;X&lt;/item&gt;
    ///     //         &lt;item key="Type04" type="System.String"&gt;Hello&lt;/item&gt;
    ///     //         &lt;item key="Type05" type="System.Byte"&gt;5&lt;/item&gt;
    ///     //         &lt;item key="Type06" type="System.SByte"&gt;6&lt;/item&gt;
    ///     //         &lt;item key="Type07" type="System.Int16"&gt;7&lt;/item&gt;
    ///     //         &lt;item key="Type08" type="System.Int16"&gt;8&lt;/item&gt;
    ///     //         &lt;item key="Type09" type="System.UInt16"&gt;9&lt;/item&gt;
    ///     //         &lt;item key="Type10" type="System.UInt16"&gt;10&lt;/item&gt;
    ///     //         &lt;item key="Type11" type="System.Int32"&gt;11&lt;/item&gt;
    ///     //         &lt;item key="Type12" type="System.Int32"&gt;12&lt;/item&gt;
    ///     //         &lt;item key="Type13" type="System.UInt32"&gt;13&lt;/item&gt;
    ///     //         &lt;item key="Type14" type="System.UInt32"&gt;14&lt;/item&gt;
    ///     //         &lt;item key="Type15" type="System.Int64"&gt;15&lt;/item&gt;
    ///     //         &lt;item key="Type16" type="System.Int64"&gt;16&lt;/item&gt;
    ///     //         &lt;item key="Type17" type="System.UInt64"&gt;17&lt;/item&gt;
    ///     //         &lt;item key="Type18" type="System.UInt64"&gt;18&lt;/item&gt;
    ///     //         &lt;item key="Type19" type="System.Single"&gt;19&lt;/item&gt;
    ///     //         &lt;item key="Type20" type="System.Single"&gt;20&lt;/item&gt;
    ///     //         &lt;item key="Type21" type="System.Double"&gt;21&lt;/item&gt;
    ///     //         &lt;item key="Type22" type="System.Decimal"&gt;22&lt;/item&gt;
    ///     //         &lt;item key="Type23" type="System.Version"&gt;23.0&lt;/item&gt;
    ///     //         &lt;item key="Type24" type="System.DateTime"&gt;2017-09-27 5:01:00 PM&lt;/item&gt;
    ///     //         &lt;item key="Type25" type="System.DateTimeOffset"&gt;2017-09-27 5:01:00 PM -05:00&lt;/item&gt;
    ///     //         &lt;item key="Type26" type="System.Guid"&gt;4aa1ddd4-7cae-4001-8976-9780ee4a794b&lt;/item&gt;
    ///     //       &lt;/items&gt;
    ///     //     &lt;/group&gt;
    ///     //     &lt;group key="05 ^NET Framework Types"&gt;
    ///     //       &lt;items&gt;
    ///     //         &lt;item key="Type01" type="System.Boolean"&gt;True&lt;/item&gt;
    ///     //         &lt;item key="Type02" type="System.Byte"&gt;2&lt;/item&gt;
    ///     //         &lt;item key="Type03" type="System.SByte"&gt;3&lt;/item&gt;
    ///     //         &lt;item key="Type04" type="System.Char"&gt;X&lt;/item&gt;
    ///     //         &lt;item key="Type05" type="System.Decimal"&gt;5&lt;/item&gt;
    ///     //         &lt;item key="Type06" type="System.Double"&gt;6&lt;/item&gt;
    ///     //         &lt;item key="Type07" type="System.Single"&gt;7&lt;/item&gt;
    ///     //         &lt;item key="Type08" type="System.Int32"&gt;8&lt;/item&gt;
    ///     //         &lt;item key="Type09" type="System.UInt32"&gt;9&lt;/item&gt;
    ///     //         &lt;item key="Type10" type="System.Int64"&gt;10&lt;/item&gt;
    ///     //         &lt;item key="Type11" type="System.UInt64"&gt;11&lt;/item&gt;
    ///     //         &lt;item key="Type12" type="System.Int16"&gt;12&lt;/item&gt;
    ///     //         &lt;item key="Type13" type="System.UInt16"&gt;13&lt;/item&gt;
    ///     //         &lt;item key="Type14" type="System.String"&gt;Hello&lt;/item&gt;
    ///     //         &lt;item key="Type15" type="System.Version"&gt;15.0&lt;/item&gt;
    ///     //         &lt;item key="Type16" type="System.DateTime"&gt;2017-09-27 5:01:00 PM&lt;/item&gt;
    ///     //         &lt;item key="Type17" type="System.DateTimeOffset"&gt;2017-09-27 5:01:00 PM -05:00&lt;/item&gt;
    ///     //         &lt;item key="Type18" type="System.Guid"&gt;1301f67d-f210-4e4d-a9d8-589ca5e96b61&lt;/item&gt;
    ///     //       &lt;/items&gt;
    ///     //     &lt;/group&gt;
    ///     //   &lt;/groups&gt;
    ///     // &lt;/group&gt;
    ///     // --------------------------------
    ///     // press anykey&gt;
    /// }
    /// </code>
    /// </example>
    public class CsvConfigurationSerializer
            : IConfigurationSerializer<StringBuilder>
    {
        #region Public Static Methods
        /// <summary>
        /// Creates a clone of the <paramref name="source"/> using <see cref="CsvConfigurationSerializer"/>.
        /// </summary>
        /// <param name="source">The <see cref="IConfigurationGroup"/> to clone.</param>
        /// <returns>A clone of the <paramref name="source"/> using <see cref="CsvConfigurationSerializer"/>.</returns>
        public static IConfigurationGroup Clone(IConfigurationGroup source)
        {
            var serializer = new CsvConfigurationSerializer();
            return serializer.Deserialize(serializer.Serialize(source));
        }
        /// <summary>
        /// Will load the <paramref name="filename"/> and try to deserialize it using an <see cref="CsvConfigurationSerializer"/>.
        /// </summary>
        /// <param name="filename">The file to load and deserialize.</param>
        /// <returns>The <see cref="IConfigurationGroup"/> deserialized from the text in the <paramref name="filename"/>.</returns>
        public static IConfigurationGroup LoadFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException(nameof(filename)); }
            if (!System.IO.File.Exists(filename)) { throw new System.IO.FileNotFoundException($"File not found.", filename); }
            //
            var source = new StringBuilder(System.IO.File.ReadAllText(filename));
            return (new CsvConfigurationSerializer()).Deserialize(source);
        }
        /// <summary>
        /// Will serialize the <paramref name="source"/> using an <see cref="CsvConfigurationSerializer"/> and save it to the <paramref name="filename"/>.
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
            var dude = (new CsvConfigurationSerializer()).Serialize(source);
            System.IO.File.WriteAllText(filename, dude.ToString());
        }
        #endregion
        #region Public Constants
        /// <summary>
        /// The character used to separate the group names.
        /// </summary>
        public static readonly char _CsvGroupNameSeparator_ = ConfigurationHelper.GroupNameSeparator;
        /// <summary>
        /// The value used to separate rows.
        /// </summary>
        public static readonly string _CsvRowSeparator_ = Environment.NewLine;
        /// <summary>
        /// The value used to separate columns.
        /// </summary>
        public static readonly string _CsvColumnSeparator_ = ",";
        /// <summary>
        ///  The value used to enclose any ini element that requires enclosure.
        /// </summary>
        public static readonly string _CsvEnclosure_ = "\"";
        #endregion
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="CsvConfigurationSerializer"/>.
        /// </summary>
        public CsvConfigurationSerializer() { }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public CsvConfigurationSerializer(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "Serializer") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Serializer\". Configuration Key={configuration.Key}", nameof(configuration)); }
        }
        #endregion
        #region Private Fields
        private readonly Compression.ICompressor _Compressor = new Compression.DeflateStreamCompressor();
        private readonly DelimiterSeparatedValues.DsvSettings _Settings = new DelimiterSeparatedValues.DsvSettings(_CsvRowSeparator_, _CsvColumnSeparator_, _CsvEnclosure_,
                                                                                                                   true, true, true, true,
                                                                                                                   DelimiterSeparatedValues.ColumnEnclosingRuleEnum.EncloseAll);
        #endregion
        #region Public Methods
        /// <summary>
        /// The <see cref="DelimiterSeparatedValues.DsvSettings"/> used by the <see cref="CsvConfigurationSerializer"/>. (Cloned.)
        /// </summary>
        public DelimiterSeparatedValues.DsvSettings Settings { get { return Converters.ConvertersHelper.Clone(_Settings); } }
        #endregion
        #region IConfigurationSerializer<StringBuilder> Methods
        /// <summary>
        /// Converts the specified configuration to a <see cref="StringBuilder"/> type.
        /// </summary>
        /// <param name="configuration">The configuration to convert.</param>
        /// <returns>The configuration converted into a <see cref="StringBuilder"/> type.</returns>
        public StringBuilder Serialize(IConfigurationGroup configuration)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            var table = CreateTable();
            InternalSerializeGroup("", configuration, table);
            return new StringBuilder(table.WriteString());
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
            IConfigurationGroup workGroup = new ConfigurationGroup("");
            var table = new DelimiterSeparatedValues.DsvTable(_Settings);
            table.ReadString(source.ToString());
            foreach (var row in table.Rows)
            {
                var groupName = row[0].ToString();
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    workGroup.Items.Add(DeserializeItem(row));
                }
                else
                {
                    var group = ConfigurationShared.FindGroup(workGroup, groupName, _CsvGroupNameSeparator_);
                    var item = DeserializeItem(row);
                    if (item != null)
                    {
                        group.Items.Add(item);
                    }
                }
            }
            if ((string.IsNullOrWhiteSpace(workGroup.Key)) && (workGroup.Items.Count == 0) && (workGroup.Count == 1))
            {
                workGroup = workGroup.ElementAt(0);
            }
            workGroup.MarkDirty();
            workGroup.MarkNew();
            return workGroup;
        }
        #endregion
        #region Private Methods
        private DelimiterSeparatedValues.DsvTable CreateTable()
        {
            var theTable = new DelimiterSeparatedValues.DsvTable(_Settings);
            // create columns
            theTable.Columns.Add("Group", false, "", "", typeof(string));
            theTable.Columns.Add("Item", false, "", "", typeof(string));
            theTable.Columns.Add("Type", true, "", "", typeof(string));
            theTable.Columns.Add("Value", true, "", "", typeof(string));
            //
            return theTable;
        }
        private void InternalSerializeGroup(string parentGroupAddress, IConfigurationGroup group, DelimiterSeparatedValues.DsvTable table)
        {
            // get local Group Address
            var groupAddress = group.Key;
            if (!string.IsNullOrWhiteSpace(parentGroupAddress)) { groupAddress = parentGroupAddress + _CsvGroupNameSeparator_ + group.Key; }
            // process items
            if (group.Items.Count > 0)
            {
                // serialize items
                foreach (var item in group.Items)
                {
                    var row = table.NewRow(groupAddress, item.Key, ConfigurationShared.ConvertItemTypeToString(item), ConfigurationShared.ConvertItemObjectToString(item, _Compressor));
                    table.Rows.Add(row);
                }
            }
            else
            {
                var row = table.NewRow(groupAddress);
                if (row != null)
                {
                    if (!string.IsNullOrWhiteSpace((string)row[0]))
                    {
                        table.Rows.Add(row);
                    }
                    else if ((!string.IsNullOrWhiteSpace((string)row[1])) &&
                             (!string.IsNullOrWhiteSpace((string)row[2])) &&
                             (!string.IsNullOrWhiteSpace((string)row[3])))
                    {
                        table.Rows.Add(row);
                    }
                }
            }
            // process groups
            if (group.Count > 0)
            {
                foreach (var subGroup in group) { InternalSerializeGroup(groupAddress, subGroup, table); }
            }
        }
        private IConfigurationItem DeserializeItem(DelimiterSeparatedValues.DsvRow row)
        {
            if (row == null) { throw new ArgumentNullException(nameof(row)); }
            if (row.ItemArray.Length != 4) { throw new ArgumentNullException(nameof(row), $"Parameter {nameof(row)} is invalid."); }
            var groupName = row.ItemArray[0].ToString();
            var itemName = row.ItemArray[1].ToString();
            var valueTypeName = row.ItemArray[2].ToString();
            var content = row.ItemArray[3].ToString();
            // check for special case: empty group
            if (!(string.IsNullOrWhiteSpace(groupName)) &&
                 (string.IsNullOrWhiteSpace(itemName)) &&
                 (string.IsNullOrWhiteSpace(valueTypeName)) &&
                 (string.IsNullOrWhiteSpace(content)))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(valueTypeName))
            {
                // **** AUTO-DETECT
                return ConfigurationShared.CreateConfigurationItem(itemName, content);
            }
            else
            {
                // **** TYPE PROVIDED
                return ConfigurationShared.CreateConfigurationItem(itemName, valueTypeName, content, _Compressor);
            }
        }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("Serializer");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                return result;
            }
        }
        #endregion
    }
}

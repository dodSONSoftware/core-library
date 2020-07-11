using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Configuration
{
    internal static class ConfigurationShared
    {
        #region Private Fields
        // if true will compress byte arrays before converting to string and decompress after converting from string
        private static readonly bool _CompressByteArrays_ = false;
        // if true will convert byte arrays to hexadecimal string; otherwise, it will convert bytes arrays to base64String
        private static readonly bool _UseHexPairStringForByteArrays_ = true;
        // the basic types recognized by the auto-detect system
        private static readonly List<Type> _DetermineBasicType_TypesList_ = new List<Type>() { typeof(int), typeof(long), typeof(double), typeof(bool) };
        // the known system types to test for when the type has been provided
        private static readonly List<string> _KnownTypes_ = new List<string>() { typeof(bool).FullName,
                                                                                 typeof(sbyte).FullName, typeof(short).FullName, typeof(int).FullName, typeof(long).FullName,
                                                                                 typeof(byte).FullName, typeof(ushort).FullName, typeof(uint).FullName, typeof(ulong).FullName,
                                                                                 typeof(float).FullName, typeof(double).FullName, typeof(decimal).FullName };
        #endregion
        #region Internal Fields
        // all known types used by various types throughout the dodSON.Core
        internal static readonly List<Type> _AllKnownTypes = new List<Type>() { typeof(bool),
                                                                                typeof(string), typeof(char),
                                                                                typeof(Version),
                                                                                typeof(DateTimeOffset), typeof(DateTime), typeof(TimeSpan),
                                                                                typeof(Guid), typeof(byte[]),
                                                                                typeof(sbyte), typeof(short), typeof(int), typeof(long),
                                                                                typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
                                                                                typeof(float), typeof(double), typeof(decimal),
                                                                                typeof(Type)};
        #endregion
        #region Internal Methods

        // ######## Find...

        internal static IConfigurationGroup FindGroup(IConfigurationGroup searchGroup, string groupKey, char groupNameSeparator)
        {
            var gk = groupKey;
            if (!gk.Contains(groupNameSeparator))
            {
                // check for existing group
                var foundExisting = searchGroup.FirstOrDefault(x => { return x.Key == gk; });
                if (foundExisting != null)
                {
                    return foundExisting;
                }
                else
                {
                    var newGroup = new ConfigurationGroup(groupKey);
                    searchGroup.Add(newGroup);
                    return newGroup;
                }
            }
            else
            {
                var parts = gk.Split(groupNameSeparator);
                gk = parts[0];
                // ----
                var subGroup = FindSubGroup(searchGroup, gk);
                if (subGroup != null)
                {
                    var dd = groupKey.Substring(groupKey.IndexOf(groupNameSeparator) + 1);
                    var group = FindGroup(subGroup, dd, groupNameSeparator);
                    return group;
                }
            }
            return FindSubGroup(searchGroup, gk);
        }

        internal static IConfigurationGroup FindSubGroup(IConfigurationGroup group, string itemKey)
        {
            foreach (var item in group)
            {
                if (item.Key == itemKey)
                {
                    return item;
                }
            }
            var newGroup = new ConfigurationGroup(itemKey);
            group.Add(newGroup);
            return newGroup;
        }

        // ######## Convert...

        internal static string ConvertItemTypeToString(IConfigurationItem item) => item.ValueType;

        internal static string ConvertItemObjectToString(IConfigurationItem item, Compression.ICompressor compressor)
        {
            string value = null;
            if (item.ValueType.StartsWith(typeof(string).FullName))
            {
                // #### String
                value = Common.StringHelper.TokenizeString(Convert.ToString(item.Value), Common.StringHelper.DefaultTokenReplacementPairs);
            }
            else if (item.ValueType.StartsWith(typeof(char).FullName))
            {
                // #### Char
                value = Common.StringHelper.TokenizeString(Convert.ToString(item.Value), Common.StringHelper.DefaultTokenReplacementPairs);
            }
            else if (item.ValueType.StartsWith(typeof(Version).FullName))
            {
                // #### Version
                var vv = (item.Value as Version);
                value = $"{Fix(vv.Major)}.{Fix(vv.Minor)}.{Fix(vv.Build)}.{Fix(vv.Revision)}";

                // ----------
                int Fix(int fixValue)
                {
                    if (fixValue < 0)
                    {
                        fixValue = 0;
                    }
                    return fixValue;
                }
            }
            else if (item.ValueType.StartsWith(typeof(DateTimeOffset).FullName))
            {
                // #### DateTimeOffset
                value = ((DateTimeOffset)item.Value).ToString("yyyy/MM/dd HH:mm:ss.fff");
            }
            else if (item.ValueType.StartsWith(typeof(DateTime).FullName))
            {
                // #### DateTime
                value = ((DateTime)item.Value).ToString("yyyy/MM/dd HH:mm:ss.fff");
            }
            else if (item.ValueType.StartsWith(typeof(TimeSpan).FullName))
            {
                // #### TimeSpan
                value = ((TimeSpan)item.Value).ToString();
            }
            else if (item.ValueType.StartsWith(typeof(Guid).FullName))
            {
                // #### Guid
                value = ((Guid)item.Value).ToString("D");
            }
            else if (item.ValueType.StartsWith(typeof(Type).FullName))
            {
                // #### Type
                value = ((string)item.Value);
            }
            else if (item.ValueType.StartsWith(typeof(byte[]).FullName))
            {
                // #### byte[]
                if (_UseHexPairStringForByteArrays_)
                {
                    if (_CompressByteArrays_)
                    {
                        value = Common.ByteArrayHelper.ConvertByteArrayToString(compressor.Compress((byte[])item.Value), Common.ByteArrayHelper.ConversionBase.Hexadecimal);
                    }
                    else
                    {
                        value = Common.ByteArrayHelper.ConvertByteArrayToString((byte[])item.Value, Common.ByteArrayHelper.ConversionBase.Hexadecimal);
                    }
                }
                else
                {
                    if (_CompressByteArrays_)
                    {
                        value = Convert.ToBase64String(compressor.Compress((byte[])item.Value));
                    }
                    else
                    {
                        value = Convert.ToBase64String((byte[])item.Value);
                    }
                }
            }
            else
            {
                // #### Known Type
                value = Convert.ToString(item.Value);
            }
            //
            return value;
        }

        // ######## AUTO-DETECT TYPE
        internal static IConfigurationItem CreateConfigurationItem(string key, string content)
        {
            var contentActual = AutoDetectBasicType(key, content);
            return new ConfigurationItem(key, contentActual, contentActual.GetType());
        }

        // ######## TYPE PROVIDED
        internal static IConfigurationItem CreateConfigurationItem(string key, string contentType, string content, Compression.ICompressor compressor)
        {
            if (contentType.StartsWith(typeof(string).FullName))
            {
                // #### STRING
                string item = null;
                try
                {
                    item = Common.StringHelper.UnTokenizeString((string)Convert.ChangeType(content, Type.GetType(contentType)), Common.StringHelper.DefaultTokenReplacementPairs);
                }
                catch
                {
                    throw new Exception($"Unable to convert content to type. Key={key}, Content={content}, Type={typeof(string).FullName}");
                }
                return new ConfigurationItem(key, item ?? "", typeof(System.String).FullName);
            }
            else if (contentType.StartsWith(typeof(char).FullName))
            {
                // #### CHAR
                char ch = ' ';
                try
                {
                    ch = (char)Convert.ChangeType(content, Type.GetType(contentType));
                }
                catch
                {
                    throw new Exception($"Unable to convert content to type. Key={key}, Content={content}, Type={typeof(char).FullName}");
                }
                string item = Common.StringHelper.UnTokenizeString(ch.ToString(), Common.StringHelper.DefaultTokenReplacementPairs);
                if ((item != null) && (item.Length > 0))
                {
                    return new ConfigurationItem(key, item[0], typeof(System.Char).FullName);
                }
                else
                {
                    return new ConfigurationItem(key, item ?? "", typeof(System.Char).FullName);
                }
            }
            else if (contentType.StartsWith(typeof(Version).FullName))
            {
                // #### VERSION
                var originalContent = content;
                if (!content.Contains('.'))
                {
                    content = content + ".0";
                }
                if (!Version.TryParse(content, out Version vResult))
                {
                    throw new Exception($"Unable to convert content to type. Key={key}, Content={originalContent}, Type={typeof(Version).FullName}");
                }
                return new ConfigurationItem(key, vResult, typeof(System.Version).FullName);
            }
            else if (contentType.StartsWith(typeof(DateTimeOffset).FullName))
            {
                // #### DATETIMEOFFSET
                if (DateTimeOffset.TryParse(content, out DateTimeOffset dtoResult))
                {
                    var newDateTime = new DateTimeOffset(dtoResult.DateTime, TimeSpan.Zero);
                    return new ConfigurationItem(key, newDateTime, typeof(System.DateTimeOffset).FullName);
                }
                else
                {
                    if (DateTimeOffset.TryParse(content + "+00:00", out DateTimeOffset dt2Result))
                    {
                        return new ConfigurationItem(key, dt2Result, typeof(System.DateTimeOffset).FullName);
                    }
                    else
                    {
                        //return new ConfigurationItem(key, DateTimeOffset.MinValue, typeof(System.DateTimeOffset).FullName);
                        throw new Exception($"Unable to convert content to type. Key={key}, Content={content}, Type={typeof(DateTimeOffset).FullName}");
                    }
                }
            }
            else if (contentType.StartsWith(typeof(DateTime).FullName))
            {
                // TODO: should this keep the DateTime data type as DateTime? Currently, it changes DateTime types to DateTimeOffset types.

                // #### DATETIME
                if (DateTimeOffset.TryParse(content, out DateTimeOffset dt1Result))
                {
                    var newDateTime = new DateTimeOffset(dt1Result.DateTime, TimeSpan.Zero);
                    return new ConfigurationItem(key, newDateTime, typeof(System.DateTimeOffset).FullName);
                }
                else
                {
                    if (DateTimeOffset.TryParse(content + " +00:00", out DateTimeOffset dt2Result))
                    {
                        return new ConfigurationItem(key, dt2Result, typeof(System.DateTimeOffset).FullName);
                    }
                    else
                    {
                        //return new ConfigurationItem(key, DateTimeOffset.MinValue, typeof(System.DateTimeOffset).FullName);
                        throw new Exception($"Unable to convert content to type. Key={key}, Content={content}, Type={typeof(DateTime).FullName}");
                    }
                }
            }
            else if (contentType.StartsWith(typeof(TimeSpan).FullName))
            {
                // #### TIMESPAN
                if (!TimeSpan.TryParse(content, out TimeSpan tsResult))
                {
                    throw new Exception($"Unable to convert content to type. Key={key}, Content={content}, Type={typeof(TimeSpan).FullName}");
                }
                return new ConfigurationItem(key, tsResult, typeof(System.TimeSpan).FullName);
            }
            else if (contentType.StartsWith(typeof(Guid).FullName))
            {
                // #### GUID
                if (!Guid.TryParse(content, out Guid gResult))
                {
                    throw new Exception($"Unable to convert content to type. Key={key}, Content={content}, Type={typeof(Guid).FullName}");
                }
                return new ConfigurationItem(key, gResult, typeof(System.Guid).FullName);
            }
            else if (contentType.StartsWith(typeof(Type).FullName))
            {
                // #### TYPE
                return new ConfigurationItem(key, content, typeof(System.Type).FullName);
            }
            else if (contentType.StartsWith(typeof(byte[]).FullName))
            {
                // #### BYTE[]
                if (!string.IsNullOrWhiteSpace(content))
                {
                    try
                    {
                        byte[] baResult;
                        if (_UseHexPairStringForByteArrays_)
                        {
                            if (_CompressByteArrays_)
                            {
                                baResult = compressor.Decompress(Common.ByteArrayHelper.ConvertStringToByteArray(content, Common.ByteArrayHelper.ConversionBase.Hexadecimal));
                            }
                            else
                            {
                                baResult = Common.ByteArrayHelper.ConvertStringToByteArray(content, Common.ByteArrayHelper.ConversionBase.Hexadecimal);
                            }
                        }
                        else
                        {
                            if (_CompressByteArrays_)
                            {
                                baResult = compressor.Decompress(Convert.FromBase64String(content));
                            }
                            else
                            {
                                baResult = Convert.FromBase64String(content);
                            }
                        }
                        return new ConfigurationItem(key, baResult, typeof(System.Byte[]).FullName);
                    }
                    catch
                    {
                        throw new Exception($"Unable to convert content to type. Key={key}, Content={content}, Type={typeof(byte[]).FullName}");
                    }
                }
                else
                {
                    return new ConfigurationItem(key, new byte[0], typeof(System.Byte[]).FullName);
                }
            }
            else if (StartsWith(contentType, _KnownTypes_))
            {
                // #### KNOWN TYPE
                var type = Type.GetType(contentType);
                object item = null;
                try
                {
                    item = Convert.ChangeType(content, type);
                }
                catch
                {
                    throw new Exception($"Unable to convert content to type. Key={key}, Content={content}, Type={type.FullName}");
                }
                return new ConfigurationItem(key, item, type.FullName);
            }
            else
            {
                // TODO: check for C#-Style type names

                // #### TEST FOR C#-STYLE TYPE NAMES

            }
            // #### UNKNOWN TYPE
            return new ConfigurationItem(key, content, contentType);
        }
        #endregion
        #region Private Methods
        private static object AutoDetectBasicType(string key, string content)
        {
            // ---- int, long, double, bool
            foreach (var candidateType in _DetermineBasicType_TypesList_)
            {
                try
                {
                    return Convert.ChangeType(content, candidateType);
                }
                catch { }
            }
            // #### these are in a specific order for a reason
            // ---- Guid
            if (Guid.TryParse(content, out Guid resultGuid))
            {
                return resultGuid;
            }
            // ---- Version
            if (Version.TryParse(content, out Version resultVersion))
            {
                return resultVersion;
            }
            // ---- TimeSpan
            if (TimeSpan.TryParse(content, out TimeSpan resultTimeSpan))
            {
                return resultTimeSpan;
            }
            // ---- DateTimeOffset
            if (DateTimeOffset.TryParse(content, out DateTimeOffset resultDateTime))
            {
                return resultDateTime;
            }
            // ---- String (default, always do last)
            try
            {
                return Convert.ChangeType(content, typeof(string));
            }
            catch
            {
                throw new Exception($"Auto Detect: Unable to convert content to type. Key={key}, Content={content}");
            }
        }
        private static bool StartsWith(string searchString, IEnumerable<string> list)
        {
            foreach (var item in list)
            {
                if (searchString.StartsWith(item))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}

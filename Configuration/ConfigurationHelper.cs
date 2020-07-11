using System;
using System.Collections.Generic;
using System.Linq;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Common, and standardizing, methods used throughout the dodSON.Core.Configuration namespace.
    /// </summary>
    public static class ConfigurationHelper
    {
        #region Public Properties
        /// <summary>
        /// The <see cref="Char"/> separating each group name in a group address.
        /// </summary>
        /// <remarks>
        /// <code language="" title="Currently">
        /// GroupNameSeparator = '.'
        /// </code>
        /// </remarks>
        public static readonly char GroupNameSeparator = '.';


        // ################ INSTANTIATE

        /// <summary>
        /// Will attempt to instantiate a type from the given <paramref name="configuration"/>. 
        /// The type may be defined in the <paramref name="configuration"/> as an <see cref="Configuration.IConfigurationItem"/> with the key: "Type" and the string value of either a <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/>.
        /// If no type is defined in the <paramref name="configuration"/>, then the <paramref name="defaultType"/> will be used.
        /// </summary>
        /// <param name="defaultType">The type to instantiate if the <paramref name="configuration"/> does not define a type.</param>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to use to configure the type being instantiated.</param>
        /// <returns>The instantiated type or null.</returns>
        public static object InstantiateTypeFromConfigurationGroup(Type defaultType, IConfigurationGroup configuration)
        {
            Type type = defaultType;
            if (configuration.Items.ContainsKey("Type"))
            {
                if (!(configuration.Items["Type"].ValueType.StartsWith(typeof(Type).FullName)))
                {
                    throw new Exception($"Configuration item invalid format. Configuration item 'Type' must be a <{typeof(Type)}>.");
                }
                try
                {
                    type = Type.GetType((string)configuration.Items["Type"].Value);
                }
                catch { type = defaultType; }
            }
            //
            if (type == null)
            {
                throw new Exception("Cannot instantiate type from configuration; missing type information and no default type given. " +
                                    "Also, it could be that the type being instantiate cannot be found; check for available assemblies and dependency package definitions.");
            }
            return Common.InstantiationHelper.InvokeCtor(type, typeof(IConfigurationGroup), configuration);
        }
        /// <summary>
        /// Will attempt to instantiate a type from the given <paramref name="configuration"/>. 
        /// The type must be defined in the <paramref name="configuration"/> as an <see cref="Configuration.IConfigurationItem"/> with the key: "Type" and the string value of either a <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to use to configure the type being instantiated.</param>
        /// <returns>The instantiated type or null.</returns>
        public static object InstantiateTypeFromConfigurationGroup(IConfigurationGroup configuration) => InstantiateTypeFromConfigurationGroup(null, configuration);
        /// <summary>
        /// Will attempt to instantiate a type from the given <paramref name="configuration"/>. 
        /// The type may be defined in the <paramref name="configuration"/> as an <see cref="Configuration.IConfigurationItem"/> with the key: "Type" and the string value of either a <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/>.
        /// If no type is defined in the <paramref name="configuration"/>, then the <paramref name="defaultType"/> will be used.
        /// </summary>
        /// <param name="defaultType">The type to instantiate if the <paramref name="configuration"/> does not define a type.</param>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to use to configure the type being instantiated.</param>
        /// <param name="result">The instantiated type or null.</param>
        /// <param name="exception">An exception describing any problems trying to instantiate the type.</param>
        /// <returns><b>True</b> if successfully instantiated; otherwise, <b>false</b> if not.</returns>
        public static bool TryInstantiateTypeFromConfigurationGroup(Type defaultType, IConfigurationGroup configuration, out object result, out Exception exception)
        {
            try
            {
                result = InstantiateTypeFromConfigurationGroup(defaultType, configuration);
                exception = null;
                return (result != null);
            }
            catch (Exception ex)
            {
                result = null;
                exception = ex;
                return false;
            }
        }
        /// <summary>
        /// Will attempt to instantiate a type from the given <paramref name="configuration"/>. 
        /// The type must be defined in the <paramref name="configuration"/> as an <see cref="Configuration.IConfigurationItem"/> with the key: "Type" and the string value of either a <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to use to configure the type being instantiated.</param>
        /// <param name="result">The instantiated type or null.</param>
        /// <param name="exception">An exception describing any problems trying to instantiate the type.</param>
        /// <returns><b>True</b> if successfully instantiated; otherwise, <b>false</b> if not.</returns>
        public static bool TryInstantiateTypeFromConfigurationGroup(IConfigurationGroup configuration, out object result, out Exception exception)
        {
            try
            {
                result = InstantiateTypeFromConfigurationGroup(configuration);
                exception = null;
                return (result != null);
            }
            catch (Exception ex)
            {
                result = null;
                exception = ex;
                return false;
            }
        }
        // ----------------
        /// <summary>
        /// Will search the given <paramref name="configuration"/> for the specified <see cref="IConfigurationGroup"/> by <paramref name="name"/> and attempt to instantiate a type from the found <see cref="IConfigurationGroup"/>.
        /// The type must be defined in the <paramref name="configuration"/> with an <see cref="Configuration.IConfigurationItem"/> with the key: "Type" and the string value of either a <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> the configuration should be instantiated into.</typeparam>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The group name to search for.</param>
        /// <param name="throwException">If the group is not found, <b>True</b> will throw an <see cref="Exception"/>, <b>false</b> will return null.</param>
        /// <returns>The instantiated type or null.</returns>
        public static T InstantiateTypeFromConfigurationSubGroup<T>(IConfigurationGroup configuration, string name, bool throwException)
            where T : class
        {
            var group = FindConfigurationGroup(configuration, name, throwException);
            if (group != null)
            {
                return InstantiateTypeFromConfigurationGroup(typeof(T), group) as T;
            }
            return null;
        }
        /// <summary>
        /// Finds the named <see cref="IConfigurationItem"/> in the given configuration group and tests it's type and returns an instance of the <paramref name="configuration"/> strongly typed to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> the configuration item should be instantiated into.</typeparam>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The name of the <see cref="IConfigurationItem"/> to search for.</param>
        /// <param name="throwException">If the item is not found, <b>True</b> will throw an <see cref="Exception"/>, <b>false</b> will return null.</param>
        /// <returns>An instance of the <paramref name="configuration"/> strongly typed to <typeparamref name="T"/>.</returns>
        public static T InstantiateTypeFromConfigurationItem<T>(IConfigurationGroup configuration, string name, bool throwException)
        {
            Type searchType = typeof(T);
            if (!ConfigurationShared._AllKnownTypes.Contains(searchType))
            {
                // if it is not a known type then assume it is a "System.Type"
                searchType = typeof(Type);
            }
            //
            var item = FindConfigurationItem(configuration, name, searchType);
            if (item != null)
            {
                if (searchType == typeof(Type))
                {
                    var typeActual = Type.GetType(item.Value as string);
                    return (T)Common.InstantiationHelper.InvokeDefaultCtor(typeActual);
                }
                else
                {
                    return (T)item.Value;
                }
            }
            //
            if (throwException)
            {
                throw new Exception($"Configuration missing item. Configuration must have item: '{name}'.");
            }
            return default(T);
        }
        /// <summary>
        /// Will search the given <paramref name="configuration"/> for the specified <see cref="IConfigurationGroup"/> by <paramref name="name"/> and attempt to instantiate a type from the found <see cref="IConfigurationGroup"/>.
        /// The type must be defined in the <paramref name="configuration"/> with an <see cref="Configuration.IConfigurationItem"/> with the key: "Type" and the string value of either a <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> the configuration should be instantiated into.</typeparam>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The group name to search for.</param>
        /// <param name="result">The instantiated type or null.</param>
        /// <param name="exception">An exception describing any problems with trying to instantiate the found <see cref="IConfigurationGroup"/>.</param>
        /// <returns><b>True</b> if the <see cref="IConfigurationGroup"/> was found and successfully instantiated; otherwise, <b>false</b>.</returns>
        public static bool TryInstantiateTypeFromConfigurationSubGroup<T>(IConfigurationGroup configuration, string name, out T result, out Exception exception)
            where T : class
        {
            try
            {
                result = InstantiateTypeFromConfigurationSubGroup<T>(configuration, name, true);
                exception = null;
                return (result != null);
            }
            catch (Exception ex)
            {
                result = null;
                exception = ex;
                return false;
            }
        }
        /// <summary>
        /// Finds the named <see cref="IConfigurationItem"/> in the given configuration group and tests it's type and returns an instance of the <paramref name="configuration"/> strongly typed to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> the configuration item should be instantiated into.</typeparam>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The name of the <see cref="IConfigurationItem"/> to search for.</param>
        /// <param name="result">The value of the named <see cref="IConfigurationItem"/> or the default value for <typeparamref name="T"/>.</param>
        /// <param name="exception">An exception describing any problems with trying to instantiate the found <see cref="IConfigurationItem"/>.</param>
        /// <returns><b>True</b> if the <see cref="IConfigurationItem"/> was found and successfully instantiated; otherwise, <b>false</b>.</returns>
        public static bool TryInstantiateTypeFromConfigurationItem<T>(IConfigurationGroup configuration, string name, out T result, out Exception exception)
        {
            try
            {
                result = InstantiateTypeFromConfigurationItem<T>(configuration, name, true);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                result = default(T);
                exception = ex;
                return false;
            }
        }


        // ################ FIND

        /// <summary>
        /// Finds the named <see cref="IConfigurationGroup"/> in the given configuration group.
        /// </summary>
        /// <remarks>Only searches the groups in the <paramref name="configuration"/>, it will not search subgroups; to search through the hierarchy of groups, see: <see cref="SearchForGroup(IConfigurationGroup, string)"/>.</remarks>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The group name to search for.</param>
        /// <param name="throwException">If the group is not found, <b>True</b> will throw an <see cref="Exception"/>, <b>false</b> will return null.</param>
        /// <returns>The named <see cref="IConfigurationGroup"/> or null.</returns>
        public static IConfigurationGroup FindConfigurationGroup(IConfigurationGroup configuration, string name, bool throwException)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                // FYI: this is not subject to the "throwException" check because this indicates a coding error.
                throw new ArgumentNullException(nameof(name));
            }
            if (!configuration.ContainsKey(name))
            {
                if (throwException)
                {
                    throw new Exception($"Configuration missing group. Configuration must have group: '{name}'.");
                }
                else
                {
                    return null;
                }
            }
            return configuration[name];
        }
        /// <summary>
        /// Attempts to finds the named <see cref="IConfigurationGroup"/> in the given configuration group.
        /// </summary>
        /// <remarks>Only searches the groups in the <paramref name="configuration"/>, it will not search subgroups; to search through the hierarchy of groups, see: <see cref="SearchForGroup(IConfigurationGroup, string)"/>.</remarks>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The group name to search for.</param>
        /// <param name="foundConfigurationGroup">The named <see cref="IConfigurationGroup"/> or null.</param>
        /// <param name="exception">An exception describing any problems with trying to find the specified <see cref="IConfigurationGroup"/>.</param>
        /// <returns>
        /// Returns whether the <paramref name="configuration"/> was found or not. 
        /// <b>True</b> indicates that the <paramref name="configuration"/> was found; otherwise, <b>false</b> indicates that the <paramref name="configuration"/> was not found.
        /// </returns>
        public static bool TryFindConfigurationGroup(IConfigurationGroup configuration, string name, out IConfigurationGroup foundConfigurationGroup, out Exception exception)
        {
            try
            {
                foundConfigurationGroup = FindConfigurationGroup(configuration, name, true);
                exception = null;
                return (foundConfigurationGroup != null);
            }
            catch (Exception ex)
            {
                exception = ex;
                foundConfigurationGroup = null;
                return false;
            }
        }
        // ----------------
        /// <summary>
        /// Finds the named <see cref="IConfigurationItem"/> in the given configuration group and tests it's type.
        /// </summary>
        /// <remarks>Only searches the items in the <paramref name="configuration"/>, it will not search items from other subgroups; to search through the hierarchy of groups and their items, see: <see cref="SearchForGroupItem(IConfigurationGroup, string)"/> and <see cref="SearchForGroupItem(IConfigurationGroup, string, string)"/>.</remarks>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The name of the <see cref="IConfigurationItem"/> to search for.</param>
        /// <param name="type">The <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/> the item should be.</param>
        /// <returns>The named <see cref="IConfigurationItem"/> or null.</returns>
        private static IConfigurationItem FindConfigurationItem(IConfigurationGroup configuration, string name, string type)
        {
            // check name
            if (!configuration.Items.ContainsKey(name))
            {
                throw new Exception($"Configuration missing item. Configuration must have item: '{name}'.");
            }
            // test for internal, runtime only, type (in theory, this should never happen)
            if (type.StartsWith("System.RuntimeType"))
            {
                throw new Exception("dodSON.Core.Configuration.ConfigurationHelper::FindConfigurationItem(IConfigurationGroup, string, string) --> Received a System.RuntimeType...");
            }
            // check type
            if (!configuration.Items[name].ValueType.StartsWith(type))
            {
                throw new Exception($"Configuration item invalid format. Configuration item '{name}' must be a <{type}>.");
            }
            return configuration.Items[name];
        }
        /// <summary>
        /// Finds the named <see cref="IConfigurationItem"/> in the given configuration group and tests it's type.
        /// </summary>
        /// <remarks>Only searches the items in the <paramref name="configuration"/>, it will not search items from other subgroups; to search through the hierarchy of groups and their items, see: <see cref="SearchForGroupItem(IConfigurationGroup, string)"/> and <see cref="SearchForGroupItem(IConfigurationGroup, string, string)"/>.</remarks>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The name of the <see cref="IConfigurationItem"/> to search for.</param>
        /// <param name="type">The <see cref="Type"/> the item should be.</param>
        /// <returns>The named <see cref="IConfigurationItem"/> or null.</returns>
        public static IConfigurationItem FindConfigurationItem(IConfigurationGroup configuration, string name, Type type)
        {
            if (ConfigurationShared._AllKnownTypes.Contains(type))
            {
                return FindConfigurationItem(configuration, name, type.FullName);
            }
            else
            {
                return FindConfigurationItem(configuration, name, type.AssemblyQualifiedName);
            }
        }
        /// <summary>
        /// Finds the named <see cref="IConfigurationItem"/> in the given configuration group and tests it's type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> the item should be.</typeparam>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The name of the <see cref="IConfigurationItem"/> to search for.</param>
        /// <returns>The named <see cref="IConfigurationItem"/> or null.</returns>
        public static IConfigurationItem FindConfigurationItem<T>(IConfigurationGroup configuration, string name) => FindConfigurationItem(configuration, name, typeof(T));
        /// <summary>
        /// Finds the named <see cref="IConfigurationItem"/> in the given configuration group and returns it's strongly typed <see cref="IConfigurationItem.Value"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> the <see cref="IConfigurationItem"/>'s <see cref="IConfigurationItem.Value"/> should be.</typeparam>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The name of the <see cref="IConfigurationItem"/> to search for.</param>
        /// <returns>The named <see cref="IConfigurationItem"/>'s strongly typed <see cref="IConfigurationItem.Value"/>.</returns>
        public static T FindConfigurationItemValue<T>(IConfigurationGroup configuration, string name) => (T)FindConfigurationItem(configuration, name, typeof(T)).Value;
        /// <summary>
        /// Attempts to find the named <see cref="IConfigurationItem"/> in the given configuration group and tests it's type.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The name of the <see cref="IConfigurationItem"/> to search for.</param>
        /// <param name="type">The <see cref="Type.FullName"/> or <see cref="Type.AssemblyQualifiedName"/> the item should be.</param>
        /// <param name="configurationItem">The <see cref="IConfigurationItem"/> that was found or null.</param>
        /// <param name="exception">An exception describing any problems with trying to find the configuration item.</param>
        /// <returns><b>True</b> if found, with the <paramref name="configurationItem"/> populated with the <see cref="IConfigurationItem"/> matching the <paramref name="name"/>; otherwise, <b>false</b> if not found.</returns>
        public static bool TryFindConfigurationItem(IConfigurationGroup configuration,
                                                    string name,
                                                    string type,
                                                    out IConfigurationItem configurationItem,
                                                    out Exception exception)
        {
            try
            {
                configurationItem = FindConfigurationItem(configuration, name, type);
                exception = null;
                return (configurationItem != null);
            }
            catch (Exception ex)
            {
                configurationItem = null;
                exception = ex;
                return false;
            }
        }
        /// <summary>
        /// Attempts to find the named <see cref="IConfigurationItem"/> in the given configuration group and tests it's type.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The name of the <see cref="IConfigurationItem"/> to search for.</param>
        /// <param name="type">The <see cref="Type"/> the item should be.</param>
        /// <param name="configurationItem">The <see cref="IConfigurationItem"/> that was found or null.</param>
        /// <param name="exception">An exception describing any problems with trying to find the configuration item.</param>
        /// <returns><b>True</b> if found, with the <paramref name="configurationItem"/> populated with the <see cref="IConfigurationItem"/> matching the <paramref name="name"/>; otherwise, <b>false</b> if not found.</returns>
        public static bool TryFindConfigurationItem(IConfigurationGroup configuration,
                                                    string name,
                                                    Type type,
                                                    out IConfigurationItem configurationItem,
                                                    out Exception exception)
        {
            try
            {
                configurationItem = FindConfigurationItem(configuration, name, type);
                exception = null;
                return (configurationItem != null);
            }
            catch (Exception ex)
            {
                configurationItem = null;
                exception = ex;
                return false;
            }
        }
        /// <summary>
        /// Attempts to find the named <see cref="IConfigurationItem"/> in the given configuration group and tests it's type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> the item should be.</typeparam>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The name of the <see cref="IConfigurationItem"/> to search for.</param>
        /// <param name="configurationItem">The <see cref="IConfigurationItem"/> that was found or null.</param>
        /// <param name="exception">An exception describing any problems with trying to find the configuration item.</param>
        /// <returns><b>True</b> if found, with the <paramref name="configurationItem"/> populated with the <see cref="IConfigurationItem"/> matching the <paramref name="name"/>; otherwise, <b>false</b> if not found.</returns>
        public static bool TryFindConfigurationItem<T>(IConfigurationGroup configuration,
                                                       string name,
                                                       out IConfigurationItem configurationItem,
                                                       out Exception exception) => TryFindConfigurationItem(configuration, name, typeof(T), out configurationItem, out exception);
        /// <summary>
        /// Attempts to find the named <see cref="IConfigurationItem"/> in the given configuration group and returns it's strongly typed <see cref="IConfigurationItem.Value"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> the item should be.</typeparam>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="name">The name of the <see cref="IConfigurationItem"/> to search for.</param>
        /// <param name="value">The <see cref="IConfigurationItem"/>'s <see cref="IConfigurationItem.Value"/> that was found or null.</param>
        /// <param name="exception">An exception describing any problems with trying to find the configuration item or null.</param>
        /// <returns><b>True</b> if found, with the named <see cref="IConfigurationItem"/> which matched the <paramref name="name"/>; otherwise, <b>false</b> if not found.</returns>
        public static bool TryFindConfigurationItemValue<T>(IConfigurationGroup configuration,
                                                            string name,
                                                            out T value,
                                                            out Exception exception)
        {
            if (TryFindConfigurationItem<T>(configuration, name, out IConfigurationItem configurationItem, out exception))
            {
                value = (T)configurationItem.Value;
                return true;
            }
            value = default;
            return false;
        }

        // ################ SEARCH

        /// <summary>
        /// Will search the <paramref name="configuration"/> for the specified <paramref name="searchGroupAddress"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="searchGroupAddress">The Group Address to search for. This can include subgroups separated by the <see cref="GroupNameSeparator"/>.</param>
        /// <returns>If found, will return the <see cref="IConfigurationGroup"/> with matching <paramref name="searchGroupAddress"/>; otherwise, if not found, will return <b>null</b>.</returns>
        public static IConfigurationGroup SearchForGroup(IConfigurationGroup configuration, string searchGroupAddress)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (string.IsNullOrWhiteSpace(searchGroupAddress))
            {
                throw new ArgumentNullException(nameof(searchGroupAddress));
            }
            //
            return FindGroupByGroupAddressRecursiveFunc(configuration, searchGroupAddress);
        }
        /// <summary>
        /// Attempts to find the <see cref="IConfigurationGroup"/> for the specified <paramref name="searchGroupAddress"/> in the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="searchGroupAddress">The Group Address to search for. This can include subgroups separated by the <see cref="GroupNameSeparator"/>.</param>
        /// <param name="foundConfigurationGroup">The <see cref="IConfigurationGroup"/> with the matching <paramref name="searchGroupAddress"/>; otherwise, <b>null</b> if not found.</param>
        /// <param name="exception">An exception describing any problems with trying to find the configuration group.</param>
        /// <returns><b>True</b> if found, with the <paramref name="foundConfigurationGroup"/> populated with the <see cref="IConfigurationGroup"/> matching the <paramref name="searchGroupAddress"/>; otherwise, <b>false</b> if not found.</returns>
        public static bool TrySearchForGroup(IConfigurationGroup configuration, string searchGroupAddress, out IConfigurationGroup foundConfigurationGroup, out Exception exception)
        {
            try
            {
                exception = null;
                foundConfigurationGroup = SearchForGroup(configuration, searchGroupAddress);
                return (foundConfigurationGroup != null);
            }
            catch (Exception ex)
            {
                exception = ex;
                foundConfigurationGroup = null;
                return false;
            }
        }
        // ----------------
        /// <summary>
        /// Will search the <paramref name="configuration"/> for the specified <paramref name="searchGroupAddress"/> and <paramref name="searchItemName"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="searchGroupAddress">The Group Address to search for. This can include subgroups separated by the <see cref="GroupNameSeparator"/>.</param>
        /// <param name="searchItemName">The Item to search for.</param>
        /// <returns>If found, will return the <see cref="IConfigurationItem"/> with matching <paramref name="searchGroupAddress"/> and <paramref name="searchItemName"/>; otherwise, if not found, will return <b>null</b>.</returns>
        public static IConfigurationItem SearchForGroupItem(IConfigurationGroup configuration, string searchGroupAddress, string searchItemName)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (string.IsNullOrWhiteSpace(searchGroupAddress))
            {
                throw new ArgumentNullException(nameof(searchGroupAddress));
            }
            // 
            var group = FindGroupByGroupAddressRecursiveFunc(configuration, searchGroupAddress);
            if (group != null)
            {
                foreach (var item in group.Items)
                {
                    if (item.Key.Equals(searchItemName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Will search the <paramref name="configuration"/> for the specified <paramref name="searchGroupAddressAndItemName"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="searchGroupAddressAndItemName">
        /// The Group Address and Item Name to search for.
        /// The Group Address and Item Name must be separated by the <see cref="GroupNameSeparator"/>. 
        /// The Group Address can also include subgroups separated by the <see cref="GroupNameSeparator"/>.
        /// </param>
        /// <returns>If found, will return the <see cref="IConfigurationItem"/> with matching <paramref name="searchGroupAddressAndItemName"/>; otherwise, if not found, will return <b>null</b>.</returns>
        public static IConfigurationItem SearchForGroupItem(IConfigurationGroup configuration, string searchGroupAddressAndItemName)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (string.IsNullOrWhiteSpace(searchGroupAddressAndItemName))
            {
                throw new ArgumentNullException(nameof(searchGroupAddressAndItemName));
            }
            //
            if (searchGroupAddressAndItemName.Contains(GroupNameSeparator))
            {
                // search all groups
                var groupAddress = searchGroupAddressAndItemName.Substring(0, searchGroupAddressAndItemName.LastIndexOf(GroupNameSeparator));
                var itemName = searchGroupAddressAndItemName.Substring(searchGroupAddressAndItemName.LastIndexOf(GroupNameSeparator) + 1);
                return SearchForGroupItem(configuration, groupAddress, itemName);
            }
            else
            {
                // search local group's item
                var itemName = searchGroupAddressAndItemName;
                foreach (var item in configuration.Items)
                {
                    if (item.Key.Equals(itemName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return item;
                    }
                }
                //
                return null;
            }
        }
        /// <summary>
        /// Will search the <paramref name="configuration"/> for the specified <paramref name="searchGroupAddress"/> and <paramref name="searchItemName"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="searchGroupAddress">The Group Address to search for. This can include subgroups separated by the <see cref="GroupNameSeparator"/>.</param>
        /// <param name="searchItemName">The Item to search for.</param>
        /// <param name="foundConfigurationItem">The <see cref="IConfigurationItem"/> with the matching <paramref name="searchGroupAddress"/> and <paramref name="searchItemName"/>; otherwise, <b>null</b> if not found.</param>
        /// <param name="exception">An exception describing any problems with trying to find the configuration item.</param>
        /// <returns><b>True</b> if found, with the <paramref name="foundConfigurationItem"/> populated with the <see cref="IConfigurationItem"/> matching the <paramref name="searchGroupAddress"/> and <paramref name="searchItemName"/>; otherwise, <b>false</b> if not found.</returns>
        public static bool TrySearchForGroupItem(IConfigurationGroup configuration, string searchGroupAddress, string searchItemName, out IConfigurationItem foundConfigurationItem, out Exception exception)
        {
            try
            {
                exception = null;
                foundConfigurationItem = SearchForGroupItem(configuration, searchGroupAddress, searchItemName);
                return (foundConfigurationItem != null);
            }
            catch (Exception ex)
            {
                exception = ex;
                foundConfigurationItem = null;
                return false;
            }
        }
        /// <summary>
        /// Will search the <paramref name="configuration"/> for the specified <paramref name="searchGroupAddressAndItemName"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to search.</param>
        /// <param name="searchGroupAddressAndItemName">
        /// The Group Address and Item Name to search for.
        /// The Group Address and Item Name must be separated by the <see cref="GroupNameSeparator"/>. 
        /// The Group Address can also include subgroups separated by the <see cref="GroupNameSeparator"/>.
        /// </param>
        /// <param name="foundConfigurationItem">The <see cref="IConfigurationItem"/> with the matching <paramref name="searchGroupAddressAndItemName"/>; otherwise, <b>null</b> if not found.</param>
        /// <param name="exception">An exception describing any problems with trying to find the configuration item.</param>
        /// <returns><b>True</b> if found, with the <paramref name="foundConfigurationItem"/> populated with the <see cref="IConfigurationItem"/> matching the <paramref name="searchGroupAddressAndItemName"/>; otherwise, <b>false</b> if not found.</returns>
        public static bool TrySearchForGroupItem(IConfigurationGroup configuration, string searchGroupAddressAndItemName, out IConfigurationItem foundConfigurationItem, out Exception exception)
        {
            try
            {
                exception = null;
                foundConfigurationItem = SearchForGroupItem(configuration, searchGroupAddressAndItemName);
                return (foundConfigurationItem != null);
            }
            catch (Exception ex)
            {
                exception = ex;
                foundConfigurationItem = null;
                return false;
            }
        }


        // ################ INFORMATION

        /// <summary>
        /// Returns all of the Group Addresses found in the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationGroup"/> to retrieve the Group Addresses from.</param>
        /// <returns>All of the Group Addresses found in the given <paramref name="configuration"/>.</returns>
        public static IEnumerable<string> AllGroupAddresses(IConfigurationGroup configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            //
            var list = new List<string>();
            foreach (var group in configuration)
            {
                AllGroupAddressesRecursiveFunc(group, list);
            }
            //
            return list;


            // ################################
            // Internal Methods
            // ################################

            void AllGroupAddressesRecursiveFunc(IConfigurationGroup group_, List<string> list_)
            {
                if (!list_.Contains(group_.GroupAddress))
                {
                    list_.Add(group_.GroupAddress);
                    foreach (var subGroup in group_)
                    {
                        AllGroupAddressesRecursiveFunc(subGroup, list_);
                    }
                }
            }
        }
        /// <summary>
        /// For known <see cref="Type"/>s, this function will return the <paramref name="candidateTypeName"/>'s <see cref="Type.FullName"/>; otherwise, it will return the <paramref name="candidateTypeName"/>.
        /// </summary>
        /// <param name="candidateTypeName">The name of the <see cref="Type"/> to search for.</param>
        /// <returns>The <see cref="Type.FullName"/> of a known <see cref="Type"/>; otherwise, the <paramref name="candidateTypeName"/>.</returns>
        public static string IsKnownTypeName(string candidateTypeName)
        {
            var workStr = candidateTypeName;
            if (workStr.StartsWith("System.", StringComparison.InvariantCultureIgnoreCase))
            {
                workStr = workStr.Substring(7);
            }
            switch (workStr.ToLower())
            {
                // #### .NET and C# Style
                case "sbyte":
                    candidateTypeName = typeof(sbyte).FullName;
                    break;
                case "byte":
                    candidateTypeName = typeof(byte).FullName;
                    break;
                case "short":
                case "int16":
                    candidateTypeName = typeof(short).FullName;
                    break;
                case "ushort":
                case "uint16":
                    candidateTypeName = typeof(ushort).FullName;
                    break;
                case "int":
                case "int32":
                    candidateTypeName = typeof(int).FullName;
                    break;
                case "uint":
                case "uint32":
                    candidateTypeName = typeof(uint).FullName;
                    break;
                case "long":
                case "int64":
                    candidateTypeName = typeof(long).FullName;
                    break;
                case "ulong":
                case "uint64":
                    candidateTypeName = typeof(ulong).FullName;
                    break;
                case "float":
                    candidateTypeName = typeof(float).FullName;
                    break;
                case "single":
                    candidateTypeName = typeof(Single).FullName;
                    break;
                case "double":
                    candidateTypeName = typeof(double).FullName;
                    break;
                case "decimal":
                    candidateTypeName = typeof(decimal).FullName;
                    break;
                case "bool":
                case "boolean":
                    candidateTypeName = typeof(bool).FullName;
                    break;
                case "string":
                    candidateTypeName = typeof(string).FullName;
                    break;
                case "char":
                    candidateTypeName = typeof(char).FullName;
                    break;
                // #### known types
                case "datetime":
                    candidateTypeName = typeof(DateTime).FullName;
                    break;
                case "datetimeoffset":
                    candidateTypeName = typeof(DateTimeOffset).FullName;
                    break;
                case "timespan":
                    candidateTypeName = typeof(TimeSpan).FullName;
                    break;
                case "guid":
                    candidateTypeName = typeof(Guid).FullName;
                    break;
                case "version":
                    candidateTypeName = typeof(Version).FullName;
                    break;
                case "type":
                    candidateTypeName = typeof(Type).FullName;
                    break;
                // ##### 
                default:
                    // FYI: This was removed because it will load assemblies into the current application domain.
                    //      For [System.] stuff that may be alright; but, for now, I decided against it all together.
                    //      Further review is in order.

                    //if (candidateTypeName.StartsWith("System.", StringComparison.InvariantCultureIgnoreCase))
                    //{
                    //    try { candidateTypeName = Type.GetType(candidateTypeName, true, true).AssemblyQualifiedName; }
                    //    catch { }
                    //}
                    break;
            }
            return candidateTypeName;
        }
        #endregion
        #region Private Methods
        private static IConfigurationGroup FindGroupByGroupAddressRecursiveFunc(IConfigurationGroup group, string searchGroupAddress)
        {
            if (group.GroupAddress == searchGroupAddress)
            {
                return group;
            }
            //
            foreach (var subGroup in group)
            {
                var dude = FindGroupByGroupAddressRecursiveFunc(subGroup, searchGroupAddress);
                if (dude != null)
                {
                    return dude;
                }
            }
            return null;
        }
        #endregion
    }
}

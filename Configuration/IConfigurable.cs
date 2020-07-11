using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// <para>Defines a mechanism to retrieving the state of an object into an <see cref="Configuration.IConfigurationGroup"/>. Also allows for types to be constructed from <see cref="IConfigurationGroup"/>s.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Types needing to be constructed from configuration files should implement the <see cref="IConfigurable"/> interface. 
    /// </para>
    /// <para>
    /// The <see cref="Configuration"/> property will populate an <see cref="IConfigurationGroup"/> containing data needed to serialize the target object.
    /// <br/><br/>
    /// The <see cref="IConfigurable"/> interface implies a constructor with the signature Ctor(<see cref="IConfigurable"/> configuration).
    /// If this constructor is missing, an exception will be thrown. 
    /// The constructor should interpret the <see cref="IConfigurationGroup"/>, populating itself with data and instantiating required objects.
    /// <br/><br/>
    /// Types that implement <see cref="IConfigurable"/> can be instantiated using the <see cref="ConfigurationHelper.InstantiateTypeFromConfigurationGroup(IConfigurationGroup)"/> or the <see cref="ConfigurationHelper.InstantiateTypeFromConfigurationGroup(Type, IConfigurationGroup)"/> methods.
    /// To configure the <see cref="Type"/> of object to instantiate, include an <see cref="IConfigurationItem"/> with the key="Type" containing the <see cref="Type"/> to instantiate.
    /// The <see cref="IConfigurationGroup"/> can be serialized and deserialized into a variety of forms; XML, INI and Comma-Delimited Values are some of the <see cref="IConfigurationSerializer{T}"/> implementations included in the dodSON Software Core Library.
    /// </para>
    /// </remarks>
    /// <seealso cref="BinaryConfigurationSerializer"/>
    /// <seealso cref="CsvConfigurationSerializer"/>
    /// <seealso cref="IniConfigurationSerializer"/>
    /// <seealso cref="XmlConfigurationSerializer"/>
    /// <seealso cref="IConfigurationSerializer{T}"/>
    /// <seealso cref="ConfigurationHelper.InstantiateTypeFromConfigurationGroup(IConfigurationGroup)"/>
    /// <seealso cref="ConfigurationHelper.InstantiateTypeFromConfigurationGroup(Type, IConfigurationGroup)"/>
    public interface IConfigurable
    {
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        Configuration.IConfigurationGroup Configuration { get; }
    }
}

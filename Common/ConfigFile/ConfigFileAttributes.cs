using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Gongchengshi.ConfigFile
{
    /// <summary>
    /// Identifies the root/document element of the configuration file.
    /// </summary>
    public class ConfigurationRootAttribute : XmlRootAttribute
    {
        protected readonly int _version;

        public ConfigurationRootAttribute(int version)
        {
            _version = version;
        }

        protected ConfigurationRootAttribute(string name, int version) : base(name)
        {
            _version = version;
        }

        public static string Name(Type type)
        {
            return type.Get<XmlRootAttribute>().ElementName;
        }

        public static string Version(Type type)
        {
            return type.Get<ConfigurationRootAttribute>()._version.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Identifies a top level section in the configuration file
    /// </summary>
    public class ConfigurationSectionAttribute : ConfigurationRootAttribute
    {
        public ConfigurationSectionAttribute(string name, int version) : base(name, version)
        {
        }
    }
}
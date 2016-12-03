using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Gongchengshi
{
    public static class EnumExtensions
    {
        public static T[] GetEnumValues<T>()
        {
            var type = typeof(T);
            
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            return (
                from field in type.GetFields(BindingFlags.Public | BindingFlags.Static)
                where field.IsLiteral
                select (T)field.GetValue(null)
                ).ToArray();
        }

        public static string[] GetEnumStrings<T>()
        {
            var type = typeof(T);

            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            return (
                from field in type.GetFields(BindingFlags.Public | BindingFlags.Static)
                where field.IsLiteral
                select field.Name
                ).ToArray();
        }

        public static string GetDescription(this Enum value)
        {
            return GetAttribute<DescriptionAttribute>(value).Description;
        }

        public static T GetAttribute<T>(this Enum value) where T: Attribute
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    return Attribute.GetCustomAttribute(field, typeof(T)) as T;
                }
            }
            return null;
        }

#if SILVERLIGHT
        public static T[] GetValues<T>()
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            var fields = enumType.GetFields().Where(field => field.IsLiteral);

            return fields.Select(field => field.GetValue(enumType)).Select(value => (T) value).ToArray();            
        }

        public static object[] GetValues(this Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            var fields = enumType.GetFields().Where(field => field.IsLiteral);

            return fields.Select(field => field.GetValue(enumType)).ToArray();
        }
#endif
    }


    /// <summary>
    /// Retrieves the XmlEnum attribute from enum values.  Note that if using the 
    /// XmlEnum attribute, the first item in the enum (the one with value 0) must
    /// be the N/A value, and must not have an XmlEnum attribute.
    /// </summary>
    public static class XmlEnumHelper
    {
        public static string ToString(Enum value)
        {
            // 0 is the N/A value.
            if (value == null || Convert.ToInt32(value) == 0)
            {
                return null;
            }

            var xmlEnumAttribute = value.GetAttribute<XmlEnumAttribute>();

            if (xmlEnumAttribute == null)
            {
                var result = Enum.GetName(value.GetType(), value);

                return result;
            }

            return xmlEnumAttribute.Name;
        }

        public static T FromString<T>(string value) where T : struct
        {
#if SILVERLIGHT
            foreach (var o in EnumExtensions.GetValues(typeof(T)))
#else
            foreach (var o in Enum.GetValues(typeof(T)))
#endif
            {
                var enumValue = o as Enum;

                var xmlEnumAttribute = enumValue.GetAttribute<XmlEnumAttribute>();

                var name = (xmlEnumAttribute != null) ? 
                    xmlEnumAttribute.Name :
                    Enum.GetName(o.GetType(), o);

                if(name == value)
                {
                    return (T)o;
                }
            }

            throw new Exception(string.Format("No enumeration value exists for '{0}'.", value));
        }
    }
}

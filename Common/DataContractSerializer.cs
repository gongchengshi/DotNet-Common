using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Gongchengshi
{
    /// <summary>
    /// Generic version of DataContractSerializer
    /// </summary>
    /// <typeparam name="T">Type to be serialized</typeparam>
    public class DataContractXmlSerializer<T>
    {
#if !SILVERLIGHT
        private readonly string _path;
        private readonly DataContractSerializer _serializer = new DataContractSerializer(typeof (T));

        public DataContractXmlSerializer(string path)
        {
            _path = path;
        }

        public void Write(T obj)
        {
            using (var writer = XmlWriter.Create(_path))
            {
                _serializer.WriteObject(writer, obj);
            }
        }

        public T Read()
        {
            using (var reader = XmlReader.Create(_path))
            {
                return (T) _serializer.ReadObject(reader);
            }
        }
#endif

        public static string ToString(T obj)
        {
            return DataContractXmlSerializer.ToString(obj);
        }

        public static T FromString(string xml)
        {
            return DataContractXmlSerializer.FromString<T>(xml);
        }
    }

    public static class DataContractXmlSerializer
    {
        private static readonly XmlWriterSettings _writerSettings = new XmlWriterSettings
                                                                        {
                                                                            NewLineHandling = NewLineHandling.Replace,
                                                                            NewLineChars = "\n"
                                                                        };

        public static string ToString<T>(T obj)
        {
            return ToString(obj, typeof (T));
        }

        public static string ToString<T>(T obj, Type type)
        {            
            var stringBuilder = new StringBuilder();

            using (var writer = XmlWriter.Create(stringBuilder, _writerSettings))
            {
                new DataContractSerializer(type).WriteObject(writer, obj);
            }

            return stringBuilder.ToString();
        }

        public static T FromString<T>(string xml)
        {
            return (T)FromString(xml, typeof (T));
        }

        public static object FromString(string xml, Type type)
        {
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                return new DataContractSerializer(type).ReadObject(reader);
            }
        }
    }
}

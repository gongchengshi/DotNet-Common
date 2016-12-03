using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Gongchengshi
{
    public static class XmlSerializer<T>
    {
        public static string ToString(T obj)
        {
            using (var writer = new StringWriter(new StringBuilder()))
            {
                new XmlSerializer(typeof (T)).Serialize(writer, obj);

                return writer.ToString();
            }
        }
    }
}
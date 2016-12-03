using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Gongchengshi.ConfigFile
{
    /// <summary>
    /// The ConfigFile class allows any module to write to an XML config file without any information about the module
    /// known by the configuration file before hand.  This is especially usefull in a plug-in architecture where
    /// a single configuration file is shared by multiple plug-ins.
    /// 
    /// public class ConfigFile : ConfigFile&ltConfiguration&rt
    /// {
    ///     public ConfigFile(string path) : base(path) {}
    /// }
    /// 
    /// [Serializable]
    /// [ConfigurationRoot(1)]
    /// public class Configuration {}
    /// </summary>
    /// <typeparam name="TRoot">The class that will contain the sections</typeparam>
    public class ConfigFile<TRoot> where TRoot : new()
    {
        protected readonly string _path;
        public readonly string BackupPath;

        protected readonly string _version;
        protected const string _versionAttributeName = "Version";

        protected static readonly XmlReaderSettings _readerSettings = new XmlReaderSettings {CloseInput = false};
        protected static readonly XmlWriterSettings _writerSettings = new XmlWriterSettings
                                                                       { CloseOutput = true, Indent = true };

        protected readonly Action _deleteConfigFile;
        
        static ConfigFile()
        {
            Debug.Assert(typeof (TRoot).IsOn<ConfigurationRootAttribute>(), 
                string.Format("TRoot must have the {0} attribute.", typeof(ConfigurationRootAttribute)));
        }

        public ConfigFile(string path)
        {
            _version = ConfigurationRootAttribute.Version(typeof (TRoot));

            _path = path;
            BackupPath = _path + ".bak";

            _deleteConfigFile = () => File.Delete(_path);
        }

        private static XmlNode CreateElementFromSection<T>(T section)
        {
            var sectionDoc = new XmlDocument();
            sectionDoc.LoadXml(XmlSerializer<T>.ToString(section));

            var element = sectionDoc.DocumentElement;
            element.RemoveAttribute("xmlns:xsi");
            element.RemoveAttribute("xmlns:xsd");
            element.SetAttribute(_versionAttributeName, ConfigurationSectionAttribute.Version(typeof(T)));
            return element;
        }

        private XmlDocument OpenDocument()
        {
            var doc = new XmlDocument();

            try
            {
                using (var reader = GetReader())
                {
                    doc.Load(reader);
                }
            }
            catch (XmlException ex)
            {
                if (ex.Message.Contains("Root element is missing"))
                {
                    doc.LoadXml(XmlSerializer<TRoot>.ToString(new TRoot()));
                }
            }

            if (string.IsNullOrEmpty(doc.DocumentElement.GetAttribute(_versionAttributeName)))
            {
                doc.DocumentElement.SetAttribute(_versionAttributeName, _version);
            }

            return doc;
        }

        private void WriteDocument(XmlDocument doc)
        {
            using (var writer = XmlWriter.Create(GetWriter(BackupPath), _writerSettings))
            {
                // doc.WriteTo(writer) appears to do the same thing as doc.Save(writer)
                // but if there is ever a problem, tak a look at WriteTo().
                doc.Save(writer);
            }

            _deleteConfigFile.WhileCatch<IOException>(() => Thread.Sleep(100), 50);
            SwapConfigFiles();
        }

        private void SwapConfigFiles()
        {
            try
            {
                File.Move(BackupPath, _path);
            }
            catch (IOException)
            {
                // Ignore this exception.  Another process beat us to it--which 
                // is just fine, the file got moved.
            }
        }

        private static void DeleteSection(XmlDocument doc, string sectionName)
        {
            var root = doc.DocumentElement;

            // This overwrites the previous section of the same name if there is one
            for (int i = 0; i < root.ChildNodes.Count; ++i)
            {
                if (root.ChildNodes[i].Name == sectionName)
                {
                    root.RemoveChild(root.ChildNodes[i]);
                    break;
                }
            }
        }

        public void DeleteSection<T>()
        {
            var sectionName = ConfigurationSectionAttribute.Name(typeof(T));

            var doc = OpenDocument();
            DeleteSection(doc, sectionName);
            WriteDocument(doc);
        }

        /// <summary>
        /// This overwrites the previous section of the same name if there is one
        /// </summary>
        private void Write<T>(T section, XmlDocument doc)
        {
            var sectionElement = CreateElementFromSection(section);

            DeleteSection(doc, sectionElement.Name);

            doc.DocumentElement.AppendChild(doc.ImportNode(sectionElement, true));
        }

        public void Write<T>(T section)
        {
            var doc = OpenDocument();
            Write(section, doc);
            WriteDocument(doc);
        }

        public void Write<T1, T2>(T1 section1, T2 section2)
        {
            var doc = OpenDocument();
            Write(section1, doc);
            Write(section2, doc);
            WriteDocument(doc);
        }

        public void Write<T1, T2, T3>(T1 section1, T2 section2, T3 section3)
        {
            var doc = OpenDocument();
            Write(section1, doc);
            Write(section2, doc);
            Write(section3, doc);
            WriteDocument(doc);
        }

        private static T Read<T>(Stream reader) where T : new()
        {
            using (var xmlReader = XmlReader.Create(reader, _readerSettings))
            {
                var type = typeof (T);

                var sectionName = ConfigurationSectionAttribute.Name(type);

                try
                {
                    if (!xmlReader.ReadToDescendant(sectionName))
                    {
                        // There is no section with this name in the file so return an empty one
                        return new T();
                    }
                }
                catch (XmlException)
                {
                    // Most likely caused by "Root element is missing"
                    return new T();
                }

                var serializer = new XmlSerializer(type, type.Get<ConfigurationSectionAttribute>());
                return (T) serializer.Deserialize(xmlReader.ReadSubtree());
            }
        }

        public T Read<T>() 
            where T : new()
        {
            using (var reader = GetReader())
            {
                return Read<T>(reader);
            }
        }

        public KeyedByTypeCollection<object> Read<T1, T2>()
            where T1 : new()
            where T2 : new()
        {
            var result = new KeyedByTypeCollection<object>();

            using (var reader = GetReader())
            {
                result.Add(Read<T1>(reader));
                reader.Position = 0;
                result.Add(Read<T2>(reader));
            }

            return result;
        }

        public KeyedByTypeCollection<object> Read<T1, T2, T3>()
            where T1 : new()
            where T2 : new()
            where T3 : new()
        {
            var result = new KeyedByTypeCollection<object>();

            using (var reader = GetReader())
            {
                result.Add(Read<T1>(reader));
                reader.Position = 0;
                result.Add(Read<T2>(reader));
                reader.Position = 0;
                result.Add(Read<T3>(reader));
            }

            return result;
        }

        private Stream GetReader()
        {
            if (!File.Exists(_path))
            {
                if (File.Exists(BackupPath))
                {
                    SwapConfigFiles();
                }
                else
                {
                    // If no config file, return an empty stream as a dummy.
                    return new MemoryStream();
                }
            }

            return File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private FileStream GetWriter(string path)
        {
            return File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }
    }
}

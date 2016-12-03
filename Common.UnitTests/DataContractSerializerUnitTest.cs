using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEL.Common.UnitTests
{
    [TestClass]
    public class DataContractSerializerUnitTest
    {
        [TestMethod]
        public void NoDefaultConstructor()
        {
            var shared = new SharedObject();

            var target = new Constructorless(shared, new ConcreteObject("John"), "George", "Ringo");

            var serializedResult = DataContractXmlSerializer<Constructorless>.ToString(target);

            Assert.IsNotNull(serializedResult);

            var deserializedResult = DataContractXmlSerializer<Constructorless>.FromString(serializedResult);
            deserializedResult.Initialize(shared);

            Assert.AreEqual("George", deserializedResult.ReadonlyName);
            Assert.AreEqual("Ringo", deserializedResult.Name);
            //Assert.AreEqual("John", deserializedResult.AbstractObject.Name);
        }

#if !SILVERLIGHT
        [TestMethod]
        public void ReadWriteFile()
        {
            var target = new ConcreteObject("Tom");
            var serializer = new DataContractXmlSerializer<ConcreteObject>(Path.GetTempFileName());
            serializer.Write(target);
            var deserialized = serializer.Read();
            Assert.AreEqual(target.Name, deserialized.Name);
        }
#endif
    }

    [DataContract]
    public abstract class AbstractObject
    {
        public abstract string GetName();

        [DataMember]
        public abstract string Name { get; set; }
    }

    [DataContract]
    public class ConcreteObject : AbstractObject
    {
        public ConcreteObject(string name)
        {
            Name = name;
        }

        public override string GetName()
        {
            return Name;
        }

        [DataMember]
        public override string Name { get; set; }
    }

    public class SharedObject
    {
        public string Name = "Paul";
    }

    [DataContract]
    public class Constructorless
    {
        private SharedObject _shared;

        public void Initialize(SharedObject shared)
        {
            _shared = shared;
        }

        [DataMember]
        public string Name { get { return _name; } private set { _name = value; } }
        
        private string _name;

        public AbstractObject AbstractObject;

        public string ReadonlyName { get { return _readonlyName; } }

        [DataMember]
        private readonly string _readonlyName;
        //private string _readonlyName;

        public Constructorless(SharedObject shared, AbstractObject abstractObject, string readonlyName, string name)
        {
            _shared = shared;
            AbstractObject = abstractObject;
            _readonlyName = readonlyName;
            _name = name;
        }
    }
}

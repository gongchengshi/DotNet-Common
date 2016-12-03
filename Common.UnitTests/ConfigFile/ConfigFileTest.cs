using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEL.UnitTest;

namespace SEL.ConfigFile.UnitTests
{
    [TestClass]
    public class ConfigFileTest
    {
        [ConfigurationRoot(1)]
        public class Configuration
        {}

        [ConfigurationSection("Test", 1)]
        public class TestConfigSection
        {
            public TestConfigSection()
            {
            }

            public int Value = 9;
        }
        [ConfigurationSection("Another", 1)]
        public class AnotherConfigSection
        {
            public AnotherConfigSection()
            {
            }

            public int Number = 15;
        }
        [ConfigurationSection("AThird", 1)]
        public class AThirdSection
        {
            public AThirdSection()
            {
            }

            public int Int = 23;
        }

        public static string FilePath = "configFile.xml";

        private static void Delete()
        {
            File.Delete(FilePath);
        }

        [TestMethod]
        public void MultipleReaderOrSingleWriter()
        {
            File.Delete(FilePath);
            var configFile = new ConfigFile<Configuration>(FilePath);
            configFile.Write(new TestConfigSection { Value = 6 });

            // Verify multiple readers allowed.
            const int count = 4;
            bool[] done = new bool[count];
            Parallel.For(0, count, i =>
                                   {
                                       for (int j = 0; j < 10; j++)
                                       {
                                           var section = configFile.Read<TestConfigSection>();
                                           Assert.AreEqual(6, section.Value);
                                       }
                                       done[i] = true;
                                   });

            Array.ForEach(done, Assert.IsTrue);
            

            // Verify only single writer allowed.
            bool exceptionThrown = false;
            Parallel.For(0, 2, i =>
                                   {
                                       for (int j = 0; j < 5; j++)
                                       {
                                           try
                                           {
                                               configFile.Write(new TestConfigSection { Value = j });
                                           }
                                           catch (IOException)
                                           {
                                               exceptionThrown = true;
                                           }
                                       }
                                   });
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void NonExistingFileTest()
        {
            Delete();

            var configFile = new ConfigFile<Configuration>(FilePath);

            var actual = configFile.Read<TestConfigSection>();

            Assert.IsInstanceOfType(actual, typeof(TestConfigSection));
        }

        [TestMethod]
        public void OpenTest()
        {
            var configFile = new ConfigFile<Configuration>(FilePath);
        }

        [TestMethod]
        public void WriteSectionTest()
        {
            Delete();

            var configFile = new ConfigFile<Configuration>(FilePath);

            var input = new TestConfigSection();

            configFile.Write(input);
        }

        [TestMethod]
        public void WriteReadSectionTest()
        {
            Delete();

            var configFile = new ConfigFile<Configuration>(FilePath);

            var input = new TestConfigSection { Value = 12 };

            configFile.Write(input);

            var output = configFile.Read<TestConfigSection>();

            Assert.AreEqual(input.Value, output.Value);
        }

        [TestMethod]
        public void NoRootElementTest()
        {
            Delete();
            File.WriteAllText(FilePath, String.Empty);
            new ConfigFile<Configuration>(FilePath).Write(new TestConfigSection());
        }

        [TestMethod]
        public void TestWaitingForFileAccess()
        {
            Delete();
            File.WriteAllText(FilePath, string.Empty);
            var file = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var configFile = new ConfigFile<Configuration>(FilePath);
            var task = Task.Factory.StartNew(() => configFile.Write(new TestConfigSection { Value = 43 }));
            Thread.Sleep(500);
            file.Close();
            task.Wait();
            Assert.AreEqual(43, configFile.Read<TestConfigSection>().Value);
        }

        [TestMethod]
        public void DeleteSectionTest()
        {
            Delete();
            var configFile = new ConfigFile<Configuration>(FilePath);
            var another = new AnotherConfigSection();
            var test = new TestConfigSection() { Value = 155 };
            configFile.Write(another, test);
            
            Assert.AreEqual(155, configFile.Read<TestConfigSection>().Value);
            
            configFile.DeleteSection<TestConfigSection>();
            Assert.AreEqual(15, configFile.Read<AnotherConfigSection>().Number);
            Assert.AreEqual(9, configFile.Read<TestConfigSection>().Value);
        }

        [TestMethod]
        public void WriteMultipleTest()
        {
            Delete();
            var configFile = new ConfigFile<Configuration>(FilePath);
            var one = new TestConfigSection() { Value = 1 };
            var two = new AnotherConfigSection() { Number = 2 };
            configFile.Write(one, two);

            Assert.AreEqual(1, configFile.Read<TestConfigSection>().Value);
            Assert.AreEqual(2, configFile.Read<AnotherConfigSection>().Number);
            
            Delete();
            var three = new AThirdSection() {Int = 3};
            configFile.Write(one, two, three);

            Assert.AreEqual(1, configFile.Read<TestConfigSection>().Value);
            Assert.AreEqual(2, configFile.Read<AnotherConfigSection>().Number);
            Assert.AreEqual(3, configFile.Read<AThirdSection>().Int);
        }

        [TestMethod]
        public void ReadEmptyFile()
        {
            var configFile = new ConfigFile<Configuration>(FilePath);
            Delete();
            File.WriteAllText(FilePath, String.Empty);
            Assert.AreEqual(9, configFile.Read<TestConfigSection>().Value);
        }

        [TestMethod]
        public void ReadMultipleTest()
        {
            Delete();
            var configFile = new ConfigFile<Configuration>(FilePath);
            var one = new TestConfigSection() { Value = 1 };
            var two = new AnotherConfigSection() { Number = 2 };
            var three = new AThirdSection() { Int = 3 };
            configFile.Write(one, two, three);

            var read = configFile.Read<TestConfigSection, AnotherConfigSection>();
            Assert.AreEqual(1, read.Find<TestConfigSection>().Value);
            Assert.AreEqual(2, read.Find<AnotherConfigSection>().Number);

            read = configFile.Read<TestConfigSection, AnotherConfigSection, AThirdSection>();
            Assert.AreEqual(1, read.Find<TestConfigSection>().Value);
            Assert.AreEqual(2, read.Find<AnotherConfigSection>().Number);
            Assert.AreEqual(3, read.Find<AThirdSection>().Int);
        }
    }
}

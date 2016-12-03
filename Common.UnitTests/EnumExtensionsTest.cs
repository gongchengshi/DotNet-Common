///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using SEL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SEL.UnitTest;
using System.Xml.Serialization;

namespace SEL.Common.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for EnumExtensionsTest and is intended
    ///to contain all EnumExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EnumExtensionsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        class MyAttribute : Attribute
        {
            public int Num;
            public MyAttribute(int num)
            {
                Num = num;
            }
        }
        enum TestGetAttributeEnum
        {
            Yo,
            [MyAttribute(42)]
            Dude
        }

        [TestMethod()]
        public void GetAttributeTestNull1()
        {
            object result = EnumExtensions.GetAttribute<MyAttribute>((TestGetAttributeEnum)44);
           Assert.IsTrue(result == null);
        }

        /// <summary>
        ///  This test represents a corner case that was tested manually.
        /// </summary>
        [TestMethod()]
        public void GetAttributeTestNull2()
        {
            object result = EnumExtensions.GetAttribute<MyAttribute>(ConsoleColor.Black);
            Assert.IsTrue(result == null);
        }

        [TestMethod()]
        public void GetAttributeTest()
        {
           MyAttribute att = EnumExtensions.GetAttribute<MyAttribute>(TestGetAttributeEnum.Dude);
           Assert.IsTrue(att.Num == 42);
        }

        enum TestDescriptionEnum
        {
            [System.ComponentModel.Description("The First One")]
            First,
            Second
        }
        
        /// <summary>
        ///A test for GetDescription
        ///</summary>
        [TestMethod()]
        public void GetDescriptionTest()
        {
            Assert.AreEqual(EnumExtensions.GetDescription(TestDescriptionEnum.First), "The First One");
        }

        /// <summary>
        ///A test for GetEnumStrings
        ///</summary>
        public void GetEnumStringsTestHelper<T>()
        {
            string[] expected = new string[] { "First", "Second", "Third" };
            string[] actual;
            actual = EnumExtensions.GetEnumStrings<TestEnum>();
            Assert.IsTrue(expected.Length == actual.Length);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(expected[i] == actual[i]);
            }
        }


        [TestMethod()]
        public void GetEnumStringsTest()
        {
            GetEnumStringsTestHelper<GenericParameterHelper>();
        }

        [TestMethod()]
        public void GetEnumStringsTestExcepion()
        {
            string[] expected = new string[] { "First", "Second", "Third" };
            string[] actual;
            bool exceptionThrown = false;
            try
            {
                actual = EnumExtensions.GetEnumStrings<int>();
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown == true);
        }
        
        private enum TestEnum { First, Second, Third };
        /// <summary>
        ///A test for GetEnumValues
        ///</summary>
        public void GetEnumValuesTestHelper<T>()
        {
            TestEnum[] expected = new TestEnum[]{TestEnum.First, TestEnum.Second, TestEnum.Third}; // TODO: Initialize to an appropriate value
            TestEnum[] actual;
            actual = EnumExtensions.GetEnumValues<TestEnum>();

            Assert.IsTrue(expected.Length == actual.Length);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(expected[i] == actual[i]);
            }
        }

        [TestMethod()]
        public void GetEnumValuesTest()
        {
            GetEnumValuesTestHelper<GenericParameterHelper>();
        }

        [TestMethod()]
        public void GetEnumValuesTestException()
        {
            TestEnum[] expected = new TestEnum[] { TestEnum.First, TestEnum.Second, TestEnum.Third }; // TODO: Initialize to an appropriate value
            
            bool exceptionThrown = false;
            try
            {
                EnumExtensions.GetEnumValues<int>();
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown == true);
        }

        /// <summary>
        ///A test for GetValueFromXmlEnumName
        ///</summary>
        private enum TestXmlEnum 
        {
            NA,
            [XmlEnum("1st")]
            First,
            Second,
            [XmlEnum("3rd")]
            Third 
        };

        [TestMethod()]
        public void MathExtensionsMaxTest()
        {
           int maxResult = MathExtensions.Min<int>(4, 5);
           Assert.IsTrue(maxResult == 4);
        }

        [TestMethod()]
        public void MathExtensionsMinTest()
        {
            int maxResult = MathExtensions.Max<int>(4, 5);
            Assert.IsTrue(maxResult == 5);
        }

        [TestMethod()]
        public void MathExtensionsMaxRealTest()
        {
            double val1 = 4.55363;
            double val2 = 4.2212;
            double maxResult = MathExtensions.Max<double>(val1, val2);
            Assert.IsTrue(maxResult == val1);
        }

        [TestMethod()]
        public void MathExtensionsMinRealTest()
        {
            double val1 = 4.55363;
            double val2 = 4.2212;
            double minResult = MathExtensions.Min<double>(val1, val2);
            Assert.IsTrue(minResult == val2);
        }

        [TestMethod]
        public void XmlEnumHelperTest()
        {
            Assert.IsNull(XmlEnumHelper.ToString(null));
            Assert.IsNull(XmlEnumHelper.ToString(TestXmlEnum.NA));
            Assert.AreEqual("1st", XmlEnumHelper.ToString(TestXmlEnum.First));
            Assert.AreEqual("Second", XmlEnumHelper.ToString(TestXmlEnum.Second));

            ExceptionAssert.Throws<Exception>(() => Assert.IsNull(XmlEnumHelper.FromString<TestXmlEnum>(null)));
            Assert.AreEqual(TestXmlEnum.First, XmlEnumHelper.FromString<TestXmlEnum>("1st"));
            Assert.AreEqual(TestXmlEnum.Second, XmlEnumHelper.FromString<TestXmlEnum>("Second"));
        }
    }
}

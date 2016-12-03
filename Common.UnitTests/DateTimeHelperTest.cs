using SEL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SEL.Common.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for DateTimeHelperTest and is intended
    ///to contain all DateTimeHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DateTimeHelperTest
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


        /// <summary>
        ///A test for Divide
        ///</summary>
        [TestMethod()]
        public void DivideTest()
        {
            TimeSpan span = new TimeSpan(100);
            int divisor = 20; 
            TimeSpan expected = new TimeSpan(5); 
            TimeSpan actual;
            actual = DateTimeHelper.Divide(span, divisor);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Max
        ///</summary>
        [TestMethod()]
        public void MaxTest1()
        {
            DateTime val1 = new DateTime(100);
            DateTime val2 = new DateTime(101); 
            DateTime expected = new DateTime(101);
            DateTime actual;
            actual = DateTimeHelper.Max(val1, val2);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Max
        ///</summary>
        [TestMethod()]
        public void MaxTest2()
        {
            DateTime val1 = new DateTime(101);
            DateTime val2 = new DateTime(100);
            DateTime expected = new DateTime(101);
            DateTime actual;
            actual = DateTimeHelper.Max(val1, val2);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Min
        ///</summary>
        [TestMethod()]
        public void MinTest1()
        {
            DateTime val1 = new DateTime(204); 
            DateTime val2 = new DateTime(205); 
            DateTime expected = new DateTime(204); 
            DateTime actual;
            actual = DateTimeHelper.Min(val1, val2);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Min
        ///</summary>
        [TestMethod()]
        public void MinTest2()
        {
            DateTime val1 = new DateTime(205);
            DateTime val2 = new DateTime(204);
            DateTime expected = new DateTime(204);
            DateTime actual;
            actual = DateTimeHelper.Min(val1, val2);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Multiply
        ///</summary>
        [TestMethod()]
        public void MultiplyTest()
        {
            TimeSpan span = new TimeSpan(403); 
            int multiplier = 201; 
            TimeSpan expected = new TimeSpan(81003);
            TimeSpan actual;
            actual = DateTimeHelper.Multiply(span, multiplier);
        }
    }
}

using SEL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;
using SEL.UnitTest;

namespace SEL.Common.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for HashBuilderTest and is intended
    ///to contain all HashBuilderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HashBuilderTest
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
        ///A test for HashBuilder Constructor
        ///</summary>
        [TestMethod()]
        public void HashBuilderConstructorTest()
        {
            int startHash = 0;
            HashBuilder target = new HashBuilder(startHash);
        }

        /// <summary>
        ///A test for HashBuilder Constructor
        ///</summary>
        [TestMethod()]
        public void HashBuilderConstructorTest1()
        {
            HashBuilder target = new HashBuilder();
        }

        [TestMethod()]
        public void AddTest1()
        {
            HashBuilder target = new HashBuilder();

            target.Add<int>(2);
            target.Add<int>(1);
            target.Add<int>(4);
            target.Add<int>(3);

            int result1 = target.Result;
            HashBuilder target2 = new HashBuilder();

            target2.Add<int>(2);
            target2.Add<int>(1);
            target2.Add<int>(4);
            target2.Add<int>(3);

            Assert.IsTrue(target.Result == target2.Result);
        }

        [TestMethod()]
        public void AddItemsTest1()
        {
            HashBuilder target = new HashBuilder();

            int[] arr = new int[] { 1, 2, 3, 4, 5 };

            target.AddItems((IEnumerable<int>) arr);

            int result1 = target.Result;           
            HashBuilder target2 = new HashBuilder();
            target2.AddItems((IEnumerable<int>)arr);

            Assert.IsTrue(target.Result == target2.Result);
        }

        [TestMethod()]
        public void AddItemsTest2()
        {
            HashBuilder target = new HashBuilder();

            target.AddItems(1, 2, 3, 4, 5);
            int result1 = target.Result;
            HashBuilder target2 = new HashBuilder();
            target2.AddItems(1, 2, 3, 4, 5);

            Assert.IsTrue(target.Result == target2.Result);
        }

        /// <summary>
        ///A test for GetEnumerator
        ///</summary>
        [TestMethod()]
        public void GetEnumeratorTest()
        {
            HashBuilder target = new HashBuilder(); 
            ExceptionAssert.Throws<System.NotImplementedException>(() => { target.GetEnumerator(); });       
        }

    }
}

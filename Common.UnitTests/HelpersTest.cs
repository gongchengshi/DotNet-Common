using System.Collections.Generic;
using System.Runtime.Serialization;
using SEL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SEL.UnitTest;

namespace SEL.Common.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for HelpersTest and is intended
    ///to contain all HelpersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HelpersTest
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

        [TestMethod()]
        public void SwapTest()
        {
            int a = 3;
            int b = 4;

            Helpers.Swap<int>(ref a, ref b);

            Assert.IsTrue(a == 4);
            Assert.IsTrue(b == 3);
        }

        [TestMethod]
        public void IfIsATest()
        {
            Base s1 = new Sub1();
            Base s2 = new Sub2();
            Base s3 = new Sub3();
            
            s1.IfIsA((Sub1 s) => { s.Value = 2; });
            Assert.AreEqual(2, ((Sub1)s1).Value);
            s2.IfIsA((Sub1 s) => { s.Value = 3; });
            Assert.AreEqual(42, ((Sub2)s2).Value);

            Assert.AreEqual(2, s1.IfIsA((Sub1 s) => s.Value));
            Assert.AreEqual(0, s1.IfIsA((Sub2 s) => s.Value));

            Assert.AreEqual(2, s1.IfIsA((Sub1 s) => s.Value,
                     (Sub2 s) => s.Value));
            Assert.AreEqual(42, s2.IfIsA((Sub1 s) => s.Value,
                     (Sub2 s) => s.Value));
            Assert.AreEqual(0, s3.IfIsA((Sub1 s) => s.Value,
                     (Sub2 s) => s.Value));
        }

        class Base
        {
        }
        class Sub1 : Base
        {
            public int Value;
        }
        class Sub2 : Base
        {
            public int Value = 42;
        }
        class Sub3 : Base {}


        [TestMethod]
        public void WhileCatchTest()
        {
            int count = 0;
            Action thrower = () => { if (count < 4) throw new Exception(); };
            thrower.WhileCatch<Exception>(() => count++, 5);
            Assert.AreEqual(4, count);
            count = 0;
            ExceptionAssert.Throws<Exception>(
                () => thrower.WhileCatch<Exception>(() => count++, 3));
        }

        [TestMethod]
        public void DeepCopyTest()
        {
            var deep = new ADeepClass();
            deep.DeepList.Add(new List<string>(new[] { "A" }));
            deep.DeepList.Add(new List<string>(new[] { "B", "C" }));

            var copy = deep.DeepCopy();
            CollectionAssert.AreEqual(new[] { "A" }, copy.DeepList[0]);
            CollectionAssert.AreEqual(new[] { "B", "C" }, copy.DeepList[1]);
        }

        [TestMethod]
        public void DeepCompareTest()
        {
            var deep = new ADeepClass();
            deep.DeepList.Add(new List<string>(new[] { "A" }));
            deep.DeepList.Add(new List<string>(new[] { "B", "C" }));

            var copy = new ADeepClass();
            copy.DeepList.Add(new List<string>(new[] { "A" }));
            copy.DeepList.Add(new List<string>(new[] { "B", "D" }));

            Assert.AreEqual(true, deep.DeepList[0].DeepCompare(copy.DeepList[0]));
            Assert.AreEqual(false, deep.DeepList[1].DeepCompare(copy.DeepList[1]));



        }



        [DataContract]
        class ADeepClass
        {
            [DataMember]
            public List<List<string>> DeepList = new List<List<string>>();
        }
    }
}

///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using SEL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using SEL.UnitTest;

namespace SEL.Common.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for ValueTypeExtensionsTest and is intended
    ///to contain all ValueTypeExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ValueTypeExtensionsTest
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
        ///A test for AddMicroseconds
        ///</summary>
        [TestMethod()]
        public void AddMicrosecondsTest()
        {
            var dateTime = new DateTime(2011, 1, 1);
            Assert.AreEqual(dateTime.AddTicks(10), dateTime.AddMicroseconds(1));
            Assert.AreEqual(dateTime.AddMilliseconds(1), dateTime.AddMicroseconds(1000));
            Assert.AreEqual(dateTime.AddTicks(9999990), dateTime.AddMicroseconds(999999));
            Assert.AreEqual(dateTime.AddTicks(10000000), dateTime.AddMicroseconds(1000000));
            Assert.AreEqual(dateTime.AddTicks(-10), dateTime.AddMicroseconds(-1));
            Assert.AreEqual(dateTime.AddTicks(-9999990), dateTime.AddMicroseconds(-999999));
        }

        /// <summary>
        ///A test for IsBitSetAt
        ///</summary>
        [TestMethod()]
        public void IsBitSetAtTestTrue()
        {
            ushort num = 0x0100; 
            int bitpos = 8; 
            bool expected = true; 
            bool actual;
            actual = ValueTypeExtensions.IsBitSetAt(num, bitpos);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsBitSetAt
        ///</summary>
        [TestMethod()]
        public void IsBitSetAtTestFalse()
        {
            ushort num = 0xFEFF;
            int bitpos = 8;
            bool expected = false;
            bool actual;
            actual = ValueTypeExtensions.IsBitSetAt(num, bitpos);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsBitSetAt
        ///</summary>
        [TestMethod()]
        public void IsBitSetAtTest1True()
        {
            int num = 0x00000100; // TODO: Initialize to an appropriate value
            int bitpos = 8; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = ValueTypeExtensions.IsBitSetAt(num, bitpos);
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for IsBitSetAt
        ///</summary>
        [TestMethod()]
        public void IsBitSetAtTest1False()
        {
            int num = 0x00000010; 
            int bitpos = 4; 
            bool expected = false; 
            bool actual;
            actual = ValueTypeExtensions.IsBitSetAt(num, bitpos);
            Assert.AreNotEqual(expected, actual);

        }

        /// <summary>
        ///A test for IsBitSetAt
        ///</summary>
        [TestMethod()]
        public void IsBitSetAtTest2True()
        {
            short num = 0x0100; 
            int bitpos = 8;
            bool expected = true; 
            bool actual;
            actual = ValueTypeExtensions.IsBitSetAt(num, bitpos);
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for IsBitSetAt
        ///</summary>
        [TestMethod()]
        public void IsBitSetAtTest2False()
        {
            short num = 0x0100;
            int bitpos = 7;
            bool expected = false;
            bool actual;
            actual = ValueTypeExtensions.IsBitSetAt(num, bitpos);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsBitSetAt
        ///</summary>
        [TestMethod()]
        public void IsBitSetAtTest3True()
        {
            uint num = 0x01000000;
            int bitpos = 24; 
            bool expected = true; 
            bool actual;
            actual = ValueTypeExtensions.IsBitSetAt(num, bitpos);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsBitSetAt
        ///</summary>
        [TestMethod()]
        public void IsBitSetAtTest3False()
        {
            uint num = 0xFEFFFFFF; 
            int bitpos = 24; 
            bool expected = false; 
            bool actual;
            actual = ValueTypeExtensions.IsBitSetAt(num, bitpos);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Matches
        ///</summary>
        [TestMethod()]
        public void MatchesTestTrue1()
        {
            byte[] left  = new byte[] { 4, 6, 8, 2, 3, 4, 5 };
            byte[] right = new byte[] { 4, 6, 8, 2, 3, 4, 5 }; 
            bool expected = true; 
            bool actual;
            actual = ValueTypeExtensions.Matches(left, right);
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for Matches
        ///</summary>
        [TestMethod()]
        public void MatchesTestTrue2()
        {
            byte[] left = new byte[] { 4, 6, 8, 2, 3, 4, 5 };
            byte[] right = left;
            bool expected = true;
            bool actual;
            actual = ValueTypeExtensions.Matches(left, right);
            Assert.AreEqual(expected, actual);

        }
        /// <summary>
        ///A test for Matches
        ///</summary>
        [TestMethod()]
        public void MatchesTestFalse1()
        {
            byte[] left = new byte[] { 4, 6, 8, 2, 3, 4};
            byte[] right = new byte[] { 4, 6, 8, 2, 3, 4, 5 };
            bool expected = false;
            bool actual;
            actual = ValueTypeExtensions.Matches(left, right);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for Matches
        ///</summary>
        [TestMethod()]
        public void MatchesTestFalse2()
        {
            byte[] left = new byte[] { 4, 6, 8, 2, 3, 4 };
            byte[] right = null;
            bool expected = false;
            bool actual;
            actual = ValueTypeExtensions.Matches(left, right);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for Matches
        ///</summary>
        [TestMethod()]
        public void MatchesTestFalse3()
        {
            byte[] left = new byte[] { 4, 6, 8, 2, 3, 4, 9 };
            byte[] right = new byte[] { 4, 6, 8, 2, 3, 4, 5 }; ;
            bool expected = false;
            bool actual;

            actual = ValueTypeExtensions.Matches(left, right);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for Microseconds
        ///</summary>
        [TestMethod()]
        public void MicrosecondsTest()
        {
            DateTime dateTime = new DateTime(1234567); 
            Assert.AreEqual(123456, dateTime.Microseconds());
        }

        /// <summary>
        ///A test for ReverseBytes
        ///</summary>
        [TestMethod()]
        public void ReverseBytesTest()
        {
            byte[] bytes = new byte[] { 4, 6, 3, 7, 8, 3, 7, 8, 3 };
            byte[] original = new byte[bytes.Length];

            Array.Copy(bytes, original, bytes.Length);

            int offset = 0;
            int count = bytes.Length; 
            ValueTypeExtensions.ReverseBytes(bytes, offset, count);

            // Compare bytes in order to popping original bytes off stack
            Stack<byte> stack = new Stack<byte>();
            // Push original bytes on stack
            foreach (byte b in original)
            {
                stack.Push(b);
            }

            // Compare            
            for (int i = 0; i < original.Length; i++)
            {
                byte popped = stack.Pop();
                Assert.IsTrue(popped == bytes[i]);
            }

        }

        /// <summary>
        ///A test for ReverseBytes. Reverse a subset of the bytes in the original array.
        ///</summary>
        [TestMethod()]
        public void ReverseBytesTest2()
        {
            byte[] bytes =    new byte[] { 4, 6, 3, 7, 8, 3, 7, 8, 3 };
            byte[] expected = new byte[] { 4, 6, 3, 8, 7, 3, 7, 8, 3 };

            int offset = 2;
            int count = 4;
            ValueTypeExtensions.ReverseBytes(bytes, offset, count);

            SELAssert.AreCollectionsEqual(expected, bytes);          
        }

        /// <summary>
        ///A test for TotalMicroseconds
        ///</summary>
        [TestMethod()]
        public void TotalMicrosecondsTest()
        {
            TimeSpan timeSpan = new TimeSpan(876543210987654321);
            Assert.AreEqual(87654321098765432, timeSpan.TotalMicroseconds());

        }
    }
}

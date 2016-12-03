// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEL.Collections.Generic;

namespace SEL.Common.UnitTests
{
    /// <summary>
    ///This is a test class for IntervalTest and is intended
    ///to contain all IntervalTest Unit Tests
    ///</summary>
    [TestClass]
    public class IntervalTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

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
        ///A test for AreWithinSameInterval
        ///</summary>
        [TestMethod]
        public void AreWithinSameIntervalTest()
        {
            var referenceDT = new DateTime(2011, 1, 1, 1, 0, 0);
            var testDTs =
                new TupleList<DateTime, bool[]>
                    {
                        // DateTime To Test                         Interval:    Day   Hour  Min   Sec         
                        {new DateTime(2011, 1, 1, 1, 0, 0, 999), new[] {true, true, true, true}},
                        {new DateTime(2011, 1, 1, 1, 0, 1), new[] {true, true, true, false}},
                        {new DateTime(2011, 1, 1, 1, 0, 59), new[] {true, true, true, false}},
                        {new DateTime(2011, 1, 1, 1, 1, 0), new[] {true, true, false, false}},
                        {new DateTime(2011, 1, 1, 1, 59, 59), new[] {true, true, false, false}},
                        {new DateTime(2011, 1, 1, 2, 0, 0), new[] {true, false, false, false}},
                        {new DateTime(2011, 1, 1, 23, 59, 59), new[] {true, false, false, false}},
                        {new DateTime(2011, 1, 2, 0, 0, 0), new[] {false, false, false, false}},
                    };

            foreach (var test in testDTs)
            {
                int boolIndex = 0;
                foreach (var interval in new[] {Interval.Day, Interval.Hour, Interval.Minute, Interval.Second})
                {
                    Assert.AreEqual(test.Item2[boolIndex++], interval.AreWithinSameInterval(referenceDT, test.Item1));
                }
            }
        }


        private readonly TupleList<Interval, DateTime, bool, DateTime> TestCases =
            new TupleList<Interval, DateTime, bool, DateTime>
                {
                    {Interval.Day, new DateTime(2011, 1, 1), true, new DateTime(2011, 1, 1)},
                    {Interval.Day, new DateTime(2011, 1, 1, 0, 0, 0, 1), false, new DateTime(2011, 1, 1)},
                    {Interval.Day, new DateTime(2011, 1, 1, 12, 0, 0), false, new DateTime(2011, 1, 1)},
                    {Interval.Day, new DateTime(2011, 1, 1, 23, 59, 59, 999), false, new DateTime(2011, 1, 1)},
                    {Interval.Hour, new DateTime(2011, 1, 1, 5, 0, 0), true, new DateTime(2011, 1, 1, 5, 0, 0)},
                    {Interval.Hour, new DateTime(2011, 1, 1, 5, 0, 0, 1), false, new DateTime(2011, 1, 1, 5, 0, 0)},
                    {Interval.Hour, new DateTime(2011, 1, 1, 5, 30, 0), false, new DateTime(2011, 1, 1, 5, 0, 0)},
                    {Interval.Hour, new DateTime(2011, 1, 1, 5, 59, 59, 999), false, new DateTime(2011, 1, 1, 5, 0, 0)},
                    {Interval.Minute, new DateTime(2011, 1, 1, 5, 15, 0), true, new DateTime(2011, 1, 1, 5, 15, 0)},
                    {Interval.Minute, new DateTime(2011, 1, 1, 5, 15, 0, 1), false, new DateTime(2011, 1, 1, 5, 15, 0)},
                    {Interval.Minute, new DateTime(2011, 1, 1, 5, 15, 45, 0), false, new DateTime(2011, 1, 1, 5, 15, 0)},
                    {
                        Interval.Minute, new DateTime(2011, 1, 1, 5, 15, 59, 999), false,
                        new DateTime(2011, 1, 1, 5, 15, 0)
                        },
                    {Interval.Second, new DateTime(2011, 1, 1, 5, 15, 5), true, new DateTime(2011, 1, 1, 5, 15, 5)},
                    {Interval.Second, new DateTime(2011, 1, 1, 5, 15, 5, 1), false, new DateTime(2011, 1, 1, 5, 15, 5)},
                    {
                        Interval.Second, new DateTime(2011, 1, 1, 5, 15, 5, 500), false, new DateTime(2011, 1, 1, 5, 15, 5)
                        },
                    {
                        Interval.Second, new DateTime(2011, 1, 1, 5, 15, 5, 999), false, new DateTime(2011, 1, 1, 5, 15, 5)
                        }
                };

        /// <summary>
        ///A test for IsAtTop
        ///</summary>
        [TestMethod]
        public void IsAtTopTest()
        {
            foreach (var testCase in TestCases)
            {
                Assert.AreEqual(testCase.Item3, testCase.Item1.IsAtTop(testCase.Item2));
            }
        }

        /// <summary>
        ///A test for Truncate
        ///</summary>
        [TestMethod]
        public void TruncateTest()
        {
            foreach (var testCase in TestCases)
            {
                Assert.AreEqual(testCase.Item4, testCase.Item1.Truncate(testCase.Item2));
            }
        }

        [TestMethod]
        public void IntervalPrivateConstructorNullTest()
        {
            bool exceptionThrown = false;
            try
            {
                Interval_Accessor inverval = new Interval_Accessor(new TimeSpan(1, 0, 1));
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void TopOfCurrentOrNextTest()
        {
            var target = new Interval(TimeSpan.FromSeconds(5));

            var input = new DateTime(2012, 1, 1, 5, 0, 56);
            var expected = new DateTime(2012, 1, 1, 5, 1, 0);
            var actual = target.TopOfCurrentOrNext(input);
            Assert.AreEqual(expected, actual);

            input = expected;
            actual = target.TopOfCurrentOrNext(input);
            Assert.AreEqual(expected, actual);
        }
    }
}

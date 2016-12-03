///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using SEL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace SEL.Common.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for RepeatedEventLogEntryFilterTest and is intended
    ///to contain all RepeatedEventLogEntryFilterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RepeatedEventLogEntryFilterTest
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


        // Unit tested manually.
        ///// <summary>
        /////A test for WriteEntry
        /////</summary>
        //[TestMethod()]
        //public void WriteEntryTest()
        //{
        //    EventLog eventLog = new EventLog();
        //    RepeatedEventLogEntryFilter target = new RepeatedEventLogEntryFilter(eventLog);
        //    string message = "Hello world"; 
        //    EventLogEntryType severity = new EventLogEntryType();
        //    severity = EventLogEntryType.FailureAudit;

        //    target.WriteEntry(message, severity);
        //}

        // This function is tested manually.
        ///// <summary>
        /////A test for ManageRepeatedEntries
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("Common.dll")]
        //public void ManageRepeatedEntriesTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    RepeatedEventLogEntryFilter_Accessor target = new RepeatedEventLogEntryFilter_Accessor(param0); // TODO: Initialize to an appropriate value
        //    target.ManageRepeatedEntries();
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for RepeatedEventLogEntryFilter Constructor
        ///</summary>
        [TestMethod()]
        public void RepeatedEventLogEntryFilterConstructorTest()
        {
            EventLog eventLog = null; 
            RepeatedEventLogEntryFilter target = new RepeatedEventLogEntryFilter(eventLog);
        }
    }
}

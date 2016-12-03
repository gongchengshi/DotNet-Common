///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using SEL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SEL.Collections.Generic;

namespace SEL.Common.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for PhasorTest and is intended
    ///to contain all PhasorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PhasorTest
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
        ///A test for Phasor Constructor
        ///</summary>
        [TestMethod()]
        public void PhasorConstructorTest()
        {
            float real = 0F; 
            float imaginary = 0F; 
            Phasor target = new Phasor(real, imaginary);
            
        }

        TupleList<float, float, float, float> MagAngleTestCases = new TupleList<float, float, float, float> 
            {//     real    imag        mag                         ang     
                {   50,     0,          50,                         0 },
                {   50,     50,         (float)(50 * Math.Sqrt(2)), 45 },
                {   0,      50,         50,                         90 },
                {   -50,    0,          50,                         180 },
                {   -50,    -0.0000001f,50,                         -180 },
                {   -50,    -50,        (float)(50 * Math.Sqrt(2)), -135 },
            };
        /// <summary>
        ///A test for AngleFromRect
        ///</summary>
        [TestMethod()]
        public void AngleFromRectTest()
        {
            foreach (var testCase in MagAngleTestCases)
            {
                Assert.AreEqual(testCase.Item4, Phasor.AngleDegreesFromRect(testCase.Item1, testCase.Item2));
            }
        }
        
        /// <summary>
        ///A test for FromPolar
        ///</summary>
        [TestMethod()]
        public void FromPolarTest()
        {
            var actual = Phasor.FromPolar(1, (float)(Math.PI / 4));
            Assert.AreEqual(1.0f, actual.Magnitude);
            Assert.AreEqual(45.0f, actual.AngleDegrees);
        }

        /// <summary>
        ///A test for ImaginaryFromPolar
        ///</summary>
        [TestMethod()]
        public void ImaginaryFromPolarTest()
        {
            foreach (var testCase in MagAngleTestCases)
            {
                Assert.AreEqual(testCase.Item2, Math.Round(Phasor.ImaginaryFromPolar(testCase.Item3, 
                    (float)(testCase.Item4 * Math.PI / 180))), 0.0000001f);
            }
        }

        /// <summary>
        ///A test for MagnitudeFromRect
        ///</summary>
        [TestMethod()]
        public void MagnitudeFromRectTest()
        {
            foreach (var testCase in MagAngleTestCases)
            {
                Assert.AreEqual(testCase.Item3, Phasor.MagnitudeFromRect(testCase.Item1, testCase.Item2));
            }
        }

        /// <summary>
        ///A test for RealFromPolar
        ///</summary>
        [TestMethod()]
        public void RealFromPolarTest()
        {
            foreach (var testCase in MagAngleTestCases)
            {
                Assert.AreEqual(testCase.Item1, (float)Math.Round(Phasor.RealFromPolar(testCase.Item3,
                    (float)(testCase.Item4 * Math.PI / 180))), 0.0000001f);
            }
        }

        /// <summary>
        ///A test for Angle
        ///</summary>
        [TestMethod()]
        public void AngleTest()
        {
            Phasor target = new Phasor(1, 1);
            Assert.AreEqual(45.0f, target.AngleDegrees);
        }

        /// <summary>
        ///A test for Magnitude
        ///</summary>
        [TestMethod()]
        public void MagnitudeTest()
        {
            Phasor target = new Phasor(1, 1);
            Assert.AreEqual((float)Math.Sqrt(2.0f), target.Magnitude);
        }
    }
}

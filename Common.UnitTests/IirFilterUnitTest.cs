// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEL.Collections.Generic;
using SEL.UnitTest;

namespace SEL.Common.UnitTests
{
    [TestClass]
    public class InfinateImpulseFilterUnitTest
    {
        // taken from matlab code
        private static readonly double[] _a = new[]
                                                  {
                                                      1,
                                                      -3.316807910624417,
                                                      4.174245550076570,
                                                      -2.357402780562255,
                                                      0.503375360741703
                                                  };

        // taken from matlab code
        private static readonly double[] _b = new[]
                                                  {
                                                      0.000213138726975,
                                                      0.000852554907900,
                                                      0.001278832361851,
                                                      0.000852554907900,
                                                      0.000213138726975
                                                  };

        public static readonly double[] A60 = new[]
                                                  {
                                                      1.000000000000000,
                                                      -3.280818373873355,
                                                      4.090993977964895,
                                                      -2.291166229510521,
                                                      0.485588094661278
                                                  };

        public static readonly double[] B60 = new[]
                                                  {
                                                      0.011257351372128,
                                                      -0.026722049379451,
                                                      0.035526865256943,
                                                      -0.026722049379451,
                                                      0.011257351372128
                                                  };

        private void GetInputAndExpected(out List<double> input, out List<double> expectedOutput)
        {
            input = new List<double>();
            expectedOutput = new List<double>();

            using (var reader = new StreamReader("Input\\60Hz.txt"))
            {
                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    var split = line.Split();
                    input.Add(double.Parse(split[0]));
                    expectedOutput.Add(double.Parse(split[1]));
                }
            }
        }

        [TestMethod]
        public void Test60HzBlockFilterTaps()
        {
            List<double> input;
            List<double> expectedOutput;

            GetInputAndExpected(out input, out expectedOutput);

            var filter = new BlockIirFilter(A60, B60);

            var segmentLength = input.Count / 100;

            var actualOutput = new List<double>(input.Count);

            var rangeStart = 0;

            for (int i = 0; i < 100 - 1; ++i)
            {
                actualOutput.AddRange(filter.Filter(input.GetRange(rangeStart, segmentLength).ToArray()));
                rangeStart += segmentLength;
            }

            actualOutput.AddRange(filter.Filter(input.GetRange(rangeStart, input.Count - rangeStart).ToArray()));

            var equal = expectedOutput.GetRange(1, expectedOutput.Count - 1).ContentsEqual(actualOutput.GetRange(1, actualOutput.Count - 1), .000001);

            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void Test60HzFilterTaps()
        {
            List<double> input;
            List<double> expectedOutput;

            GetInputAndExpected(out input, out expectedOutput);

            var filter = new IirFilter(A60, B60);


            var actualOutput = filter.Filter(input.ToArray());

            var equal = expectedOutput.ContentsEqual(actualOutput, .000001);

            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestIirInitialResponse()
        {
            var filter = new IirFilter(_a, _b);

            var actual = filter.Filter(GetInput1().ToArray());
            // This is the initial response
            var expected = GetOutput1();

            var equal = expected.ContentsEqual(actual, .0001);

            if (!equal)
            {
                expected.Print();
                Console.WriteLine();
                actual.Print();
            }

            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestIirAllZeros()
        {
            var input = Enumerable.Repeat(0.0, 300);

            var filter = new IirFilter(_a, _b);

            var actual = filter.Filter(input.ToArray());
            var expected = input.ToArray();

            var equal = expected.ContentsEqual(actual, .0001);

            if (!equal)
            {
                expected.Print();
                Console.WriteLine();
                actual.Print();
            }

            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void BasicTest()
        {
            var array2 = new[] {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0};

            var target = new BlockIirFilter(_a, _b);



            target.Filter(array2);
        }

        [TestMethod]
        public void TestIir()
        {
            var filter = new IirFilter(_a, _b);

            var actual = filter.Filter(GetInput2().ToArray());
            var expected = GetOutput2();

            var equal = expected.ContentsEqual(actual, .0001);

            if (!equal)
            {
                expected.Print();
                Console.WriteLine();
                actual.Print();
            }

            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestBlockIirAllZeros()
        {
            var input = Enumerable.Repeat(0.0, 300);

            var filter = new BlockIirFilter(_a, _b);

            var actual = filter.Filter(input.ToArray());
            var expected = input.ToArray();

            var equal = expected.ContentsEqual(actual, .0001);

            if (!equal)
            {
                expected.Print();
                Console.WriteLine();
                actual.Print();
            }

            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestBlockIir()
        {
            var input = GetInput3();

            var filter = new BlockIirFilter(_a, _b);

            var actual = filter.Filter(input.ToArray()).Skip(58);
            var expected = GetOutput3().Skip(58);

            var equal = expected.ContentsEqual(actual, .0001);

            if (!equal)
            {
                expected.Print();
                Console.WriteLine();
                actual.Print();
            }

            Assert.IsTrue(equal);
        }

        public IEnumerable<double> GetInput1()
        {
            var input = new List<double> { 1.0 };
            input.AddRange(Enumerable.Repeat(0.0, 99));
            return input;
        }

        public IEnumerable<double> GetInput2()
        {
            return ReadFile("Input\\ButterworthIn2.txt");
        }

        public IEnumerable<double> GetInput3()
        {
            return ReadFile("Input\\ButterworthIn3.txt");
        }

        public IEnumerable<double> GetOutput1()
        {
            return ReadFile("Input\\ButterworthOut1.txt");
        }

        public IEnumerable<double> GetOutput2()
        {
            return ReadFile("Input\\ButterworthOut2.txt");
        }

        public IEnumerable<double> GetOutput3()
        {
            return ReadFile("Input\\ButterworthOut3.txt");
        }
        
        public IEnumerable<double> ReadFile(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    yield return double.Parse(line);
                }
            }
        }
    }
}
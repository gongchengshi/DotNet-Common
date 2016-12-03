using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEL.Common.UnitTests
{
   [TestClass]
   public class CombinatoricsTest
   {
      public CombinatoricsTest()
      {
         //
         // TODO: Add constructor logic here
         //
      }

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
      // You can use the following additional attributes as you write your tests:
      //
      // Use ClassInitialize to run code before running the first test in the class
      // [ClassInitialize()]
      // public static void MyClassInitialize(TestContext testContext) { }
      //
      // Use ClassCleanup to run code after all tests in a class have run
      // [ClassCleanup()]
      // public static void MyClassCleanup() { }
      //
      // Use TestInitialize to run code before running each test 
      // [TestInitialize()]
      // public void MyTestInitialize() { }
      //
      // Use TestCleanup to run code after each test has run
      // [TestCleanup()]
      // public void MyTestCleanup() { }
      //
      #endregion

      private static BitArray GetBitArray(IEnumerable<int> c, int n)
      {
         var a = new BitArray(n);
         foreach(var index in c)
         {
            a[index] = true;
         }

         return a;
      }

      [TestMethod]
      public void CombinationsTest()
      {
         var tests = new[] { Tuple.Create(4, 0), Tuple.Create(4, 1), Tuple.Create(4, 2), Tuple.Create(4, 3), Tuple.Create(4, 4) };

         foreach (var t in tests)
         {
            foreach (var combination in Combinatorics.Combinations(t.Item1, t.Item2))
            {
               foreach (var index in combination)
               {
                  Console.Write(index + " ");
               }

               Console.WriteLine(GetBitArray(combination, t.Item1).ToBinaryString());
            }
            Console.WriteLine();
         }
      }
   }
}

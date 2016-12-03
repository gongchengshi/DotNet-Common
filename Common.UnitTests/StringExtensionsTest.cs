using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEL.Common.UnitTests
{
   [TestClass]
   public class StringExtensionsTest
   {
      [TestMethod]
      public void FirstFieldsTest()
      {
         Assert.AreEqual("", "".FirstFields(0, '/'));
         Assert.AreEqual("", "/".FirstFields(0, '/'));
         Assert.AreEqual("", "1".FirstFields(0, '/'));
         Assert.AreEqual("", "1/2".FirstFields(0, '/'));
         Assert.AreEqual("", "/1/2".FirstFields(0, '/'));

         Assert.AreEqual("1", "1/2/".FirstFields(1, '/'));
         Assert.AreEqual("", "/1/2/".FirstFields(1, '/'));

         Assert.AreEqual("1/2", "1/2".FirstFields(2, '/'));
         Assert.AreEqual("/1", "/1/2".FirstFields(2, '/'));
         Assert.AreEqual("1/", "1/".FirstFields(2, '/'));
         Assert.AreEqual("/", "//".FirstFields(2, '/'));
         Assert.AreEqual("1/", "1//".FirstFields(2, '/'));
         Assert.AreEqual("1/2", "1/2/".FirstFields(2, '/'));
         Assert.AreEqual("1/2", "1/2/3".FirstFields(2, '/'));
      }

      [TestMethod]
      public void PythonSubstringTest()
      {
         const string prefix = "http://mimeo/job1/";
         var input = prefix + "#frag";
         var actual = input.PythonSubstring(prefix.Length, input.IndexOf('#'));
         Assert.AreEqual("", actual);

         input = prefix + "site1/#frag";
         actual = input.PythonSubstring(prefix.Length, input.IndexOf('#'));
         Assert.AreEqual("site1/", actual);

         input = "#frag";
         actual = input.PythonSubstring(0, input.IndexOf('#'));
         Assert.AreEqual("", actual);

         input = "0123456789";
         actual = input.PythonSubstring(0, 0);
         Assert.AreEqual("", actual);

         actual = input.PythonSubstring(0, 1);
         Assert.AreEqual("0", actual);

         actual = input.PythonSubstring(0, 10);
         Assert.AreEqual(input, actual);

         actual = input.PythonSubstring(9, 9);
         Assert.AreEqual("", actual);

         actual = input.PythonSubstring(8, 9);
         Assert.AreEqual("8", actual);

         actual = input.PythonSubstring(9, 10);
         Assert.AreEqual("9", actual);

         actual = input.PythonSubstring(0, -1);
         Assert.AreEqual("012345678", actual);

         actual = input.PythonSubstring(-2, -1);
         Assert.AreEqual("8", actual);

         actual = input.PythonSubstring(-10, -9);
         Assert.AreEqual("0", actual);

         actual = input.PythonSubstring(-10, -7);
         Assert.AreEqual("012", actual);
      }

      [ExpectedException(typeof (ArgumentOutOfRangeException))]
      public void PythonSubstringException1()
      {
         "0123456789".PythonSubstring(10, 11);
      }

      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void PythonSubstringException2()
      {
         "0123456789".PythonSubstring(-2, -8);
      }

      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void PythonSubstringException3()
      {
         "0123456789".PythonSubstring(11, 9);
      }

      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void PythonSubstringException4()
      {
         "0123456789".PythonSubstring(5, 4);
      }
   }
}

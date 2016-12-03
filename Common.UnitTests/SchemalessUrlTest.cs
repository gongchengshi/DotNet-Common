using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEL.Common.UnitTests
{
   public static class Constants
   {
      public static string ExampleUrl = "http://www.example.com/One/Two/file.aspx?c=4&a=5&d=3&b=2#FirstArticle";
   }

   [TestClass]
   public class SchemalessUrlTest
   {
      [TestMethod]
      public void Test()
      {
         var url = new SchemalessUrl(Constants.ExampleUrl);
         var sorted = url.SortQueryString();
         Debugger.Break();
      }
   }
}

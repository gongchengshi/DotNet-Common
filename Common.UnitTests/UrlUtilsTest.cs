using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEL.Common.UnitTests
{
   [TestClass]
   public class UrlUtilsTest
   {
      [TestMethod]
      public void ReplaceDomainTest()
      {
         var url = new Uri(Constants.ExampleUrl);
         var newUrl = url.ReplaceHost("www.google.com");
      }

      [TestMethod]
      public void SortQueryStringTest()
      {
         const string inputUrl = "http://www.selinc.com/WorkArea/linkit.aspx?LinkIdentifier=id&ItemID=6454&libID=6475";
         var sorted = UrlUtils.SortQueryString(new Uri(inputUrl).Query);
      }
   }
}

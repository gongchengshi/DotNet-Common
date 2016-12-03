using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEL.Common.UnitTests
{
    [TestClass]
    public class CompareTest
    {
        [TestMethod]
        public void TestAreEqual()
        {
            Assert.IsTrue(Compare.AreEqual(5.0, 7.0, 2.000001));
            Assert.IsFalse(Compare.AreEqual(5.0, 7.0, 2.0));
        }

        [TestMethod]
        public void TestComparer()
        {
            var intComparer = new Comparer<int>();
            Assert.AreEqual(0, intComparer.Compare(5,5));
            Assert.AreEqual(-1, intComparer.Compare(4, 5));

            var defaultComparer = Comparer<int>.Default;
            Assert.AreEqual(1, defaultComparer.Compare(5, 4));

        }
    }
}

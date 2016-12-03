using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEL.Common.UnitTests
{
    [TestClass]
    public class ReflectionHelpersTest
    {
        [TestMethod]
        public void CreateObjectTest()
        {
            Assert.AreEqual(null, ReflectionHelpers.CreateObject("Yo"));
            var myObject = (MyClass)ReflectionHelpers.CreateObject(typeof (MyClass).Name, 5);
            Assert.AreEqual(5, myObject.X);
        }

        public class MyClass
        {
            public MyClass(int x)
            {
                X = x;
            }

            public int X;
        }
    }
}

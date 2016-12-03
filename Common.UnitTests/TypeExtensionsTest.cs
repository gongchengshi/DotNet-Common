using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEL.UnitTest;

namespace SEL.Common.UnitTests
{
    [TestClass]
    public class TypeExtensionsTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsFalse(typeof(WithoutAttribute).IsOn<MyAttribute>());
            Assert.IsTrue(typeof(WithAttribute).IsOn<MyAttribute>());
            ExceptionAssert.Throws<InvalidOperationException>(() => 
                typeof(WithoutAttribute).Get<MyAttribute>());
            Assert.AreEqual(42, typeof(WithAttribute).Get<MyAttribute>().X);
        }
    }

    public class MyAttribute : Attribute
    {
        public MyAttribute(int x)
        {
            X = x;
        }
        public int X;
    }
    public class WithoutAttribute
    {
    }
    [MyAttribute(42)]
    public class WithAttribute
    {
    }
}

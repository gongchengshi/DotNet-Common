using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace SEL.Common.UnitTests
{
    [TestClass]
    public class DependantInvokerTest
    {
        [TestMethod]
        public void IDDependantInvoker()
        {
            bool executed = false;

            var target = new IDDependantInvoker(() => executed = true);

            int task1ID = target.Add();
            var task1 = Task.Factory.StartNew(() => target.Fullfill(task1ID));
            int task2ID = target.Add();
            var task2 = Task.Factory.StartNew(() => target.Fullfill(task2ID));
            int task3ID = target.Add();
            var task3 = Task.Factory.StartNew(() => target.Fullfill(task3ID));

            Task.WaitAll(new Task[] { task1, task2, task3}, 1000);

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void CountDependantInvoker()
        {
            bool executed = false;

            var target = new CountDependantInvoker(() => executed = true);

            target.Add();
            var task1 = Task.Factory.StartNew(() => target.Fullfill());
            target.Add();
            var task2 = Task.Factory.StartNew(() => target.Fullfill());
            target.Add();
            var task3 = Task.Factory.StartNew(() => target.Fullfill());

            Task.WaitAll(new Task[] { task1, task2, task3 }, 1000);

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void UniqueObjectDependantInvoker()
        {
            bool executed = false;

            var target = new UniqueObjectDependantInvoker(() => executed = true, 3);

            var object1 = new object();

            var task1 = Task.Factory.StartNew(() => target.Fullfill(object1));
            var task2 = Task.Factory.StartNew(() => target.Fullfill(object1));
            var task3 = Task.Factory.StartNew(() => target.Fullfill(new object()));
            var task4 = Task.Factory.StartNew(() => target.Fullfill(new object()));

            Task.WaitAll(new Task[] { task1, task2, task3, task4 }, 1000);

            Assert.IsTrue(executed);
        }
    }
}

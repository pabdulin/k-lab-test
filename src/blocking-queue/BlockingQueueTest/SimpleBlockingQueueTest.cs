using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlockingQueue;
using System.Threading;

namespace BlockingQueueTest
{
    [TestClass]
    public class SimpleBlockingQueueTest
    {
        private static readonly SimpleBlockingQueue _static1 = new SimpleBlockingQueue();

        [TestMethod]
        [Timeout(1000)]
        public void ShouldPutAndGet()
        {
            SimpleBlockingQueue target = new SimpleBlockingQueue();
            int testValue = 1;
            target.Put(testValue);
            var testValueBack = target.Get();
            Assert.AreEqual(testValue, testValueBack);
        }

        [TestMethod]
        [Timeout(1000)]
        public void ShouldReturnNullAfterDisabling()
        {
            SimpleBlockingQueue target = new SimpleBlockingQueue();
            int testValue = 1;
            target.Put(testValue);
            target.Disable();
            var testValueBack = target.Get();
            Assert.AreEqual(null, testValueBack);
        }

        [TestMethod]
        [Timeout(1000)]
        public void ShouldBlockOnEmptyQueue_TestMustFail()
        {
            SimpleBlockingQueue target = new SimpleBlockingQueue();
            var testValueBack = target.Get();
        }

        [TestMethod]
        [Timeout(1000)]
        public void Multithread()
        {
            Thread[] threads = new Thread[2];
            object _res1 = null;
            object _res2 = null;

            threads[0] = new Thread(() =>
            {
                _res1 = _static1.Get();
                _res2 = _static1.Get();
            });

            threads[1] = new Thread(() =>
            {
                _static1.Put(1);
                _static1.Put(2);
            });

            for (int i = 0; i < threads.Length; i += 1)
            {
                threads[i].Start();
            }

            for (int i = 0; i < threads.Length; i += 1)
            {
                threads[i].Join();
            }

            Assert.AreEqual(1, _res1);
            Assert.AreEqual(2, _res2);
        }
    }
}

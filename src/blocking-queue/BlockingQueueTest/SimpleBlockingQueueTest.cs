using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlockingQueue;
using System.Threading;

namespace BlockingQueueTest
{
    [TestClass]
    public class SimpleBlockingQueueTest
    {
        [TestMethod]
        [Timeout(1000)]
        public void ShouldPutAndGet()
        {
            SimpleBlockingQueue target = new SimpleBlockingQueue();
            {
                object testValue = 1;
                target.Put(testValue);
                var testValueBack = target.Get();
                Assert.AreEqual(testValue, testValueBack);
            }
            {
                object testValue = "test";
                target.Put(testValue);
                var testValueBack = target.Get();
                Assert.AreEqual(testValue, testValueBack);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void ShouldReturnNullAfterDisabling()
        {
            SimpleBlockingQueue target = new SimpleBlockingQueue();
            int testValue = 1;
            target.Put(testValue);
            var testValueBackBeforeDisable = target.Get();
            Assert.AreEqual(testValue, testValueBackBeforeDisable);

            target.Put(testValue);
            target.Disable();
            var testValueBackAfterDisable = target.Get();
            Assert.AreEqual(null, testValueBackAfterDisable);
        }

        [TestMethod]
        [Timeout(1000)]
        public void ShouldWaitOnEmptyQueue_TestMustFail()
        {
            SimpleBlockingQueue target = new SimpleBlockingQueue();
            var testValueBack = target.Get();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ShouldNotAlterOrder()
        {
            var blockingQueue = new SimpleBlockingQueue();
            object _res1 = null;
            object _res2 = null;
            object _res3 = null;

            var thread1 = new Thread(() =>
            {
                _res1 = blockingQueue.Get();
                _res2 = blockingQueue.Get();
                _res3 = blockingQueue.Get();
            });
            thread1.Start();

            var thread2 = new Thread(() =>
            {
                blockingQueue.Put(1);
                blockingQueue.Put(10f);
                blockingQueue.Put("hello");
            });
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreEqual(1, _res1);
            Assert.AreEqual(10f, _res2);
            Assert.AreEqual("hello", _res3);
        }

        [TestMethod]
        [Timeout(1000)]
        public void ShouldNotDeadlock()
        {
            SimpleBlockingQueue _bq = new SimpleBlockingQueue();

            const int THREAD_COUNT = 50;
            Thread[] putThreads = new Thread[THREAD_COUNT];
            Thread[] getThreads = new Thread[THREAD_COUNT + (THREAD_COUNT / 2)];
            Thread[] disableThreads = new Thread[THREAD_COUNT / 2];

            for (int i = 0; i < getThreads.Length; i += 1)
            {
                getThreads[i] = new Thread(() =>
                {
                    _bq.Get();
                    _bq.Get();
                });
                getThreads[i].Start();

            }

            for (int i = 0; i < putThreads.Length; i += 1)
            {
                var capture = i;
                putThreads[i] = new Thread(() =>
                {
                    _bq.Put(capture);
                    _bq.Put(capture + putThreads.Length);
                });
                putThreads[i].Start();

            }

            for (int i = 0; i < disableThreads.Length; i += 1)
            {
                disableThreads[i] = new Thread(() =>
                {
                    _bq.Disable();
                });
                disableThreads[i].Start();

            }

            for (int i = 0; i < putThreads.Length; i += 1)
            {
                putThreads[i].Join();
            }

            for (int i = 0; i < getThreads.Length; i += 1)
            {
                getThreads[i].Join();
            }

            for (int i = 0; i < disableThreads.Length; i += 1)
            {
                disableThreads[i].Join();
            }

            // if we get here then there was no deadlock
        }
    }
}

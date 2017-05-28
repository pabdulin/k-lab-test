using BlockingQueue;
using System;
using System.Diagnostics;
using System.Threading;

namespace BlockingQueueApp
{
    public class Program
    {
        private static readonly SimpleBlockingQueue _bq = new SimpleBlockingQueue();

        public static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            TestMultiThreadReadWrite();
            if (Debugger.IsAttached)
            {
                Console.WriteLine("---");
                Console.WriteLine("Press any key to exit..");
                Console.ReadKey();
            }
        }

        private static void TestMultiThreadReadWrite()
        {
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

            Console.WriteLine("---");
            Console.WriteLine("All threads completed without deadlocks.");
        }
    }
}

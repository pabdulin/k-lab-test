using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace BlockingQueue
{
    public class SimpleBlockingQueue : IBlockingQueue
    {
        private bool _isDisabled = false;
        private Queue<object> _store = new Queue<object>();
        //private readonly AutoResetEvent _empty = new AutoResetEvent(false);
        private readonly Semaphore _empty = new Semaphore(0, int.MaxValue);
        private readonly object _lock = new object();
        private int _waitingThreads = 0;
#if DEBUG
        private const int OPERATION_PAUSE = 50;
        private Random rnd = new Random();
#endif

        /// <summary>
        /// Put кладет новый элемент в очередь.
        /// </summary>
        public void Put(object o)
        {
#if DEBUG
            Thread.Sleep(rnd.Next(OPERATION_PAUSE));
#endif
            lock (_lock)
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Put: ({o})");
                if (_isDisabled)
                {
                    Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Put: Queue is disabled, return");
                    return;
                }
                _store.Enqueue(o);
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Put: About to signal..");
                //var signalSuccess = _empty.Set();
                var semCount = _empty.Release();

                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Put: exit, PREV_SEM={semCount}");
            }
        }

        /// <summary>
        /// Get должен блокировать выполнение потока если в очереди нет элементов до тех пор, 
        /// пока не появится элемент, который можно вернуть.
        /// </summary>
        public object Get()
        {
#if DEBUG
            Thread.Sleep(rnd.Next(OPERATION_PAUSE));
#endif
            object result = null;

            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Get: ( )");
            if (!_isDisabled)
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Get: Queue not disabled, wait..");
                Interlocked.Increment(ref _waitingThreads);
                _empty.WaitOne();
                Interlocked.Decrement(ref _waitingThreads);
                lock (_lock)
                {
                    if (_isDisabled)
                    {
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Get: Wait complete, but queue is disabled now");
                    }
                    else
                    {
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Get: Wait complete, proceed to get value");
                        result = _store.Dequeue();
                    }
                }
            }

            var resStr = result ?? "'null'";
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Get: exit, got value: {resStr}");
            return result;
        }

        /// <summary>
        /// Disable должен освобождать все ждущие потоки. 
        /// При этом Get должен возвращать null (иликидать исключение - как больше нравится). 
        /// Все последующие вызовы к Get должны просто возвращать null.
        /// </summary>
        public void Disable()
        {
#if DEBUG
            Thread.Sleep(rnd.Next(OPERATION_PAUSE * 8));
#endif
            lock (_lock)
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Disable: ( )");
                if (_isDisabled)
                {
                    Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Disable: Already disabled, return");
                    return;
                }

                _isDisabled = true;
                // TODO free all waiting threads
                var threadsToFree = _waitingThreads;
                if (threadsToFree > 0)
                {
                    var semCount = _empty.Release(threadsToFree);
                    Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Disable: released {threadsToFree} threads, PREV_SEM={semCount}");
                }

                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Disable: exit");
            }
        }
    }
}

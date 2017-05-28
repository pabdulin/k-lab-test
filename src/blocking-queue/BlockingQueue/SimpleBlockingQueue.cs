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
        //private readonly Semaphore _empty = new Semaphore(0, int.MaxValue);
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
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Put: ({o})");
            if (!_isDisabled) // fast check
            {
                lock (_lock)
                {
                    if (_isDisabled) // check if not locked by this time
                    {
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Put: Queue is disabled, return");
                        return;
                    }
                    _store.Enqueue(o);
                    Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Put: About to signal..");
                    //var semCount = _empty.Release(); // semaphore
                    Monitor.Pulse(_lock);

                    //Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Put: PREV_SEM={semCount}"); // semaphore
                }
            }
#if DEBUG
            else
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Put: Queue is disabled (no lock)");
            }
#endif
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Put: exit");
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
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Get: ( )");
            object result = null;
            if (!_isDisabled)
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Get: Queue not disabled, wait..");
                Interlocked.Increment(ref _waitingThreads);
                //_empty.WaitOne(); // semaphore
                lock (_lock)
                {
                    while (_store.Count == 0 && !_isDisabled)
                    {
                        Monitor.Wait(_lock);
                    }
                    _waitingThreads -= 1;
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
#if DEBUG
            else
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Get: Queue is disabled (no lock)");
            }
#endif

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
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Disable: ( )");
            if (!_isDisabled) // fast check
            {
                lock (_lock)
                {
                    if (_isDisabled) // check if not locked by this time
                    {
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Disable: already disabled (inside lock)");
                        return;
                    }

                    _isDisabled = true;
                    // TODO free all waiting threads
                    var threadsToFree = _waitingThreads;
                    if (threadsToFree > 0)
                    {
                        //for (int i = 0; i < threadsToFree; i += 1)
                        //{
                            Monitor.PulseAll(_lock);
                        //}
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Disable: released {threadsToFree} threads");

                        //var semCount = _empty.Release(threadsToFree); // semaphore
                        // Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Disable: released {threadsToFree} threads, PREV_SEM={semCount}"); // semaphore
                    }
                }
            }
#if DEBUG
            else
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Disable: already disabled (exit with no lock)");
            }
#endif
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId} Disable: exit");
        }
    }
}

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
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Put {o}: enter");
#if DEBUG
            Thread.Sleep(rnd.Next(OPERATION_PAUSE));
#endif
            // fast unsafe check
            if (!_isDisabled)
            {
                lock (_lock)
                {
                    // check if was disabled while we were waiting
                    if (_isDisabled)
                    {
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Put {o}: Queue was disabled (in lock)");
                    }
                    else
                    {
                        _store.Enqueue(o);
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Put {o}: value stored");
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Put {o}: About to pulse..");
                        // release one thread
                        Monitor.Pulse(_lock);
                    }
                }
            }
            else
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Put {o}: Queue is disabled (fast check)");
            }
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Put {o}: exit");
        }

        /// <summary>
        /// Get должен блокировать выполнение потока если в очереди нет элементов до тех пор, 
        /// пока не появится элемент, который можно вернуть.
        /// </summary>
        public object Get()
        {
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Get: enter");
#if DEBUG
            Thread.Sleep(rnd.Next(OPERATION_PAUSE));
#endif
            object result = null;
            // fast unsafe check
            if (!_isDisabled)
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Get: queue not disabled (fast check)");
                lock (_lock)
                {
                    _waitingThreads += 1;
                    // wait while empty queue and but not disabled
                    // will exit loop, if there is new element in queue,
                    // or queue was disabled
                    while (_store.Count == 0 && !_isDisabled)
                    {
                        Monitor.Wait(_lock);
                    }
                    _waitingThreads -= 1;

                    // check if was disabled while we were waiting for next item
                    if (_isDisabled)
                    {
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Get: wait complete, but queue is disabled now (in lock)");
                    }
                    else
                    {
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Get: wait complete, proceed to get value");
                        result = _store.Dequeue();
                    }
                }
            }
            else
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Get: queue is disabled (no lock)");
            }
            var resStr = result ?? "null";
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Get: exit, got return value: {resStr}");
            return result;
        }

        /// <summary>
        /// Disable должен освобождать все ждущие потоки. 
        /// При этом Get должен возвращать null (иликидать исключение - как больше нравится). 
        /// Все последующие вызовы к Get должны просто возвращать null.
        /// </summary>
        public void Disable()
        {
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Disable: enter");
#if DEBUG
            Thread.Sleep(rnd.Next(OPERATION_PAUSE * 8));
#endif
            // fast check
            if (!_isDisabled)
            {
                lock (_lock)
                {
                    // check if still not disabled by this time
                    if (_isDisabled)
                    {
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Disable: already disabled (inside lock)");
                    }
                    else
                    {
                        _isDisabled = true;
                        // release all waiting threads
                        Monitor.PulseAll(_lock);
                        Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Disable: released {_waitingThreads} waiting threads");
                    }
                }
            }
            else
            {
                Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Disable: already disabled (fast check)");
            }
            Trace.WriteLine($"T{Thread.CurrentThread.ManagedThreadId,3} Disable: exit");
        }
    }
}

using System.Collections.Generic;
using System.Threading;

namespace BlockingQueue
{
    public class SimpleBlockingQueue : IBlockingQueue
    {
        private bool _isDisabled = false;
        private Queue<object> _store = new Queue<object>();
        private readonly AutoResetEvent _empty = new AutoResetEvent(false);

        /// <summary>
        /// Put кладет новый элемент в очередь.
        /// </summary>
        public void Put(object o)
        {
            _store.Enqueue(o);
            _empty.Set();
        }

        /// <summary>
        /// Get должен блокировать выполнение потока если в очереди нет элементов до тех пор, 
        /// пока не появится элемент, который можно вернуть.
        /// </summary>
        public object Get()
        {
            if (!_isDisabled)
            {
                _empty.WaitOne();
            }

            if (_store.Count > 0)
            {
                var result = _store.Dequeue();
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Disable должен освобождать все ждущие потоки. 
        /// При этом Get должен возвращать null (иликидать исключение - как больше нравится). 
        /// Все последующие вызовы к Get должны просто возвращать null.
        /// </summary>
        public void Disable()
        {
            _isDisabled = true;
            _empty.Set();
        }
    }
}

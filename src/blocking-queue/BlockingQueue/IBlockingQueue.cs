namespace BlockingQueue
{
    public interface IBlockingQueue
    {
        /// <summary>
        /// Put кладет новый элемент в очередь.
        /// </summary>
        void Put(object o);
        /// <summary>
        /// Get должен блокировать выполнение потока если в очереди нет элементов до тех пор, пока не появится элемент, который можно вернуть.
        /// </summary>
        object Get();
        /// <summary>
        /// Disable должен освобождать все ждущие потоки. 
        /// При этом Get должен возвращать null (иликидать исключение - как больше нравится). 
        /// Все последующие вызовы к Get должны просто возвращать null.
        /// </summary>
        void Disable();
    }
}

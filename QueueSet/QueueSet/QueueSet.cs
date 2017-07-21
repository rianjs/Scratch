using System;
using System.Collections.Generic;

namespace QueueSet
{
    public class QueueSet<T> where T : IEquatable<T>
    {
        private readonly Dictionary<T, int> _dict = new Dictionary<T, int>();
        private readonly object _lock = new object();
        private readonly Queue<T> _queue = new Queue<T>();

        public void Enqueue(T item)
        {
            lock (_lock)
            {
                var newCount = (_dict.ContainsKey(item) ? _dict[item] : 0) + 1;
                _dict[item] = newCount;
                _queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            lock (_lock)
            {
                var item = _queue.Dequeue();
                _dict[item]--;
                return item;
            }
        }

        public bool Contains(T item)
        {
            lock (_lock)
            {
                return _dict.ContainsKey(item) && _dict[item] > 0;
            }
        }
    }
}

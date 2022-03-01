using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SufficitBlazorClient
{
    public class MessagesBuffer<T> : ICollection<T>
    {
        private readonly object _lockCollection;
        private readonly List<T> _collection;
        private readonly int _max;

        private int _counter;

        public MessagesBuffer(int max = 20)
        {
            _lockCollection = new object();
            _collection = new List<T>();
            _max = max;
        }

        public int Count { get { lock (_lockCollection) return _collection.Count; } }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            lock (_lockCollection)
            {
                if (_counter >= _max)
                {
                    _collection.RemoveAt(0);
                    _counter--;
                }

                _collection.Add(item);
                _counter++;
            }
        }

        public void Clear()
        {
            lock (_lockCollection)
            {
                _collection.Clear();
                _counter = 0;
            }
        }

        public bool Contains(T item)
        {
            lock (_lockCollection)
                return _collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lockCollection)
                _collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            lock (_lockCollection)
            {
                if(_collection.Remove(item))
                {
                    _counter--;
                    return true;
                }
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lockCollection) return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}

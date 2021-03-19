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

        public int Count { get { lock (_lockCollection) return  _collection.Count(); } }

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
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lockCollection) return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}

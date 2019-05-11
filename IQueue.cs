using System;
using System.Collections.Generic;

namespace RedTrust
{
    public interface IQueue<T>
    {
        List<T> List;
        public void Enqueue(T item);
        public string Dequeue();
        public int Count();
    }
}


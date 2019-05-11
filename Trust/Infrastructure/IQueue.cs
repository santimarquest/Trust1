namespace Trust
{
    public interface IQueue<T>
    {
        void Enqueue(T item);
        string Dequeue();
        int Count();
    }
}


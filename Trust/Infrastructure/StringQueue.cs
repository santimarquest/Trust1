using Trust.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Trust
{
    // Este va a ser nuestro wrapper para la lista de strings
    public class StringQueue : IQueue<string>, IDisposable
    {
        private ConcurrentQueue<string> QList { get; }

        public event EventHandler<ItemEventArgs<string>> ItemEnqueued;
        public event EventHandler<ItemEventArgs<string>> ItemDequeued;

        static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        // Consola MultiThreading, para escribir los mensajes al usuario
        private static ConsoleMultiThreading Console = new ConsoleMultiThreading();

        public static StringQueue CreateStringQueue(List<string> list)
        {
            // Mejor una factory para validar los parámetros del constructor
            if (list == null)
            {
                throw new ArgumentNullException();
            }

            return new StringQueue(list);
        }

        private StringQueue(List<string> list)
        {
            // A partir de una lista, creamos una cola concurrente thread-safe

            QList = new ConcurrentQueue<string>(list);

        // Dotamos a nuestra cola de eventos, para poder ejecutar nuestro código cada
        // vez que se produzca uno de estos eventos

        // Handler para cuando eliminamos un elemento de la cola
            ItemDequeued += (s, e) =>
            {
                Console.WriteLine($"Thread: {Thread.CurrentThread.Name}. " +
                                  $"Dequeued: {e.Item}.");

                if (QList.Count == 0)
                {
                    autoResetEvent.WaitOne();
                    Console.WriteLine($"Thread: {Thread.CurrentThread.Name}. " +
                        $"No tengo elementos que procesar. Me voy a dormir.....");
                }
            };

            // Handler para cuando insertamos un elemento de la cola
            ItemEnqueued += (s, e) =>
            {
                Console.WriteLine($"Thread: {Thread.CurrentThread.Name}. " +
                                  $"Enqueued: {e.Item}.");
               autoResetEvent.Set();
            };
        }

        public int Count()
        {
            return QList.Count;
        }

        public string Dequeue()
        {
            string str;
            var success = QList.TryDequeue(out str);
            if (success)
            {
                OnItemDequeued(str);
            }
            return str;
        }

        public void Enqueue(string item)
        {
           if (item != null)
            {
                QList.Enqueue(item);
                OnItemEnqueued(item);
            }
        }

        private void OnItemEnqueued(string item)
        {
            // Se levanta el evento ItemEnqueued, que será manejado por el delegado
            // que hemos asociado en el constructor.
            ItemEnqueued?.Invoke(this, new ItemEventArgs<string> { Item = item });
        }

        private void OnItemDequeued(string item)
        {
            // Se levanta el evento ItemDequeued, que será manejado por el delegado
            // que hemos asociado en el constructor. 
            ItemDequeued?.Invoke(this, new ItemEventArgs<string> { Item = item });
        }

        public sealed class ItemEventArgs<T> : EventArgs
        {
            public T Item { get; set; }
        }

        public void Dispose()
        {

        }
    }
}
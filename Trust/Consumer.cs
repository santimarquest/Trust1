using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trust
{
    public class Consumer : IConsumer
    {
        private static StringQueue StringQueue;

        private Consumer(StringQueue sq)
        {
            StringQueue = sq;
        }

        public static Consumer CreateConsumerNewThread (StringQueue sq, string name)
        {
            var consumer = new Consumer(sq);
            var t1 = new Thread(() => consumer.Consume());
            t1.Name = name;
            t1.Start();
            return consumer;
        }

        public string Consume()
        {
            while (true)
            {
                StringQueue.Dequeue();
            }
        }
    }
}

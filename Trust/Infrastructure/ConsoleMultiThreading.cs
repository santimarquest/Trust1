using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trust.Infrastructure
{
    // La consola no es un objeto thread-safe, lo encapsulamos de forma que 
    // sí lo sea

    public class ConsoleMultiThreading
    {
        private AutoResetEvent _blockThread = new AutoResetEvent(true);

        public void WriteLine(string message)
        {
            _blockThread.WaitOne();
            Console.WriteLine(message);
            _blockThread.Set();
        }
    }
}

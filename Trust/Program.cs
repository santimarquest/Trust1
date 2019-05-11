using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trust
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Eventos para la gestión de excepciones.
            // Esta es la gestión mínima que se debería hacer siempre
            // Cuando existen varios hilos (multi-threading), es una forma
            // sencilla de tratar todas las excepciones que se produzcan,
            // sin tener que recurrir al try/catch/finally en varios puntos
            // del código.

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Thread.CurrentThread.Name = "Proceso principal";

            using (var sq = StringQueue.CreateStringQueue(
                new List<string> {"A0","B0", "C0", "D0", "E0",
                                  "A1","B1", "C1", "D1", "E1",
                                  "A2","B2", "C2", "D2", "E2",
                                  "A3","B3", "C3", "D3", "E3",
                                  "A4","B4", "C4", "D4", "E4",
                                  "A5","B5", "C5", "D5", "E5",
                                  "A6","B6", "C6", "D6", "E6",
                                  "A7","B7", "C7", "D7", "E7",
                                  "A8","B8", "C8", "D8", "E8",
                                  "A9","B9", "C9", "D9", "E9"
                                                    }))
            {
                Consumer.CreateConsumerNewThread(sq, "Consumer 1");
                Consumer.CreateConsumerNewThread(sq, "Consumer 2");

                // Para verificar que los dos threads anteriores están esperando
                // que existan de nuevo elementos en la cola

                Thread.Sleep(1000);

                for (var i = 0; i < 20; i++)
                {
                    sq.Enqueue("TEST" + i.ToString());
                }
            }
        }

        //Occurs when an untrapped thread exception is thrown.
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // Log the exception, display it, etc
            Debug.WriteLine(e.Exception.Message);
        }

        //Occurs when an exception is not caught.
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Log the exception, display it, etc
            Debug.WriteLine((e.ExceptionObject as Exception).Message);
        }
    }
}

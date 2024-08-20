using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReadManyWriteOne
{
    /// <summary>
    /// Runs the multithreading demo
    /// </summary>
    class Program
    {
        private const int ITERATIONS = 100_000;
        private const int LOG_ITERATIONS = 10_000;

        private static StringReverserThdSafe rw = new();

        static private void WriteWork(string threadName)
        {
            for (int i = 1; i <= ITERATIONS; i++)
            {
                rw.Write();
                if (i % LOG_ITERATIONS == 0)
                {
                    Console.WriteLine($"Thread {threadName}: Iteration {i}");
                }
            }
        }

        static void ReadWork(string threadName)
        {
            for (int i = 1; i <= ITERATIONS; i++)
            {
                rw.ReadTest();
                if (i % LOG_ITERATIONS == 0)
                {
                    Console.WriteLine($"Thread {threadName}: Iteration {i}");
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting threads...");

            // Create and start threads
            Task taskR1 = Task.Run(() => ReadWork("R1"));
            Task taskR2 = Task.Run(() => ReadWork("R2"));
            Task taskR3 = Task.Run(() => ReadWork("R3"));

            Task taskW1 = Task.Run(() => WriteWork("Write1"));
            Task taskW2 = Task.Run(() => WriteWork("Write2"));

            // Wait for threads to finish
            taskR1.Wait();
            taskR2.Wait();
            taskR3.Wait();

            taskW1.Wait();
            taskW2.Wait();

            Console.WriteLine($"All threads completed.  Multithreading error count: {rw.ErrorCount}");
        }
    }
}

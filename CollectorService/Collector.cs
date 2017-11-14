using System;
using System.Threading;

namespace CollectorService
{
    class Collector
    {
        //private static readonly HttpClient client = new HttpClient();
        Provider provider;

        public Collector()
        {
            provider = new Provider();
            // Set max threads to 10 for the ThreadPool and let it handle the management
            // and reuse of individual threads
            ThreadPool.SetMaxThreads(11, 11);
            // Start the 'main' thread which will poll the provider regardless of the existence of data
            (new Thread(() => { CollectData(); })).Start();
        }

        private void CollectData()
        {
            // Kind of a master process, checks if there is anything to read,
            // if so, it will spawn a child to help with the data processing
            while (true)
            {
                int result = provider.Get();
                PrintThreadInfo();
                if (result == 0)
                {
                    Console.WriteLine("MainProcess is sleeping, no data found");
                    Thread.Sleep(10000);
                }
                else
                {
                    Console.WriteLine("MainProcess spawns a child");
                    ThreadPool.QueueUserWorkItem(GetData);
                    Thread.Sleep(result);
                }
            }
        }

        private void GetData(object stateInfo)
        {
            Console.WriteLine("Reading data from provider");
            PrintThreadInfo();
            int result = provider.Get();
            // Child calls for additional help if there is more data to read
            if(result != 0)
            {
                Console.WriteLine("Child spawns a child");
                ThreadPool.QueueUserWorkItem(GetData);
                Thread.Sleep(result);
            }
        }

        private void PrintThreadInfo()
        {
            int max, available;
            ThreadPool.GetMaxThreads(out max, out _);
            ThreadPool.GetAvailableThreads(out available, out _);

            Console.WriteLine("Max = " + max + ", Available = " + available + ", Running = " + (max - available));
        }
    }
}

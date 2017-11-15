using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace CollectorService
{
    class Collector
    {
        private static readonly int maxThreads, pollingTime;
        private static readonly string restProviderUrl;
        private int activeThreads;

        static Collector()
        {
            restProviderUrl = "http://autoscaleproducer.azurewebsites.net";
            maxThreads = 10; // Number of maximum concurrent threads
            pollingTime = 1000; // Time in milliseconds, the main polling thread should wait after a unsuccessful GET request
        }

        public Collector()
        {
            // Start the 'main' thread which will poll the provider regardless of the existence of data
            activeThreads = 1;
            (new Thread(() => { PollProvider(); })).Start();
        }

        private void PollProvider()
        {
            // Kind of a master process, checks if there is anything to read,
            // if so, it will spawn a child to help with the data processing
            while (true)
            {
                string result = GetMeasurementData();

                if (!result.Equals(String.Empty))
                {
                    SpawnHelper();
                    Console.WriteLine(result);
                    Thread.Sleep(Int32.Parse(result));
                }
                else
                {
                    Thread.Sleep(pollingTime);
                }
            }
        }

        private void SpawnHelper()
        {
            if(activeThreads < maxThreads)
            {
                Console.WriteLine("New thread started, active threads: " + ++activeThreads);
                (new Thread(() => { PollUntilFailure(); })).Start();
            }
        }

        private void PollUntilFailure()
        {
            string result;

            do
            {
                // As we should not know if there is more than one data, the spawned child process
                // will always try to read at least once, instead of relying on the queue length
                // from the HTTP header
                result = GetMeasurementData();

                if (!result.Equals(String.Empty))
                {
                    // Helpers will also spawn additional threads if needed (while also being under the limit)
                    SpawnHelper();
                    Console.WriteLine(result);
                    Thread.Sleep(Int32.Parse(result));
                }
            }
            while (!result.Equals(String.Empty));

            Console.WriteLine("Thread suspended, active threads: " + --activeThreads);
        }

        private string GetMeasurementData()
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(restProviderUrl);
                HttpResponseMessage response = client.GetAsync("/api/meter").Result;

                if(response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    Console.WriteLine("Queue Length: " + response.Headers.ToDictionary(l => l.Key, k => k.Value)["QueueLength"].First());
                    return response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }
}

using System;

namespace CollectorService
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Making API Call...");
            //using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            //{
            //    client.BaseAddress = new Uri("http://autoscaleproducer.azurewebsites.net/api/meter");
            //    HttpResponseMessage response = client.GetAsync("answers?order=desc&sort=activity&site=stackoverflow").Result;
            //    response.EnsureSuccessStatusCode();
            //    string result = response.Content.ReadAsStringAsync().Result;
            //    Console.WriteLine("Result: " + result);
            //}
            //Console.ReadLine();

            new Collector();
            Console.ReadLine();
        }
    }
}

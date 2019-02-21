using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using RestSharp;

namespace ServiceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("use: dotnet ServiceTest.dll <params>");
                Console.WriteLine("params: value or inc");
                return;

            }
            if (args[0] == "value")
            {
                TestCallValue();
            }
            else if (args[0] == "inc")
            {
                TestIncrement();
            }
        }

        static void TestCallValue()
        {
            while (true)
            {
                var url = GetServiceUrl();
                if (url != null)
                {
                    ShowValues(url);
                }
                else
                {
                    Console.WriteLine("No service's endpoint found");
                }
                Thread.Sleep(2000);
            }
        }

        static void TestIncrement()
        {
            while (true)
            {
                var url = GetServiceUrl();
                if (url != null)
                {
                    IncrementValue(url);
                }
                else
                {
                    Console.WriteLine("No service's endpoint found");
                }
                Thread.Sleep(2000);
            }
        }

        private static void IncrementValue(Uri url)
        {
            Console.WriteLine($"Using endpoint {url}");

            var client = new RestClient(url);
            var request = new RestRequest("api/values/inc", Method.GET);
            var response = client.Execute<int>(request);

            Console.WriteLine($"New value {response.Data}");

            Console.WriteLine("-----------------------------------------------------------");
        }

        private static void ShowValues(Uri url)
        {
            Console.WriteLine($"Using endpoint {url}");

            var client = new RestClient(url);
            var request = new RestRequest("api/values", Method.GET);
            var response = client.Execute<List<string>>(request);

            foreach (var value in response.Data)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine("-----------------------------------------------------------");
        }

        private static Uri GetServiceUrl()
        {
            Uri url = null;

            var consulClient = new ConsulClient();
            try
            {
                var query = consulClient.Catalog.Service("ois-test-service").Result;
                if (query?.Response != null && query.Response.Any())
                {
                    var servicesCount = query.Response.Length;
                    var random = new Random();
                    var index = random.Next(servicesCount);
                    Console.WriteLine($"registried {servicesCount} services. used {index + 1}");
                    url = new Uri($"http://{query.Response[index].Address}:{query.Response[index].ServicePort}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}: {e.StackTrace}");
            }

            return url;
        }
    }
}

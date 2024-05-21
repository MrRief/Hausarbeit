using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Client
{
    public class HttpClientSingleton
    {
        private static readonly Lazy<HttpClient> lazyHttpClient = new Lazy<HttpClient>(() =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:44351/")
            };
            return client;
        });

        public static HttpClient Instance => lazyHttpClient.Value;
    }

}

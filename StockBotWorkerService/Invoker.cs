using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StockBotWorkerService
{
    public class Invoker
    {
        private readonly IConfiguration _configuration;
        static readonly HttpClient _client = new HttpClient();
        public Invoker(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string InvokeApi(string stockCode)
        {
            string content = string.Empty;
            try
            {
                var urlApi = _configuration["UrlApiStock"];
                // No need to use templates engine like Handlebars, it's just one field to replace.
                urlApi = urlApi.Replace("{{StockCode}}", stockCode);
                content = _client.GetStringAsync(urlApi).Result;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return content;
        }
    }
}

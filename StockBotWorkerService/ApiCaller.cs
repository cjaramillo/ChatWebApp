﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StockBotWorkerService
{
    public class ApiCaller
    {
        private readonly IConfiguration _configuration;
        public ApiCaller(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Call(string stockCode)
        {
            string content = string.Empty;
            try
            {
                var urlApi = _configuration["UrlApiStock"];
                // No need to use templates engine like Handlebars, it's just one field to replace.
                urlApi = urlApi.Replace("{{StockCode}}", stockCode);
                int secondsTimeout = 30;
                int.TryParse(_configuration["TimeoutSecondsApiStock"] ?? "30", out secondsTimeout);
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(secondsTimeout);
                content = httpClient.GetStringAsync(urlApi).Result;
            }
            catch (Exception e) 
            {
                content = e.Message;
            }
            return content;
        }
    }
}

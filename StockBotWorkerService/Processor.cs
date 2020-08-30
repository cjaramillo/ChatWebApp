using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace StockBotWorkerService
{
    public class Processor
    {
        private readonly IConfiguration _configuration;
        private ApiCaller _invoker;
        private Parser _parser;
        public Processor(IConfiguration configuration)
        {
            _configuration = configuration;
            _invoker = new ApiCaller(_configuration);
            _parser = new Parser(_configuration);
        }

        public string ProcessMessage(string message)
        {
            string response = string.Empty;
            string[] commandSplitted;
            if (message.StartsWith("/stock="))
            {
                commandSplitted = message.Split("/stock=");
                string stockCode = commandSplitted[1];
                if (string.IsNullOrEmpty(stockCode))
                {
                    response = "Stock code is required.";
                }
                else
                {
                    string apiResponse = _invoker.Call(commandSplitted[1]);
                    var parsedResponse = _parser.ParseContent(apiResponse);
                    if (parsedResponse.ContainsKey("Symbol") && parsedResponse.ContainsKey("Close"))
                    {
                        if (parsedResponse["Close"] == "N/D")
                            response = $"No data available for {parsedResponse["Symbol"]}";
                        else
                            response = $"{parsedResponse["Symbol"]} quote is ${parsedResponse["Close"]} per share";
                    }
                    else
                    {
                        response = "Stock quote API response could not be recognized";
                    }
                }
            }
            else
            {
                response = "Command not recognized";
            }
            return response;
        }
    }
}

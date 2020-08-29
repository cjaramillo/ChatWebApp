using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockBotWorkerService
{
    public class Parser
    {
        private readonly IConfiguration _configuration;
        public Parser(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Parse the response API
        /// </summary>
        /// <param name="message">Message input</param>
        public Dictionary<string, string> ParseContent(string message) 
        {
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(message))
                return myDict;
            string[] lines = message.Split("\n");
            if (lines.Length < 2)
                return myDict;
            string[] fields = lines[0].Split(",");
            string[] values = lines[1].Split(",");
            if (fields.Length != values.Length)
                return myDict;
            for (int i = 0, limit = fields.Length; i < limit; i++)
            {
                myDict.Add(fields[i], values[i]);
            }
            return myDict;
        }
    }
}

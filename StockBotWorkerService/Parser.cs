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
        /// Parse the response API into dictionary
        /// </summary>
        /// <param name="message">Message input</param>
        public Dictionary<string, string> ParseContent(string message) 
        {
            message = message.Trim();
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(message))
                return myDict;
            string[] lines = message.Split("\n");
            if (lines.Length < 2)
                return myDict;
            lines[0] = lines[0].Trim();
            lines[1] = lines[1].Trim();
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

using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using StockBotWorkerService;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitTestProject
{
    public class ParserTest : UnitTestConfig
    {
        [Test]
        public void ParseWithOkContent()
        {
            Parser parser = new Parser(_configuration);
            Dictionary<string, string> parsedDict = parser.ParseContent("Symbol,Date,Time,Open,High,Low,Close,Volume\r\nAAPL.US,2020-08-27,22:00:02,508.57,509.94,495.33,500.04,38744808\r\n");
            Assert.AreEqual(parsedDict["Symbol"], "AAPL.US");
            Assert.AreEqual(parsedDict["Date"], "2020-08-27");
            Assert.AreEqual(parsedDict["Time"], "22:00:02");
            Assert.AreEqual(parsedDict["Open"], "508.57");
            Assert.AreEqual(parsedDict["High"], "509.94");
            Assert.AreEqual(parsedDict["Low"], "495.33");
            Assert.AreEqual(parsedDict["Close"], "500.04");
            Assert.AreEqual(parsedDict["Volume"], "38744808");
        }

        [Test]
        public void ParseWithEmptyContent()
        {
            Parser parser = new Parser(_configuration);
            Dictionary<string, string> parsedDict = parser.ParseContent(string.Empty);
            Assert.AreEqual(parsedDict.Count, 0);
        }
    }
}

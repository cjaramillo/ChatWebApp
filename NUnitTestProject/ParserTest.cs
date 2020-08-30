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
            Assert.AreEqual("AAPL.US", parsedDict["Symbol"]);
            Assert.AreEqual("2020-08-27", parsedDict["Date"]);
            Assert.AreEqual("22:00:02", parsedDict["Time"]);
            Assert.AreEqual("508.57", parsedDict["Open"]);
            Assert.AreEqual("509.94", parsedDict["High"]);
            Assert.AreEqual("495.33", parsedDict["Low"]);
            Assert.AreEqual("500.04", parsedDict["Close"]);
            Assert.AreEqual("38744808", parsedDict["Volume"]);
        }

        [Test]
        public void ParseWithEmptyContent()
        {
            Parser parser = new Parser(_configuration);
            Dictionary<string, string> parsedDict = parser.ParseContent(string.Empty);
            Assert.AreEqual(0, parsedDict.Count);
        }
    }
}

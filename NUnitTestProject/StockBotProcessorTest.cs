using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using StockBotWorkerService;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitTestProject
{
    public class StockBotProcessorTest : UnitTestConfig
    {
        private Processor _processor;

        [OneTimeSetUp]
        public override void Setup()
        {
            _configuration = BuildConfiguration(TestContext.CurrentContext.TestDirectory);
            _processor = new Processor(_configuration);
        }


        [Test]
        public void ProcessorCallWithOkStockCodes()
        {
            string response = _processor.ProcessMessage("/stock=aapl.us");
            Assert.IsTrue(response.ToUpper().Contains("AAPL.US"));
            Assert.IsTrue(response.ToUpper().EndsWith("PER SHARE"));

            response = _processor.ProcessMessage("/stock=googl.us");
            Assert.IsTrue(response.ToUpper().Contains("GOOGL.US"));
            Assert.IsTrue(response.ToUpper().EndsWith("PER SHARE"));

            response = _processor.ProcessMessage("/stock=amzn.us");
            Assert.IsTrue(response.ToUpper().Contains("AMZN.US"));
            Assert.IsTrue(response.ToUpper().EndsWith("PER SHARE"));
        }

        [Test]
        public void ProcessorCallWithNonExistentStockCodes()
        {
            string response = _processor.ProcessMessage("/stock=notvalid.us");
            Assert.AreEqual("NO DATA AVAILABLE FOR NOTVALID.US", response.ToUpper());

            response = _processor.ProcessMessage("/stock=unexistent.us");
            Assert.AreEqual("NO DATA AVAILABLE FOR UNEXISTENT.US", response.ToUpper());
        }

        [Test]
        public void ProcessorCallWithoutStockCode()
        {
            string response = _processor.ProcessMessage("/stock=");
            Assert.IsTrue(response.ToUpper().Contains("STOCK CODE IS REQUIRED"));
        }

        [Test]
        public void ProcessorCallInvalidCommand()
        {
            string response = _processor.ProcessMessage("/inval=");
            Assert.IsTrue(response.ToUpper().Contains("COMMAND NOT RECOGNIZED"));
        }

    }
}

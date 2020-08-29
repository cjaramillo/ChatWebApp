using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RPCComunicator;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitTestProject
{
    public class RpcClientTest
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = BuildConfiguration(TestContext.CurrentContext.TestDirectory);

        }

        [Test]
        public void RpcCallOnline()
        {
            // Be sure is runnning StockBotWorkerService
            // Otherwise, the response would never retreive
            RpcClient rpcClient = new RpcClient(_configuration);
            string response = rpcClient.Call("/stock=aapl.us");
            Assert.IsTrue(response.ToUpper().Contains("AAPL.US"));
        }

        [Test]
        public void RpcCallWithoutStockCode()
        {
            // Be sure is runnning StockBotWorkerService
            // Otherwise, the response would never retreive
            RpcClient rpcClient = new RpcClient(_configuration);
            string response = rpcClient.Call("/stock=");
            Assert.IsTrue(response.ToUpper().Contains("STOCK CODE IS REQUIRED"));
        }

        [Test]
        public void RpcCallInvalidCommand()
        {
            // Be sure is runnning StockBotWorkerService
            // Otherwise, the response would never retreive
            RpcClient rpcClient = new RpcClient(_configuration);
            string response = rpcClient.Call("/inval=");
            Assert.IsTrue(response.ToUpper().Contains("COMMAND NOT RECOGNIZED"));
        }

        public IConfigurationRoot BuildConfiguration(string testDirectory)
        {
            return new ConfigurationBuilder()
                .SetBasePath(testDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }

    }
}

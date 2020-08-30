using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using RPCComunicator;
using StockBotWorkerService;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitTestProject
{
    public class RpcClientTest : UnitTestConfig
    {
        private RpcClient _rpcClient;

        [OneTimeSetUp]
        public override void Setup()
        {
            _configuration = BuildConfiguration(TestContext.CurrentContext.TestDirectory);
            _rpcClient = new RpcClient(_configuration);

            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                }).Build().RunAsync();
        }

        [Test]
        public void RpcCallWithOkStockCodes()
        {
            string response = _rpcClient.Call("/stock=aapl.us");
            Assert.IsTrue(response.ToUpper().Contains("AAPL.US"));
            Assert.IsTrue(response.ToUpper().EndsWith("PER SHARE"));

            response = _rpcClient.Call("/stock=googl.us");
            Assert.IsTrue(response.ToUpper().Contains("GOOGL.US"));
            Assert.IsTrue(response.ToUpper().EndsWith("PER SHARE"));

            response = _rpcClient.Call("/stock=amzn.us");
            Assert.IsTrue(response.ToUpper().Contains("AMZN.US"));
            Assert.IsTrue(response.ToUpper().EndsWith("PER SHARE"));
        }

        [Test]
        public void RpcCallWithNonExistentStockCodes()
        {
            string response = _rpcClient.Call("/stock=notvalid.us");
            Assert.AreEqual("NO DATA AVAILABLE FOR NOTVALID.US", response.ToUpper());
            
            response = _rpcClient.Call("/stock=unexistent.us");
            Assert.AreEqual("NO DATA AVAILABLE FOR UNEXISTENT.US", response.ToUpper());
        }

        [Test]
        public void RpcCallWithoutStockCode()
        {
            string response = _rpcClient.Call("/stock=");
            Assert.IsTrue(response.ToUpper().Contains("STOCK CODE IS REQUIRED"));
        }

        [Test]
        public void RpcCallInvalidCommand()
        {
            string response = _rpcClient.Call("/inval=");
            Assert.IsTrue(response.ToUpper().Contains("COMMAND NOT RECOGNIZED"));
        }
    }
}

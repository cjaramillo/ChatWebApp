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
        private IConfiguration _configuration;
        private RpcClient _rpcClient;

        [OneTimeSetUp]
        public void Setup()
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
        public void RpcCallOnline()
        {
            string response = _rpcClient.Call("/stock=aapl.us");
            Assert.IsTrue(response.ToUpper().Contains("AAPL.US"));
            Assert.IsTrue(response.ToUpper().EndsWith("PER SHARE"));
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

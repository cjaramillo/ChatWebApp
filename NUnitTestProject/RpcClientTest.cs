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
    public class RpcClientTest
    {
        private IConfiguration _configuration;

        [OneTimeSetUp]
        public void Setup()
        {
            _configuration = BuildConfiguration(TestContext.CurrentContext.TestDirectory);
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                }).Build().RunAsync();
        }

        [Test]
        public void RpcCallOnline()
        {
            RpcClient rpcClient = new RpcClient(_configuration);
            string response = rpcClient.Call("/stock=aapl.us");
            Assert.IsTrue(response.ToUpper().Contains("AAPL.US"));
            Assert.IsTrue(response.ToUpper().EndsWith("PER SHARE"));
        }

        [Test]
        public void RpcCallWithoutStockCode()
        {
            RpcClient rpcClient = new RpcClient(_configuration);
            string response = rpcClient.Call("/stock=");
            Assert.IsTrue(response.ToUpper().Contains("STOCK CODE IS REQUIRED"));
        }

        [Test]
        public void RpcCallInvalidCommand()
        {
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

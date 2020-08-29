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
            RpcClient rpcClient = new RpcClient(_configuration);
            string response = rpcClient.Call("/stock=aapl.us");
            Assert.IsTrue(response.ToUpper().Contains("AAPL.US"));
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

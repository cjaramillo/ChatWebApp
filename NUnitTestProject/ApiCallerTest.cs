using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using StockBotWorkerService;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitTestProject
{
    class ApiCallerTest
    {
        private IConfiguration _configuration;
        private ApiCaller _apiCaller;

        [SetUp]
        public void Setup()
        {
            _configuration = BuildConfiguration(TestContext.CurrentContext.TestDirectory);
            _apiCaller = new ApiCaller(_configuration);
        }

        [Test]
        public void ApiCallWithOkStockCode()
        {
            string response = _apiCaller.Call(stockCode: "aapl.us");
            if (string.IsNullOrEmpty(response))
                Assert.Fail();
            Assert.Pass();
        }

        [Test]
        public void ApiCallWithNonExistentStockCode()
        {
            string response = _apiCaller.Call(stockCode: "nonexist.code");
            if (string.IsNullOrEmpty(response))
                Assert.Fail();
            Assert.Pass();
        }

        [Test]
        public void ApiCallWithoutStockCode()
        {
            string response = _apiCaller.Call(stockCode: string.Empty);
            if (string.IsNullOrEmpty(response))
                Assert.Fail();
            Assert.Pass();
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

using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitTestProject
{
    public partial class UnitTestConfig
    {
        protected IConfiguration _configuration;

        [OneTimeSetUp]
        public virtual void Setup()
        {
            _configuration = BuildConfiguration(TestContext.CurrentContext.TestDirectory);
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

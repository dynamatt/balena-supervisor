namespace Balena.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Balena.Cloud;
    using Balena.Models;
    using NUnit.Framework;

    [TestFixture]
    public class CloudTests
    {
        private static readonly string ApiKey = Environment.GetEnvironmentVariable("BALENA_API_TOKEN");
        private const string Url = @"https://api.balena-cloud.com/v5";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestApplicationSearch()
        {
            var connection = new BalenaCloudConnection(Url, ApiKey);
            List<Application> applications = await connection.GetAllApplications();
            Assert.That(applications, Is.Not.Empty);
            foreach (var application in applications)
            {
                TestContext.WriteLine(application);
            }

            int id = applications[0].Id;
            string name = applications[0].AppName;
            Assert.That(id, Is.Not.EqualTo(0));

            Application applicationIdResult = await connection.GetApplicationById(id);
            Assert.That(applicationIdResult.AppName, Is.EqualTo(name));
        }
    }
}
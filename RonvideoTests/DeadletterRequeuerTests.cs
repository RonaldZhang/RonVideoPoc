using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using RonVideo;
using RonVideo.Utilities;
using RonvideoTests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class DeadletterRequeuerTests
    {
        private readonly ILogger logger = NullLoggerFactory.Instance.CreateLogger("DeadletterRequeuerTests");

        readonly DeadletterRequeuer _sut;
        //private DeadletterRequeuer _sut;

        public DeadletterRequeuerTests()
        {
            var startup = new UnitTestStartup();
            var host = new HostBuilder()
                .ConfigureWebJobs(startup.Configure)
                .Build();

            _sut = new DeadletterRequeuer(host.Services.GetRequiredService<ICloudQueueManager>());
        }

        [TestMethod]
        public async Task EmptyCloudQueueTest()
        {
            string payload = "I am testing payload";
            Mock<HttpRequest> mockRequest = MockHelper.CreateMockRequest(payload);
            string queueName = "myQueueName";

            //Mock CloudQueue
            var mockCloudQueueClient = new Mock<CloudQueueClient>();
           //mockCloudQueueClient.Setup(x=>x.GetQueueReference(It.IsAny<string>())).Returns(new CloudQueue)

            var result=await _sut.Run(mockRequest.Object, queueName, logger);
            Assert.IsNotNull(result);
            OkObjectResult okRes = result as OkObjectResult;
            Assert.IsNotNull(okRes);
            Assert.AreEqual("Reprocessed 0 messages from the local queue.", okRes.Value);
        }

        [TestMethod]
        public async Task ThreePoisonedMessageCloudQueueTest()
        {
            string payload = "I am testing payload";
            Mock<HttpRequest> mockRequest = MockHelper.CreateMockRequest(payload);
            string queueName = "myBadQueue";

            //Mock CloudQueue
            var mockCloudQueueClient = new Mock<CloudQueueClient>();
            //mockCloudQueueClient.Setup(x=>x.GetQueueReference(It.IsAny<string>())).Returns(new CloudQueue)


            var result = await _sut.Run(mockRequest.Object, queueName, logger);
            Assert.IsNotNull(result);
            OkObjectResult okRes = result as OkObjectResult;
            Assert.IsNotNull(okRes);
            Assert.AreEqual("Reprocessed 3 messages from the local queue.", okRes.Value);
        }
    }
}

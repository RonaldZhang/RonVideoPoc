using BonsReceiver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using RonVideo.Utilities;
using RonvideoTests.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class BonsEventReceiverTests:FunctionTester
    {
        private readonly ILogger logger = NullLoggerFactory.Instance.CreateLogger("Test");


        [TestMethod]
        public async Task BonEventReceiverSuccess()
        {
            string payload = "I am testing payload";
            Mock<HttpRequest> mockRequest = MockHelper.CreateMockRequest(payload);

            Mock<ICollector<string>> outputQueueItem = MockHelper.CreateMockCollector();
            IKeyVaultManager kvm = new LocalKVManager();
            var func = new BonsEventReceiver(kvm);
            var result = await func.Run(mockRequest.Object, outputQueueItem.Object, new Mock<ILogger>().Object) as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Event Received", result.Value);
            
        }

        [TestMethod]
        public async Task Request_With_Query()
        {
            var query = new Dictionary<String, StringValues>();
            //query.TryAdd("name", "ushio");
            var body = "{\"name\":\"yamada\"}";

            Mock<ICollector<string>> outputQueueItem = MockHelper.CreateMockCollector();
            IKeyVaultManager kvm = new LocalKVManager();
            var func = new BonsEventReceiver(kvm);
            var result = await func.Run(req: HttpRequestSetup(query, body), outputQueueItem.Object, new Mock<ILogger>().Object) as OkObjectResult;

            //var result = await HttpTrigger.RunAsync(req: HttpRequestSetup(query, body), log: log);
            var resultObject = (OkObjectResult)result;
            Assert.AreEqual("Event Received", resultObject.Value);

        }


    }

}

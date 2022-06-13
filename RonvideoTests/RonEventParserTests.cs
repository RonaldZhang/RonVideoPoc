using BonsReceiver;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RonVideo.Models;
using RonVideo.Utilities;
using RonvideoTests.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class RonEventParserTests : FunctionTester
    {
        private readonly ILogger logger = NullLoggerFactory.Instance.CreateLogger("Test");

        [TestMethod]
        public void RonEventParserSuccess()
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string msg = File.ReadAllText(currentDirectory+@"\data\event.json");
            // Mock<HttpRequest> mockRequest = CreateMockRequest(payload);

            //Mock<ICollector<string>> outputQueueItem = MockHelper.CreateMockCollector();
            MockCollector<string> outputQueueItem = new MockCollector<string>();
            IKeyVaultManager kvm = new LocalKVManager();
            var func = new RonEventParser();// BonsEventReceiver(kvm);
            func.Run(msg, outputQueueItem, new Mock<ILogger>().Object);


            Assert.IsNotNull(outputQueueItem);
            var abc = JsonConvert.DeserializeObject<VideoQueueItem>( outputQueueItem.Items[0]);
            Assert.AreEqual("f192e61f-7f40-4d37-be19-e5a92da8c5f8", abc.BlendId);
            Assert.AreEqual("c8dbb6ca-72b4-41eb-815a-e398995840f9", abc.CloseId);
            Assert.AreEqual("Test1", abc.FileId);
            Assert.AreEqual("", abc.LoanId);
        }

        //[TestMethod]
        //public void Recieve_Queue_And_Emit_To_Table()
        //{
        //    var col = new MockCollector<string>();
        //    var json = "{\"name\": \"ushio\"}";

        //    IKeyVaultManager kvm = new LocalKVManager();
        //    var func = new RonEventParser();

        //    func.Run(json, col, new Mock<ILogger>().Object);
        //    Assert.AreEqual(json,"123");

        //}
    }
}

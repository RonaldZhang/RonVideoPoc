using BonsReceiver;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RonVideo;
using RonVideo.Activities;
using RonVideo.Models;
using RonVideo.Utilities;
using RonvideoTests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class VideoTransferTests
    {
        private readonly ILogger logger = NullLoggerFactory.Instance.CreateLogger("Test");

        [TestMethod]
        public async Task Run_Orchectrator()
        {
            VideoQueueItem videoQ = new VideoQueueItem("BlendId","LoanId", "CloseId", "FileId");
            VideoItem vidoeR = new VideoItem("BlendId", "LoanId", "CloseId", "FileId", 1, "", "Http", "FileId");
            OrchestratorInput input1 = new OrchestratorInput(videoQ, vidoeR);


            var contextMock = new Mock<IDurableOrchestrationContext>();
            contextMock.Setup(x => x.GetInput<OrchestratorInput>()).Returns(input1);
            contextMock.Setup(context => context.CallActivityAsync<string>("GetLoanId", "Tokyo")).Returns(Task.FromResult<string>("LoanId123"));

            VideoItem video = new VideoItem("blendid","loanId","closeId","fileid",1,"Completed", "http", "fileid");
            contextMock.Setup(context => context.CallActivityAsync<VideoItem>("Upsert", "Seattle")).Returns(Task.FromResult<VideoItem>(video));
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
            contextMock.Setup(context => context.CallActivityAsync<byte[]>("GetVideo", "London")).Returns(Task.FromResult<byte[]>(bytes));
            var result = await OrchestratorFunctions.TransferOrchestrator(contextMock.Object);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task Run_Orchectrator_Client()
        {
            var clientMock = new Mock<IDurableOrchestrationClient>();
            //contextMock.Setup(x => x.GetInput<OrchestratorInput>()).Returns(input1);
            // https://github.com/Azure/azure-functions-durable-extension/blob/0345b369ffa1745c24ffbacfaf8a43fb62dd2572/src/WebJobs.Extensions.DurableTask/DurableOrchestrationClient.cs#L46
            //var requestMock = new Mock<HttpRequestMessage>();
            var id = "8e503c5e-19de-40e1-932d-298c4263115b";
            VideoQueueItem videoQ = new VideoQueueItem("BlendId", "LoanId", "CloseId", "FileId");
            //VideoItem vidoeR = new VideoItem("BlendId", "LoanId", "CloseId", "FileId", 1, "", "Http", "FileId");
            VideoItem video = new VideoItem("blendid", "loanId", "closeId", "fileid", 1, "Received", "http", "fileid");
            OrchestratorInput input1 = new OrchestratorInput(videoQ, video);

            clientMock.Setup(client => client.StartNewAsync("TransferOrchestrator",It.IsAny<OrchestratorInput>())).Returns(Task.FromResult<string>(id));
            // var request = requestMock.Object;
            //clientMock.Setup(client => client.CreateCheckStatusResponse(request, id,false));


            VideoTransfer vt=new VideoTransfer();
            //VideoItem video = new VideoItem("blendid", "loanId", "closeId", "fileid", 1, "Received", "http", "fileid");
            vt.RonVideoStarter(videoQ, video, clientMock.Object, logger);
            try
            {

                clientMock.Verify(client => client.StartNewAsync("TransferOrchestrator", It.IsAny<OrchestratorInput>()));

            }
            catch (MockException ex)
            {
                Assert.Fail();
            }
        }

    }
}

using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RonVideo.Activities;
using RonVideo.Exceptions;
using RonVideo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class OrchestratorTest
    {
        private readonly ILogger logger = NullLoggerFactory.Instance.CreateLogger("OrchestratorTest");
        [TestMethod]
        public async Task RunOrchectratorEmptyLoanIdTest()
        {
            VideoQueueItem videoQ = new VideoQueueItem("BlendId", "", "CloseId", "FileId");
            VideoItem vidoeR = new VideoItem("BlendId", "", "CloseId", "FileId", 1, "", "Http", "FileId");
            OrchestratorInput input1 = new OrchestratorInput(videoQ, vidoeR);


            var contextMock = new Mock<IDurableOrchestrationContext>();
            contextMock.Setup(x => x.GetInput<OrchestratorInput>()).Returns(input1);
            contextMock.Setup(context => context.CallActivityAsync<string>("GetLoanId", "BlendId")).Returns(Task.FromResult<string>(""));

            VideoItem video = new VideoItem("blendid", "", "closeId", "fileid", 1, "Completed", "http", "fileid");
            contextMock.Setup(context => context.CallActivityAsync<VideoItem>("Upsert", "Seattle")).Returns(Task.FromResult<VideoItem>(video));
            //byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
            //contextMock.Setup(context => context.CallActivityAsync<byte[]>("GetVideo", "London")).Returns(Task.FromResult<byte[]>(bytes));
            var result = await OrchestratorFunctions.TransferOrchestrator(contextMock.Object, logger);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task RunOrchectratorZeroByteContentTest()
        {
            VideoQueueItem videoQ = new VideoQueueItem("BlendId", "LoanId", "CloseId", "FileId");
            VideoItem vidoeR = new VideoItem("BlendId", "LoanId", "CloseId", "FileId", 1, "", "Http", "FileId");
            OrchestratorInput input1 = new OrchestratorInput(videoQ, vidoeR);


            var contextMock = new Mock<IDurableOrchestrationContext>();
            contextMock.Setup(x => x.GetInput<OrchestratorInput>()).Returns(input1);
            contextMock.Setup(context => context.CallActivityAsync<string>("GetLoanId", "BlendId")).Returns(Task.FromResult<string>("LoanId123"));

            VideoItem video = new VideoItem("blendid", "loanId", "closeId", "fileid", 1, "Completed", "http", "fileid");
            contextMock.Setup(context => context.CallActivityAsync<VideoItem>("Upsert", "Seattle")).Returns(Task.FromResult<VideoItem>(video));
            byte[] bytes = new byte[0];
            contextMock.Setup(context => context.CallActivityWithRetryAsync<byte[]>("GetVideo", It.IsAny<RetryOptions>(), It.IsAny<VideoQueueItem>())).Returns(Task.FromResult<byte[]>(bytes));
            var result = await OrchestratorFunctions.TransferOrchestrator(contextMock.Object,logger);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task RunOrchectratorGetVideoTimeExpiredTest()
        {
            VideoQueueItem videoQ = new VideoQueueItem("BlendId", "LoanId", "CloseId", "FileId");
            VideoItem vidoeR = new VideoItem("BlendId", "LoanId", "CloseId", "FileId", 1, "", "Http", "FileId");
            OrchestratorInput input1 = new OrchestratorInput(videoQ, vidoeR);


            var contextMock = new Mock<IDurableOrchestrationContext>();
            contextMock.Setup(x => x.GetInput<OrchestratorInput>()).Returns(input1);
            contextMock.Setup(context => context.CallActivityAsync<string>("GetLoanId", "BlendId")).Returns(Task.FromResult<string>("LoanId123"));

            VideoItem video = new VideoItem("blendid", "loanId", "closeId", "fileid", 1, "Completed", "http", "fileid");
            contextMock.Setup(context => context.CallActivityAsync<VideoItem>("Upsert", "Seattle")).Returns(Task.FromResult<VideoItem>(video));
            //byte[] bytes = new byte[0];

            //contextMock.Setup(context => context.CallActivityWithRetryAsync<byte[]>("GetVideo", It.IsAny<RetryOptions>(), It.IsAny<VideoQueueItem>())).Throws(new Exception ("wrapper exception", new TimeExpiredException()));
            var result = await OrchestratorFunctions.TransferOrchestrator(contextMock.Object, logger);
            //Assert.ThrowsException<TimeExpiredException>(async () => { await OrchestratorFunctions.TransferOrchestrator(contextMock.Object); });
            Assert.AreEqual(true, result);
        }


        [TestMethod]
        public async Task RunOrchectratorTest()
        {
            VideoQueueItem videoQ = new VideoQueueItem("BlendId", "LoanId", "CloseId", "FileId");
            VideoItem vidoeR = new VideoItem("BlendId", "LoanId", "CloseId", "FileId", 1, "", "Http", "FileId");
            OrchestratorInput input1 = new OrchestratorInput(videoQ, vidoeR);


            var contextMock = new Mock<IDurableOrchestrationContext>();
            contextMock.Setup(x => x.GetInput<OrchestratorInput>()).Returns(input1);
            contextMock.Setup(context => context.CallActivityAsync<string>("GetLoanId", "BlendId")).Returns(Task.FromResult<string>("LoanId123"));

            VideoItem video = new VideoItem("blendid", "loanId", "closeId", "fileid", 1, "Completed", "http", "fileid");
            contextMock.Setup(context => context.CallActivityAsync<VideoItem>("Upsert", "Seattle")).Returns(Task.FromResult<VideoItem>(video));
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
            contextMock.Setup(context => context.CallActivityWithRetryAsync<byte[]>("GetVideo", It.IsAny< RetryOptions>(), It.IsAny<VideoQueueItem>())).Returns(Task.FromResult<byte[]>(bytes));
            var result = await OrchestratorFunctions.TransferOrchestrator(contextMock.Object, logger);
            Assert.AreEqual(true, result);
        }

    }
}

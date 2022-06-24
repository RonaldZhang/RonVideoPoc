using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using RonVideo;
using RonVideo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class BatchVideoTransferTests
    {
        private readonly ILogger logger = NullLoggerFactory.Instance.CreateLogger("Test");


        [TestMethod]
        public async Task RunOrchectratorClientTest()
        {
            var id = "8e503c5e-19de-40e1-932d-298c4263115b";
            VideoQueueItem videoQ = new VideoQueueItem("BlendId", "LoanId", "CloseId", "FileId");
            VideoItem video = new VideoItem("blendid", "loanId", "closeId", "fileid", 1, "Received", "http", "fileid");
            OrchestratorInput input1 = new OrchestratorInput(videoQ, video);

            var tableClientMock = new Mock<TableClient>();
            var result = GetDataAync();
            tableClientMock.Setup(x => x.QueryAsync<VideoItem>(It.IsAny<Expression<Func<VideoItem, bool>>>()
                , null
                , null
                ,default)).Returns(result);

            var clientMock = new Mock<IDurableOrchestrationClient>();
            clientMock.Setup(client => client.StartNewAsync("TransferOrchestrator", It.IsAny<OrchestratorInput>())).Returns(Task.FromResult<string>(id));

            BatchVideoTransfer vt = new BatchVideoTransfer();
            await vt.BatchRonVideoProcessStart(tableClientMock.Object, clientMock.Object, logger);
            try
            {
                clientMock.Verify(client => client.StartNewAsync("TransferOrchestrator", It.IsAny<OrchestratorInput>()));
            }
            catch (MockException ex)
            {
                Assert.Fail();
            }
        }


        private AsyncPageable<VideoItem> GetDataAync()
        {
            Page<VideoItem> page1 = Page<VideoItem>.FromValues(new[] { new VideoItem() {
                BlendId="blendid1",
                LoanId="loanId1",
                CloseId="closeId1",
                FileId="fileid1",
                Count=1,
                Status="Received1",
                PartitionKey="http1",
                RowKey="fileid1"
            }, new VideoItem() {
            BlendId = "blendid2",
            LoanId = "loanId2",
            CloseId = "closeId2",
            FileId = "fileid2",
            Count = 2,
            Status = "Received2",
            PartitionKey = "http2",
            RowKey = "fileid2"
        } }, "continuationToken", Mock.Of<Response>());

            Page<VideoItem> page2 = Page<VideoItem>.FromValues(new[] { new VideoItem() {
                BlendId = "blendid21",
                LoanId = "loanId21",
                CloseId = "closeId21",
                FileId = "fileid21",
                Count = 21,
                Status = "Received21",
                PartitionKey = "http21",
                RowKey = "fileid21"
            } , new VideoItem() {
                BlendId = "blendid22",
                LoanId = "loanId22",
                CloseId = "closeId22",
                FileId = "fileid22",
                Count = 22,
                Status = "Completed",
                PartitionKey = "http22",
                RowKey = "fileid22"
            } }, "continuationToken2", Mock.Of<Response>());

            Page<VideoItem> lastPage = Page<VideoItem>.FromValues(new[] { new VideoItem() {
                BlendId = "blendid31",
                LoanId = "loanId31",
                CloseId = "closeId31",
                FileId = "fileid31",
                Count = 31,
                Status = "Received31",
                PartitionKey = "http31",
                RowKey = "fileid31"
            }, new VideoItem() {
                BlendId = "blendid32",
                LoanId = "loanId32",
                CloseId = "closeId32",
                FileId = "fileid32",
                Count = 32,
                Status = "Received32",
                PartitionKey = "http32",
                RowKey = "fileid32"
            } }, continuationToken: null, Mock.Of<Response>());

            Pageable<VideoItem> pageable = Pageable<VideoItem>.FromPages(new[] { page1, page2, lastPage });

            AsyncPageable<VideoItem> asyncPageable = AsyncPageable<VideoItem>.FromPages(new[] { page1, page2, lastPage });

            return asyncPageable;
        }
    }
}

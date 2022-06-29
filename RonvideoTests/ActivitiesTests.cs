using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using RichardSzalay.MockHttp;
using RonVideo.Activities;
using RonVideo.Exceptions;
using RonVideo.Models;
using RonVideo.Utilities;
using RonvideoTests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class ActivitiesTests
    {
        private readonly ILogger logger = NullLoggerFactory.Instance.CreateLogger("Test");
        //private readonly ITestOutputHelper output;
        //private Mock<MockHttpMessageHandler> mockHttpMessageHandler;
        private HttpRequestMessage request;

        //[TestMethod]
        //public async Task Get_LoanId_Activity()
        //{
        //    var result = await ActivityFunctions.GetLoanId("Test", logger);
        //    Assert.AreEqual("ABC", result);
        //}

        [TestMethod]
        public async Task  GetLoanIdActivityOkTests()
        {
           var mockHttpMessageHandler = new Mock<HttpMessageHandler> { CallBase = true };
            //mockHttpMessageHandler = new Mock<MockHttpMessageHandler>( MockBehavior.Strict);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"LoanId\":\"ABD\",\"Id\":\"DEF\"}")
                })            
                .Verifiable();
            //.Callback<HttpRequestMessage>(x => this.request = x);
            var obj = mockHttpMessageHandler.Object;
            var client = new HttpClient(obj)
            {
                BaseAddress = new Uri("http://test.com/")
            };
            ActivityFunctions.client = client;

            string resp=await ActivityFunctions.GetLoanId("blendId1", logger);
            Assert.AreEqual("ABD", resp);

        }

        [TestMethod]
        public async Task GetLoanIdActivityFailedTests()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler> { CallBase = true };
            //mockHttpMessageHandler = new Mock<MockHttpMessageHandler>( MockBehavior.Strict);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent("")
                })
                .Verifiable();
            //.Callback<HttpRequestMessage>(x => this.request = x);
            var obj = mockHttpMessageHandler.Object;
            var client = new HttpClient(obj)
            {
                BaseAddress = new Uri("http://test.com/")
            };
            ActivityFunctions.client = client;

            string resp = await ActivityFunctions.GetLoanId("blendId1", logger);
            Assert.AreEqual("", resp);

        }

        [TestMethod]
        public async Task GetDownloadUrlActivityOkTests()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler> { CallBase = true };
            //mockHttpMessageHandler = new Mock<MockHttpMessageHandler>( MockBehavior.Strict);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"downloadUrl\":\"http://abc.mysite.com/dload\",\"expiresAt\":\"ABCDEF\"}")
                })
                .Verifiable();
            //.Callback<HttpRequestMessage>(x => this.request = x);
            var obj = mockHttpMessageHandler.Object;
            var client = new HttpClient(obj)
            {
                BaseAddress = new Uri("http://test.com/")
            };
            ActivityFunctions.client = client;

            var resp = await ActivityFunctions.IntGetDownloadUrl("blendId1","123", logger);
            Assert.AreEqual("http://abc.mysite.com/dload", resp);

        }

        [TestMethod]
        public async Task GetDownloadUrlActivityFailedTests()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler> { CallBase = true };
            //mockHttpMessageHandler = new Mock<MockHttpMessageHandler>( MockBehavior.Strict);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent("")
                })
                .Verifiable();
            //.Callback<HttpRequestMessage>(x => this.request = x);
            var obj = mockHttpMessageHandler.Object;
            var client = new HttpClient(obj)
            {
                BaseAddress = new Uri("http://test.com/")
            };
            ActivityFunctions.client = client;

            var resp = await ActivityFunctions.IntGetDownloadUrl("blendId1", "123", logger);
            Assert.AreEqual("", resp);

        }

        [TestMethod]
        public async Task IntGetVideoOkTests()
        {
            //string txt= "This is a test";
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
            var mockHttpMessageHandler = new Mock<HttpMessageHandler> { CallBase = true };
            //mockHttpMessageHandler = new Mock<MockHttpMessageHandler>( MockBehavior.Strict);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => !x.RequestUri.AbsoluteUri.Contains("http://abc.mysite.com/dload")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"downloadUrl\":\"http://abc.mysite.com/dload\",\"expiresAt\":\"ABCDEF\"}")

                })
                .Verifiable();
            mockHttpMessageHandler.Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.AbsoluteUri.Contains("http://abc.mysite.com/dload")),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = System.Net.HttpStatusCode.OK,
                   Content = new ByteArrayContent(bytes)
               })
               .Verifiable();
            //.Callback<HttpRequestMessage>(x => this.request = x);
            var obj = mockHttpMessageHandler.Object;
            var client = new HttpClient(obj)
            {
                BaseAddress = new Uri("http://test.com/")
            };
            ActivityFunctions.client = client;

            VideoQueueItem qItem = new VideoQueueItem();
            qItem.BlendId = "blendId1";
            qItem.CloseId = "closeId1";
            qItem.FileId = "fileId1";
            qItem.LoanId = "loanId1";
 
            var resp = await ActivityFunctions.GetVideo(qItem, logger);
            //var res = Encoding.Default.GetBytes(txt);
            Assert.IsTrue(bytes.SequenceEqual(resp));

        }


        [TestMethod]
        public async Task IntGetVideoFailedTests()
        {
            //string txt= "This is a test";
            byte[] bytes = new byte[0] ;
            var mockHttpMessageHandler = new Mock<HttpMessageHandler> { CallBase = true };
            //mockHttpMessageHandler = new Mock<MockHttpMessageHandler>( MockBehavior.Strict);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => !x.RequestUri.AbsoluteUri.Contains("http://abc.mysite.com/dload")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"downloadUrl\":\"http://abc.mysite.com/dload\",\"expiresAt\":\"ABCDEF\"}")

                })
                .Verifiable();
            mockHttpMessageHandler.Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.AbsoluteUri.Contains("http://abc.mysite.com/dload")),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = System.Net.HttpStatusCode.BadRequest,
                   Content = new ByteArrayContent(bytes)
               })
               .Verifiable();
            //.Callback<HttpRequestMessage>(x => this.request = x);
            var obj = mockHttpMessageHandler.Object;
            var client = new HttpClient(obj)
            {
                BaseAddress = new Uri("http://test.com/")
            };
            ActivityFunctions.client = client;

            VideoQueueItem qItem = new VideoQueueItem();
            qItem.BlendId = "blendId1";
            qItem.CloseId = "closeId1";
            qItem.FileId = "fileId1";
            qItem.LoanId = "loanId1";

            var resp = await ActivityFunctions.GetVideo(qItem, logger);
            //var res = Encoding.Default.GetBytes(txt);
            Assert.IsTrue(bytes.SequenceEqual(resp));

        }


        [TestMethod]
        public async Task GetVideoOkTests()
        {
            //string txt= "This is a test";
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
            var mockHttpMessageHandler = new Mock<HttpMessageHandler> { CallBase = true };
            //mockHttpMessageHandler = new Mock<MockHttpMessageHandler>( MockBehavior.Strict);
          
            mockHttpMessageHandler.Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   //ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.AbsoluteUri.Contains("http://abc.mysite.com/dload")),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = System.Net.HttpStatusCode.OK,
                   Content = new ByteArrayContent(bytes)
               })
               .Verifiable();
            //.Callback<HttpRequestMessage>(x => this.request = x);
            var obj = mockHttpMessageHandler.Object;
            var client = new HttpClient(obj)
            {
                BaseAddress = new Uri("http://test.com/")
            };
            ActivityFunctions.client = client;

            //VideoQueueItem qItem = new VideoQueueItem();
            //qItem.BlendId = "blendId1";
            //qItem.CloseId = "closeId1";
            //qItem.FileId = "fileId1";
            //qItem.LoanId = "loanId1";

            var resp = await ActivityFunctions.IntGetVideo("http://abc.mysite.com/dload", logger);
            //var res = Encoding.Default.GetBytes(txt);
            Assert.IsTrue(bytes.SequenceEqual(resp.bytes));

        }


        [TestMethod]
        public async Task GetIntVideoGoneTests()
        {
            //string txt= "This is a test";
            byte[] bytes = new byte[0];
            var mockHttpMessageHandler = new Mock<HttpMessageHandler> { CallBase = true };
            //mockHttpMessageHandler = new Mock<MockHttpMessageHandler>( MockBehavior.Strict);

            mockHttpMessageHandler.Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   //ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.AbsoluteUri.Contains("http://abc.mysite.com/dload")),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = System.Net.HttpStatusCode.Gone,
                   Content = new ByteArrayContent(bytes)
               })
               .Verifiable();
            //.Callback<HttpRequestMessage>(x => this.request = x);
            var obj = mockHttpMessageHandler.Object;
            var client = new HttpClient(obj)
            {
                BaseAddress = new Uri("http://test.com/")
            };
            ActivityFunctions.client = client;


            var resp = await ActivityFunctions.IntGetVideo("http://abc.mysite.com/dload", logger);
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.bytes.SequenceEqual(bytes));
            Assert.IsTrue(resp.HttpStatus==HttpStatusCode.Gone);
           
        }

        [TestMethod]
        public async Task GetVideoGoneTests()
        {
            //string txt= "This is a test";
            byte[] bytes = new byte[0];
            var mockHttpMessageHandler = new Mock<HttpMessageHandler> { CallBase = true };
            //mockHttpMessageHandler = new Mock<MockHttpMessageHandler>( MockBehavior.Strict);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => !x.RequestUri.AbsoluteUri.Contains("http://abc.mysite.com/dload")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"downloadUrl\":\"http://abc.mysite.com/dload\",\"expiresAt\":\"ABCDEF\"}")

                })
                .Verifiable();
            mockHttpMessageHandler.Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.AbsoluteUri.Contains("http://abc.mysite.com/dload")),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = System.Net.HttpStatusCode.Gone,
                   Content = new ByteArrayContent(bytes)
               })
               .Verifiable();
            //.Callback<HttpRequestMessage>(x => this.request = x);
            var obj = mockHttpMessageHandler.Object;
            var client = new HttpClient(obj)
            {
                BaseAddress = new Uri("http://test.com/")
            };
            ActivityFunctions.client = client;

            VideoQueueItem qItem = new VideoQueueItem();
            qItem.BlendId = "blendId1";
            qItem.CloseId = "closeId1";
            qItem.FileId = "fileId1";
            qItem.LoanId = "loanId1";

            //var resp = await ActivityFunctions.GetVideo(qItem, logger);
            //var res = Encoding.Default.GetBytes(txt);
            //Assert.IsTrue(bytes.SequenceEqual(resp));
            await Assert.ThrowsExceptionAsync<TimeExpiredException>(async () => { var resp = await ActivityFunctions.GetVideo(qItem, logger); });
      
        }

        [TestMethod]
        public async Task UpsertETagUpdatedTest()
        {
            VideoItem videoRow = new VideoItem("BlendId1", "LoanId1", "CloseId1", "FileId1",
                3, "Status", "PartitionKey1", "RowKey1");


            VideoQueueItem vQueueItem = new VideoQueueItem()
            {
                BlendId = videoRow.BlendId,
                LoanId = videoRow.LoanId,
                CloseId = videoRow.CloseId,
                FileId = videoRow.FileId
            };

            string status = "video file download unsuccessful";

            var vv = (videoRow, vQueueItem, status);
            var ret = await ActivityFunctions.Upsert(vv, logger);

            //vidoe already exists
            Assert.IsTrue(ret.ETag.ToString() == "*");


        }


        [TestMethod]
        public async Task UpsertETagNotUpdatedTest()
        {
            VideoItem videoRow = null;

            VideoQueueItem vQueueItem = new VideoQueueItem()
            {
                BlendId = "BlendId2",
                LoanId = "LoanId2",
                CloseId = "CloseId2",
                FileId = "FileId2",
            };

           
            string status = "video file download unsuccessful";

            var vv = (videoRow, vQueueItem, status);
            var ret = await ActivityFunctions.Upsert(vv, logger);

            //vidoe already exists
            Assert.IsTrue(ret.ETag.ToString() == "");
            Assert.IsTrue(ret.Count == 1);


        }
    }
}

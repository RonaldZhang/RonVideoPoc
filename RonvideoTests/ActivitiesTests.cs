using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using RichardSzalay.MockHttp;
using RonVideo.Activities;
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

        [TestMethod]
        public async Task Get_LoanId_Activity()
        {
            var result = await ActivityFunctions.GetLoanId("Test", logger);
            Assert.AreEqual("ABC", result);
        }


        public ActivitiesTests()
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
                    Content = new StringContent("{\"LoanId\":\"ABC\",\"Id\":\"DEF\"}")
                })            
                .Verifiable();
            //.Callback<HttpRequestMessage>(x => this.request = x);
            var obj = mockHttpMessageHandler.Object;
            var client = new HttpClient(obj)
            {
                BaseAddress = new Uri("http://test.com/")
            };
            ActivityFunctions.client = client;
        }
    }
}

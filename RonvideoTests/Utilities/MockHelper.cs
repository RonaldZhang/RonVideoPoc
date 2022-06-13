using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests.Utilities
{
    public class MockHelper
    {
        public static Mock<ICollector<string>> CreateMockCollector()
        {
            var mockCollector = new Mock<ICollector<string>>();
            //mockCollector.Setup(x => x.Add).Returns(ms);

            return mockCollector;
        }

        public static Mock<HttpRequest> CreateMockRequest(object body)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }
    }
}

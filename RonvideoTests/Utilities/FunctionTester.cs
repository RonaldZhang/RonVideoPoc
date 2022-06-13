using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests.Utilities
{
    public abstract class FunctionTester 
    {
        //protected TraceWriter log = new DebuggerTraceWriter();

        public HttpRequest HttpRequestSetup(Dictionary<String, StringValues> query, string body)
        {
            var reqMock = new Mock<HttpRequest>();

            reqMock.Setup(req => req.Query).Returns(new QueryCollection(query));
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(body);
            writer.Flush();
            stream.Position = 0;
            reqMock.Setup(req => req.Body).Returns(stream);
            return reqMock.Object;
        } 
    }
}

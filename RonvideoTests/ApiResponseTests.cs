using Microsoft.VisualStudio.TestTools.UnitTesting;
using RonVideo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class ApiResponseTests
    {
        [TestMethod]
        public void Http404Test()
        {
            ApiResponse resp = new ApiResponse(404);
            Assert.AreEqual("Resource not found", resp.Message);
            Assert.AreEqual(404, resp.StatusCode);
        }

        [TestMethod]
        public void Http500Test()
        {
            ApiResponse resp = new ApiResponse(500);
            Assert.AreEqual("An unhandled error occurred", resp.Message);
            Assert.AreEqual(500, resp.StatusCode);
        }

        [TestMethod]
        public void SpecificMessageTest()
        {
            ApiResponse resp = new ApiResponse(500,"customized error");
            Assert.AreEqual("customized error", resp.Message);
            Assert.AreEqual(500, resp.StatusCode);
        }

        [TestMethod]
        public void Http200Test()
        {
            ApiResponse resp = new ApiResponse(200);
            Assert.AreEqual(null, resp.Message);
            Assert.AreEqual(200, resp.StatusCode);
        }  
    }
}

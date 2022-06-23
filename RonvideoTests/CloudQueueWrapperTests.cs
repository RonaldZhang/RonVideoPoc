using Microsoft.Azure.Storage.Queue;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RonVideo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class CloudQueueWrapperTests
    {
        [TestMethod]
        public  async Task  AddMessageTest()
        {
            var mockCloudQueue = new Mock<CloudQueue>(new Uri("http://dotnetperls.com/" + "MyQueue"));
            mockCloudQueue.Setup(x => x.AddMessageAsync(It.IsAny<CloudQueueMessage>())).Returns(Task.CompletedTask);
            var wrapper = new CloudQueueWrapper(mockCloudQueue.Object);

            CloudQueueMessage message = new CloudQueueMessage("Message1");
            await wrapper.AddMessageAsync(message);
            mockCloudQueue.Verify(x => x.AddMessageAsync(message), Times.Once());
        }

        [TestMethod]
        public async Task GetMessageTest()
        {
            var mockCloudQueue = new Mock<CloudQueue>(new Uri("http://dotnetperls.com/" + "MyQueue"));
            mockCloudQueue.Setup(x => x.GetMessageAsync()).Returns(Task.FromResult(new CloudQueueMessage("Message1")));
            var wrapper = new CloudQueueWrapper(mockCloudQueue.Object);

            //CloudQueueMessage message = new CloudQueueMessage("Message1");
            var ret=await wrapper.GetMessageAsync();
            Assert.IsTrue(ret.AsString.SequenceEqual("Message1"));
            mockCloudQueue.Verify(x => x.GetMessageAsync(), Times.Once());
        }

        [TestMethod]
        public async Task DeleteMessageTest()
        {
            var mockCloudQueue = new Mock<CloudQueue>(new Uri("http://dotnetperls.com/" + "MyQueue"));
            mockCloudQueue.Setup(x => x.DeleteMessageAsync(It.IsAny<CloudQueueMessage>())).Returns(Task.CompletedTask);
            var wrapper = new CloudQueueWrapper(mockCloudQueue.Object);

            await wrapper.DeleteMessageAsync("1", "Receipte1");
            mockCloudQueue.Verify(x => x.DeleteMessageAsync("1", "Receipte1"), Times.Once());
        }


    }
}

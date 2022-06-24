using Microsoft.VisualStudio.TestTools.UnitTesting;
using RonVideo.Activities;
using RonVideo.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class TimeExpiredExceptionTests
    {
        private  void MockTimeExpiredExcepiton()
        {
            throw new TimeExpiredException();
        }

        private void MockTimeExpiredExcepitonWithMessage()
        {
            throw new TimeExpiredException("My Message");
        }


        private void MockTimeExpiredExcepitonWithInner()
        {
            throw new Exception("My Message",new TimeExpiredException("Inner"));
        }

        [TestMethod]
        public void TimeExpiredExcepitonTest()
        {
            Action invocation = () => MockTimeExpiredExcepiton();

            var ex = Assert.ThrowsException<TimeExpiredException>(invocation);
            Assert.IsNotNull(ex);
            Assert.IsNull(ex.InnerException);
       

        }

        [TestMethod]
        public void TimeExpiredExcepitonWithMessageTest()
        {
            Action invocation = () => MockTimeExpiredExcepitonWithMessage();

            var ex = Assert.ThrowsException<TimeExpiredException>(invocation);
            Assert.IsNotNull(ex);
            Assert.IsNull(ex.InnerException);
            Assert.AreEqual("My Message", ex.Message);

        }

        [TestMethod]
        public void TimeExpiredExcepitonWithInnerTest()
        {
            Action invocation = () => MockTimeExpiredExcepitonWithInner();

            var ex = Assert.ThrowsException<Exception>(invocation);
            Assert.IsNotNull(ex);
            Assert.IsNotNull(ex.InnerException);
            Assert.AreEqual("Inner", ex.InnerException.Message);

        }

        [TestMethod]
        public void TimeExpiredExcepitonHandlerOKTest()
        {
            var exception= new Exception("My Message", new TimeExpiredException("Inner"));
            Func<Exception, bool> handler = OrchestratorFunctions.TimexpiredExceptionHandler();

            Assert.IsTrue(handler(exception));

        }

        [TestMethod]
        public void TimeExpiredExcepitonHandlerInnerNotTimeExpiredTest()
        {
            var exception = new TimeExpiredException("My Message", new Exception("Inner"));
            Func<Exception, bool> handler = OrchestratorFunctions.TimexpiredExceptionHandler();
            Assert.IsFalse(handler(exception) );

        }

        [TestMethod]
        public void TimeExpiredExcepitonHandlerNoInnerdTest()
        {
            var exception = new TimeExpiredException();
            Func<Exception, bool> handler = OrchestratorFunctions.TimexpiredExceptionHandler();
            Assert.IsFalse(handler(exception));

        }
    }
}

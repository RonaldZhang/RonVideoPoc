using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RonVideo;
using RonVideo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    [TestClass]
    public class StartupTests
    {
        [TestMethod]
        public void TestRunLocally()
        {
            Environment.SetEnvironmentVariable("IS_RUNNING_LOCALLY", "true");
            Startup startup = new Startup();
            Mock<IFunctionsHostBuilder> builderMock = new Mock<IFunctionsHostBuilder>();
            IServiceCollection serviceCollection = new ServiceCollection();
            builderMock.SetupGet(mock => mock.Services).Returns(serviceCollection);
            //IFunctionsHostBuilder builder = builderMock.Object;

            startup.Configure(builderMock.Object);

            ServiceProvider provider = serviceCollection.BuildServiceProvider();

            object keyVaultManage = provider.GetService(typeof(IKeyVaultManager));
            Assert.IsInstanceOfType(keyVaultManage,  typeof(LocalKVManager));

            object cloudQueueManager = provider.GetService(typeof(ICloudQueueManager));
            Assert.IsInstanceOfType(cloudQueueManager, typeof(CloudQueueManager));


        }

        [TestMethod]
        public void TestRunInAzure()
        {
            Environment.SetEnvironmentVariable("IS_RUNNING_LOCALLY", "false");
            Startup startup = new Startup();
            Mock<IFunctionsHostBuilder> builderMock = new Mock<IFunctionsHostBuilder>();
            IServiceCollection serviceCollection = new ServiceCollection();
            builderMock.SetupGet(mock => mock.Services).Returns(serviceCollection);
            //IFunctionsHostBuilder builder = builderMock.Object;

            startup.Configure(builderMock.Object);

            ServiceProvider provider = serviceCollection.BuildServiceProvider();

            object keyVaultManage = provider.GetService(typeof(IKeyVaultManager));
            Assert.IsInstanceOfType(keyVaultManage, typeof(NavyKVManager));

            object cloudQueueManager = provider.GetService(typeof(ICloudQueueManager));
            Assert.IsInstanceOfType(cloudQueueManager, typeof(CloudQueueManager));


        }
    }
}

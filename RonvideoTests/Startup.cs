using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RonVideo.Utilities;
using RonvideoTests.Utilities;
using System;
using System.Collections.Generic;
using System.Text;


[assembly: FunctionsStartup(typeof(RonVideo.Startup))]
namespace RonVideo
{
    public class UnitTestStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddSingleton<IKeyVaultManager>((s) => {
                return new LocalKVManager();
            });


            builder.Services.AddSingleton<ICloudQueueManager>((s) => {
                return new LocalCloudQueueManager();
            });

            builder.Services.AddSingleton<ICloudQueueWrapper>((s) => {
                return new LocalCloudQueueWrapper();
            });
        }
    }
}

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RonVideo.Utilities;
using System;
using System.Collections.Generic;
using System.Text;


[assembly: FunctionsStartup(typeof(RonVideo.Startup))]
namespace RonVideo
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            bool isLocal = Environment.GetEnvironmentVariable("IS_RUNNING_LOCALLY") == "true";
            if (isLocal)
                builder.Services.AddSingleton<IKeyVaultManager>((s) => {
                    return new LocalKVManager();
                });
            else
                builder.Services.AddSingleton<IKeyVaultManager>((s) => {
                    return new KeyVaultManager();
                });
        }
    }
}

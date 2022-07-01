using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RonVideo.Models;
using System.Diagnostics.CodeAnalysis;
using RonVideo.Utilities;

namespace RonVideo.Simulators
{
    [ExcludeFromCodeCoverage]
    public static class BlendUrlSimulator
    {
        static string baseURL = Environment.GetEnvironmentVariable("BlendBaseUrl") + Constants.RouteDownload;

        [FunctionName("BlendUrlSimulator")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = Constants.RouteUrl)] HttpRequest req,
            string closingId,
            string fileId,
            ILogger log)
        {
            log.LogInformation($"Requesting Download URL. ClosingId: {closingId}, FileId: {fileId}");


            BlendUrlResponse rep = new BlendUrlResponse();
            rep.DownloadUrl = baseURL + "?token="+closingId+"_" +fileId;
            rep.ExpiresAt = DateTime.UtcNow.AddSeconds(10).ToString("s") + "Z";
            await Task.Delay(500);
            return new OkObjectResult(rep);
        }

       
    }
}

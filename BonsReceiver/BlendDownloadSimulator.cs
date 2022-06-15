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
using System.Web;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Net.Http.Headers;

namespace RonVideo
{
    public static class BlendDownloadSimulator
    {
        [FunctionName("BlendSimulator")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = Constants.RouteDownload)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Download request.");

            var token = req.Query.ContainsKey("token")?req.Query["token"].ToString():string.Empty;
            log.LogInformation($"Token is {token}");

            var res = await GetAllBytes(token);

            //return new OkObjectResult(res);

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(res);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;

        }
        private static async Task<byte[]> GetAllBytes(string token)
        {
            string path = @"c:\myDownloads";
            byte[] bytes = null;
            var id = token.Split("_")[1];
            string path2 = id + ".mp4";
            var fullPath = Path.Combine(path, path2);
            bytes = await File.ReadAllBytesAsync(fullPath);
            return bytes;
        }

        //private static StringDictionary ConvertToDictionary(NameValueCollection valueCollection)
        //{
        //    var dictionary = new StringDictionary();
        //    foreach (var key in valueCollection.AllKeys)
        //    {
        //        if (key != null)
        //        {
        //            dictionary.Add(key.ToLowerInvariant(), valueCollection[key]);
        //        }
        //    }
        //    return dictionary ;
        //}
    }
}

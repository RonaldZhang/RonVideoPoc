using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Azure.Storage.Blobs;
using System.Collections.Generic;
using Azure.Storage.Blobs.Models;
using System.Diagnostics.CodeAnalysis;
using RonVideo.Utilities;

namespace RonVideo.Simulators
{
    [ExcludeFromCodeCoverage]
    public static class BlendDownloadSimulator
    {
      
        [FunctionName("BlendSimulator")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = Constants.RouteDownload)] HttpRequest req,
              //[Blob("vidoeblob/samples/{blobName}.mp4", FileAccess.Read)] Stream myBlob,
            //[Blob("vidoeblob")] BlobContainerClient blobContainerClient,
            [Blob("videoblob/samples")] IEnumerable<BlobClient> blobs,
            ILogger log)
        {
            log.LogInformation("Download request.");

            //foreach (BlobClient blob in blobs)
            //{
            //    log.LogInformation(blob.Name);
            //}
            var token = req.Query.ContainsKey("token")?req.Query["token"].ToString():string.Empty;
            log.LogInformation($"Token is {token}");

            var id = token.Split("_")[1];
            string path2 = id + ".mp4";

            var b = blobs.Where(x => string.Compare(x.Name, "samples/"+ path2,true)==0).FirstOrDefault();

            var res = new byte[0];
            if (b != null)
            {
                BlobDownloadResult downloadResult = await b.DownloadContentAsync();
                res = downloadResult.Content.ToArray();
            }
            //foreach (BlobClient blob in blobs)
            //{
            //    log.LogInformation(blob.Name);
            //}

            //var res = await GetAllBytes(token);

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

        //private static async Task<byte[]> GetAllbytesFromBlob(string token)
        //{
        //    byte[] bytes = null;
        //    var id = token.Split("_")[1];


        //    // Download the blob to a local file
        //    // Append the string "DOWNLOADED" before the .txt extension 
        //    // so you can compare the files in the data directory
        //    //string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");

        //    //Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

        //    // Download the blob's contents and save it to a file

        //    BlobContainerClient container = new BlobContainerClient(connectionString, Randomize("sample-container"));

        //    await blobClient.DownloadToAsync(downloadFilePath);

        //    return bytes;
        //}
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

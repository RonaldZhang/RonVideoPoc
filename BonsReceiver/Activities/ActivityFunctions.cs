using Azure;
using RonVideo.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RonVideo.Exceptions;
using System.Collections.Generic;
using Azure.Storage.Blobs;
using System.Diagnostics.CodeAnalysis;

namespace RonVideo.Activities
{
    public static class ActivityFunctions
    {
        //private static readonly HttpClient client;
        public static  HttpClient client;

        private static string urlLoanId = Constants.BaseURL + Constants.RouteLoanId;  // "http://localhost:8079/loanid/";
        private static string urlGetUrl = Constants.BaseURL + Constants.RouteUrl;  //  "http://localhost:8079/url/";

        static ActivityFunctions()
        {
            var handler = new HttpClientHandler();
            handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
            client = new HttpClient(handler);
        }

        [FunctionName(nameof(GetLoanId))]
        public static async Task<string> GetLoanId([ActivityTrigger] string blendId, ILogger log)
        {

            var response = await client.GetAsync(urlLoanId.Replace("{id}", blendId));
            if (response.IsSuccessStatusCode)
            {
                log.LogInformation($"Succcess in getting loan Id for blend id {blendId}.");
                var contents = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<BlendLoanIdResponse>(contents);
                await Task.Delay(100);
                return dto.LoanId;
            }

            log.LogError($"{response.StatusCode} {response.ReasonPhrase}: ");
            return "";
        }


        //[FunctionName(nameof(GetDownloadUrl))]
        //public static async Task<string> GetDownloadUrl([ActivityTrigger] string fileId, ILogger log)
        //{
        //    var response = await client.GetAsync(urlGetUrl + fileId);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        log.LogInformation($"Ok in {nameof(GetDownloadUrl)}");
        //        var contents = await response.Content.ReadAsStringAsync();
        //        var dto = JsonConvert.DeserializeObject<BlendUrlResponse>(contents);
        //        await Task.Delay(1000);
        //        return dto.Url;
        //    }
   
        //    log.LogError($"{response.StatusCode} {response.ReasonPhrase}: ");
        //    return "";
        //}
        
        public static async Task<string> IntGetDownloadUrl(string closingId, string fileId, ILogger log)
        {

            var response = await client.GetAsync(urlGetUrl
                .Replace("{closingId}", closingId)
                .Replace("{fileId}",fileId));

            if (response.IsSuccessStatusCode)
            {
                log.LogInformation($"Ok in {nameof(IntGetDownloadUrl)}");
                var contents = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<BlendUrlResponse>(contents);
                await Task.Delay(100);
                return dto.DownloadUrl;
            }

            log.LogError($"{response.StatusCode} {response.ReasonPhrase}: ");
            return "";
        }

        public static async Task<GetVideoResult> IntGetVideo(string url, ILogger log)
        {
            log.LogInformation("Download URL " + url);
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                log.LogInformation($"Ok in {nameof(GetVideo)}");
                var contents = await response.Content.ReadAsByteArrayAsync();
                await Task.Delay(100);

                return new GetVideoResult(response.StatusCode, contents);
            }

            log.LogError($"{response.StatusCode} {response.ReasonPhrase}: ");
            return new GetVideoResult(response.StatusCode, new byte[0]);
        }

        [FunctionName(nameof(GetVideo))]
        public static async Task<byte[]> GetVideo([ActivityTrigger] VideoQueueItem qItem, ILogger log)
        {
            string fileId = qItem.FileId;
            string closingId = qItem.CloseId; 
            var response = await IntGetDownloadUrl(closingId, fileId, log);
            if (!string.IsNullOrWhiteSpace(response))
            {
                log.LogInformation($"Success in Getting the Download URL for {fileId}.");
                var contents = await IntGetVideo(response, log);

                if (contents.HttpStatus == HttpStatusCode.OK)
                    return contents.bytes;
                else if (contents.HttpStatus == HttpStatusCode.Gone)
                {
                    
                    log.LogWarning($"Expired {contents.HttpStatus} in Getting the Download URL for {fileId}");
                    throw new TimeExpiredException();
                        
                }
                else
                {
                    log.LogError($"Error {contents.HttpStatus} in Getting the Download URL for {fileId}.");
                }
            }
            else
            {
                log.LogError($"Error in Getting the Download URL for {fileId}.");
            }
            
            return new byte[0];
        }

        [ExcludeFromCodeCoverage]
        [FunctionName(nameof(UploadVideo))]
        public static async Task<bool> UploadVideo([ActivityTrigger] VideoContent vc, ILogger log)
        {
            log.LogInformation("Upload Video Started");
            string basePath = @"c:\myazFuncuploads";
            string filename = vc.LoanId + "_" + vc.FileId + ".mp4";
            string filePath = Path.Combine(basePath, filename);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.CreateNew, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(vc.Bytes, 0, vc.Bytes.Length);
            };

            log.LogInformation("Upload Video Completed");
            return true;
        }


        [ExcludeFromCodeCoverage]
        [FunctionName(nameof(UploadVideo2Blob))]
        public static async Task<bool> UploadVideo2Blob([ActivityTrigger] VideoContent vc,
            [Blob("videoblob/uploads/{data.LoanId}_{data.FileId}.mp4", FileAccess.Write)] Stream outVideo,
            ILogger log)
        {
            log.LogInformation("Upload Video to Blob Started");
   
            string filename = vc.LoanId + "_" + vc.FileId + ".mp4";

            await outVideo.WriteAsync(vc.Bytes, 0, vc.Bytes.Length);
           
            log.LogInformation("Upload Video to Blob Completed");
            return true;
        }


        [FunctionName(nameof(Upsert))]
        [return: Table("videoTable")]
        public static async Task<VideoItem> Upsert([ActivityTrigger] (VideoRowItem, VideoQueueItem, string) vv, ILogger log)
        {
            log.LogInformation("Upsert Table Activity Called");
            var videoRow = vv.Item1;
            var myQueueItem = vv.Item2;
            var status = vv.Item3;

            if (videoRow != null)
            {
                log.LogInformation($"Update A row LoanId: {videoRow.LoanId}, FileId: {videoRow.FileId}.status: {status}, count: {videoRow.Count + 1}");
                var v = new VideoItem(videoRow.BlendId, videoRow.LoanId, videoRow.CloseId, videoRow.FileId,
                    videoRow.Count + 1, status, videoRow.PartitionKey, videoRow.FileId);

                v.ETag = new ETag("*");
                return v;
            }
            else
            {
                log.LogInformation($"Insert A row LoanId: {myQueueItem.LoanId}, FileId: {myQueueItem.FileId}.status: {status}");
                return new VideoItem(myQueueItem.BlendId, myQueueItem.LoanId, myQueueItem.CloseId, myQueueItem.FileId, 1, status, "Http", myQueueItem.FileId);
            }
        }

        ////Requeue
        //[FunctionName(nameof(Requeue))]
        //public static async Task<string> Requeue([ActivityTrigger] VideoQueueItem vq,
        //    [Queue("videobatchqueue")] ICollector<string> outputQueueItem,
        //    ILogger log)
        //{ 
        //    log.LogInformation($"Requeue {vq}");
        //    //outputQueueItem.Add(JsonConvert.SerializeObject(vq));
        //    return "success";
        //}
    }
}
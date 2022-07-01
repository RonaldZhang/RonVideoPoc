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
using System.Diagnostics.CodeAnalysis;
using RonVideo.Utilities;
using System;

namespace RonVideo.Activities
{
    public static class ActivityFunctions
    {
        public static  HttpClient client;

        private static string urlLoanId; 
        private static string urlGetUrl; 

        static ActivityFunctions()
        {
            var handler = new HttpClientHandler();
            handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
            client = new HttpClient(handler);
            urlLoanId = Environment.GetEnvironmentVariable("BlendBaseUrl") + Constants.RouteLoanId;
            urlGetUrl = Environment.GetEnvironmentVariable("BlendBaseUrl") + Constants.RouteUrl;
    }

        #region Get Loan ID Function
        [FunctionName(nameof(GetLoanId))]
        public static async Task<string> GetLoanId([ActivityTrigger] string blendId, ILogger log)
        {

            var response = await client.GetAsync(urlLoanId.Replace("{id}", blendId));
            if (response.IsSuccessStatusCode)
            {
                log.LogDebug($"Succcess in getting loan Id for blendId: {blendId}.");
                var contents = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<BlendLoanIdResponse>(contents);
                await Task.Delay(100);
                return dto.LoanId;
            }

            log.LogError($"GetLoanId Error. blend Id: {blendId} resp status:{response.StatusCode} reason: {response.ReasonPhrase} ");
            return "";
        }
        #endregion

        #region Get Url
        private static async Task<string> IntGetDownloadUrl(string closingId, string fileId, ILogger log)
        {

            var response = await client.GetAsync(urlGetUrl
                .Replace("{closingId}", closingId)
                .Replace("{fileId}",fileId));

            if (response.IsSuccessStatusCode)
            {
                log.LogDebug($"Ok in Getting the Url for closingId: {closingId} fileId: {fileId}");
                var contents = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<BlendUrlResponse>(contents);
                await Task.Delay(100);
                return dto.DownloadUrl;
            }

            log.LogError($"Get Url Error. closingId: {closingId} fileId: {fileId} resp status: {response.StatusCode} reason: {response.ReasonPhrase} ");
            return "";
        }

        #endregion

        #region Get URL and download
        private static async Task<GetVideoResult> IntGetVideo(string url, ILogger log)
        {
            log.LogDebug($"Inside the GetVideo. The download Url: {url}");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                log.LogDebug($"Ok in Getting the Video from {url}");
                var contents = await response.Content.ReadAsByteArrayAsync();
                await Task.Delay(100);

                return new GetVideoResult(response.StatusCode, contents);
            }

            log.LogError($"Get Video Error from {url} resp status: {response.StatusCode} reason: {response.ReasonPhrase}");
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
                log.LogDebug($"Getting a non-empty download Url for closingId: {closingId} fileId: {fileId}");
                var contents = await IntGetVideo(response, log);

                if (contents.HttpStatus == HttpStatusCode.OK)
                    return contents.bytes;
                else if (contents.HttpStatus == HttpStatusCode.Gone)
                {
                    log.LogWarning($"Expired {contents.HttpStatus} in Getting the Download URL for closingId: {closingId} fileId: {fileId}");
                    throw new TimeExpiredException();          
                }
                else
                {
                    log.LogError($"Error {contents.HttpStatus} in Getting the Download URL for closingId: {closingId} fileId: {fileId}");
                }
            }
            else
            {
                log.LogError($"Empty return in the Download URL for closingId: {closingId} fileId: {fileId}");
            }
            
            return new byte[0];
        }
        #endregion

        #region Upload
        [ExcludeFromCodeCoverage]
        [FunctionName(nameof(UploadVideo))]
        public static async Task<bool> UploadVideo([ActivityTrigger] VideoContent vc, ILogger log)
        {
            log.LogDebug($"Upload Video Started for {vc}");
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
            log.LogDebug($"Upload Video to Blob Started for {vc}");
   
            string filename = vc.LoanId + "_" + vc.FileId + ".mp4";

            await outVideo.WriteAsync(vc.Bytes, 0, vc.Bytes.Length);
           
            log.LogDebug($"Upload Video to Blob Completed fro {vc}");
            return true;
        }
        #endregion

        #region Table row insert/update
        [FunctionName(nameof(Upsert))]
        [return: Table("videoTable")]
        public static async Task<VideoItem> Upsert([ActivityTrigger] (VideoItem, VideoQueueItem, string) vv, ILogger log)
        {

            //var videoRow = vv.Item1;
            //var myQueueItem = vv.Item2;
            //var status = vv.Item3;
            var (videoRow, myQueueItem, status) = vv;

            log.LogDebug($"Upsert Table Activity Called videorow:{videoRow} queue: {myQueueItem}");
            if (videoRow != null)
            {
                log.LogDebug($"Update A row LoanId: {videoRow.LoanId}, FileId: {videoRow.FileId}.status: {status}, count: {videoRow.Count + 1}");
                var v = new VideoItem(videoRow.BlendId, videoRow.LoanId, videoRow.CloseId, videoRow.FileId,
                    videoRow.Count + 1, status, videoRow.PartitionKey, videoRow.FileId);

                v.ETag = new ETag("*");
                return v;
            }
            else
            {
                log.LogDebug($"Insert A row LoanId: {myQueueItem.LoanId}, FileId: {myQueueItem.FileId}.status: {status}");
                return new VideoItem(myQueueItem.BlendId, myQueueItem.LoanId, myQueueItem.CloseId, myQueueItem.FileId, 1, status, "Http", myQueueItem.FileId);
            }
        }
        #endregion
    }
}
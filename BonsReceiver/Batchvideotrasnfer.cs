using System;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using RonVideo.Models;
using RonVideo.Utilities;

namespace RonVideo
{
    public class Batchvideotrasnfer
    {
        [FunctionName("BatchRonVideo")]
        //public  async Task<IActionResult> BatchRonVideoStarter(
        public async void BatchRonVideoStarter(
            [TimerTrigger("%TimerInterval%")] TimerInfo myTimer,
           // [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Table("videotable")] TableClient tableClient,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await BatchRonVideoProcessStart(tableClient, starter, log);
            string responseMessage = "Event Received";
            // return new OkObjectResult(responseMessage);
        }

        public async Task BatchRonVideoProcessStart(
        TableClient tableClient ,
        IDurableOrchestrationClient starter,
        ILogger log)
        {
            log.LogInformation($"Video Records process started");
            log.LogInformation(Helper.GetEnvironmentVariable("AzureWebJobsStorage"));

            //AsyncPageable<VideoItem> queryResults = tableClient.QueryAsync<VideoItem>(filter: $"PartitionKey eq 'Http' and Status neq 'Completed' ");
            AsyncPageable<VideoItem> queryResults = tableClient.QueryAsync<VideoItem>(ent=>ent.Status!="Completed");
            await foreach (VideoItem entity in queryResults)
            {
                log.LogInformation($"{entity.PartitionKey}\t{entity.RowKey}\t{entity.Timestamp}\t{entity.FileId}\t{entity.Status}");
                VideoItem videoRow = entity;
                VideoQueueItem myQueueItem = new VideoQueueItem(entity.BlendId, entity.LoanId, entity.CloseId, entity.FileId);
                VideoQueueItem dto = new VideoQueueItem(entity.BlendId, entity.LoanId, entity.CloseId, entity.FileId);
                VideoQueueItem video = new VideoQueueItem(entity.BlendId, entity.LoanId, entity.CloseId, entity.FileId);

                VideoItem vidoeR = null;
                if (videoRow != null)
                    vidoeR = new VideoItem(videoRow.BlendId, videoRow.LoanId, videoRow.CloseId, videoRow.FileId, videoRow.Count, videoRow.Status, videoRow.PartitionKey, videoRow.RowKey);
                OrchestratorInput input1 = new OrchestratorInput(video, vidoeR);
                //await starter.StartNewAsync("TransferOrchestrator",v);

                string success = "";

                //Loook up the record
                if (videoRow != null)
                {
                    //log.LogInformation($"Table Record found with {myQueueItem.FileId}: {JsonConvert.SerializeObject(videoRow)}");
                    //Existing record
                    if ( videoRow.Status.Equals("Completed"))
                    //Already processed completely
                    {
                        await Task.Delay(5000);
                        log.LogInformation($"Alredy Processed. Skip the File : {myQueueItem.FileId}");
                        continue;
                    }
                    else
                    {
                        //Tried last time, need to rransfer again
                        log.LogInformation($"Reprocessing : {myQueueItem.FileId}");
                        success = await starter.StartNewAsync("TransferOrchestrator", input1);
                        //success= await _processing.ProcessFlow(input1, starter);
                    }
                }
                //else
                //{
                //    //New fileId
                //    log.LogInformation($"No Record found with {myQueueItem.FileId}.");
                //    success = await starter.StartNewAsync("TransferOrchestrator", input1);
                //}

                string status = string.IsNullOrWhiteSpace(success) ? "Failed" : "Completed";

                await Task.Delay(5000);
                log.LogInformation($"Video Queue processed: {JsonConvert.SerializeObject(myQueueItem)}");
            }
            return;
        }
    }

}

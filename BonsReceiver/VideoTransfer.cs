using System.Net.Http;
using System.Threading.Tasks;
using Azure;
using RonVideo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RonVideo
{
    public class VideoTransfer
    {

        //private readonly Processing _processing;
        //public VideoTransfer(Processing processing)
        //{
        //    _processing = processing;
        //}

        [FunctionName(nameof(RonVideoStarter))]

        public async void RonVideoStarter(
        [QueueTrigger("videosubsqueue", Connection = "")] VideoQueueItem myQueueItem,
        [Table("videotable", "Http", "{FileId}")] VideoItem videoRow,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            log.LogInformation($"Video Queue process started: {JsonConvert.SerializeObject(myQueueItem)}");

            var dto = myQueueItem;

            VideoQueueItem video = new VideoQueueItem(dto.BlendId, dto.LoanId, dto.CloseId, dto.FileId);
            VideoItem vidoeR = null;
            if (videoRow != null)
                vidoeR = new VideoItem(videoRow.BlendId, videoRow.LoanId, videoRow.CloseId, videoRow.FileId, videoRow.Count, videoRow.Status, videoRow.PartitionKey, videoRow.RowKey);
            OrchestratorInput input1 = new OrchestratorInput(video, vidoeR);
            //await starter.StartNewAsync("TransferOrchestrator",v);

            string success = "";

            //Loook up the record
            if (videoRow != null)
            {
                log.LogInformation($"Table Record found with {myQueueItem.FileId}: {JsonConvert.SerializeObject(videoRow)}");
                //Existing record
                if ("Completed" == videoRow.Status)
                //Already processed completely
                {
                    await Task.Delay(5000);
                    log.LogInformation($"Alredy Processed. Skip the File : {myQueueItem.FileId}");
                    return;
                }
                else
                {
                    //Tried last time, need to rransfer again
                    log.LogInformation($"Reprocessing : {myQueueItem.FileId}");
                    success = await starter.StartNewAsync("TransferOrchestrator", input1);
                    //success= await _processing.ProcessFlow(input1, starter);
                }
            }
            else
            {
                //New fileId
                log.LogInformation($"No Record found with {myQueueItem.FileId}.");
                success = await starter.StartNewAsync("TransferOrchestrator", input1);
            }

            string status = string.IsNullOrWhiteSpace(success) ? "Failed" : "Completed";

            await Task.Delay(5000);
            log.LogInformation($"Video Queue processed: {JsonConvert.SerializeObject(myQueueItem)}");
            return;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using RonVideo.Models;

namespace RonVideo
{
    public class BatchVideoTransfer : VideoTransferBase
    {
        private RonLoggerObject setting = null;

        [FunctionName("BatchRonVideo")]
        public async void BatchRonVideoStarter(
            [TimerTrigger("%TimerInterval%")] TimerInfo myTimer,
            [Table("videotable")] TableClient tableClient,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            await BatchRonVideoProcessStart(tableClient, starter, log);
        }

        public async Task BatchRonVideoProcessStart(
        TableClient tableClient ,
        IDurableOrchestrationClient starter,
        ILogger log)
        {

            setting = CreateRonLoggerObject(log);
            setting.LogInfomration( RonEventId.BatchVideoTransferTriggered, $"Batch Timer trigger executed at: {DateTime.Now}");

            AsyncPageable<VideoItem> queryResults = tableClient.QueryAsync<VideoItem>(ent=>string.Compare(ent.Status,"Completed",true)!=0);
            IAsyncEnumerator<VideoItem> enumerator = queryResults.GetAsyncEnumerator();


            int totalCount = 0;
            int successCount = 0;
            // await foreach (VideoItem entity in queryResults)
            while (await enumerator.MoveNextAsync())
            {
                VideoItem entity = enumerator.Current;
                totalCount++;
                setting = UpdateRonLoggerObject(setting, entity);
                setting.LogInfomration( RonEventId.BatchVideoTransferFileStarted, $"{entity.PartitionKey}\t{entity.RowKey}\t{entity.Timestamp}\t{entity.FileId}\t{entity.Status}");

                OrchestratorInput input1=prepareOrchestratorInput(entity);
                string success = "";
     
                if (input1.vr != null)
                {
                    //Existing record
                    if (input1.vr.Status.Equals("Completed"))
                    //Already processed completely
                    {
                        await Task.Delay(10);
                        setting.LogInfomration( RonEventId.BatchVideoTransferFileStarted, $"Alredy Processed. Please chekc the Query. Skip the File : {input1.vq.FileId}");
                        continue;
                    }
                    else
                    {
                        //Tried last time, need to transfer again
                        log.LogInformation($"Reprocessing : {input1.vq.FileId}");
                        setting.LogInfomration( RonEventId.BatchVideoTransferFileProcessing, $"Reprocessing : {input1.vq.FileId}");
                        string instanceId = await starter.StartNewAsync("TransferOrchestrator", input1);
                        VidoeTransferResult result =await WaitUntilCompleted(starter, instanceId);

                        if (VidoeTransferResult.Success==result)
                            successCount++;
                    }
                }

                string status = string.IsNullOrWhiteSpace(success) ? "Failed" : "Completed";

                await Task.Delay(10);
            }
            setting.LogInfomration( RonEventId.BatchVideoTransferFileProcessed, $"Batch Timer trigger executed done at: { DateTime.Now} total: { totalCount} success: {successCount}");
            return;
        }

        private static OrchestratorInput prepareOrchestratorInput(VideoItem entity)
        {
            //videoRow = entity;
            VideoQueueItem videoQ = new VideoQueueItem(entity.BlendId, entity.LoanId, entity.CloseId, entity.FileId);
            //VideoQueueItem dto = myQueueItem.ShallowCopy(); 
            //VideoQueueItem videoQ = myQueueItem.ShallowCopy();// dto.ShallowCopy();
            VideoItem videoR = entity.ShallowCopy();
            OrchestratorInput input1 = new OrchestratorInput(videoQ, videoR);

            return input1;
        }

        private static RonLoggerObject CreateRonLoggerObject(ILogger log)
        {
            return new RonLoggerObject(log)
            {
                Id = RonEventId.BatchVideoTransferTriggered,
                EntityType = EntityType.BatchVidoeTransfer.ToString(),
                BonsEventId = "",
                CloseId = "",
                BlendId = "",
                FileId = "",
                LoanId = "",
                Status = ""
            };
        }

        private static RonLoggerObject UpdateRonLoggerObject(RonLoggerObject obj, VideoItem entity)
        {
            if ((entity != null)  &&( obj!=null))
            {
                obj.BlendId = entity.BlendId;
                obj.BonsEventId = entity.BlendId;
                obj.CloseId = entity.CloseId;
                obj.FileId = entity.FileId;
                obj.LoanId = entity.LoanId;
            }

            return obj;
        }
    }

}

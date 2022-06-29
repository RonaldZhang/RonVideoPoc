using System;
using System.Diagnostics;
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
    public class BatchVideoTransfer
    {
        private RonLoggerObject setting = null;

        [FunctionName("BatchRonVideo")]
        //public  async Task<IActionResult> BatchRonVideoStarter(
        public async void BatchRonVideoStarter(
            [TimerTrigger("%TimerInterval%")] TimerInfo myTimer,
           // [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
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
            //log.LogInformation($"Video Records process started");
            //log.LogInformation(Helper.GetEnvironmentVariable("AzureWebJobsStorage"));
            setting = CreateRonLoggerObject();
            setting.LogInfomration(log, RonEventId.BatchVideoTransferTriggered, $"Batch Timer trigger executed at: {DateTime.Now}");

            AsyncPageable<VideoItem> queryResults = tableClient.QueryAsync<VideoItem>(ent=>string.Compare(ent.Status,"Completed",true)!=0);
            int count = 0;
            await foreach (VideoItem entity in queryResults)
            {
                setting = UpdateRonLoggerObject(setting, entity);
                setting.LogInfomration(log, RonEventId.BatchVideoTransferFileStarted, $"{entity.PartitionKey}\t{entity.RowKey}\t{entity.Timestamp}\t{entity.FileId}\t{entity.Status}");

                OrchestratorInput input1=prepareOrchestratorInput(entity);
                string success = "";
                //Loook up the record
                if (input1.vr != null)
                {
                    //log.LogInformation($"Table Record found with {myQueueItem.FileId}: {JsonConvert.SerializeObject(videoRow)}");
                    //Existing record
                    if (input1.vr.Status.Equals("Completed"))
                    //Already processed completely
                    {
                        await Task.Delay(10);
                        setting.LogInfomration(log, RonEventId.BatchVideoTransferFileStarted, $"Alredy Processed. Please chekc the Query. Skip the File : {input1.vq.FileId}");
                        continue;
                    }
                    else
                    {
                        //Tried last time, need to transfer again
                        log.LogInformation($"Reprocessing : {input1.vq.FileId}");
                        setting.LogInfomration(log, RonEventId.BatchVideoTransferFileProcessing, $"Reprocessing : {input1.vq.FileId}");
                        string instanceId = await starter.StartNewAsync("TransferOrchestrator", input1);
                        await WaitUntilCompleted(starter, instanceId);

                        //string instanceId = await starter.StartNewAsync("TransferOrchestrator", input1);
                        //TimeSpan timeout = TimeSpan.FromSeconds(60);
                        //TimeSpan retryInterval = TimeSpan.FromSeconds(1);
                        //await starter.WaitForCompletionOrCreateCheckStatusResponseAsync(req,instanceId,timeout, retryInterval);

                        count++;
                    }
                }
                //else
                //{
                //    //New fileId
                //    log.LogInformation($"No Record found with {myQueueItem.FileId}.");
                //    success = await starter.StartNewAsync("TransferOrchestrator", input1);
                //}

                string status = string.IsNullOrWhiteSpace(success) ? "Failed" : "Completed";

                await Task.Delay(10);
            }
            setting.LogInfomration(log, RonEventId.BatchVideoTransferFileProcessed, $"Batch Timer trigger executed done at: { DateTime.Now} total: { count}");
            return;
        }

        private static async Task WaitUntilCompleted(IDurableOrchestrationClient starter, string instanceId)
        {
            TimeSpan timeout = TimeSpan.FromSeconds(60);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DurableOrchestrationStatus s = null;
            do
            {
                s = await starter.GetStatusAsync(instanceId);
                await Task.Delay(100);
                if (sw.ElapsedMilliseconds > timeout.TotalMilliseconds)
                {
                    await starter.TerminateAsync(instanceId, $"Exceeding the time limit {timeout.TotalSeconds}");
                }
            } while (s==null ||s.RuntimeStatus == OrchestrationRuntimeStatus.Running || s.RuntimeStatus == OrchestrationRuntimeStatus.Pending || s.RuntimeStatus == OrchestrationRuntimeStatus.ContinuedAsNew || s.RuntimeStatus == OrchestrationRuntimeStatus.Unknown);
        }

        private static OrchestratorInput prepareOrchestratorInput(VideoItem entity)
        {
            //videoRow = entity;
            VideoQueueItem myQueueItem = new VideoQueueItem(entity.BlendId, entity.LoanId, entity.CloseId, entity.FileId);
            VideoQueueItem dto = myQueueItem.ShallowCopy(); // new VideoQueueItem(entity.BlendId, entity.LoanId, entity.CloseId, entity.FileId);

            VideoQueueItem queue = dto.ShallowCopy(); // new VideoQueueItem(entity.BlendId, entity.LoanId, entity.CloseId, entity.FileId);
            VideoItem item = entity.ShallowCopy();
            OrchestratorInput input1 = new OrchestratorInput(queue, item);

            return input1;
        }

        private static RonLoggerObject CreateRonLoggerObject()
        {
            return new RonLoggerObject()
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

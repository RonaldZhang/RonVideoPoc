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
using RonVideo.Utilities;

namespace RonVideo
{
    public class VideoTransfer
    {

        private RonLoggerObject setting = null;

        //private readonly Processing _processing;
        //public VideoTransfer(Processing processing)
        //{
        //    _processing = processing;
        //}
        [FunctionName(nameof(RonVideoStarter))]

        public async void RonVideoStarter(
        [QueueTrigger("videosubsqueue", Connection = "")] VideoQueueItem dtoQueue,
        [Table("videotable", "Http", "{FileId}")] VideoItem videoRow,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {

            RonLoggerObject setting = CreateRonLoggerObject(dtoQueue);
            setting.LogInfomration(log, RonEventId.VideoTransferTriggered, $"{JsonConvert.SerializeObject(dtoQueue)}");
            OrchestratorInput oInpu1 = CreateOrchestratorInput(dtoQueue, videoRow);

            string success = "";

            //Loook up the record
            if (videoRow != null)
            {
                //log.LogInformation($"Table Record found with {dtoQueue.FileId}: {JsonConvert.SerializeObject(videoRow)}");
                setting.LogInfomration(log, RonEventId.VideoTransferRecordFound, $"Table Record found with {dtoQueue.FileId}: {JsonConvert.SerializeObject(videoRow)}");

                //Existing record
                if ("Completed" == videoRow.Status)
                //Already processed completely
                {
                    await Task.Delay(10);
                    setting.LogInfomration(log, RonEventId.VideoTransferSkipped, $"Alredy Processed. Skip the File : {dtoQueue.FileId}");
                    return;
                }
                else
                {
                    //Tried last time, need to rransfer again
                    setting.LogInfomration(log, RonEventId.VideoTransferReprocessing, $"Reprocessing : {dtoQueue.FileId}");
                    success = await starter.StartNewAsync("TransferOrchestrator", oInpu1);
                }
            }
            else
            {
                //New fileId
                setting.LogInfomration(log, RonEventId.VideoTransferNewProcessing, $"No Record found with {dtoQueue.FileId}.");
                success = await starter.StartNewAsync("TransferOrchestrator", oInpu1);
            }

            string status = string.IsNullOrWhiteSpace(success) ? "Failed" : "Completed";
            await Task.Delay(10);
            setting.LogInfomration(log, RonEventId.VideoTransferReceived, $"Video Queue processed: {JsonConvert.SerializeObject(dtoQueue)}");
            return;
        }

        private static OrchestratorInput CreateOrchestratorInput(VideoQueueItem dtoQueue, VideoItem videoRow)
        {

            VideoQueueItem video = dtoQueue?.ShallowCopy();
            VideoItem vidoeR = videoRow?.ShallowCopy();

            OrchestratorInput input = new OrchestratorInput(video, vidoeR);
            return input;
        }

        private static RonLoggerObject CreateRonLoggerObject(VideoQueueItem qItem)
        {
            return new RonLoggerObject()
            {

                Id = RonEventId.VideoTransferTriggered,
                EntityType = EntityType.VidoeTransfer.ToString(),
                BonsEventId = qItem.BlendId,
                CloseId = qItem.CloseId,
                BlendId = qItem.BlendId,
                FileId = qItem.FileId,
                LoanId = qItem.LoanId,
                Status = ""
            };
        }

    }
}
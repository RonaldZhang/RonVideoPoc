using System.Threading.Tasks;
using RonVideo.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RonVideo
{
    public class VideoTransfer : VideoTransferBase
    {

        [FunctionName(nameof(RonVideoStarter))]

        public async void RonVideoStarter(
        [QueueTrigger("videosubsqueue", Connection = "")] VideoQueueItem dtoQueue,
        [Table("videotable", "Http", "{FileId}")] VideoItem videoRow,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {

            RonLoggerObject setting = CreateRonLoggerObject(log, dtoQueue);
            setting.LogInfomration(RonEventId.VideoTransferTriggered, $"{JsonConvert.SerializeObject(dtoQueue)}");
            OrchestratorInput oInput = CreateOrchestratorInput(dtoQueue, videoRow);

            string instanceId = "";

            //Loook up the record
            if (videoRow != null)
            {
                setting.LogInfomration(RonEventId.VideoTransferRecordFound, $"Table Record found with {dtoQueue.FileId}: {JsonConvert.SerializeObject(videoRow)}");

                //Existing record
                if ("Completed" == videoRow.Status)
                //Already processed completely
                {
                    await Task.Delay(10);
                    setting.LogInfomration(RonEventId.VideoTransferSkipped, $"Alredy Processed. Skip the File : {dtoQueue.FileId}");
                    return;
                }
                else
                {
                    //Tried last time, need to transfer again
                    setting.LogInfomration(RonEventId.VideoTransferReprocessing, $"Reprocessing : {dtoQueue.FileId}");
                    instanceId = await starter.StartNewAsync("TransferOrchestrator", oInput);
                    VidoeTransferResult result = await WaitUntilCompleted(starter, instanceId);
                }
            }
            else
            {
                //New fileId
                setting.LogInfomration(RonEventId.VideoTransferNewProcessing, $"No Record found with {dtoQueue.FileId}.");
                instanceId = await starter.StartNewAsync("TransferOrchestrator", oInput);
                VidoeTransferResult result = await WaitUntilCompleted(starter, instanceId);

            }

            string status = string.IsNullOrWhiteSpace(instanceId) ? "Failed" : "Completed";
            await Task.Delay(10);
            setting.LogInfomration(RonEventId.VideoTransferReceived, $"Video Queue processed: {JsonConvert.SerializeObject(dtoQueue)}");
            return;
        }

        private static OrchestratorInput CreateOrchestratorInput(VideoQueueItem dtoQueue, VideoItem videoRow)
        {

            VideoQueueItem video = dtoQueue?.ShallowCopy();
            VideoItem vidoeR = videoRow?.ShallowCopy();

            OrchestratorInput input = new OrchestratorInput(video, vidoeR);
            return input;
        }

        private static RonLoggerObject CreateRonLoggerObject(ILogger log, VideoQueueItem qItem)
        {
            return new RonLoggerObject( log)
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
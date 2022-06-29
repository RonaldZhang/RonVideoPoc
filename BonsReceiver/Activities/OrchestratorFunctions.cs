using System.Collections.Generic;
using System.Threading.Tasks;
using RonVideo.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
//using Microsoft.Azure.WebJobs.Host;
using System;
using RonVideo.Exceptions;
using Microsoft.Extensions.Logging;
using RonVideo.Utilities;
//using Microsoft.Extensions.Logging;

namespace RonVideo.Activities
{
    public static class OrchestratorFunctions
    {

        [FunctionName(nameof(TransferOrchestrator))]
        public static async Task<bool> TransferOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, 
            ILogger log
        )
        {

            log = context.CreateReplaySafeLogger(log);
            var vInput = context.GetInput<OrchestratorInput>();
            //var vQueueItem = vInput.vq;
            //var videoRow = vInput.vr;

            string status = await internalCall(context,log);

            return true;
        }

        private static async Task<string> internalCall(IDurableOrchestrationContext context, ILogger log)
        {
            var vInput = context.GetInput<OrchestratorInput>();

            var vQueueItem = vInput.vq;
            RonLoggerObject ronObj = CreateRonLoggerObject(vQueueItem);
            var videoRow = vInput.vr;
            string status = string.Empty;
            var success = false;
            VideoItem ss;
            try
            {
                try
                {
                    ronObj.LogInfomration(log, RonEventId.OrchestratorProcesssStarted,  $"Orchestrator Started. FileId:{vQueueItem.FileId}");

                    var loanId = await context.CallActivityAsync<string>("GetLoanId", vQueueItem.BlendId);
                    if (string.IsNullOrWhiteSpace(loanId))
                    {
                        ronObj.LogError(log, RonEventId.OrchestratorLoanIdNotFound, $"Orchestrator LoanId not found. FileId:{vQueueItem.FileId}");
                        status = "unable to get the loan Id";
                        ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));
                        return "failed";
                    }
                    vQueueItem.LoanId = loanId;
                    ronObj.LoanId = loanId;
                    ronObj.LogInfomration(log, RonEventId.OrchestratorLoanIdRetreived, $"Orchestrator Got LoanId. FileId:{vQueueItem.FileId} LoanID={loanId}");

                    var bytes = await context.CallActivityWithRetryAsync<byte[]>("GetVideo",
                        new RetryOptions(TimeSpan.FromSeconds(5), 4)
                        {
                            Handle = TimexpiredExceptionHandler()
                        },
                        vQueueItem);

                    if (bytes.Length == 0)
                    {
                        ronObj.LogError(log, RonEventId.OrchestratorVideoDownloadFailed, $"Orchestrator video file download unsuccessful. FileId:{vQueueItem.FileId}");
                        status = "video file download unsuccessful";
                        ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));
                        return "failed";
                    }

                    ronObj.LogInfomration(log, RonEventId.OrchestratorVideDownloaded, $"Orchestrator Got video. FileId:{vQueueItem.FileId} LoanID={loanId}");

                    VideoContent vc = new VideoContent(vQueueItem.BlendId, loanId, vQueueItem.CloseId, vQueueItem.FileId, bytes);

                    bool upload2Blob = Environment.GetEnvironmentVariable("Upload2Blob") == "true";
                    if (upload2Blob)
                        success = await context.CallActivityAsync<bool>("UploadVideo2Blob", vc);
                    else
                        success = await context.CallActivityAsync<bool>("UploadVideo", vc);


                    if (success)
                        ronObj.LogInfomration(log, RonEventId.OrchestratorVideoUploaded, $"Orchestrator Video Uploaded. FileId:{vQueueItem.FileId} LoanID={loanId}");
                    else
                        ronObj.LogError(log, RonEventId.OrchestratorVideoUploadFailed, "Orchestrator Video Uploaded. FileId:{vQueueItem.fileId} LoanID={loanId}");

                        status = success ? "Completed" : "Failed";
                    ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));

                    ronObj.LogInfomration(log, RonEventId.OrchestratorTableUpsertCompleted, $"Orchestrator record upsert completed. FileId:{vQueueItem.FileId} LoanID={loanId}");
                }
                catch (Exception ex)
                {
                    ronObj.LogError(log, "F1xxx.");
                    ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));
                    throw ex;
                }
                finally
                {

                }
            }
            catch (Exception ex)
            {

            }
            finally { }

            return status;
        }

        public static Func<Exception, bool> TimexpiredExceptionHandler()
        {
            return ex =>
            {
                if (ex.InnerException is null)
                    return false;
                return ex.InnerException is TimeExpiredException;
            };
        }

        private static RonLoggerObject CreateRonLoggerObject(VideoQueueItem qItem)
        {
            return new RonLoggerObject()
            {

                Id = RonEventId.OrchestratorTriggered,
                EntityType = EntityType.Orchestrator.ToString(),
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
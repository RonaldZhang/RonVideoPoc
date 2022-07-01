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
        public static async Task<VidoeTransferResult> TransferOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, 
            ILogger log
        )
        {
            log = context.CreateReplaySafeLogger(log);
            var vInput = context.GetInput<OrchestratorInput>();
            VidoeTransferResult status = await internalCall(context,log);

            return status;
        }

        private static async Task<VidoeTransferResult> internalCall(IDurableOrchestrationContext context, ILogger log)
        {
            var vInput = context.GetInput<OrchestratorInput>();

            var vQueueItem = vInput.vq;
            RonLoggerObject ronObj = CreateRonLoggerObject(log, vQueueItem);
            var videoRow = vInput.vr;
            bool uploadStatus = false;
            var success = false;
            VideoItem videoItem=null;
            VidoeTransferResult sta = VidoeTransferResult.Skipped;

            VidoeTransferResult result = VidoeTransferResult.InProgress;

            string loanId = string.Empty;
            byte[] bytes = new byte[0];
            string status = string.Empty;
            try
            {
                try
                {

                    loanId = await Step1(context, ronObj, videoRow, vQueueItem);

                    if (!string.IsNullOrWhiteSpace(loanId))
                    {
                        vQueueItem.LoanId = ronObj.LoanId = loanId;
                        bytes = await Step2(context, ronObj, videoRow, vQueueItem);
                    }
                    else
                    {
                        return VidoeTransferResult.FailedNoLoanId;
                    }

                    if (bytes.Length>0)
                        uploadStatus = await Step3(context, ronObj, videoRow, vQueueItem, bytes);

                    if (uploadStatus)
                    {
                        videoItem=await Step4(context, ronObj, videoRow, vQueueItem,uploadStatus);
                    }

                    if (string.IsNullOrWhiteSpace(loanId))
                        return VidoeTransferResult.FailedNoLoanId;

                    if (bytes.Length== 0)
                        return VidoeTransferResult.FailedNoBytesVideo;

                    if (!uploadStatus)
                        return VidoeTransferResult.FailedUpladFailed;

                    if (videoItem==null)
                        return VidoeTransferResult.FailedFinalDataRecording;

                    return VidoeTransferResult.Success;

                }
                catch (Exception ex)
                {
                    ronObj.LogError( "F1xxx.");
                    VideoItem ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));
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

            return sta;
        }

        private static async Task<string> Step1(IDurableOrchestrationContext context, RonLoggerObject ronObj, VideoItem videoRow, VideoQueueItem vQueueItem)
        {
            ronObj.LogInfomration( RonEventId.OrchestratorProcesssStarted, $"Orchestrator Started. FileId:{vQueueItem.FileId}");

            var loanId = await context.CallActivityAsync<string>("GetLoanId", vQueueItem.BlendId);
            if (string.IsNullOrWhiteSpace(loanId))
            {
                ronObj.LogError(RonEventId.OrchestratorLoanIdNotFound, $"Orchestrator LoanId not found. FileId:{vQueueItem.FileId}");
                VideoItem ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, "unable to get the loan Id"));
                return string.Empty;
            }
            return loanId;
        }

        private static async Task<byte[]> Step2(IDurableOrchestrationContext context, RonLoggerObject ronObj, VideoItem videoRow, VideoQueueItem vQueueItem)
        {
            //vQueueItem.LoanId = loanId;
            //ronObj.LoanId = loanId;
            ronObj.LogInfomration(RonEventId.OrchestratorLoanIdRetreived, $"Orchestrator Got LoanId. FileId:{vQueueItem.FileId} LoanID={vQueueItem.LoanId}");

            var bytes = await context.CallActivityWithRetryAsync<byte[]>("GetVideo",
                new RetryOptions(TimeSpan.FromSeconds(5), 4)
                {
                    Handle = TimexpiredExceptionHandler()
                },
                vQueueItem);

            if (bytes.Length == 0)
            {
                ronObj.LogError(RonEventId.OrchestratorVideoDownloadFailed, $"Orchestrator video file download unsuccessful. FileId:{vQueueItem.FileId}");
                VideoItem ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, "video file download unsuccessful"));
                return bytes;
            }

            return bytes;
        }

        private static async Task<bool> Step3(IDurableOrchestrationContext context, RonLoggerObject ronObj, VideoItem videoRow, VideoQueueItem vQueueItem, byte[] bytes)
        {
            ronObj.LogInfomration(RonEventId.OrchestratorVideDownloaded, $"Orchestrator Got video. FileId:{vQueueItem.FileId} LoanID={vQueueItem.LoanId}");

            VideoContent vc = new VideoContent(vQueueItem.BlendId, vQueueItem.LoanId, vQueueItem.CloseId, vQueueItem.FileId, bytes);

            bool upload2Blob = Environment.GetEnvironmentVariable("Upload2Blob") == "true";

            bool success = false;
            if (upload2Blob)
                success = await context.CallActivityAsync<bool>("UploadVideo2Blob", vc);
            else
                success = await context.CallActivityAsync<bool>("UploadVideo", vc);

            if (success)
                ronObj.LogInfomration(RonEventId.OrchestratorVideoUploaded, $"Orchestrator Video Uploaded. FileId:{vQueueItem.FileId} LoanID={vQueueItem.LoanId}");
            else
                ronObj.LogError(RonEventId.OrchestratorVideoUploadFailed, "Orchestrator Video Uploaded. FileId:{vQueueItem.fileId} LoanID={loanId}");

            return success;
            //return success ? "Completed" : "Failed";
        }

        private static async Task<VideoItem> Step4(IDurableOrchestrationContext context, RonLoggerObject ronObj, VideoItem videoRow, VideoQueueItem vQueueItem,bool success)
        {
            string status=success ? "Completed" : "Failed";
            VideoItem sta = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));

            ronObj.LogInfomration(RonEventId.OrchestratorTableUpsertCompleted, $"Orchestrator record upsert completed. FileId:{vQueueItem.FileId} LoanID={vQueueItem.LoanId}");

            return sta;
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
        
        private static RonLoggerObject CreateRonLoggerObject(ILogger log, VideoQueueItem qItem)
        {
            return new RonLoggerObject(log)
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
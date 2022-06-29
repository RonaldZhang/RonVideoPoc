using System.Collections.Generic;
using System.Threading.Tasks;
using RonVideo.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
//using Microsoft.Azure.WebJobs.Host;
using System;
using RonVideo.Exceptions;
using Microsoft.Extensions.Logging;
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
            var vQueueItem = vInput.vq;
            var videoRow = vInput.vr;

            string status = await internalCall(context,log);

            return true;
        }

        private static async Task<string> internalCall(IDurableOrchestrationContext context, ILogger log)
        {
            var vInput = context.GetInput<OrchestratorInput>();
            var vQueueItem = vInput.vq;
            var videoRow = vInput.vr;
            string status = string.Empty;
            var success = false;
            VideoItem ss;
            try
            {
                try
                {
                    log.LogInformation("Calling F1.");

                    var loanId = await context.CallActivityAsync<string>("GetLoanId", vQueueItem.BlendId);
                    if (string.IsNullOrWhiteSpace(loanId))
                    {
                        status = "unable to get the loan Id";
                        ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));
                        return "failed";
                    }

                    log.LogInformation("Calling F2.");

                    var bytes = await context.CallActivityWithRetryAsync<byte[]>("GetVideo",
                        new RetryOptions(TimeSpan.FromSeconds(5), 4)
                        {
                            Handle = TimexpiredExceptionHandler()
                        },
                        vQueueItem);

                    log.LogInformation("Calling F3.");

                    if (bytes.Length == 0)
                    {
                        status = "video file download unsuccessful";
                        ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));
                        return "failed";
                    }

                    log.LogInformation("Calling F4.");
                    VideoContent vc = new VideoContent(vQueueItem.BlendId, loanId, vQueueItem.CloseId, vQueueItem.FileId, bytes);

                    bool upload2Blob = Environment.GetEnvironmentVariable("Upload2Blob") == "true";
                    if (upload2Blob)
                        success = await context.CallActivityAsync<bool>("UploadVideo2Blob", vc);
                    else
                        success = await context.CallActivityAsync<bool>("UploadVideo", vc);

                    log.LogInformation("Calling F5.");
                    status = success ? "Completed" : "Failed";
                    ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));

                }
                catch (Exception ex)
                {
                    log.LogError("Calling F1xxx.");
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


    }
}
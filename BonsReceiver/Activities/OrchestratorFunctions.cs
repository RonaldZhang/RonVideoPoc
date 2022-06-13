using System.Collections.Generic;
using System.Threading.Tasks;
using RonVideo.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
//using Microsoft.Azure.WebJobs.Host;
using Grpc.Core.Logging;
using System;
using RonVideo.Exceptions;
//using Microsoft.Extensions.Logging;

namespace RonVideo.Activities
{
    public static class OrchestratorFunctions
    {
        [FunctionName(nameof(TransferOrchestrator))]
        public static async Task<bool> TransferOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context
        )
        {
            //log = context.CreateReplaySafeLogger(log);
            var vInput = context.GetInput<OrchestratorInput>();
            var vQueueItem = vInput.vq;
            var videoRow = vInput.vr;

            string status = await internalCall(context);

            if (status != "Completed")
            {
                string bb = await context.CallActivityAsync<string>("Requeue", vQueueItem);
            }
            return true;
        }

        private static async Task<string> internalCall(IDurableOrchestrationContext context)
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
                   
                    var loanId = await context.CallActivityAsync<string>("GetLoanId", vQueueItem.BlendId);
                    if (string.IsNullOrWhiteSpace(loanId))
                    {
                        status = "unable to get the loan Id";
                        ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));
                        return "failed";
                    }

                    //var bytes = await context.CallActivityAsync<byte[]>("GetVideo", vQueueItem.FileId);
                    var bytes = await context.CallActivityWithRetryAsync<byte[]>("GetVideo", 
                        new RetryOptions(TimeSpan.FromSeconds(5), 4) { Handle=ex=>
                        {
                            if (ex.InnerException is null)
                                return false;
                            return ex.InnerException is TimeExpiredException;
                        }
                    },
                        vQueueItem.FileId);

                    if (bytes.Length == 0)
                    {
                        status = "video file download unsuccessful";
                        ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));
                        return "failed";
                    }

                    VideoContent vc = new VideoContent();
                    vc.BlendId = vQueueItem.BlendId;
                    vc.CloseId = vQueueItem.CloseId;
                    vc.LoanId = loanId;
                    vc.FileId = vQueueItem.FileId;
                    vc.Bytes = bytes;
                    success = await context.CallActivityAsync<bool>("UploadVideo", vc);
                    status = success ? "Completed" : "Failed";
                    ss = await context.CallActivityAsync<VideoItem>("Upsert", (videoRow, vQueueItem, status));

                }
                catch (Exception ex)
                {
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
    }
}
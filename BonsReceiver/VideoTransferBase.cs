using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using RonVideo.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RonVideo
{
    public class VideoTransferBase
    {
        protected static async Task<VidoeTransferResult> WaitUntilCompleted(IDurableOrchestrationClient starter, string instanceId)
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
            } while (s == null || s.RuntimeStatus == OrchestrationRuntimeStatus.Running || s.RuntimeStatus == OrchestrationRuntimeStatus.Pending || s.RuntimeStatus == OrchestrationRuntimeStatus.ContinuedAsNew || s.RuntimeStatus == OrchestrationRuntimeStatus.Unknown);
            //return false;
            return s.Output.ToObject<VidoeTransferResult>();
        }
    }
}
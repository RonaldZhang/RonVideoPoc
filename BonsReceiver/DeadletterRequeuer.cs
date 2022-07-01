using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Queue;
using RonVideo.Utilities;
using RonVideo.Models;


namespace RonVideo
{
    public class DeadletterRequeuer
    {
        private readonly ICloudQueueManager _cqManager;
        private RonLoggerObject setting = null;

        public DeadletterRequeuer(ICloudQueueManager cqm)
        {
            _cqManager = cqm;
        }

        [FunctionName("DeadletterRequeuer")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,  "post", Route = "support/requeue/{queueName}")] HttpRequest req,
            string queueName,
            ILogger log)
        {

            setting = CreateRonLoggerObject(log);
            setting.LogInfomration(RonEventId.DeadletterRequeueTriggered, "Reprocessing Deadletters Triggered");


            // Get the connection string from app settings
            string connectionString = "UseDevelopmentStorage=true";

            // string queuename = "bonsqueue";
            ICloudQueueWrapper targetqueue = _cqManager.GetCloudQueueRef(connectionString, queueName);
            ICloudQueueWrapper poisonqueue = _cqManager.GetCloudQueueRef(connectionString, queueName + "-poison");

            int successCount = 0;
            int totalCount = 0;
            CloudQueueMessage  msg = null;
            while (true)
            {
                try
                {
                    msg = await poisonqueue.GetMessageAsync();
                    if (msg == null)
                        break;
                    totalCount++;

                    setting = UpdateRonLoggerObject(setting, msg);
                    setting.LogInfomration( RonEventId.DeadletterRequeueFileStarted, "Poisoned Message" + msg.Id);


                    string id = msg.Id;
                    string popReceipt = msg.PopReceipt;
                    await targetqueue.AddMessageAsync(msg);
                    setting = UpdateRonLoggerObject(setting, msg);
                    setting.LogInfomration( RonEventId.DeadletterRequeueProcessing, $"Add Poisoned Message to Queue {id}");
                    await poisonqueue.DeleteMessageAsync(id, popReceipt);
                    setting.LogInfomration( RonEventId.DeadletterRequeueProcessing, $"Delete Poisoned Message to Queue {id}");
                    successCount++;
                }
                catch (Exception ex)
                {
                    setting.LogInfomration( RonEventId.DeadletterRequeueFailed, $"Exception in Requeuing Poisoned Message {ex.Message}");
          
                }
            }

            return new OkObjectResult($"Reprocessed messages from the {poisonqueue.Name} queue. total: { totalCount} success: {successCount}");
        }

        private static RonLoggerObject UpdateRonLoggerObject(RonLoggerObject obj, CloudQueueMessage entity)
        {
            if ((entity != null) && (obj != null))
            {
                obj.BlendId = entity.Id;
            }

            return obj;
        }

        private static RonLoggerObject CreateRonLoggerObject(ILogger log)
        {
            return new RonLoggerObject(log)
            {
                Id = RonEventId.BonEventTriggerred,
                EntityType = EntityType.DeadletterRequeuer.ToString(),
                BonsEventId = "",
                CloseId = "",
                BlendId = "",
                FileId = "",
                LoanId = "",
                Status = ""
            };
        }

     
    }
}

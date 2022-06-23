using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
//using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
//using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage;
//using Microsoft.WindowsAzure.Storage.Queue;
using Azure.Storage.Queues;
using Microsoft.Azure.Storage.Queue;
using RonVideo.Utilities;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Queue;

namespace RonVideo
{
    public class DeadletterRequeuer
    {
        private readonly ICloudQueueManager _cqManager;

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
            log.LogInformation("Reprocessing Deadletters Triggered");

            // Get the connection string from app settings
             string connectionString = "UseDevelopmentStorage=true";

            // string queuename = "bonsqueue";
            ICloudQueueWrapper targetqueue = _cqManager.GetCloudQueueRef(connectionString, queueName);
            ICloudQueueWrapper poisonqueue = _cqManager.GetCloudQueueRef(connectionString, queueName + "-poison");

            int count = 0;
            CloudQueueMessage  msg = null;
            while (true)
            {
                try
                {
                    msg = await poisonqueue.GetMessageAsync();
                    if (msg == null)
                        break;
                    log.LogInformation("Poisoned Message" + msg.Id);
                    string id = msg.Id;
                    string popReceipt = msg.PopReceipt;
                    await targetqueue.AddMessageAsync(msg);
                    log.LogInformation( $"Add Poisoned Message to Queue {id}");
                    await poisonqueue.DeleteMessageAsync(id, popReceipt);
                    log.LogInformation($" Delete Poisoned Message to Queue {id}");
                    count++;
                }
                catch (Exception ex)
                {
                    log.LogInformation("Exception in Requeuing Poisoned Message " + ex.Message);
                }
            }

            return new OkObjectResult($"Reprocessed {count} messages from the {poisonqueue.Name} queue.");
        }

    //    private static CloudQueue GetCloudQueueRef(string storageAccountString, string queuename)
    //    {
    //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountString);
    //        CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
    //        CloudQueue queue = queueClient.GetQueueReference(queuename);

    //        return queue;
    //    }
    }
}

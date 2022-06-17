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
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Queue;

namespace RonVideo
{
    public static class DeadletterRequeuer
    {
        [FunctionName("DeadletterRequeuer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,  "post", Route = "support/requeue/{queueName}")] HttpRequest req,
            //[Queue("{queueName}")] Microsoft.WindowsAzure.Storage.Queue.CloudQueue targetqueue,
            //[Queue("{queueName}-poison")] Microsoft.WindowsAzure.Storage.Queue.CloudQueue poisonqueue,
            //IBinder binder,
            string queueName,
            //[Queue("bonsqueue")] ICollector<string> outputQueueItem,
            ILogger log)
        {
            log.LogInformation("Reprocessing Deadletters Triggered");

            //CloudQueue targetqueue = await binder.BindAsync<CloudQueue>(new QueueAttribute(queueName));
            //CloudQueue poisonqueue = await binder.BindAsync<CloudQueue>(new QueueAttribute(queueName+ "-poison"));

            // Get the connection string from app settings
             string connectionString = "UseDevelopmentStorage=true";

            // Instantiate a QueueClient which will be used to manipulate the queue
           // QueueClient queueClient = new QueueClient(connectionString, "bonsqueue-poison");
            // string queuename = "bonsqueue";
            CloudQueue targetqueue = GetCloudQueueRef(connectionString, queueName);
            CloudQueue poisonqueue = GetCloudQueueRef(connectionString, queueName + "-poison");

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


            //if (queueClient.Exists())
            //{
            //    // Receive and process 20 messages
            //    QueueMessage[] receivedMessages = queueClient.ReceiveMessages(20, TimeSpan.FromMinutes(5));

            //    foreach (QueueMessage message in receivedMessages)
            //    {
            //        // Process (i.e. print) the messages in less than 5 minutes
            //        Console.WriteLine($"De-queued message: '{message.Body}'");

            //        // Delete the message
            //        queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
            //        outputQueueItem.Add(message);
            //    }
            //}

            return new OkObjectResult($"Reprocessed {count} messages from the {poisonqueue.Name} queue.");
        }

        private static CloudQueue GetCloudQueueRef(string storageAccountString, string queuename)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queuename);

            return queue;
        }
    }
}

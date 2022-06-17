//using Azure.Messaging.EventHubs;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Extensions.Logging;
//using RonVideo.Utilities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FileShareConsole
//{
//    public class BonsEventReceiverFromHub
//    {
//        private readonly IKeyVaultManager _kvManager;

//        public BonsEventReceiverFromHub(IKeyVaultManager kvm)
//        {
//            _kvManager = kvm;
//        }

//        [FunctionName("EventHubTriggerCSharp")]
//        public async Task Run(
//            [EventHubTrigger("samples-workitems", Connection = "EventHubConnectionAppSetting")] string myEventHubMessage,
//            //[EventHubTrigger("samples-workitems", Connection = "EventHubConnectionAppSetting")] EventData  myEventHubMessage, 
//            [Queue("bonsqueue")] ICollector<string> outputQueueItem,
//            ILogger log)
//        {
//            log.LogInformation($"C# function triggered to process a message: {myEventHubMessage}");

//            //log.LogInformation($"Event: {Encoding.UTF8.GetString(myEventHubMessage.Body)}");

//            //// Metadata accessed by binding to EventData
//            //log.LogInformation($"EnqueuedTimeUtc={myEventHubMessage.SystemProperties.EnqueuedTimeUtc}");
//            //log.LogInformation($"SequenceNumber={myEventHubMessage.SystemProperties.SequenceNumber}");
//            //log.LogInformation($"Offset={myEventHubMessage.SystemProperties.Offset}");
//            //// Metadata accessed by using binding expressions in method parameters
//            //log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
//            //log.LogInformation($"SequenceNumber={sequenceNumber}");
//            //log.LogInformation($"Offset={offset}");
//            await Task.Delay(500);
//            string data = myEventHubMessage;
//            try
//            {
//                if (!string.IsNullOrWhiteSpace(data))
//                    outputQueueItem.Add(data);
//                else
//                    log.LogInformation("No payload was found in the event. skipped!");
//            }
//            catch (Exception ex)
//            {
//                return;
//            }
//        }
//    }
//}

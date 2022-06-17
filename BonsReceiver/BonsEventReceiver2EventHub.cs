//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;


//namespace RonVideo
//{
//    public  class BonsEventReceiver2EventHub
//    {
//        [FunctionName("BonsEventReceiver2EventHub")]

//        public async Task<IActionResult> Run([
//            HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
//            [EventHub("outputEventHubMessage", Connection = "EventHubConnectionAppSetting")] IAsyncCollector<string> eventsToSend,
//            ILogger log)
//        {
//            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

//            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
//            await eventsToSend.AddAsync(requestBody).ConfigureAwait(false);
//            await Task.Delay(500);
//            string responseMessage = "Event Received";
//            return new OkObjectResult(responseMessage);
//        }
//    }

    
//}

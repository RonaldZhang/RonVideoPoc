using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using RonVideo.Utilities;

namespace BonsReceiver
{
    public  class BonsEventReceiver
    {
        private  readonly IKeyVaultManager _kvManager;

        public BonsEventReceiver(IKeyVaultManager kvm)
        {
            _kvManager = kvm;
        }
        [FunctionName("ReceiverFunc")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("bonsqueue")] ICollector<string> outputQueueItem,
            ILogger log)
        {
            log.LogInformation("In BONS Event Recevier: " + Helper.GetEnvironmentVariable("AzureWebJobsStorage"));
            log.LogInformation("HTTP trigger function processed a request.");
            string data = string.Empty;
            try
            {
                string value = await _kvManager.GetSecret("MyName");
                log.LogInformation("My Secret is " + value);
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                data = JsonConvert.DeserializeObject(requestBody).ToString();
                log.LogInformation($"Body: {data}");
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse(404));
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(data))
                    outputQueueItem.Add(data);
                else 
                    log.LogInformation("No payload was found in the event. skipped!");
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse(500));
            }
            string responseMessage = "Event Received";
            return new OkObjectResult(responseMessage);
        }
    }
}

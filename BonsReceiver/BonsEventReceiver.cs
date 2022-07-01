using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RonVideo.Utilities;
using RonVideo.Models;

namespace BonsReceiver
{
    public  class BonsEventReceiver
    {

        private RonLoggerObject setting = null;
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
            setting = CreateRonLoggerObject(log);
            setting.LogInfomration( RonEventId.BonEventTriggerred, "{req}");

            string responseMessage = "Event Received";
            try
            {
                string data = await String2Object(req);

                if (!string.IsNullOrWhiteSpace(data))
                {
                    outputQueueItem.Add(data);
                    setting.LogInfomration(RonEventId.BonEventReceived, "Successful");
                }
                else
                {
                    setting.LogError( RonEventId.BonEventNoData, "No payload was found in the event. skipped!");
                    responseMessage = "No Event Received";
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse(500));
            }

            return new OkObjectResult(responseMessage);
        }

        private async Task<string> String2Object(HttpRequest req)
        {
            string data = string.Empty;
            string value = await _kvManager.GetSecret("MyName");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            data = JsonConvert.DeserializeObject(requestBody).ToString();
            return data;
        }

        private static RonLoggerObject CreateRonLoggerObject(ILogger log)
        {
            return new RonLoggerObject(log)
            {
                Id=RonEventId.BonEventTriggerred,
                EntityType = EntityType.EventReceiver.ToString(),
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

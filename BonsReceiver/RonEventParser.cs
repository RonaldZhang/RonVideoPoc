using System;
using System.Collections.Generic;
using RonVideo.Models;
using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RonVideo.EventModels;
using System.Linq;
using RonVideo.Utilities;

namespace BonsReceiver
{
    public  class RonEventParser
    {
        [FunctionName("RonParserFunc")]
        public  void Run([QueueTrigger("bonsqueue", Connection = "")]string myQueueItem,
            [Queue("videosubsqueue")] ICollector<string> outputQueueItem,
            ILogger log)
        {
            log.LogInformation("In RON Event Parser: " +  Helper.GetEnvironmentVariable("AzureWebJobsStorage"));
            log.LogInformation($"BONSQueue triggered: {myQueueItem}");

            var dto = JsonConvert.DeserializeObject<Event>(myQueueItem);

            log.LogInformation($"BONSQueue Queue trigger processed: {JsonConvert.SerializeObject(dto)}");

            List<string> fileIds = dto.Data.Fields.RecordingFiles.Select(x => x.FileId).ToList();

            if ((dto.Data.Type == "closing") && (dto.Data.Action == "updated"))
            {
                log.LogInformation($"RON Video Event received Closing Id: {dto.Data.Id}");

                fileIds.ForEach(x =>
                {

                    outputQueueItem.Add(JsonConvert.SerializeObject(new VideoQueueItem(dto.Id, string.Empty, dto.Data.Id, x)));
                });
            }
            else
            {
                log.LogInformation($"Other Event received, Skipped.  type: {dto.Data.Type}, action:{dto.Data.Action}");
            }

        }
    }
}

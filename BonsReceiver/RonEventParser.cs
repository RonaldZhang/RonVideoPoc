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
        private RonLoggerObject setting = null;

        [FunctionName("RonParserFunc")]
        public void Run([QueueTrigger("bonsqueue", Connection = "")]string myQueueItem,
            [Queue("videosubsqueue")] ICollector<string> outputQueueItem,
            ILogger log)
        {
            setting = CreateRonLoggerObject(log);
            setting.LogInfomration( RonEventId.RonEventParserTriggerred, $"{myQueueItem}");
            Event dto = Convert2Object(myQueueItem, log);

            if (dto != null)
            {
                setting.BonsEventId = dto.Id;
                setting.LogInfomration( RonEventId.RonEventParserReceived, $"{JsonConvert.SerializeObject(dto)}");
            }

            ProcessBonsEvent(outputQueueItem, log, setting, dto);
        }

        private static void ProcessBonsEvent(ICollector<string> outputQueueItem, ILogger log, RonLoggerObject setting, Event dto)
        {
            List<string> fileIds = dto?.Data?.Fields?.RecordingFiles?.Select(x => x.FileId).ToList();

            if ((dto?.Data?.Type == "closing") && (dto?.Data?.Action == "updated"))
            {
                setting.BonsEventId = dto.Id;
                setting.LogInfomration( RonEventId.RonEventParserReceived, $"RON Video Event received Closing Id: {dto.Data.Id}");

                fileIds.ForEach(x =>
                {
                    var newitem = new VideoQueueItem(dto.Id, string.Empty, dto.Data.Id, x);
                    outputQueueItem.Add(JsonConvert.SerializeObject(newitem));
                    setting.BlendId = newitem.BlendId;
                    setting.CloseId = newitem.CloseId;
                    setting.FileId = newitem.FileId;
                    setting.LogInfomration( RonEventId.BonEventProccessed, $"{JsonConvert.SerializeObject(dto)}");
     
                });
            }
            else if (dto != null)
            {
                setting.LogInfomration( RonEventId.RonEventParserOtherEvents, $"Other Event received, Skipped.  type: {dto?.Data?.Type}, action:{dto?.Data?.Action}");
              }
            else
            {
                setting.LogInfomration( RonEventId.RonEventParserInvalidData, "Invalid payload received Event received, Skipped.");
            }
        }

        private  Event Convert2Object(string myQueueItem, ILogger log)
        {
            Event dto = null;
            try
            {
                dto = JsonConvert.DeserializeObject<Event>(myQueueItem);
            }
            catch
            {
                setting.LogInfomration( RonEventId.BonEventParseError, "Unexpected Payload received, Skipped");
            }

            return dto;
        }

        private static RonLoggerObject CreateRonLoggerObject(ILogger log)
        {
            return new RonLoggerObject(log)
            {
                Id = RonEventId.RonEventParserTriggerred,
                EntityType = EntityType.RonEventParser.ToString(),
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

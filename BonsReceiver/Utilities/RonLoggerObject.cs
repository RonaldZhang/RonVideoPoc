using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Utilities
{
    public class RonLoggerObject
    {
 
        // EntityType: Business Entity Type being processed: e.g. Receiver, Parser, etc.
        // EntityId: Id of the Business Entity being processed: e.g. EventId etc. 
        // Status: Free Status Descrption
        // CorrelationId: BonEvent Id. 
        // Description: A detailed description of the log event. 
        public const string Template = "{EventIdDescription}, {EntityType},{BonsEventId}, {CloseId}, {BlendId}, {fileId}, {loanId}, {Status}";

        public RonEventId Id { get; set; }
        public string EventIdDescription 
        { get 
            {
                return Id.ToString();
            } 
        }
        public string EntityType { get; set; }
        public string BonsEventId { get; set; }
        public string CloseId { get; set; }
        public string BlendId { get; set; }
        public string FileId { get; set; }
        public string LoanId { get; set; }
        public string Status { get; set; }

        public object[] ToObjectArray()
        {
            var objs = new object[8];
            objs[0] = EventIdDescription;
            objs[1] = EntityType;
            objs[2] = BonsEventId;
            objs[3] = CloseId;
            objs[4] = BlendId;
            objs[5] = FileId;
            objs[6] = LoanId;
            objs[7] = Status;

            return objs;
        }

        public void LogInfomration(ILogger log)
        {
            log.LogInformation(new EventId((int)Id), RonLoggerObject.Template, ToObjectArray());
        }

        public void LogInfomration(ILogger log, string status)
        {
            this.Status=status;
            log.LogInformation(new EventId((int)Id), RonLoggerObject.Template, ToObjectArray());
        }


        public void LogInfomration(ILogger log, RonEventId id, string status)
        {
            this.Id = id;
            this.Status = status;
            log.LogInformation(new EventId((int)Id), RonLoggerObject.Template, ToObjectArray());
        }

       
        public void LogError(ILogger log)
        {
            log.LogError(new EventId((int)Id), RonLoggerObject.Template, ToObjectArray());
        }

        public void LogError(ILogger log, string status)
        {
            this.Status = status;
            log.LogError(new EventId((int)Id), RonLoggerObject.Template, ToObjectArray());
        }


        public void LogError(ILogger log, RonEventId id, string status)
        {
            this.Id = id;
            this.Status = status;
            log.LogError(new EventId((int)Id), RonLoggerObject.Template, ToObjectArray());
        }

        public void LogDebug(ILogger log)
        {
            log.LogDebug(new EventId((int)Id), RonLoggerObject.Template, ToObjectArray());
        }

        public void LogDebug(ILogger log, string status)
        {
            this.Status = status;
            log.LogDebug(new EventId((int)Id), RonLoggerObject.Template, ToObjectArray());
        }


        public void LogDebug(ILogger log, RonEventId id, string status)
        {
            this.Id = id;
            this.Status = status;
            log.LogDebug(new EventId((int)Id), RonLoggerObject.Template, ToObjectArray());
        }
    }
}


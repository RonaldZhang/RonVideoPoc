using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Utilities
{
    public enum RonEventId
    {
        BonEventTriggerred = 1000,
        BonEventReceived= 1010,
        BonEventParseError = 1015,
        BonEventNoData = 1020,
        BonEventProccessed = 1030,
        RonEventParserTriggerred =2000,
        RonEventParserReceived = 2010,
        RonEventParserInvalidData = 2020,
        RonEventParserOtherEvents = 2030,
        RonEventParserProccessed =2040,
        VideoTransferTriggered = 3000,
        VideoTransferInProccess = 3005,
        VideoTransferReceived = 3010,
        VideoTransferRecordFound=3005,
        VideoTransferNewProcessing = 3014,
        VideoTransferReprocessing = 3015,
        VideoTransferInvalidData = 3020,
        VideoTransferProccessed = 3030,
        VideoTransferSkipped = 3040,
        BatchVideoTransferTriggered=4000,
        BatchVideoTransferFileStarted = 4010,
        BatchVideoTransferFileProcessing = 4020,
        BatchVideoTransferFileProcessed = 4030,
        DeadletterRequeueTriggered=5000,
        DeadletterRequeueFileStarted = 5010,
        DeadletterRequeueProcessing = 5020,
        DeadletterRequeueProcessed = 5030,
        DeadletterRequeueFailed = 5040,
        OrchestratorTriggered=6000,
        OrchestratorProcesssStarted = 6001,
        OrchestratorLoanIdNotFound= 6005,
        OrchestratorLoanIdRetreived = 6010,
        OrchestratorVideoDownloadFailed = 6015,
        OrchestratorVideDownloaded = 6020,
        OrchestratorVideoUploadFailed = 6025,
        OrchestratorVideoUploaded = 6030,
        OrchestratorTableUpsertCompleted = 6040
    }
}

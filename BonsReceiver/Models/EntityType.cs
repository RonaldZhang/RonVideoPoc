using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Models
{
    public enum EntityType
    {
        EventReceiver,
        RonEventParser,
        VidoeTransfer,
        BatchVidoeTransfer,
        DeadletterRequeuer,
        Orchestrator
    }
}


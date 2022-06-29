using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Utilities
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


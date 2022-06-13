using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Models
{
    public class OrchestratorInput
    {
        public OrchestratorInput(VideoQueueItem q, VideoItem r)
        {
            vq = q;
            vr = r;

        }
 
        public VideoQueueItem vq { get; set; }

        public VideoItem vr { get; set; }
    }
}

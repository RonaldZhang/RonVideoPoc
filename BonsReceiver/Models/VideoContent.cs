using RonVideo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Models
{
    public class VideoContent: VideoQueueItem
    {
        public VideoContent()
        {

        }
        public VideoContent(string blendId, string loanId, string closeId, string fileid, byte[] bytes):base( blendId,  loanId,  closeId,  fileid)
        {
            Bytes = bytes;
        }

        public byte[] Bytes { get; set; }
    }
}

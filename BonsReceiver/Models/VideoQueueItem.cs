using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Models
{
    public class VideoQueueItem
    {
        public VideoQueueItem()
        {

        }
        public VideoQueueItem(string blendId, string loanId, string closeId ,string fileid)
        {
            BlendId = blendId;
            LoanId = loanId;
            CloseId = closeId;
            FileId = fileid;
        }

        public String BlendId { get; set; }
        public String LoanId { get; set; }
        public String CloseId { get; set; }
        public String FileId { get; set; }

    }
}

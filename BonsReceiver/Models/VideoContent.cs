namespace RonVideo.Models
{
    public class VideoContent: VideoQueueItem
    {
        public VideoContent(string blendId, string loanId, string closeId, string fileid, byte[] bytes):base( blendId,  loanId,  closeId,  fileid)
        {
            Bytes = bytes;
        }

        public byte[] Bytes { get; set; }
    }
}

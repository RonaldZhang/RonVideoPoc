using Azure;
using System;

namespace RonVideo.Models
{
    public class VideoItem :VideoQueueItem, Azure.Data.Tables.ITableEntity   // Microsoft.Azure.Cosmos.Table.TableEntity //
    {
        public VideoItem()
        {

        }
        public VideoItem(string blendId, string loanId, string closeId, string fileid, int count, string status, string partitionKey, string rowKey) :base(blendId, loanId, closeId, fileid)
        {


            Count = count;
            Status = status;
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public int Count { get; set; }
        public string Status { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get ; set ; }
        public ETag ETag { get; set; }

        public VideoItem ShallowCopy()
        {
            return (VideoItem)this.MemberwiseClone();
        }
    }
}

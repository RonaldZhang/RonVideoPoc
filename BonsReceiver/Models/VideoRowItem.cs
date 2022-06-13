using Azure;
using Azure.Data.Tables;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RonVideo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Models
{
    public class VideoRowItem : VideoQueueItem, Azure.Data.Tables.ITableEntity
    {
        public VideoRowItem()
        {

        }
        public VideoRowItem(string blendId, string loanId, string closeId, string fileid, int count, string status, string partitionKey, string rowKey) :base(blendId, loanId, closeId, fileid)
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
        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            throw new NotImplementedException();
        }
    }
}

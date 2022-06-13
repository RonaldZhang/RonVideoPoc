using Azure;
using Azure.Data.Tables;
//using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Models
{
    public class VideoItem :VideoQueueItem, Azure.Data.Tables.ITableEntity   // Microsoft.Azure.Cosmos.Table.TableEntity //
    {
        public VideoItem()
        {

        }
        public VideoItem(string blendId, string loanId, string closeId, string fileid, int count, string status, string partitionKey, string rowKey) :base(blendId, loanId, closeId, fileid)
        //public VideoItem(string blendId, string loanId, string closeId, string fileid, int count, string status, string partitionKey, string rowKey) 

        {

            //BlendId = blendId;
            //LoanId = loanId;
            //CloseId = closeId;
            //FileId = fileid;

            Count = count;
            Status = status;
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        //public String BlendId { get; set; }
        //public String LoanId { get; set; }
        //public String CloseId { get; set; }
        //public String FileId { get; set; }

        public int Count { get; set; }
        public string Status { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get ; set ; }
        public ETag ETag { get; set; }
        //public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        //{
        //    throw new NotImplementedException();
        //}

        //public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

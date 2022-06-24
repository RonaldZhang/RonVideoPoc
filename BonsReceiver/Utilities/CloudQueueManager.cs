using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RonVideo.Utilities
{
    [ExcludeFromCodeCoverage]
    public class CloudQueueManager: ICloudQueueManager
    {
        public  ICloudQueueWrapper GetCloudQueueRef(string storageAccountString, string queuename)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queuename);

            return new CloudQueueWrapper(queue);
        }
    }
}

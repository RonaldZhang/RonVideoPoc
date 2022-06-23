using Microsoft.Azure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Utilities
{
    public interface ICloudQueueManager
    {
        public ICloudQueueWrapper GetCloudQueueRef(string storageAccountString, string queuename);
    }
}

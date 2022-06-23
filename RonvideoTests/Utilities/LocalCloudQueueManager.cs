using Microsoft.Azure.Storage.Queue;
using RonVideo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests.Utilities
{

    public class LocalCloudQueueManager : ICloudQueueManager
    {
        public ICloudQueueWrapper GetCloudQueueRef(string storageAccountString, string queuename)
        {

            CloudQueue queue = new CloudQueue(new Uri("http://dotnetperls.com/"+ queuename));
            LocalCloudQueueWrapper wrapper= new LocalCloudQueueWrapper();

            if (queuename.Contains("myBadQueue") && queuename.Contains("poison"))
            {
                for (int i = 0; i < 3; i++)
                {
                    int j = i + 1;
                    var msg = new CloudQueueMessage(j.ToString(), "PopReceipt"+j.ToString());
                    wrapper.AddMessageAsync(msg).ConfigureAwait(true);
                }
            }
            
            return wrapper;
        }
    }

}

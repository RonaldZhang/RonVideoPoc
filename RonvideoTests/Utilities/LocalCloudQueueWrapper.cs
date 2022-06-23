using Microsoft.Azure.Storage.Queue;
using RonVideo.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests.Utilities
{
    public class LocalCloudQueueWrapper : ICloudQueueWrapper
    {
        //private readonly CloudQueue _cloudQueue;
        private ConcurrentDictionary<string, CloudQueueMessage> _queue = null;
        private string _name = "local";

        public LocalCloudQueueWrapper()
        {
            //_cloudQueue = cloudQueue;
            _queue = new ConcurrentDictionary<string, CloudQueueMessage>();
        }

        public string Name { get { return _name; } }

        public async Task AddMessageAsync(CloudQueueMessage message)
        {
            _queue.TryAdd(message.Id, message);
            await Task.Delay(10);
        }

        public async Task<CloudQueueMessage> GetMessageAsync()
        {
            CloudQueueMessage item;

            item = _queue.FirstOrDefault().Value;
            await Task.Delay(10);
            return item;
        }

        public async Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            CloudQueueMessage item;
            _queue.TryRemove(messageId, out item);
            await Task.Delay(10);
        }

    }
}

using Microsoft.Azure.Storage.Queue;
using System.Threading.Tasks;

namespace RonVideo.Utilities
{
    public class CloudQueueWrapper : ICloudQueueWrapper
    {
        private CloudQueue _cloudQueue;

        public CloudQueueWrapper()
        {

        }
        public CloudQueueWrapper(CloudQueue cloudQueue)
        {
            _cloudQueue = cloudQueue;
        }

        public CloudQueue Queue { get { return _cloudQueue; } set { _cloudQueue = value; } }
        public string Name { get { return _cloudQueue.Name; } }

        public async Task AddMessageAsync(CloudQueueMessage message)
        {
            await _cloudQueue.AddMessageAsync(message);
        }

        public async Task<CloudQueueMessage> GetMessageAsync()
        {
            return await _cloudQueue.GetMessageAsync();
        }

        public async Task DeleteMessageAsync(string messageId, string popReceipt)
        {
             await _cloudQueue.DeleteMessageAsync(messageId, popReceipt);
        }


 
    }
}

using Microsoft.Azure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RonVideo.Utilities
{
    public interface ICloudQueueWrapper
    {
        string Name { get; }
        Task AddMessageAsync(CloudQueueMessage message);
        Task<CloudQueueMessage> GetMessageAsync();
        Task DeleteMessageAsync(string messageId, string popReceipt);
    }
}

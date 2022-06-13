using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RonvideoTests.Utilities
{
    public class MockAsyncCollector<T> : IAsyncCollector<T>
    {
        public readonly List<T> Items = new List<T>();

        public Task AddAsync(T item, CancellationToken cancellationToken = default(CancellationToken))
        {

            Items.Add(item);

            return Task.FromResult(true);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }
    }
}

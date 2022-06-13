using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests.Utilities
{
    class MockCollector
    {
    }

    public class MockCollector<T> : ICollector<T>
    {
        public readonly List<T> Items = new List<T>();

        public void Add(T item)
        {
            Items.Add(item);
        }

       
    }
}

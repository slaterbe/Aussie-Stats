using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtlEnqueue.Command
{
    public interface IQueueCommand
    {
        Task Enqueue(List<string> items);
    }

    public class QueueCommand : IQueueCommand
    {
        public Task Enqueue(List<string> items)
        {
            throw new NotImplementedException();
        }
    }
}

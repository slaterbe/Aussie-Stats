using Amazon.SQS;
using Amazon.SQS.Model;
using Common.Extension;
using EtlEnqueue.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtlEnqueue.Command
{
    public interface IQueueCommand
    {
        Task Enqueue(List<string> bucketKeys);
    }

    public class QueueCommand : IQueueCommand
    {
        private readonly IAmazonSQS sqs;
        private readonly EnvironmentModel environmentModel;

        public QueueCommand(IAmazonSQS sqs, EnvironmentModel environmentModel)
        {
            this.sqs = sqs;
            this.environmentModel = environmentModel;
        }

        public async Task Enqueue(List<string> bucketKeys)
        {
            var censusQueue = environmentModel.CensusEtlQueueUrl;

            var entries = bucketKeys
                .Select((entry, index) => new SendMessageBatchRequestEntry
                {
                    Id = index.ToString(),
                    MessageBody = JsonConvert.SerializeObject(entry)
                })
                .ToList();

            var batches = entries.Split(10);

            foreach(var batch in batches)
                await this.sqs.SendMessageBatchAsync(censusQueue, batch);
        }
    }
}

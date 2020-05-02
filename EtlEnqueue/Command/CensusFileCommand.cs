using Amazon.S3;
using Amazon.S3.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EtlEnqueue.Command
{
    public interface ICensusFileCommand
    {
        Task<List<string>> GetCensusFiles();
    }

    public class Test : IRequest<string>
    {

    }

    public class TestHandler : IRequestHandler<Test, string>
    {
        public Task<string> Handle(Test request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class CensusFileCommand : ICensusFileCommand
    {
        private readonly IAmazonS3 awsClient;

        public CensusFileCommand(IAmazonS3 awsClient)
        {
            this.awsClient = awsClient;
        }

        public async Task<List<string>> GetCensusFiles()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = "",
                Prefix = ""
            };
            
            var listResponse = await awsClient.ListObjectsV2Async(request);
            var keys = listResponse.S3Objects
                .Select(a => a.Key)
                .ToList();

            while (listResponse.IsTruncated)
            {
                request = new ListObjectsV2Request
                {
                    ContinuationToken = listResponse.NextContinuationToken
                };

                listResponse = await awsClient.ListObjectsV2Async(request);

                var newKeys = listResponse.S3Objects
                    .Select(a => a.Key)
                    .ToList();

                keys.AddRange(newKeys);
            }

            return keys;
        }
    }
}

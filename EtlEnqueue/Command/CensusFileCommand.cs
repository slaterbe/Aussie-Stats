using Amazon.S3;
using Amazon.S3.Model;
using EtlEnqueue.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtlEnqueue.Command
{
    public interface ICensusFileCommand
    {
        Task<List<string>> GetCensusFiles();
    }

    public class CensusFileCommand : ICensusFileCommand
    {
        private readonly IAmazonS3 awsClient;
        private readonly EnvironmentModel environmentModel;

        public CensusFileCommand(IAmazonS3 awsClient, EnvironmentModel environmentModel)
        {
            this.awsClient = awsClient;
            this.environmentModel = environmentModel;
        }

        public async Task<List<string>> GetCensusFiles()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = environmentModel.Bucket,
                Prefix = environmentModel.KeyPrefix
            };

            ListObjectsV2Response listResponse = null;
            listResponse = await awsClient.ListObjectsV2Async(request);
            
            var keys = listResponse.S3Objects
                .Select(a => a.Key)
                .ToList();

            while (listResponse.IsTruncated)
            {
                request = new ListObjectsV2Request
                {
                    BucketName = environmentModel.Bucket,
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

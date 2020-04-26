using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SQS;
using System;

namespace Infrastructure.Stacks
{
    public class CensusEtlStackProps : StackProps
    {
        public string AuroraSeucrityGroupId { get; set; } = String.Empty;
        public string LambdaArtifactBucket { get; set; } = String.Empty;
    }

    public class CensusEtlStack : Stack
    {
        internal CensusEtlStack(Construct scope, string id, CensusEtlStackProps props) : base(scope, id, props)
        {
            var securityGroup = SecurityGroup.FromSecurityGroupId(this, "aurora-sg", props.AuroraSeucrityGroupId);

            var bucket = Bucket.FromBucketName(this, "bucket", props.LambdaArtifactBucket);

            new Function(this, "census-etl", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Handler = "",
                Code = Code.FromBucket(bucket, "default.zip"),
                SecurityGroups = new ISecurityGroup[] { securityGroup }
            });

            var workerFunction = new Function(this, "worker-function", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Handler = "",
                Code = Code.FromBucket(bucket, "default.zip"),
                SecurityGroups = new ISecurityGroup[] { securityGroup }
            });

            var queue = new Queue(this, "queue", new QueueProps
            {
                QueueName = "etl-queue"
            });

            workerFunction.AddEventSource(new SqsEventSource(queue));
        }
    }
}

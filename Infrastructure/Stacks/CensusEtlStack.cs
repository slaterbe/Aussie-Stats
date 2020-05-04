using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;
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
        public string CensusArtifactBucket { get; set; } = String.Empty;
    }

    public class CensusEtlStack : Stack
    {
        internal CensusEtlStack(Construct scope, string id, CensusEtlStackProps props) : base(scope, id, props)
        {
            var securityGroup = SecurityGroup.FromSecurityGroupId(this, "aurora-sg", props.AuroraSeucrityGroupId);

            var artifactBucket = Bucket.FromBucketName(this, "artifactBucket", props.LambdaArtifactBucket);
            var censusBucket = Bucket.FromBucketName(this, "lambdaBucket", props.CensusArtifactBucket);
            var awsManagedLambdaPolicy = ManagedPolicy.FromAwsManagedPolicyName("AWSLambdaBasicExecutionRole");

            var queue = new Queue(this, "queue", new QueueProps
            {
                QueueName = "etl-queue"
            });

            var enqueueRole = new Role(this, "enqueue-role", new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });

            enqueueRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Resources = new string[] { censusBucket.BucketArn },
                Actions = new string[] { "s3:ListBucket" }
            }));
            enqueueRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Resources = new string[] { queue.QueueArn },
                Actions = new string[] { "sqs:SendMessage" }
            }));
            enqueueRole.AddManagedPolicy(awsManagedLambdaPolicy);

            new Function(this, "census-etl", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Handler = "something",
                Code = Code.FromBucket(artifactBucket, "default.zip"),
                SecurityGroups = new ISecurityGroup[] { securityGroup },
                Role = enqueueRole
            });

            var workerRole = new Role(this, "worker-role", new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });
            workerRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Resources = new string[] { censusBucket.BucketArn },
                Actions = new string[] { "s3:GetObject" }
            }));
            workerRole.AddManagedPolicy(awsManagedLambdaPolicy);
            workerRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Resources = new string[] { queue.QueueArn },
                Actions = new string[]
                {
                    "sqs:ReceiveMessage",
                    "sqs:DeleteMessage",
                    "sqs:GetQueueAttributes",
                    "logs:CreateLogGroup",
                    "logs:CreateLogStream",
                    "logs:PutLogEvents"
                }
            }));

            var workerFunction = new Function(this, "worker-function", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Handler = "something",
                Code = Code.FromBucket(artifactBucket, "default.zip"),
                SecurityGroups = new ISecurityGroup[] { securityGroup },
                ReservedConcurrentExecutions = 2,
                Role = workerRole
            });

            workerFunction.AddEventSource(new SqsEventSource(queue));
        }
    }
}

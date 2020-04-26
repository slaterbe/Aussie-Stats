using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.S3;
using System;

namespace Infrastructure.Stacks
{
    public class ApiStackProps : StackProps
    {
        public string LambdaArtifactBucket { get; set; } = String.Empty;
        public string AuroraSecurityGroupId { get; set; } = String.Empty;
    }

    public class ApiStack : Stack
    {
        internal ApiStack(Construct scope, string id, ApiStackProps props) : base(scope, id, props)
        {
            var securityGroup = SecurityGroup.FromSecurityGroupId(this, "aurora-sg", props.AuroraSecurityGroupId);

            var bucket = Bucket.FromBucketName(this, "bucket", props.LambdaArtifactBucket);

            var apiLambda = new Function(this, "apiLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Handler = "something",
                Code = Code.FromBucket(bucket, "default.zip"),
                SecurityGroups = new ISecurityGroup[] { securityGroup },
                ReservedConcurrentExecutions = 2
            });

            new LambdaRestApi(this, "apiGateway", new LambdaRestApiProps
            {
                Handler = apiLambda
            });
        }
    }
}

using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using System;

namespace Infrastructure.Stacks
{
    public class DemoStack : Stack
    {
        internal DemoStack(Construct scope, string id, DemoStackProps props) : base(scope, id, props)
        {
            new Bucket(this, "Aussie-Stats-Asset-Bucket", new BucketProps
            {
                BucketName = props.BucketName,
                PublicReadAccess = true,
                Encryption = BucketEncryption.UNENCRYPTED,
                RemovalPolicy = RemovalPolicy.DESTROY
            });
        }
    }

    public class DemoStackProps : StackProps
    {
        public string BucketName { get; set; } = String.Empty;
    }
}

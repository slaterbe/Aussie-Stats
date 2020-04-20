using Amazon.CDK;
using Amazon.CDK.AWS.S3;

namespace Infrastructure.Stacks
{
    public class S3AssetBucketStack : Stack
    {
        internal S3AssetBucketStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            new Bucket(this, "Aussie-Stats-Asset-Bucket", new BucketProps
            {
                BucketName = "aussie-stats-assets",
                PublicReadAccess = true,
                Encryption = BucketEncryption.UNENCRYPTED,
                RemovalPolicy = RemovalPolicy.RETAIN
            });
        }
    }
}

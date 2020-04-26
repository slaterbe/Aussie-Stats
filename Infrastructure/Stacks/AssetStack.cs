using Amazon.CDK;
using Amazon.CDK.AWS.S3;

namespace Infrastructure.Stacks
{
    public class AssetStackProps : StackProps
    {
        public string AssetBucket { get; set; } = string.Empty;
        public string LambdaAritifactBucket { get; set; } = string.Empty;
    }


    public class AssetStack : Stack
    {
        internal AssetStack(Construct scope, string id, AssetStackProps props) : base(scope, id, props)
        {
            new Bucket(this, "Aussie-Stats-Asset-Bucket", new BucketProps
            {
                BucketName = props.AssetBucket,
                PublicReadAccess = true,
                Encryption = BucketEncryption.UNENCRYPTED,
                RemovalPolicy = RemovalPolicy.RETAIN
            });

            new Bucket(this, "Aussie-Stats-Lambda-Artifacts", new BucketProps
            {
                BucketName = props.LambdaAritifactBucket,
                PublicReadAccess = true,
                Encryption = BucketEncryption.UNENCRYPTED,
                RemovalPolicy = RemovalPolicy.RETAIN
            });
        }
    }
}

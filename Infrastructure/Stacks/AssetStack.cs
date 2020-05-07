using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
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
            var censusBucket = new Bucket(this, "Aussie-Stats-Asset-Bucket", new BucketProps
            {
                BucketName = props.AssetBucket,
                PublicReadAccess = false,
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

            BuildUserAccessTokens(censusBucket);
        }

        private void BuildUserAccessTokens(Bucket bucket)
        {
            var user = new User(this, "census-upload", new UserProps
            {
                UserName = "census-upload"
            });
            user.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Resources = new string[] { bucket.BucketArn },
                Actions = new string[] { "s3:*" }
            }));

            var accessKey = new CfnAccessKey(this, "myAccessKey", new CfnAccessKeyProps
            {
                UserName = user.UserName
            });

            new CfnOutput(this, "userName", new CfnOutputProps
            {
                ExportName = "userName",
                Value = user.UserName
            });
            new CfnOutput(this, "accessKeyId", new CfnOutputProps
            {
                ExportName = "accessKey",
                Value = accessKey.Ref
            });
            new CfnOutput(this, "secretAccessKey", new CfnOutputProps
            {
                ExportName = "secretAccessKey",
                Value = accessKey.AttrSecretAccessKey
            });
        }
    }
}

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
                RemovalPolicy = RemovalPolicy.RETAIN,
                BlockPublicAccess = new BlockPublicAccess(new BlockPublicAccessOptions
                {
                    BlockPublicAcls= true,
                    IgnorePublicAcls = true,
                    BlockPublicPolicy = true,
                    RestrictPublicBuckets = true
                }),
                LifecycleRules = new ILifecycleRule[]
                {
                    new LifecycleRule
                    {
                        Enabled = true,
                        Transitions = new ITransition[]
                        {
                            new Transition
                            {
                                StorageClass = StorageClass.INFREQUENT_ACCESS,
                                TransitionAfter = Duration.Days(30)
                            }
                        }
                    }
                }
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
                Sid = "ListObjectsInBucket",
                Effect = Effect.ALLOW,
                Resources = new string[] { bucket.BucketArn },
                Actions = new string[] { "s3:ListBucket" }
            }));
            user.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Sid = "AllObjectActions",
                Effect = Effect.ALLOW,
                Resources = new string[] { $"{bucket.BucketArn}/*" },
                Actions = new string[] { "s3:*Object" }
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

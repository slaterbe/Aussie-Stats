using Amazon.CDK;
using Infrastructure.Stacks;

namespace Infrastructure
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var accountId = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT");
            var region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION");

            var app = new App();
            
            new S3AssetBucketStack(app, "AssetStack", new StackProps
            {
                StackName = "AssetStack",
                Env = new Amazon.CDK.Environment()
                {
                    Account = accountId,
                    Region = region
                }
            });

            new AuroraDatabaseStack(app, "DatabaseStack", new StackProps
            {
                StackName = "DatabaseStack",
                Env = new Amazon.CDK.Environment()
                {
                    Account = accountId,
                    Region = region
                }
            });

            new DemoStack(app, "DemoStack", new DemoStackProps
            {
                BucketName = "mudbath-demo-stack",
                Env = new Amazon.CDK.Environment()
                {
                    Account = accountId,
                    Region = region
                }
            });

            app.Synth();
        }
    }
}

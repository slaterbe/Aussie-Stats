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
            var auroraSecurityGroupId = "aurora-security-group";
            var assetBucket = "aussie-stats-assets";
            var lambdaArtifactBucket = "aussie-stats-lambda-artifacts";

            var app = new App();
            
            new AssetStack(app, "AssetStack", new AssetStackProps
            {
                StackName = "InfrastructureStack",
                Env = new Amazon.CDK.Environment()
                {
                    Account = accountId,
                    Region = region
                },
                AssetBucket = assetBucket,
                LambdaAritifactBucket = lambdaArtifactBucket
            });

            new AuroraDatabaseStack(app, "DatabaseStack", new AuroraDatabaseStackProps
            {
                StackName = "DatabaseStack",
                Env = new Amazon.CDK.Environment()
                {
                    Account = accountId,
                    Region = region
                }
            });

            new CensusEtlStack(app, "CensusEtlStack", new CensusEtlStackProps
            {
                StackName = "CensusEtlStack",
                Env = new Amazon.CDK.Environment()
                {
                    Account = accountId,
                    Region = region
                },
                AuroraSeucrityGroupId = auroraSecurityGroupId,
                LambdaArtifactBucket = lambdaArtifactBucket
            });

            new ApiStack(app, "ApiStack", new ApiStackProps
            {
                LambdaArtifactBucket = lambdaArtifactBucket,
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

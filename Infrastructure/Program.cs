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

            var env = new Amazon.CDK.Environment()
            {
                Account = accountId,
                Region = region
            };

            var app = new App();
            
            new AssetStack(app, "AssetStack", new AssetStackProps
            {
                StackName = "InfrastructureStack",
                Env = env,
                AssetBucket = assetBucket,
                LambdaAritifactBucket = lambdaArtifactBucket
            });

            new AuroraDatabaseStack(app, "DatabaseStack", new AuroraDatabaseStackProps
            {
                StackName = "DatabaseStack",
                Env = env,
            });

            new CensusEtlStack(app, "CensusEtlStack", new CensusEtlStackProps
            {
                StackName = "CensusEtlStack",
                Env = env,
                AuroraSeucrityGroupId = auroraSecurityGroupId,
                LambdaArtifactBucket = lambdaArtifactBucket
            });

            new ApiStack(app, "ApiStack", new ApiStackProps
            {
                LambdaArtifactBucket = lambdaArtifactBucket,
                Env = env
            });

            app.Synth();
        }
    }
}

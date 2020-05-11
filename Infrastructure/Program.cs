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
            var censusEtlQueue = "census-etl-queue";

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
                CensusArtifactBucket = assetBucket,
                CensusEtlQueue = censusEtlQueue
            });

            new ApiStack(app, "ApiStack", new ApiStackProps
            {
                Env = env
            });

            app.Synth();
        }
    }
}

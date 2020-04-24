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
            var auroraSecurityGroup = "aurora-security-group";

            var app = new App();
            
            new S3AssetBucketStack(app, "AssetStack", new StackProps
            {
                StackName = "InfrastructureStack",
                Env = new Amazon.CDK.Environment()
                {
                    Account = accountId,
                    Region = region
                }
            });

            new AuroraDatabaseStack(app, "DatabaseStack", new AuroraDatabaseStackProps
            {
                StackName = "DatabaseStack",
                AuroraSeucrityGroupId = auroraSecurityGroup,
                Env = new Amazon.CDK.Environment()
                {
                    Account = accountId,
                    Region = region
                }
            });

            new CensusEtlStack(app, "CensusEtlStack", new CensusEtlStackProps
            {
                StackName = "CensusEtlStack",
                AuroraSeucrityGroupId = auroraSecurityGroup,
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

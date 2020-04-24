using Amazon.CDK;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.EC2;
using static Amazon.CDK.AWS.RDS.CfnDBCluster;
using System;

namespace Infrastructure.Stacks
{
    public class AuroraDatabaseStackProps : StackProps
    {
        public string AuroraSeucrityGroupId { get; set; } = String.Empty;
    }

    public class AuroraDatabaseStack : Stack
    {
        internal AuroraDatabaseStack(Construct scope, string id, IStackProps props) : base(scope, id, props)
        {
            var defaultVpc = Vpc.FromLookup(this, "default", new VpcLookupOptions
            {
                IsDefault = true
            });

            var securityGroup = new SecurityGroup(this, id, new SecurityGroupProps
            {
                AllowAllOutbound = true,
                Description = "Primary Aurora Serverless SG.  Used by Lambdas",
                SecurityGroupName = "Aurora SG",
                Vpc = defaultVpc
            });

            securityGroup.AddIngressRule(securityGroup, Port.AllTraffic());

            new CfnDBCluster(this, "aurora-cluster", new CfnDBClusterProps
            {
                Engine = "aurora",
                EngineMode = "serverless",
                Port = 3306,
                MasterUsername = "admin",
                MasterUserPassword = "password",
                DeletionProtection = false,
                StorageEncrypted = true,
                VpcSecurityGroupIds = new string[] { securityGroup.SecurityGroupId },
                ScalingConfiguration = new ScalingConfigurationProperty
                {
                    AutoPause = true,
                    MinCapacity = 1,
                    MaxCapacity = 1,
                    SecondsUntilAutoPause = 300
                }
            });

            new BastionHostLinux(this, "bastion-host", new BastionHostLinuxProps
            {
                Vpc = defaultVpc,
                SecurityGroup = securityGroup
            });
        }
    }
}
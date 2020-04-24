using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Lambda;
using System;

namespace Infrastructure.Stacks
{
    public class CensusEtlStackProps : StackProps
    {
        public string AuroraSeucrityGroupId { get; set; } = String.Empty;
    }

    public class CensusEtlStack : Stack
    {
        internal CensusEtlStack(Construct scope, string id, CensusEtlStackProps props) : base(scope, id, props)
        {
            var defaultVpc = Vpc.FromLookup(this, "default", new VpcLookupOptions
            {
                IsDefault = true
            });

            var securityGroup = SecurityGroup.FromSecurityGroupId(this, "", "");

            //new Function(this, "census-etl", new FunctionProps
            //{
            //    Runtime = Runtime.DOTNET_CORE_3_1,
            //    Handler = "",
            //    //SecurityGroup 
            //});
        }
    }
}

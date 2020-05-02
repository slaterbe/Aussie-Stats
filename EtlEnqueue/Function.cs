
using Amazon.Lambda.Core;
using EtlEnqueue.Command;
using EtlEnqueue.Model;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.LambdaJsonSerializer))]

namespace EtlEnqueue
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            var environment = new EnvironmentModel();

            var collection = new ServiceCollection();
            collection.AddSingleton<EnvironmentModel>(environment);
            collection.AddScoped<ICensusFileCommand, CensusFileCommand>();
            collection.AddScoped<IQueueCommand, QueueCommand>();

            collection.AddMediatR(typeof(Function));

            return input?.ToUpper();
        }
    }
}

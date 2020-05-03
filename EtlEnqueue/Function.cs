
using Amazon.Lambda.Core;
using Amazon.S3;
using EtlEnqueue.Command;
using EtlEnqueue.Model;
using EtlEnqueue.Request;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.AWS.Logger;
using NLog.Config;
using System;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

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
            collection.AddScoped<IAmazonS3, AmazonS3Client>();

            collection.AddMediatR(typeof(Function));

            ConfigureLogging(collection);

            var provider = collection.BuildServiceProvider();
            var mediatr = provider.GetService<IMediator>();
            var request = new EtlEnqueueRequest();

            mediatr.Send(request).Wait();

            return input?.ToUpper();
        }

        private void ConfigureLogging(ServiceCollection collection)
        {
            var config = new LoggingConfiguration();

            var awsTarget = new AWSTarget()
            {
                LogGroup = "census-etl.enqueue",
                Region = "ap-souteast-2"
            };
            config.AddTarget("aws", awsTarget);

            config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Debug, awsTarget));

            LogManager.Configuration = config;

            collection.AddLogging(builder =>
            {
                // configure Logging with NLog
                builder.ClearProviders();
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.AddNLog(config);
            });
        }
    }
}

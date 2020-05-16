using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.SQS;
using EtlEnqueue.Command;
using EtlEnqueue.Model;
using EtlEnqueue.Pipeline;
using EtlEnqueue.Request;
using EtlEnqueue.Service;
using MediatR;
using MediatR.Pipeline;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
        public async Task<string> FunctionHandler(Object obj, ILambdaContext context)
        {
            try
            {
                return await this.Run(context);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return "Error thrown";
        }

        private async Task<string> Run(ILambdaContext context)
        {
            var environment = new EnvironmentModel();

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            var assemblies = GetAssemblies().ToArray();
            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            RegisterHandlers(container, typeof(INotificationHandler<>), assemblies);
            RegisterHandlers(container, typeof(IRequestExceptionAction<,>), assemblies);
            RegisterHandlers(container, typeof(IRequestExceptionHandler<,,>), assemblies);

            //Register Pipeline - ORDER MATTERS
            container.Collection.Register(typeof(IPipelineBehavior<,>), new [] { 
                typeof(CensusFilePipeline)
            });

            container.RegisterInstance<EnvironmentModel>(environment);
            container.RegisterInstance<ILogger>(new Logger(context.Logger));

            //Commands
            container.Register<ICensusFileCommand, CensusFileCommand>();
            container.Register<IQueueCommand, QueueCommand>();

            //Register AWS Services
            container.Register<IAmazonS3>(() => new AmazonS3Client(), Lifestyle.Singleton);
            container.Register<IAmazonSQS>(() => new AmazonSQSClient(), Lifestyle.Singleton);

            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);

            container.Verify();
            var mediatr = container.GetInstance<IMediator>();
            var request = new EtlEnqueueRequest();

            await mediatr.Send(request);

            return "Enqueue Successful";
        }

        private static void RegisterHandlers(Container container, Type collectionType, Assembly[] assemblies)
        {
            // we have to do this because by default, generic type definitions (such as the Constrained Notification Handler) won't be registered
            var handlerTypes = container.GetTypesToRegister(collectionType, assemblies, new TypesToRegisterOptions
            {
                IncludeGenericTypeDefinitions = true,
                IncludeComposites = false,
            });

            container.Collection.Register(collectionType, handlerTypes);
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IMediator).GetTypeInfo().Assembly;
            yield return typeof(Function).GetTypeInfo().Assembly;
        }
    }
}

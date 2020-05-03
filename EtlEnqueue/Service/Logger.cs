using Amazon.Lambda.Core;
using System;

namespace EtlEnqueue.Service
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogError(Exception exception);
    }

    public class Logger : ILogger
    {
        private readonly ILambdaLogger lambdaLogger;

        public Logger(ILambdaLogger lambdaLogger)
        {
            this.lambdaLogger = lambdaLogger;
        }

        public void LogError(Exception exception)
        {
            lambdaLogger.LogLine($"Error: {exception.Message}");
        }

        public void LogInfo(string message)
        {
            lambdaLogger.LogLine(message);
        }
    }
}

using EtlEnqueue.Request;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EtlEnqueue.Pipeline
{
    public class ExceptionHandlerPipeline : IRequestExceptionHandler<EtlEnqueueRequest, Unit>
    {
        private readonly ILogger<ExceptionHandlerPipeline> logger;

        public ExceptionHandlerPipeline(ILogger<ExceptionHandlerPipeline> logger)
        {
            this.logger = logger;
        }

        public async Task Handle(EtlEnqueueRequest request, 
            Exception exception, 
            RequestExceptionHandlerState<Unit> state, 
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Hello world");
            state.SetHandled();
        }
    }
}

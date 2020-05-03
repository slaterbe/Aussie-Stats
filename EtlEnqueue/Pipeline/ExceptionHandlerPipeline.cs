using EtlEnqueue.Request;
using EtlEnqueue.Service;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EtlEnqueue.Pipeline
{
    public class ExceptionHandlerPipeline : IRequestExceptionHandler<EtlEnqueueRequest, Unit>
    {
        private readonly ILogger logger;

        public ExceptionHandlerPipeline(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task Handle(EtlEnqueueRequest request, 
            Exception exception, 
            RequestExceptionHandlerState<Unit> state, 
            CancellationToken cancellationToken)
        {
            state.SetHandled();
        }
    }
}

using EtlEnqueue.Command;
using EtlEnqueue.Request;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EtlEnqueue.Handler
{
    public class EtlEnqueueHandler : AsyncRequestHandler<EtlEnqueueRequest>
    {
        private readonly ICensusFileCommand censusFileCommand;
        private readonly IQueueCommand queueCommand;

        public EtlEnqueueHandler(ICensusFileCommand censusFileCommand,
            IQueueCommand queueCommand)
        {
            this.censusFileCommand = censusFileCommand;
            this.queueCommand = queueCommand;
        }

        protected override async Task Handle(EtlEnqueueRequest request, CancellationToken cancellationToken)
        {
            var files = await censusFileCommand.GetCensusFiles();
            await queueCommand.Enqueue(files);
        }
    }
}

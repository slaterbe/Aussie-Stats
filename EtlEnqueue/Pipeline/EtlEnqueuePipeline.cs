using EtlEnqueue.Command;
using MediatR;
using System.Threading.Tasks;

namespace EtlEnqueue.Pipeline
{
    public interface IEtlEnqueuePipeline
    {
        Task Execute();
    }

    public class EtlEnqueuePipeline : IEtlEnqueuePipeline
    {
        private readonly IMediator mediator;
        private readonly ICensusFileCommand censusFileCommand;
        private readonly IQueueCommand queueCommand;

        public EtlEnqueuePipeline(IMediator mediator,
            ICensusFileCommand censusFileCommand,
            IQueueCommand queueCommand)
        {
            this.mediator = mediator;
            this.censusFileCommand = censusFileCommand;
            this.queueCommand = queueCommand;
        }

        public async Task Execute()
        {
            

            var files = await censusFileCommand.GetCensusFiles();
            await queueCommand.Enqueue(files);
        }
    }
}

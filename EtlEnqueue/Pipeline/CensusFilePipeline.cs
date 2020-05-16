using EtlEnqueue.Command;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EtlEnqueue.Pipeline
{
    public interface ICensusData
    {
        List<string> CensusFiles { get; set; }
    }

    public class CensusFilePipeline : IPipelineBehavior<ICensusData, Unit>
    {
        private readonly ICensusFileCommand censusFileCommand;

        public CensusFilePipeline(ICensusFileCommand censusFileCommand)
        {
            this.censusFileCommand = censusFileCommand;
        }

        public async Task<Unit> Handle(ICensusData request, 
            CancellationToken cancellationToken, 
            RequestHandlerDelegate<Unit> next)
        {
            request.CensusFiles = await censusFileCommand.GetCensusFiles();
            return await next();
        }
    }
}

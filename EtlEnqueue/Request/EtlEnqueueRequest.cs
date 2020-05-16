using EtlEnqueue.Pipeline;
using MediatR;
using System.Collections.Generic;

namespace EtlEnqueue.Request
{
    public class EtlEnqueueRequest : IRequest, ICensusData
    {
        public List<string> CensusFiles { get; set; }
    }
}

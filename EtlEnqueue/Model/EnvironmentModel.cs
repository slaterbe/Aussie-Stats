namespace EtlEnqueue.Model
{
    public class EnvironmentModel
    {
        public EnvironmentModel()
        {
            KeyPrefix = System.Environment.GetEnvironmentVariable("AUSSIE_STATS_ENQUEUE_KEY_PREFIX");
            Bucket = System.Environment.GetEnvironmentVariable("AUSSIE_STATS_ASSET_BUCKET");
            CensusEtlQueueUrl = System.Environment.GetEnvironmentVariable("AUSSIE_STATS_CENSUS_ETL_QUEUE");
        }

        public string KeyPrefix { get; }
        public string Bucket { get; }
        public string CensusEtlQueueUrl { get; }
    }
}

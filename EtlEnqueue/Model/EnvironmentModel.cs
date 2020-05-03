namespace EtlEnqueue.Model
{
    public class EnvironmentModel
    {
        public EnvironmentModel()
        {
            KeyPrefix = System.Environment.GetEnvironmentVariable("ENQUEUE_KEY_PREFIX");
            Bucket = System.Environment.GetEnvironmentVariable("ASSET_BUCKET");
            TargetQueue = System.Environment.GetEnvironmentVariable("ENQUEUE_SQS");
        }

        public string KeyPrefix { get; }
        public string Bucket { get; }
        public string TargetQueue { get; }
    }
}

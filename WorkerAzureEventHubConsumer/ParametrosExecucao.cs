namespace WorkerAzureEventHubConsumer
{
    public class ParametrosExecucao
    {
        public string EventHubsConnectionString { get; set; }
        public string EventHub { get; set; }
        public string ConsumerGroup { get; set; }
        public string BlobStorageConnectionString { get; set; }
        public string BlobContainerName { get; set; }
    }
}
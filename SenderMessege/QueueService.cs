namespace SenderMessege
{
    public class QueueService : IQueueService
    {
        private readonly IQueueClientFactory _queueClientFactory;

        public QueueService(IQueueClientFactory queueClientFactory)
        {
            _queueClientFactory = queueClientFactory;
        }

        public async Task SendMessageAsync(string message, string queueName)
        {
            var queueClient = _queueClientFactory.Create(queueName);

            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }
    }
}
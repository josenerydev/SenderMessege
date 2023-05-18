using Azure.Storage.Queues;

using Microsoft.Extensions.ObjectPool;

using SenderMessege;

public class QueueService : IQueueService
{
    private readonly IDictionary<string, ObjectPool<QueueClient>> _queuePools;

    public QueueService(IDictionary<string, ObjectPool<QueueClient>> queuePools)
    {
        _queuePools = queuePools;
    }

    public async Task SendMessageAsync(string message, string queueName)
    {
        var queuePool = _queuePools[queueName];
        var queueClient = queuePool.Get();

        try
        {
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }
        finally
        {
            queuePool.Return(queueClient);
        }
    }
}
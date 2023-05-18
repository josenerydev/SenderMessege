namespace SenderMessege
{
    public interface IQueueService
    {
        Task SendMessageAsync(string queueName, string message);
    }
}
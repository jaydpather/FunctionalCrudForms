namespace RebelSoftware.MessageQueue
{
    public interface IMessageQueueService
    {
        string WriteMessageAndGetResponse(string message);
    }

    public static class MessageQueueServiceFactory
    {
        public static IMessageQueueService CreateMessageQueueService()
        {
            return new MessageQueueService();
        }
    }
}
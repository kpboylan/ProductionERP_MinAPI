namespace ProductionERP_MinAPI.Service
{
    public interface IEmailServiceBusPublisher
    {
        Task PublishEmailNotificationAsync<T>(T productData, string eventType);
    }
}

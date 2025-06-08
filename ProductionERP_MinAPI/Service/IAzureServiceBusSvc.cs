
namespace ProductionERP_MinAPI.Service
{
    public interface IAzureServiceBusSvc<in T> where T : class
    {
        Task<string> PublishAsync(T item);
    }
}
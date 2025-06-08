using Azure.Messaging.ServiceBus;
using ProductionERP_MinAPI.Enum;
using ProductionERP_MinAPI.Model;
using System.Text.Json;

namespace ProductionERP_MinAPI.Service
{
    public class AzureServiceBusSvc<T> : IAzureServiceBusSvc<T> where T : class
    {
        private readonly string _connectionString;
        private readonly MessageBus _bus;

        public AzureServiceBusSvc(IConfiguration configuration, MessageBus bus)
        {
            _bus = bus;
            _bus.QueueName = MessageQueue.AzureQueueName.materialqueue.ToString();
            //_connectionString = configuration["ServiceBus__ConnectionString"];

            _connectionString = configuration.GetConnectionString("ServiceBus__ConnectionString");
        }
        public async Task<string> PublishAsync(T item)
        {
            var client = new ServiceBusClient(_connectionString);
            var sender = client.CreateSender(_bus.QueueName);

            string messageBody = JsonSerializer.Serialize(item);

            var message = new ServiceBusMessage(messageBody)
            {
                ContentType = "application/json",
                Subject = typeof(T).Name
            };

            await sender.SendMessageAsync(message);

            await sender.DisposeAsync();
            await client.DisposeAsync();

            return $"Processed item of type {typeof(T).Name}: {item}";
        }
    }
}

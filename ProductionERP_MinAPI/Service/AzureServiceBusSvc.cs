using Azure.Messaging.ServiceBus;
using ProductionERP_MinAPI.Enum;
using ProductionERP_MinAPI.Model;
using System.Text.Json;

namespace ProductionERP_MinAPI.Service
{
    public class AzureServiceBusSvc<T> : IAzureServiceBusSvc<T> where T : class
    {
        private readonly string? _connectionString;
        private readonly MessageBus _bus;
        private readonly IEmailServiceBusPublisher _emailPublisher;
        private readonly string _materialQueueName;
        private readonly string _productQueueName;

        public AzureServiceBusSvc(IConfiguration configuration, MessageBus bus, IEmailServiceBusPublisher emailPublisher)
        {
            _bus = bus;
            _connectionString = configuration.GetConnectionString("ServiceBusConn");
            _materialQueueName = configuration["AzureServiceBus:MaterialQueueName"] ?? "Material_Queue";
            _productQueueName = configuration["AzureServiceBus:ProductQueueName"] ?? "Product_Queue";
            _emailPublisher = emailPublisher;
        }
        public async Task<string> PublishAsync(T item)
        {
            string queueName = GetQueueName(typeof(T).Name);
            _bus.QueueName = queueName;

            await using var client = new ServiceBusClient(_connectionString);
            await using var sender = client.CreateSender(_bus.QueueName);

            string messageBody = JsonSerializer.Serialize(item);

            var message = new ServiceBusMessage(messageBody)
            {
                ContentType = "application/json",
                Subject = typeof(T).Name
            };

            await sender.SendMessageAsync(message);

            if (queueName == _productQueueName)
            {
                await _emailPublisher.PublishEmailNotificationAsync(item, "ProductAdded");
            }

            return $"Processed item of type {typeof(T).Name}: {item}";
        }

        private string GetQueueName(string objectName)
        {
            if (objectName.ToLower() == "material") 
            { 
                return _materialQueueName;
            }
            else if (objectName.ToLower() == "product")
            {
                return _productQueueName;
            }
            else
            {
                throw new ArgumentException("Queue name not found");
            }
        }
    }
}

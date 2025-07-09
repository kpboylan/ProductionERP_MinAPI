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
            
            _connectionString = configuration.GetConnectionString("ServiceBusConn");
        }
        public async Task<string> PublishAsync(T item)
        {
            _bus.QueueName = GetQueueName(typeof(T).Name);

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

        private string GetQueueName(string objectName)
        {
            if (objectName.ToLower() == "material") 
            { 
                return MessageQueue.AzureMaterialQueueName.Material_Queue.ToString();
            }
            else if (objectName.ToLower() == "product")
            {
                return MessageQueue.AzureProductQueueName.Product_Queue.ToString();
            }
            else
            {
                throw new Exception("Queue name not found");
            }
        }
    }
}

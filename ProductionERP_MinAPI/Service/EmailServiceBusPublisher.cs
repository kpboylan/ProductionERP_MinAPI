using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace ProductionERP_MinAPI.Service
{
    public class EmailServiceBusPublisher : IEmailServiceBusPublisher
    {
        private readonly string? _connectionString;
        private readonly string _emailQueueName;

        public EmailServiceBusPublisher(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ServiceBusConn");
            _emailQueueName = configuration["AzureServiceBus:EmailQueueName"] ?? "Email_Queue";
        }

        public async Task PublishEmailNotificationAsync<T>(T productData, string eventType)
        {
            await using var client = new ServiceBusClient(_connectionString);
            await using var sender = client.CreateSender(_emailQueueName);

            var emailMessageContent = new
            {
                EventType = eventType,
                ProductData = productData,
                Timestamp = DateTimeOffset.UtcNow
            };
            string emailMessageBody = JsonSerializer.Serialize(emailMessageContent);

            var message = new ServiceBusMessage(emailMessageBody)
            {
                ContentType = "application/json",
                Subject = $"{eventType}Notification"
            };

            await sender.SendMessageAsync(message);
        }
    }
}

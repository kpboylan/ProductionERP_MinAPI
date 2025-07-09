using System.ComponentModel;

namespace ProductionERP_MinAPI.Enum
{
    public class MessageQueue
    {
        public enum MessageQueueName
        {
            [Description("AddMaterialQueue")]
            AddMaterialQueue,

            [Description("UpdateMaterialQueue")]
            UpdateMaterialQueue,
        }

        public enum AzureMaterialQueueName
        {
            [Description("Material_Queue")]
            Material_Queue
        }

        public enum AzureProductQueueName
        {
            [Description("Product_Queue")]
            Product_Queue
        }

        public enum MessageHostName
        {
            [Description("Localhost")]
            LocalHost,
        }

        public enum DockerMessageHostName
        {
            [Description("rabbitmq")]
            rabbitmq,
        }
    }
}

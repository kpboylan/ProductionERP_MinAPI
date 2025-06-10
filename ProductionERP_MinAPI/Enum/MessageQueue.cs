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

        public enum AzureQueueName
        {
            [Description("Material_Queue")]
            materialqueue
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

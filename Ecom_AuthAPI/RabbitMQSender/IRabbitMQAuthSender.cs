namespace Ecom_AuthAPI.RabbitMQSender
{
    public interface IRabbitMQAuthSender
    {
        void SendMessage(Object message, string queueName);
    }
}

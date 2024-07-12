namespace Ecom_ShoppingCartAPI.RabbitMQSender
{
    public interface IRabbitMQCartSender
    {
        void SendMessage(Object message, string queueName);
    }
}

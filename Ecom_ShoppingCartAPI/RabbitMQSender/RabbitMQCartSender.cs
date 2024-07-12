using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Ecom_ShoppingCartAPI.RabbitMQSender
{
    public class RabbitMQCartSender : IRabbitMQCartSender
    {
        private readonly string _hostName;
        private readonly string _password;

        private readonly string _username;

        // for connection
        private  IConnection _connection;
        public RabbitMQCartSender()
        {
            _hostName = "localhost";
            _username = "guest";
            _password = "guest";

        }

        public void SendMessage(Object message, string queueName)
        {
            // in ordet to use rabbit mq , we need to create connection factory and using that we can access 
            // for connection facotry we need to install package rabbitmq client then it will allow and we can implmennt flow
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                Password = _password,
                UserName = _username,

            };

            // now create connectionto establish the connection to rabbit mq
            _connection = factory.CreateConnection();

            // create channel to communicate to rabbit mq
            using var channel = _connection.CreateModel(); // this will create fresh channel/session/model 

            // declare a queue making all false so that we can get the message other config also we need to see and check
            channel.QueueDeclare(queueName,false,false,false,null);

            // sereliazed message and return to body
            var json= JsonConvert.SerializeObject(message);
            var body=Encoding.UTF8.GetBytes(json);

            // send message using channel publish using exchange/queuename as routingkey then body and null for base
            // properties so that we cna get the message
            channel.BasicPublish(exchange:"",routingKey:queueName,null,body:body);

        }
    }
}

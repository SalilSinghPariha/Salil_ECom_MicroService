
using Ecom.Service.EmailAPI.Model;
using Ecom.Service.EmailAPI.Service;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Ecom.Service.EmailAPI.Messaging
{
    public class RabbitMQCartConsumer : BackgroundService
    {
        private IConnection _Connection;
        private IModel _model;
        private EmailService _emailService;
        private IConfiguration _configuration;
        public RabbitMQCartConsumer(IConfiguration configuration,EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

        // initialize and create connection
            _Connection = factory.CreateConnection();
            // create model
            _model = _Connection.CreateModel();
            // cdecalre queue to read message
            _model.QueueDeclare(_configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCart"),
                false, false, false, null);


        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // if request cancellation then straightway stop
            stoppingToken.ThrowIfCancellationRequested();

            // if not cancel then create consumer to publish messsage or consume message
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (ch,ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                CartDTO cart = JsonConvert.DeserializeObject<CartDTO>(content);
                //message processed
                 HandleMessage(cart).GetAwaiter().GetResult();

                //message acknowledge
                _model.BasicAck(ea.DeliveryTag,false);
                //now event handled creation done and we need to assign it our channel for that
                //we have basic consume method
            };

            //basic consume method
            _model.BasicConsume(_configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCart"),
                false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(CartDTO cart)
        {
            _emailService.EmailCartAndLog(cart);
        }
    }
}

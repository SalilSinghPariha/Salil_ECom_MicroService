using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecom.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://ecomweb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ACX7AxCz0s1va8gzOkI2QDDlh4WxJ7saM+ASbPTVRBA=";
       
        public async Task PublishMessage(object message, string topic_queue_name)
        {
            // create service bus client
            await using var client = new ServiceBusClient(connectionString);

            // create sender
            ServiceBusSender sender = client.CreateSender(topic_queue_name);
            //seralized mesage

            var jsonMessage= JsonConvert.SerializeObject(message);
            // final message

            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(
                Encoding.UTF8.GetBytes(jsonMessage)
                )
            {
                //identifier
                CorrelationId = Guid.NewGuid().ToString(),
            };
            // send message
            await sender.SendMessageAsync(serviceBusMessage);
            await client.DisposeAsync();
        }
    }
}

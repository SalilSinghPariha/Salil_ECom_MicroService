using Azure.Messaging.ServiceBus;
using Ecom.Service.EmailAPI.Model;
using Ecom.Service.EmailAPI.Service;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Ecom.Service.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly string ServiceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string emailRegisterQueue;
        private readonly IConfiguration _configuration;
        //Processor to listen queue and topic
        private ServiceBusProcessor _emailCartProcessor;
        //processor to listen register queue
        private ServiceBusProcessor _emailRegisterProcessor;
        //for email service singleton
        private readonly EmailService _emailService;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCart");
            emailRegisterQueue = _configuration.GetValue<string>("TopicAndQueueNames:emailRegisterUser");

            // create service bus client

            var client = new ServiceBusClient(ServiceBusConnectionString);
            // listen to queue or topic then we need to create processor for that
            // if we are creating processor for queue then only queue name require and if
            // it's topic then it will require topic and subscription since we are using only queue then queue is enough.
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
           
            // create processor for register queue
            _emailRegisterProcessor= client.CreateProcessor(emailRegisterQueue);    
            // register handler for processor


        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += emailCartRequestRecieved;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            // for register
            _emailRegisterProcessor.ProcessMessageAsync += emailRegisterRequestRecieved;
            _emailRegisterProcessor.ProcessErrorAsync += ErrorHandler;
            // start procressing the request
            await _emailCartProcessor.StartProcessingAsync();
            await _emailRegisterProcessor.StartProcessingAsync();
        }

        private async Task emailRegisterRequestRecieved(ProcessMessageEventArgs args)
        {
            var message= args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            string objMessage=JsonConvert.DeserializeObject<string>(body);
            try 
            {
                await _emailService.EmailUserAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        // this mehtod will automatically reached here in this method when any message their
        // in the azure queue since we have given connection string above and once will go here
        // and complete the message then it means it's done and message will be removed from queue
        // we can put breakpoint here ands check as well
        private async Task emailCartRequestRecieved(ProcessMessageEventArgs arg)
        {
            // here this is the way we recive the message
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDTO objMessage= JsonConvert.DeserializeObject<CartDTO>(body);

            try 
            {
                // log and save email 
                await _emailService.EmailCartAndLog(objMessage);
                // mark message as complete
                await arg.CompleteMessageAsync(arg.Message);
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Stop()
        {
           await  _emailCartProcessor.StopProcessingAsync();
           await  _emailCartProcessor.DisposeAsync();
           await _emailRegisterProcessor.StopProcessingAsync();
            await _emailRegisterProcessor.DisposeAsync();
        }
    }
}

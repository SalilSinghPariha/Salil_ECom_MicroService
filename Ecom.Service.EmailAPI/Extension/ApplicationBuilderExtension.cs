using Ecom.Service.EmailAPI.Messaging;

namespace Ecom.Service.EmailAPI.Extension
{
    public static class ApplicationBuilderExtension
    {
        private static IAzureServiceBusConsumer azureServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            //azure service bus consumer
            azureServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            // tell application life time
            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            // register azure service bus consumer on start
            hostApplicationLife.ApplicationStarted.Register(OnStart);

            //same on stop as well
            hostApplicationLife.ApplicationStopped.Register(OnStop);

            return app;
            
        }

        private static void OnStop()
        {
            azureServiceBusConsumer.Stop();
        }

        private static void OnStart()
        {
            azureServiceBusConsumer.Start();
        }
    }
}

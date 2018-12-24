using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Sample.ServiceBusFiltering.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets("32874672-80BB-4246-85E2-0D29BE2F19A5")
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            using (var receiver = new Receiver(config["serviceBus:connectionString"], config["serviceBus:topicName"], config["serviceBus:subscriptionName"]))
            {
                Console.WriteLine("Receiving Messages, press return to stop");
                Console.ReadLine();
            }
        }
    }

    internal class Receiver : IDisposable
    {
        private readonly SubscriptionClient _subscriptionClient;

        public Receiver(string connectionString, string topicName, string subscriptionName)
        {
            _subscriptionClient = new SubscriptionClient(
                connectionString,
                topicName,
                subscriptionName);

            _subscriptionClient.RegisterMessageHandler(
                (message, cancellation) => {
                    var alertBody = System.Text.Encoding.UTF8.GetString(message.Body);
                    Console.WriteLine(alertBody);

                    return Task.CompletedTask;
                }, 
                (exceptionArgs) => {
                    return Task.CompletedTask;
                });
        }

        public void Dispose()
        {
            _subscriptionClient.CloseAsync().GetAwaiter().GetResult();
        }
    }
}

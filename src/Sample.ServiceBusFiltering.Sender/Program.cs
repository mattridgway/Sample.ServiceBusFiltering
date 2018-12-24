using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sample.ServiceBusFiltering.Sender.Models;

namespace Sample.ServiceBusFiltering.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets("2F8231B3-D81F-4EB6-B5E2-150C5F8B331C")
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            var sender = new Sender(config["serviceBus:connectionString"], config["serviceBus:topicName"]);
            sender.SendFakeDeviceData().GetAwaiter().GetResult();

            Console.WriteLine("Sent Data");
            Console.ReadLine();
        }
    }

    internal class Sender
    {
        private readonly ITopicClient _topicClient;
        private readonly List<DeviceReading> _fakeData;

        public Sender(string connectionString, string topicName)
        {
            _topicClient = new TopicClient(connectionString, topicName);
            _fakeData = new List<DeviceReading>
            {
                new DeviceReading { DeviceId = "home", Temperature = 12.5, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 50, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "work", Temperature = 50.5, TemperatureUnit = "fahrenheit", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 50, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.7, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 51, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.7, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 52, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.8, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 53, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.7, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 54, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.7, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 55, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "work", Temperature = 50.0, TemperatureUnit = "fahrenheit", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 55, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.9, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 56, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 13.0, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 57, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.9, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 58, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.7, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 22, 59, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 31.0, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 23, 00, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "work", Temperature = 50.0, TemperatureUnit = "fahrenheit", Timestamp = new DateTimeOffset(2018, 12, 23, 23, 00, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.9, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 23, 51, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.8, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 23, 52, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.7, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 23, 53, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.6, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 23, 54, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.5, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 23, 55, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "work", Temperature = 51.0, TemperatureUnit = "fahrenheit", Timestamp = new DateTimeOffset(2018, 12, 23, 23, 55, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.7, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 23, 56, 00, new TimeSpan(0, 0, 0)) },
                new DeviceReading { DeviceId = "home", Temperature = 12.7, TemperatureUnit = "celsius", Timestamp = new DateTimeOffset(2018, 12, 23, 23, 56, 00, new TimeSpan(0, 0, 0)) }
            };
        }

        public async Task SendFakeDeviceData()
        {
            await _topicClient.SendAsync(
                _fakeData.Select(message => {
                    var json = JsonConvert.SerializeObject(message);
                    var encoded = Encoding.UTF8.GetBytes(json);
                    var brokeredMessage = new Message(encoded);
                    brokeredMessage.UserProperties.Add("deviceName", message.DeviceId);
                    brokeredMessage.UserProperties.Add("temp", message.Temperature);
                    brokeredMessage.UserProperties.Add("unit", message.TemperatureUnit);
                    return brokeredMessage;
                }).ToList()
            );
        }
    }
}

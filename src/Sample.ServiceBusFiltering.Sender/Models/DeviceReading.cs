using System;

namespace Sample.ServiceBusFiltering.Sender.Models
{
    internal class DeviceReading
    {
        public string DeviceId { get; set; }
        public double Temperature { get; set; }
        public string TemperatureUnit { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}

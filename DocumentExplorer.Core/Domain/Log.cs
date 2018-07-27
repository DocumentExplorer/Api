using System;
using NodaTime;

namespace DocumentExplorer.Core.Domain
{
    public class Log
    {
        public Guid Id { get; private set; }
        public string Event { get; private set;}
        public DateTime Date { get; private set; }
        public string EventDate 
        {
            get
            {
                var timeZone = DateTimeZoneProviders.Tzdb["Europe/Warsaw"];
                var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(Date, DateTimeKind.Utc));
                return instant.InZone(timeZone).ToDateTimeUnspecified().ToString(@"dd.MM.yyyy HH:mm:ss");
            }
        }
        public Guid OrderId { get; private set; }
        public int Number { get; private set; }
        public string ClientCountry { get; private set; }
        public string ClientIdentificationNumber { get; private set; }
        public string BrokerCountry { get; private set; }
        public string BrokerIdentificationNumber { get; private set; }
        public string Owner1Name { get; private set; }
        public string Username { get; private set; }
        public int InvoiceNumber { get; private set; }
        
        public Log(string @event, Order order, string username, DateTime date)
        {
            Id = Guid.NewGuid();
            Event = @event;
            if(date==default(DateTime))
                Date = DateTime.UtcNow;
            else
                Date = date;
            OrderId = order.Id;
            Number = order.Number;
            ClientCountry = order.ClientCountry;
            ClientIdentificationNumber = order.ClientIdentificationNumber;
            BrokerCountry = order.BrokerCountry;
            BrokerIdentificationNumber = order.BrokerIdentificationNumber;
            Owner1Name = order.Owner1Name;
            Username = username;
            InvoiceNumber = order.InvoiceNumber;
        }

        protected Log()
        {
            
        }
    }
}
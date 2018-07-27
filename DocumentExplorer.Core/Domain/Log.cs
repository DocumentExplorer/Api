using System;

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
                //var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                //DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(Date, timeZoneInfo);
                return Date.ToString(@"dd.MM.yyyy HH:mm:ss");
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
        
        public Log(string @event, Order order, string username)
        {
            Id = Guid.NewGuid();
            Event = @event;
            Date = DateTime.UtcNow;
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
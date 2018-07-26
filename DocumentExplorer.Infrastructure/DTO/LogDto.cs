using System;

namespace DocumentExplorer.Infrastructure.DTO
{
    public class LogDto
    {
        public Guid Id { get; set; }
        public string Event { get; set;}
        public string EventDate { get; set; }
        public Guid OrderId { get; set; }
        public int Number { get; set; }
        public string ClientCountry { get; set; }
        public string ClientIdentificationNumber { get; set; }
        public string BrokerCountry { get; set; }
        public string BrokerIdentificationNumber { get; set; }
        public string Owner1Name { get; set; }
        public string Username { get; set; }
        public int InvoiceNumber { get; set; }
    }
}
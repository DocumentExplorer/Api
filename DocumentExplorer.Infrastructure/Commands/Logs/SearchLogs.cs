using System;

namespace DocumentExplorer.Infrastructure.Commands.Logs
{
    public class SearchLogs : ICommand
    {
        public Guid CacheId { get; set; }
        public string Event { get; set; }
        public DateTime Date { get; set; }
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
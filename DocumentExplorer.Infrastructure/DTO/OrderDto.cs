namespace DocumentExplorer.Infrastructure.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string ClientCountry {get; set;}
        public string ClientIdentificationNumber { get; set; }
        public string BrokerCountry { get; set; }
        public string BrokerIdentificationNumber { get; set; }
        public string Owner1Name { get; set; }
        public string Owner2Name { get; set; }
        public int InvoiceNumber { get; set; }
        public string Date { get; set; }
    }
}
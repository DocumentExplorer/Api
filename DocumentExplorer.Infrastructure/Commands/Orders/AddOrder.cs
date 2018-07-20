namespace DocumentExplorer.Infrastructure.Commands.Orders
{
    public class AddOrder : AuthenticatedCommandBase, ICommand 
    {
        public int Id { get; set; }
        public string ClientCountry { get; set; }
        public string ClientIdentificationNumber { get; set; }
        public string BrokerCountry { get; set; }
        public string BrokerIdentificationNumber { get; set; }
    }
}

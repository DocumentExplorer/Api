namespace DocumentExplorer.Infrastructure.Commands.Orders
{
    public class EditOrder : ICommand, IAuthenticatedCommand
    {
        public int Id { get; set; }
        public string ClientCountry { get; set; }
        public string ClientIdentificationNumber { get; set; }
        public string BrokerCountry { get; set; }
        public string BrokerIdentificationNumber { get; set; }
        public string Owner2Name { get; set; }
        public string Username { get; set; }
    }
}
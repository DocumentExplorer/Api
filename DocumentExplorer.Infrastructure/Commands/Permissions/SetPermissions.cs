namespace DocumentExplorer.Infrastructure.Commands.Permissions
{
    public class SetPermissions : ICommand
    {
        public string CMR {get; set;}
        public string FVK {get; set;}
        public string FVP {get; set;}
        public string NIP {get; set;}
        public string Nota {get; set;}
        public string PP {get; set;}
        public string RK {get; set;}
        public string ZK {get; set;}
        public string ZP {get; set;}
    }
}
namespace DocumentExplorer.Infrastructure.Commands
{
    public class AuthenticatedCommandBase : IAuthenticatedCommand
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }
}

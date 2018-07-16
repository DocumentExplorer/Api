namespace DocumentExplorer.Infrastructure.Commands.Users
{
    public class Login : ICommand
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}
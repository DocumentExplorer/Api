namespace DocumentExplorer.Infrastructure.Commands.Users
{
    public class Register : ICommand
    {
        public string Username { get;  set; }
        public string Password { get;  set; }
        public string Role { get;  set; }
    }
}
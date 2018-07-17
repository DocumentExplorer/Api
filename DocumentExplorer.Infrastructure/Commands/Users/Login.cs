using System;

namespace DocumentExplorer.Infrastructure.Commands.Users
{
    public class Login : ICommand
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid TokenId {get; set;}
    }
}
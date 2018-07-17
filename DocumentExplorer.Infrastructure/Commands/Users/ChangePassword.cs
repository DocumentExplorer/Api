using System;

namespace DocumentExplorer.Infrastructure.Commands.Users
{
    public class ChangePassword : ICommand
    {
        public Guid Id {get; set; }
        public string Password { get; set; }
    }
}
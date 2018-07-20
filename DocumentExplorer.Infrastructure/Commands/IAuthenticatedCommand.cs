using System;

namespace DocumentExplorer.Infrastructure.Commands
{
    public interface IAuthenticatedCommand
    {
        string Username { get; set; }
    }
}

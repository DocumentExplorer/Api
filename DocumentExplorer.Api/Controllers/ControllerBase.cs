using DocumentExplorer.Infrastructure.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentExplorer.Api.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected readonly IMemoryCache Cache;
        protected readonly ICommandDispatcher CommandDispatcher;

        protected ControllerBase(ICommandDispatcher commandDispatcher, IMemoryCache cache)
        {
            CommandDispatcher = commandDispatcher;
            Cache = cache;
        }

        protected string Username => User.Claims.ElementAt(0).Value;

        protected string Role => User.Claims.ElementAt(1).Value;

        protected async Task DispatchAsync<T>(T command) where T : ICommand
        {
            if (command is IAuthenticatedCommand authenticatedCommand)
            {
                authenticatedCommand.Username = Username;
                authenticatedCommand.Role = Role;
            }
            await CommandDispatcher.DispatchAsync(command);
        }
    }
}

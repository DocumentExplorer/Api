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

        protected bool IsAuthorized(string username)
        {
            if(username== null) return false;
            return IsRequestedByTheUser(username) || IsRequestedByAdmin();
        }

        protected bool IsAuthorizedPlusComplementer(string username)
        {
            if(username== null) return false;
            return IsRequestedByTheUser(username) || IsRequestedByAdmin() || IsRequestedByComplementer();
        }

        protected bool IsRequestedByTheUser(string username)
            => Username == username;

        protected bool IsRequestedByAdmin()
           => User.Claims.ElementAt(1).Value == "admin";
        protected bool IsRequestedByComplementer()
           => User.Claims.ElementAt(1).Value == "complementer";

        protected string Username => User.Claims.ElementAt(0).Value;

        protected async Task DispatchAsync<T>(T command) where T : ICommand
        {
            if (command is IAuthenticatedCommand authenticatedCommand)
            {
                authenticatedCommand.Username = Username;
            }
            await CommandDispatcher.DispatchAsync(command);
        }
    }
}

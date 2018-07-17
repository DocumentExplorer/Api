using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Users;
using DocumentExplorer.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMemoryCache _cache;
        public UsersController(ICommandDispatcher commandDispatcher, IMemoryCache cache)
        {
            _commandDispatcher = commandDispatcher;
            _cache = cache;
        }

        [Authorize("admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]Register command)
        {
            await _commandDispatcher.DispatchAsync(command);
            return Created($"users/{command.Username}", new object());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]Login command)
        {
            command.TokenId = Guid.NewGuid();
            await _commandDispatcher.DispatchAsync(command);
            var jwt = _cache.GetJwt(command.TokenId);
            return Json(jwt);
        }
    }
}

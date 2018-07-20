using System;
using System.Linq;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Users;
using DocumentExplorer.Infrastructure.Extensions;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IUserService _userService;
        private readonly ITokenManager _tokenManager;
        public UsersController(ICommandDispatcher commandDispatcher, IMemoryCache cache, 
            IUserService userService, ITokenManager tokenManager) : base(commandDispatcher)
        {
            _cache = cache;
            _userService = userService;
            _tokenManager = tokenManager;
        }

        [Authorize]
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserInfo(string username)
        {
            if(!IsAuthorized(username))
            {
                return StatusCode(403);
            }
            var user = await _userService.GetAsync(username);
            return Json(user);
        }

        [Authorize]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetUserInfo(Guid id)
        {
            var user = await _userService.GetAsync(id);
            if(!IsAuthorized(user.Username))
            {
                return StatusCode(403);
            }
            return Json(user);
        }

        [Authorize("admin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetAllAsync();
            return Json(users);
        }

        [Authorize("admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]Register command)
        {
            await DispatchAsync(command);
            return Created($"users/username/{command.Username}", new object());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]Login command)
        {
            command.TokenId = Guid.NewGuid();
            await DispatchAsync(command);
            var jwt = _cache.GetJwt(command.TokenId);
            return Json(jwt);
        }

        [Authorize]
        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            _tokenManager.DeactivateCurrent();
            return NoContent();
        }

        [Authorize("admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUser(id);
            return NoContent();
        }

        [Authorize("admin")]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePassword command)
        {
            await DispatchAsync(command);
            return NoContent();
        }
    }
}

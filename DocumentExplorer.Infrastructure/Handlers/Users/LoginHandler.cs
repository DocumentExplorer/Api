using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Users;
using DocumentExplorer.Infrastructure.Extensions;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Infrastructure.Handlers.Users
{
    public class LoginHandler : ICommandHandler<Login>
    {
        private readonly IUserService _userService;
        private readonly IJwtHandler _jwtHandler;
        private readonly IMemoryCache _cache;
        public LoginHandler(IUserService userService, IJwtHandler jwtHandler, IMemoryCache cache)
        {
            _userService = userService;
            _jwtHandler = jwtHandler;
            _cache = cache;
        }
        public async Task HandleAsync(Login command)
        {
            await _userService.LoginAsync(command.Username, command.Password);
            var user = await _userService.GetAsync(command.Username);
            var jwt = _jwtHandler.CreateToken(command.Username,user.Role);
            _cache.SetJwt(command.TokenId,jwt);
        }
    }
}
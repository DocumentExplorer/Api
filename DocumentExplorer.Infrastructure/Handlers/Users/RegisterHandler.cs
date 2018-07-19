using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Users;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;

namespace DocumentExplorer.Infrastructure.Handlers.Users
{
    public class RegisterHandler : ICommandHandler<Register>
    {
        private readonly IUserService _userService;
        private readonly IHandler _handler;
        public RegisterHandler(IUserService userService, IHandler handler)
        {
            _userService = userService;
            _handler = handler;
        }
        public async Task HandleAsync(Register command)
            => await _handler
            .Run(async () => await _userService.RegisterAsync(command.Username,command.Password,command.Role))
            .OnCustomError((DocumentExplorerException ex) =>
            {
                throw new ServiceException(ex.Code);
            }, true)
            .ExecuteAsync();
    }
}
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Users;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;

namespace DocumentExplorer.Infrastructure.Handlers.Users
{
    public class ChangePasswordHandler : ICommandHandler<ChangePassword>
    {
        private readonly IUserService _userService;
        private readonly IHandler _handler;

        public ChangePasswordHandler(IHandler handler,IUserService userService)
        {
            _userService = userService;
            _handler = handler;
        }
        public async Task HandleAsync(ChangePassword command)
            => await _handler
            .Run(async () => await _userService.ChangePassword(command.Id, command.Password))
            .OnCustomError((DocumentExplorerException ex) =>
            {
                throw new ServiceException(ex.Code);
            },true)
            .ExecuteAsync();
    }
}
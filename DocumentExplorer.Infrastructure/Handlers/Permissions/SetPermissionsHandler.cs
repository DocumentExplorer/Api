using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Permissions;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;

namespace DocumentExplorer.Infrastructure.Handlers.Permissions
{
    public class SetPermissionsHandler : ICommandHandler<SetPermissions>
    {
        private readonly IHandler _handler;
        private readonly IPermissionsService _permissionService;
        public SetPermissionsHandler(IHandler handler, IPermissionsService permissionService)
        {
            _handler = handler;
            _permissionService = permissionService;
        }
        public async Task HandleAsync(SetPermissions command)
            => await _handler
            .Run(async ()=> await _permissionService.UpdatePermissions(command.CMR, 
            command.FVK, command.FVP, command.NIP, command.Nota, command.PP, command.RK, 
            command.ZK, command.ZP))
            .OnCustomError(x=> throw new ServiceException(x.Code), true)
            .ExecuteAsync();
    }
}
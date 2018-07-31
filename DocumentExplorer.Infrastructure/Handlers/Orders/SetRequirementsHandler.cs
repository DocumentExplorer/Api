using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Orders;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;

namespace DocumentExplorer.Infrastructure.Handlers.Orders
{
    public class SetRequirementsHandler : ICommandHandler<SetRequirements>
    {
        private readonly IHandler _handler;
        private readonly IOrderService _orderService;
        private readonly IPermissionsService _permissionsService;
        public SetRequirementsHandler(IHandler handler, IOrderService orderService, 
            IPermissionsService permissionsService)
        {
            _handler = handler;
            _orderService = orderService;
            _permissionsService = permissionsService;
        }
        public async Task HandleAsync(SetRequirements command)
            => await _handler
            .Validate(async ()=>{
                await _orderService
                    .ValidatePermissionsToOrder(command.Username, command.Role, command.OrderId);
                await _permissionsService.Validate(command.FileType, command.Role);
            })
            .Run(async ()=> await _orderService
                .SetRequirementsAsync(command.OrderId,command.FileType,command.IsRequired, command.Username))
            .OnCustomError(x=> throw new ServiceException(x.Code), true)
            .ExecuteAsync();
    }
}
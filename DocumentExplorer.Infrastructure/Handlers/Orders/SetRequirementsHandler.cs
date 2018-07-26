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
        public SetRequirementsHandler(IHandler handler, IOrderService orderService)
        {
            _handler = handler;
            _orderService = orderService;
        }
        public async Task HandleAsync(SetRequirements command)
            => await _handler
            .Run(async ()=> await _orderService.SetRequirementsAsync(command.OrderId,command.FileType,command.IsRequired, command.Username, command.Role))
            .OnCustomError(x=> throw new ServiceException(x.Code), true)
            .ExecuteAsync();
    }
}
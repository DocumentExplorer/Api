using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Orders;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;

namespace DocumentExplorer.Infrastructure.Handlers.Orders
{
    public class DeleteOrderHandler : ICommandHandler<DeleteOrder>
    {
        private readonly IHandler _handler;
        private readonly IOrderService _orderService;
        public DeleteOrderHandler(IHandler handler, IOrderService orderService)
        {
            _orderService = orderService;
            _handler = handler;
        }
        public async Task HandleAsync(DeleteOrder command)
            => await _handler
            .Validate(async ()=> 
            {
                _orderService.ValidateIsNotComplementer(command.Role);
                await _orderService
                    .ValidatePermissionsToOrder(command.Username, command.Role, command.OrderId);
            })
            .Run(async ()=> await _orderService.DeleteAsync(command.OrderId, command.Username))
            .OnCustomError(x => throw new ServiceException(x.Code), true)
            .ExecuteAsync();
    }
}
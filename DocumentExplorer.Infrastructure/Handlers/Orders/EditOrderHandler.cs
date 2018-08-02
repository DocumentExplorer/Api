using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Orders;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;

namespace DocumentExplorer.Infrastructure.Handlers.Orders
{
    public class EditOrderHandler : ICommandHandler<EditOrder>
    {
        private readonly IHandler _handler;
        private readonly IOrderService _orderService;

        public EditOrderHandler(IHandler handler, IOrderService orderService)
        {
            _handler = handler;
            _orderService = orderService;
        }

        public async Task HandleAsync(EditOrder command)
            => await _handler
            .Validate(async ()=> 
                await _orderService.ValidatePermissionsToOrder(command.Username, command.Role, command.Id))
            .Run(async ()=> await _orderService.EditOrderAsync(command.Id,command.Number, 
            command.ClientCountry, command.ClientIdentificationNumber,
            command.BrokerCountry, command.BrokerIdentificationNumber, command.Username))
            .OnCustomError(x => throw new ServiceException(x.Code), true)
            .ExecuteAsync();
    }
}
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Orders;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;

namespace DocumentExplorer.Infrastructure.Handlers.Orders
{
    public class AddOrderHandler : ICommandHandler<AddOrder>
    {
        private readonly IHandler _handler;
        private readonly IOrderService _orderService;

        public AddOrderHandler(IHandler handler, IOrderService orderService)
        {
            _handler = handler;
            _orderService = orderService;
        }


        public async Task HandleAsync(AddOrder command)
            => await _handler
            .Run(async () => 
            {
                await _orderService.AddOrderAsync(command.CacheId,command.Number,
                command.ClientCountry, command.ClientIdentificationNumber,
                command.BrokerCountry, command.BrokerIdentificationNumber,
                command.Username);
            })
            .OnCustomError((DocumentExplorerException ex) =>
            {
                throw new ServiceException(ex.Code);
            }, true)
            .ExecuteAsync();
    }
}

﻿using System.Threading.Tasks;
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
                command.Username, command.Role);
            })
            .OnCustomError(x => throw new ServiceException(x.Code), true)
            .ExecuteAsync();
    }
}

using System;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Orders;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Infrastructure.Handlers.Orders
{
    public class GetLackingFilesHandler : ICommandHandler<GetLackingFiles>
    {
        private readonly IHandler _handler;
        private readonly IOrderService _orderService;
        private readonly IMemoryCache _memoryCache;

        public GetLackingFilesHandler(IHandler handler, IOrderService orderService, 
            IMemoryCache memoryCache)
        {
            _handler = handler;
            _orderService = orderService;
            _memoryCache = memoryCache;
        }

        public async Task HandleAsync(GetLackingFiles command)
            => await _handler
            .Run(async ()=>
            {
                var result = await _orderService.GetLackingFilesAsync(command.Username, command.Role);
                _memoryCache.Set(command.LackingFilesId, result, TimeSpan.FromSeconds(10));
            })
            .OnCustomError(x => throw new ServiceException(x.Code), true)
            .ExecuteAsync();
    }
}